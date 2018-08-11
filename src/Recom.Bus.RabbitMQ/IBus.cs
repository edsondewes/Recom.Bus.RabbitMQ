using System;
using System.Collections.Generic;

namespace Recom.Bus.RabbitMQ
{
    public interface IBus<T>
    {
        void Publish(T obj, string routingKey = "", string exchange = null);
        void Subscribe(IEnumerable<string> routingKeys, Action<T> callback, string exchange = null, string queue = null);
    }
}
