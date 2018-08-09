using Recom.Bus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RecomServiceCollectionExtensions
    {
        public static IServiceCollection AddRecomRabbitMQ(this IServiceCollection collection)
        {
            collection.AddSingleton<RabbitConnection>();
            collection.AddSingleton(typeof(IBus<>), typeof(EventManager<>));
            collection.AddHostedService<HostedServiceRabbitMQ>();
            return collection;
        }
    }
}