using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Recom.Bus.RabbitMQ
{
    public class EventManager<T> : IBus<T>
    {
        private readonly ConfigRabbitMQ _config;
        private readonly IConsumerManager _consumerManager;
        private readonly IModel _model;

        public EventManager(IOptions<ConfigRabbitMQ> config, RabbitConnection connection)
        {
            _config = config.Value;
            _consumerManager = new DefaultConsumerManager();
            _model = connection.Model;
        }

        internal EventManager(ConfigRabbitMQ config, IConnection connection, IConsumerManager consumerManager)
        {
            _config = config;
            _consumerManager = consumerManager;
            _model = connection.CreateModel();
        }

        public void Publish(T obj, string exchange, string routingKey = null)
        {
            var properties = _model.CreateBasicProperties();
            properties.Persistent = _config.PersistentDelivery;

            _model.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public void Subscribe(string exchange, IEnumerable<string> routingKeys, string queue, Action<T> callback)
        {
            EnsureExchangeCreated(exchange);

            var queueName = _model.QueueDeclare(
                queue: queue ?? string.Empty,
                autoDelete: !_config.DurableQueues || string.IsNullOrEmpty(queue),
                durable: _config.DurableQueues,
                exclusive: false).QueueName;

            foreach (var routingKey in routingKeys)
            {
                _model.QueueBind(
                    queue: queueName,
                    exchange: exchange,
                    routingKey: routingKey);
            }

            var consumer = _consumerManager.CreateEventingConsumer(_model);
            _model.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                var obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body));
                callback(obj);
            };
        }

        private void EnsureExchangeCreated(string name)
        {
            _model.ExchangeDeclare(name, ExchangeType.Topic, durable: true, autoDelete: false);
        }
    }
}