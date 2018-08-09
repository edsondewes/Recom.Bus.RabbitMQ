using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Recom.Bus.RabbitMQ
{
    internal interface IConsumerManager
    {
        EventingBasicConsumer CreateEventingConsumer(IModel model);
    }
}
