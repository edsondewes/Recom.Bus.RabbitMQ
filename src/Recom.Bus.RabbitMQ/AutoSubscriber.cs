using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Recom.Bus.RabbitMQ
{
    public class AutoSubscriber
    {
        private readonly Func<Type, object> _resolve;
        private readonly Type MessageSubscriberInterface = typeof(IMessageSubscriber);

        public AutoSubscriber(Func<Type, object> resolve)
        {
            _resolve = resolve;
        }

        public AutoSubscriber(IServiceProvider serviceProvider) : this(serviceProvider.GetService)
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
            var busType = typeof(IBus);
            var subscribe = busType.GetMethod("Subscribe");
            var busImpl = _resolve(busType);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes<RabbitSubscriptionAttribute>();
                foreach (var info in attributes)
                {
                    var paramType = method.GetParameters().First().ParameterType;
                    var genericSubscribe = subscribe.MakeGenericMethod(paramType);

                    var serviceImpl = _resolve(method.DeclaringType);

                    Action<object> callback = (object obj) => method.Invoke(serviceImpl, new object[] { obj });
                    genericSubscribe.Invoke(busImpl, new object[] { info.RoutingKeys, callback, info.Exchange, info.Queue });
                }
            }
        }
    }
}
 