using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Recom.Bus.RabbitMQ
{
    public class HostedServiceRabbitMQ : IHostedService
    {
        private readonly ConfigRabbitMQSubscription config;
        private readonly IBus bus;
        private readonly IServiceProvider provider;

        public HostedServiceRabbitMQ(IOptions<ConfigRabbitMQSubscription> config, IBus bus, IServiceProvider provider)
        {
            this.config = config.Value ?? new ConfigRabbitMQSubscription();
            this.bus = bus;
            this.provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var subscriber = new AutoSubscriber(bus, provider.GetService);

            var assemblies = config.SubscriptionAssemblies == null || !config.SubscriptionAssemblies.Any()
                ? new[] { Assembly.GetEntryAssembly() }
                : config.SubscriptionAssemblies;

            foreach (var assembly in assemblies)
            {
                subscriber.Subscribe(assembly);
            }

            config.OnStart?.Invoke(bus);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
