using System;
using System.Threading.Tasks;
using Recom.Bus.RabbitMQ;

namespace ConsoleAutofac
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription("TestExchange", "ExplicitQueueName", "Key")]
        public async Task AsyncMethod(SomeData data)
        {
            await Task.Delay(10);
            Console.WriteLine($"Async: {data.Date}");
        }

        [RabbitSubscription(
            exchange: "TestExchange",
            routingKeys: new[] { "Key", "Key2" })]
        public void SyncMethod(SomeData data)
        {
            Console.WriteLine($"Sync: {data.Date}");
        }
    }
}