using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Recom.Bus.RabbitMQ;

namespace Tests.Recom.Bus.RabbitMQ.Assets
{
    public class Service1 : IMessageSubscriber
    {
        [RabbitSubscription(Exchange = "TestExchange", Queue = "Service1Queue", RoutingKey = "Service.Event")]
        [RabbitSubscription(Exchange = "OtherExchange", Queue = "Service1Queue2", RoutingKey = "Other.Event")]
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
