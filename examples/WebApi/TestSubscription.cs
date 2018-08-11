using System;
using Recom.Bus.RabbitMQ;

namespace WebApi
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription(exchange: "TestExchange1", routingKeys: new string[] { "WebKey" })]
        [RabbitSubscription(exchange: "TestExchange2", routingKeys: new string[] { "WebKey" })]
        public void Callback(SomeData data)
        {
            Console.WriteLine($"[{DateTime.Now}] {data.Text}");
        }
    }
}