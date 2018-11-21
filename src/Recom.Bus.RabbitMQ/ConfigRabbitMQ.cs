namespace Recom.Bus.RabbitMQ
{
    public class ConfigRabbitMQ
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public string VirtualHost { get; set; }

        public string DefaultExchange { get; set; } = null;
        public bool DurableQueues { get; set; } = true;
        public bool PersistentDelivery { get; set; } = true;
    }
}
