using System;
using System.Threading.Tasks;
using Recom.Bus.RabbitMQ;

namespace ConsoleAutofac
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription(
            Exchange = "TestExchange",
            Queue = "ExplicitQueueName",
            RoutingKey = "Key")]
        public async Task AsyncMethod(SomeData data)
        {
            await Task.Delay(10);
            Console.WriteLine($"Async: {data.Date}");
        }

        [RabbitSubscription(
            Exchange = "TestExchange",
            RoutingKey = "Key")]
        public void SyncMethod(SomeData data)
        {
            Console.WriteLine($"Sync: {data.Date}");
        }
    }
}