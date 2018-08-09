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
        private readonly ConfigRabbitMQSubscription _config;
        private readonly IServiceProvider _provider;

        public HostedServiceRabbitMQ(IOptions<ConfigRabbitMQSubscription> config, IServiceProvider provider)
        {
            _config = config.Value ?? new ConfigRabbitMQSubscription();
            _provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var subscriber = new AutoSubscriber(_provider.GetService);

            var assemblies = _config.SubscriptionAssemblies == null || !_config.SubscriptionAssemblies.Any()
                ? new[] { Assembly.GetEntryAssembly() }
                : _config.SubscriptionAssemblies;

            foreach (var assembly in assemblies)
            {
                subscriber.Subscribe(assembly);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
