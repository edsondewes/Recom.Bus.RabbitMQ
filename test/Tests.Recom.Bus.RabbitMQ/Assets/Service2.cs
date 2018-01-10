using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Recom.Bus.RabbitMQ;

namespace Tests.Recom.Bus.RabbitMQ.Assets
{
    public class Service2 : IMessageSubscriber
    {
        [RabbitSubscription("TestExchange", "Service2Queue", "Key.*")]
        public async Task Method(string text)
        {
            Debug.WriteLine($"[{DateTime.Now}]: {text}");
            await Task.Delay(1);
        }

        public async Task OtherMehod(Message msg)
        {
            await Task.Delay(1);
        }
    }
}
