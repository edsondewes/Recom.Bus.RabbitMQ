using System;

namespace Recom.Bus.RabbitMQ
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false)]
    public class RabbitSubscriptionAttribute : Attribute
    {
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
    }
}
