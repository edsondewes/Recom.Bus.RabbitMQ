using System;
using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace Recom.Bus.RabbitMQ
{
    public interface IBus : IDisposable
    {
        void CreateExchange(string name);
        EventingBasicConsumer Subscribe(string exchange, IEnumerable<string> routingKeys, string queue);
        void Subscribe<T>(string exchange, IEnumerable<string> routingKeys, string queue, Action<T> callback);
        void Publish<T>(T obj, string exchange, string routingKey = null);
    }
}
