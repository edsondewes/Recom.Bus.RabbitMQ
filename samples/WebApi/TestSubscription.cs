using System;
using System.Threading.Tasks;
using Recom.Bus.RabbitMQ;

namespace WebApi
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription(
            Exchange = "TestExchange",
            Queue = "MyQueue3",
            RoutingKey = "WebKey")]
        public void Callback(SomeData data)
        {
            Console.WriteLine($"{DateTime.Now}: {data.Text}");
        }
    }
}