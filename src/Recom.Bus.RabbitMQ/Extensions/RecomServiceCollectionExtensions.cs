using MessagePack;
using Recom.Bus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RecomServiceCollectionExtensions
    {
        public static IServiceCollection AddRecomRabbitMQ(this IServiceCollection collection)
        {
            MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            collection.AddSingleton<RabbitConnection>();
            collection.AddSingleton<IBus, EventManager>();
            collection.AddHostedService<HostedServiceRabbitMQ>();
            return collection;
        }
    }
}