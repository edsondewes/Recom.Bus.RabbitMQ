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
        private readonly IBus bus;
        private readonly Func<Type, object> resolve;
        private readonly Type MessageSubscriberInterface = typeof(IMessageSubscriber);

        public AutoSubscriber(IBus bus, Func<Type, object> resolve)
        {
            this.bus = bus;
            this.resolve = resolve;
        }

        public AutoSubscriber(IBus bus, IServiceProvider serviceProvider) : this(bus, serviceProvider.GetService)
        {
        }

        public IEnumerable<MethodInfo> ListSubscriptionMethods(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(t => MessageSubscriberInterface.IsAssignableFrom(t))
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                .Where(m => m.GetCustomAttributes<RabbitSubscriptionAttribute>().Any());
        }

        public void Subscribe(Assembly assembly)
        {
            Subscribe(ListSubscriptionMethods(assembly));
        }

        public void Subscribe(IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes<RabbitSubscriptionAttribute>();
                foreach (var info in attributes)
                {
                    var consumer = bus.Subscribe(info.Exchange, info.RoutingKey, info.Queue);
                    consumer.Received += (model, ea) => ProcessMessage(ea.Body, method);
                }
            }
        }

        private void ProcessMessage(byte[] message, MethodInfo method)
        {
            var paramType = method.GetParameters().First().ParameterType;
            var obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message), paramType);

            var service = resolve(method.DeclaringType);
            method.Invoke(service, new[] { obj });
        }
    }
}