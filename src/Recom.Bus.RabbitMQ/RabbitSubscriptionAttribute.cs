using System;
using System.Collections.Generic;

namespace Recom.Bus.RabbitMQ
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RabbitSubscriptionAttribute : Attribute
    {
        public RabbitSubscriptionAttribute(string exchange = null, string queue = null, string[] routingKeys = null)
        {
            Exchange = exchange;
            Queue = queue;
            RoutingKeys = routingKeys;
        }

        public string Exchange { get; }
        public string Queue { get; }
        public IEnumerable<string> RoutingKeys { get; }
    }
}
