using System;
using System.Collections.Generic;

namespace Recom.Bus.RabbitMQ
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RabbitSubscriptionAttribute : Attribute
    {
        public RabbitSubscriptionAttribute(string exchange, params string[] routingKeys)
            : this(exchange, null, routingKeys)
        {
        }

        public RabbitSubscriptionAttribute(string exchange, string queue, params string[] routingKeys)
        {
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentNullException(nameof(exchange));

            Exchange = exchange;
            Queue = queue;
            RoutingKeys = routingKeys;
        }

        public string Exchange { get; }
        public string Queue { get; }
        public IEnumerable<string> RoutingKeys { get; }
    }
}
