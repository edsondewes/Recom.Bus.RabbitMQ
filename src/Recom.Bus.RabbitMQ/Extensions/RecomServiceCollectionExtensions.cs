using System;
using Microsoft.Extensions.Configuration;
using Recom.Bus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RecomServiceCollectionExtensions
    {
        public static IServiceCollection AddRecomRabbitMQ(this IServiceCollection collection, ConfigRabbitMQ config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            collection.AddSingleton(config);
            collection.AddSingleton<IBus, EventManager>();
            return collection;
        }

        public static IServiceCollection AddRecomRabbitMQ(this IServiceCollection collection, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var config = configuration.GetSection("RabbitMQ").Get<ConfigRabbitMQ>();
            return collection.AddRecomRabbitMQ(config);
        }
    }
}