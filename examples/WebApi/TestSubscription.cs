using System;
using Recom.Bus.RabbitMQ;

namespace WebApi
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription(exchange: "TestExchange1", routingKeys: "WebKey")]
        [RabbitSubscription(exchange: "TestExchange2", routingKeys: "WebKey")]
        public void Callback(SomeData data)
        {
            Console.WriteLine($"[{DateTime.Now}] {data.Text}");
        }
    }
}