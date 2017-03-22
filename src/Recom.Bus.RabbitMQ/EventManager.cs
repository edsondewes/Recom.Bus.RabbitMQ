using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Recom.Bus.RabbitMQ
{
    public class EventManager : IBus
    {
        private IConnection connection;
        private IModel channel;

        public EventManager(IOptions<ConfigRabbitMQ> options)
        {
            var factory = new ConnectionFactory { HostName = options.Value.Host };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void CreateExchange(string name)
        {
            channel.ExchangeDeclare(name, ExchangeType.Topic, durable: true, autoDelete: false);
        }

        public EventingBasicConsumer Subscribe(string exchange, string routingKey, string queue)
        {
            CreateExchange(exchange);

            var queueName = channel.QueueDeclare(
                queue: queue,
                autoDelete: false,
                durable: true,
                exclusive: false).QueueName;

            channel.QueueBind(
                queue: queueName,
                exchange: exchange,
                routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);

            return consumer;
        }

        public void Subscribe<T>(string exchange, string routingKey, string queue, Action<T> callback)
        {
            var consumer = Subscribe(exchange, routingKey, queue);
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
    }
}