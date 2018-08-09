using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Recom.Bus.RabbitMQ
{
    internal class DefaultConsumerManager : IConsumerManager
    {
        public EventingBasicConsumer CreateEventingConsumer(IModel model)
        {
            return new EventingBasicConsumer(model);
        }
    }
}
