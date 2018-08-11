using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Recom.Bus.RabbitMQ;

namespace Tests.Recom.Bus.RabbitMQ.Assets
{
    public class Service1 : IMessageSubscriber
    {
        [RabbitSubscription(
            exchange: "TestExchange",
            queue: "Service1Queue",
            routingKeys: new[] { "Service.Event", "Service.OtherEvent" })]
        [RabbitSubscription(
            exchange: "OtherExchange",
            queue: "Service1Queue2",
            routingKeys: new[] { "Other.Event" })]
        public async Task Execute(Message msg)
        {
            Debug.WriteLine($"[{DateTime.Now}]: {msg.Text}");
            await Task.Delay(1);
        }

        public async Task OtherMehod(Message msg)
        {
            await Task.Delay(1);
        }
    }
}
