using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recom.Bus.RabbitMQ;

namespace ConsoleApp
{
    class Program
    {
        public static async Task Main()
        {
            var builder = CreateHostBuilder();
            await builder.RunConsoleAsync();
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
                services.Configure<ConfigRabbitMQ>(hostContext.Configuration.GetSection("RabbitMQ"));
                services.AddRecomRabbitMQ();
                services.AddSingleton<TestSubscription>();

                services.AddHostedService<PublishTimerHostedService>();
            });
    }
}
