using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Recom.Bus.RabbitMQ
{
    public class AutoSubscriber
    {
        private IBus bus;
        private IServiceProvider serviceProvider;

        private readonly Type RabbitSubscriptionAttribute;
        private readonly Type MessageSubscriberInterface;

        public AutoSubscriber(IBus bus, IServiceProvider serviceProvider)
        {
            this.bus = bus;
            this.serviceProvider = serviceProvider;
            RabbitSubscriptionAttribute = typeof(RabbitSubscriptionAttribute);
            MessageSubscriberInterface = typeof(IMessageSubscriber);
        }

        public IEnumerable<MethodInfo> ListSubscriptionMethods(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(t => MessageSubscriberInterface.IsAssignableFrom(t))
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                .Where(m => m.GetCustomAttribute(RabbitSubscriptionAttribute) != null);
        }

        public void ProcessMessage(byte[] message, MethodInfo method)
        {
            var paramType = method.GetParameters().First().ParameterType;
            var obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message), paramType);

            var service = serviceProvider.GetService(method.DeclaringType);
            method.Invoke(service, new[] { obj });
        }

        public void Subscribe(Assembly assembly)
        {
            Subscribe(ListSubscriptionMethods(assembly));
        }

        public void Subscribe(IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var info = method.GetCustomAttribute(RabbitSubscriptionAttribute) as RabbitSubscriptionAttribute;
                var consumer = bus.Subscribe(info.Exchange, info.RoutingKey, info.Queue);
                consumer.Received += (model, ea) => ProcessMessage(ea.Body, method);
            }
        }
    }
}