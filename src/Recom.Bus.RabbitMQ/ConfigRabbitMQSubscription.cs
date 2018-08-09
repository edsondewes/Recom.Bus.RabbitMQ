using System.Collections.Generic;
using System.Reflection;

namespace Recom.Bus.RabbitMQ
{
    public class ConfigRabbitMQSubscription
    {
        public IEnumerable<Assembly> SubscriptionAssemblies { get; set; }
    }
}
