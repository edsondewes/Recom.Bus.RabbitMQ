using System;
using System.Collections.Generic;

namespace Recom.Bus.RabbitMQ
{
    public interface IBus
    {
        void Publish<T>(T obj, string routingKey = "", string exchange = null);
        void Subscribe<T>(IEnumerable<string> routingKeys, Action<T> callback, string exchange = null, string queue = null);
    }
}
