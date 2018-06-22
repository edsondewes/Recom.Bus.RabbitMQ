using System;
using System.Collections.Generic;
using System.Reflection;

namespace Recom.Bus.RabbitMQ
{
    public class ConfigRabbitMQSubscription
    {
        public Action<IBus> OnStart { get; set; }
        public IEnumerable<Assembly> SubscriptionAssemblies { get; set; }
    }
}
