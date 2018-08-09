using System;
using System.Collections.Generic;

namespace Recom.Bus.RabbitMQ
{
    public interface IBus<T>
    {
        void Subscribe(string exchange, IEnumerable<string> routingKeys, string queue, Action<T> callback);
        void Publish(T obj, string exchange, string routingKey = null);
    }
}
