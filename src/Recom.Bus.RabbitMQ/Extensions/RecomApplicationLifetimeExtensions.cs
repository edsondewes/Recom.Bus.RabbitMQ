using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Recom.Bus.RabbitMQ;

namespace Microsoft.AspNetCore.Hosting
{
    public static class RecomApplicationLifetimeExtensions
    {
        public static IApplicationBuilder UseRecomRabbitMQ(this IApplicationBuilder app, Action<IBus> onStart = null)
        {
            var serviceProvider = app.ApplicationServices;

            var appLifetime = serviceProvider.GetService<IApplicationLifetime>();
            appLifetime.ApplicationStarted.Register(() =>
            {
                var bus = serviceProvider.GetService<IBus>();
                var subscriber = new AutoSubscriber(bus, serviceProvider);
                subscriber.Subscribe(Assembly.GetEntryAssembly());

                if (onStart != null)
                {
                    onStart(bus);
                }
            });

            appLifetime.ApplicationStopped.Register(() =>
            {
                serviceProvider.GetService<IBus>().Dispose();
            });

            return app;
        }
    }
}