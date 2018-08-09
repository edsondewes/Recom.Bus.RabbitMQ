using System;
using System.Threading;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Recom.Bus.RabbitMQ
{
    public class RabbitConnection : IDisposable
    {
        public IConnection Connection { get; }
        public IModel Model { get; }

        public RabbitConnection(IOptions<ConfigRabbitMQ> config)
        {
            Connection = TryCreateConnection(config.Value.Host);
            Model = Connection.CreateModel();
        }

        public void Dispose()
        {
            Model.Dispose();
            Connection.Dispose();
        }

        private static IConnection TryCreateConnection(string host)
        {
            var factory = new ConnectionFactory { HostName = host };
            IConnection connection = null;
            do
            {
                try
                {
                    connection = factory.CreateConnection();
                }
                catch (BrokerUnreachableException)
                {
                    Console.WriteLine($"{DateTime.Now} - Cannot connect to rabbitmq. Retrying in 5 seconds");
                    Thread.Sleep(5000);
                }
            } while (connection == null);

            return connection;
        }
    }
}
