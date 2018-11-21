using System;
using System.Threading;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Recom.Bus.RabbitMQ
{
    public class RabbitConnection : IDisposable
    {
        private ConfigRabbitMQ _config;

        public IConnection Connection { get; }
        public IModel Model { get; }

        public RabbitConnection(IOptions<ConfigRabbitMQ> config)
        {
            _config = config.Value;

            Connection = TryCreateConnection();
            Model = Connection.CreateModel();
        }

        public void Dispose()
        {
            Model.Dispose();
            Connection.Dispose();
        }

        private IConnection TryCreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.Host,
                Password = _config.Password ?? ConnectionFactory.DefaultPass,
                UserName = _config.User ?? ConnectionFactory.DefaultUser,
                VirtualHost = _config.VirtualHost ?? ConnectionFactory.DefaultVHost
            };

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
