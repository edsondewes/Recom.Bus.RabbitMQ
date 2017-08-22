using System;
using Recom.Bus.RabbitMQ;

namespace WebApi
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription(Exchange = "TestExchange1", RoutingKey = "WebKey")]
        [RabbitSubscription(Exchange = "TestExchange2", RoutingKey = "WebKey")]
        public void Callback(SomeData data) => Console.WriteLine($"[{DateTime.Now}] {data.Text}");
    }
}