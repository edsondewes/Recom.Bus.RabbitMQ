using System;
using Recom.Bus.RabbitMQ;

namespace WebApi
{
    public class TestSubscription : IMessageSubscriber
    {
        [RabbitSubscription("TestExchange1", "WebKey")]
        [RabbitSubscription("TestExchange2", "WebKey")]
        public void Callback(SomeData data) => Console.WriteLine($"[{DateTime.Now}] {data.Text}");
    }
}