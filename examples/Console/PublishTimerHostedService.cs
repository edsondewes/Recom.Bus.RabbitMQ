using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Recom.Bus.RabbitMQ;

namespace ConsoleApp
{
    public class PublishTimerHostedService : IHostedService
    {
        private readonly IBus bus;
        private readonly System.Timers.Timer timer;

        public PublishTimerHostedService(IBus bus)
        {
            this.bus = bus;
            timer = new System.Timers.Timer(3000);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer.Elapsed += (sender, e) => bus.Publish(new SomeData { Date = DateTime.Now }, "TestExchange", "Key");
            timer.Elapsed += (sender, e) => bus.Publish(new SomeData { Date = DateTime.Now.AddDays(1) }, "TestExchange", "Key2");
            timer.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Stop();
            return Task.CompletedTask;
        }
    }
}
