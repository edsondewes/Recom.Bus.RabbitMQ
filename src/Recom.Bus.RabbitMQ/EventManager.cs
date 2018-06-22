using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Recom.Bus.RabbitMQ
{
    public class EventManager : IBus
    {
        private IConnection connection;
        private IModel channel;

        public EventManager(IOptions<ConfigRabbitMQ> config)
        {
            connection = TryCreateConnection(config.Value.Host);
            channel = connection.CreateModel();
        }

        public void CreateExchange(string name)
        {
            channel.ExchangeDeclare(name, ExchangeType.Topic, durable: true, autoDelete: false);
        }

        public EventingBasicConsumer Subscribe(string exchange, IEnumerable<string> routingKeys, string queue)
        {
            CreateExchange(exchange);

            var queueName = channel.QueueDeclare(
                queue: queue ?? string.Empty,
                autoDelete: string.IsNullOrEmpty(queue),
                durable: true,
                exclusive: false).QueueName;

            foreach (var routingKey in routingKeys)
            {
                channel.QueueBind(
                    queue: queueName,
                    exchange: exchange,
                    routingKey: routingKey);
            }

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return consumer;
        }

        public void Subscribe<T>(string exchange, IEnumerable<string> routingKeys, string queue, Action<T> callback)
        {
            var consumer = Subscribe(exchange, routingKeys, queue);
            consumer.Received += (model, ea) =>
            {
                var obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body));
                callback(obj);
            };
        }

        public void Publish<T>(T obj, string exchange, string routingKey = null)
        {
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }

        private static IConnection TryCreateConnection(string host)
        {
            var factory = new ConnectionFactory { HostName = host };
            IConnection connection = null;
            do
            {
                try
                {
                    connection = factory.CreateConnection();
                }
                catch (BrokerUnreachableException)
                {
                    Console.WriteLine($"{DateTime.Now} - Cannot connect to rabbitmq. Retrying in 5 seconds");
                    Thread.Sleep(5000);
                }
            } while (connection == null);

            return connection;
        }
    }
}