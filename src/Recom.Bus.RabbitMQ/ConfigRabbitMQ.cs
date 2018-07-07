namespace Recom.Bus.RabbitMQ
{
    public class ConfigRabbitMQ
    {
        public string Host { get; set; }

        public bool DurableQueues { get; set; } = true;
        public bool PersistentDelivery { get; set; } = true;
    }
}
