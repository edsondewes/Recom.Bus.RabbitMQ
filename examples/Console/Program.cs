using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recom.Bus.RabbitMQ;

namespace ConsoleAutofac
{
    class Program
    {
        public static async Task Main()
        {
            var builder = CreateHostBuilder();
            var host = builder.Build();

            using (host)
            {
                await host.StartAsync();

                var bus = host.Services.GetRequiredService<IBus>();
                var timer = new System.Timers.Timer(3000);

                bus.CreateExchange("TestExchange");

                var subscriber = new AutoSubscriber(bus, host.Services.GetService);
                subscriber.Subscribe(Assembly.GetExecutingAssembly());

                timer.Elapsed += (sender, e) => bus.Publish(new SomeData { Date = DateTime.Now }, "TestExchange", "Key");
                timer.Elapsed += (sender, e) => bus.Publish(new SomeData { Date = DateTime.Now.AddDays(1) }, "TestExchange", "Key2");
                timer.Start();

                await host.WaitForShutdownAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .UseEnvironment("Development")
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(new ConfigRabbitMQ { Host = hostContext.Configuration.GetValue<string>("RabbitMQ") });

                services.AddSingleton<IBus, EventManager>();
                services.AddSingleton<TestSubscription>();
            });
    }
}
