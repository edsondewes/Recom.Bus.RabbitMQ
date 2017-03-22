using System;
using RabbitMQ.Client.Events;

namespace Recom.Bus.RabbitMQ
{
    public interface IBus : IDisposable
    {
        EventingBasicConsumer Subscribe(string exchange, string routingKey, string queue);
        void Subscribe<T>(string exchange, string routingKey, string queue, Action<T> callback);
        void Publish<T>(T obj, string exchange, string routingKey = null);
    }
}
