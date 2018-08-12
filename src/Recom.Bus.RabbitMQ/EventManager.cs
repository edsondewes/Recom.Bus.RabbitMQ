using System;
using System.Collections.Generic;
using MessagePack;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Recom.Bus.RabbitMQ
{
    public class EventManager : IBus
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

        public void Publish<T>(T obj, string routingKey = "", string exchange = null)
        {
            var properties = _model.CreateBasicProperties();
            properties.Persistent = _config.PersistentDelivery;
            
            _model.BasicPublish(
                exchange: exchange ?? _config.DefaultExchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: MessagePackSerializer.Serialize(obj));
        }

        public void Subscribe<T>(IEnumerable<string> routingKeys, Action<T> callback, string exchange = null, string queue = null)
        {
            if (exchange == null)
                exchange = _config.DefaultExchange;

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
                var obj = MessagePackSerializer.Deserialize<T>(ea.Body);
                callback(obj);
            };
        }

        private void EnsureExchangeCreated(string name)
        {
            _model.ExchangeDeclare(name, ExchangeType.Topic, durable: true, autoDelete: false);
        }
    }
}