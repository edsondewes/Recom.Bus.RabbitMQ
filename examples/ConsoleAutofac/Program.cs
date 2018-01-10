using System;
using System.IO;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Recom.Bus.RabbitMQ;

namespace ConsoleAutofac
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false)
                .Build();

            ConfigureContainer();
            Listen();
        }


        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new ConfigRabbitMQ { Host = Configuration.GetValue<string>("RabbitMQ") }).SingleInstance();
            builder.RegisterType<EventManager>().As<IBus>().SingleInstance();
            builder.RegisterType<TestSubscription>().SingleInstance();
            Container = builder.Build();
        }

        public static void Listen()
        {
            using (var scope = Container.BeginLifetimeScope())
            using (var bus = scope.Resolve<IBus>())
            using (var timer = new System.Timers.Timer(3000))
            {
                bus.CreateExchange("TestExchange");

                var subscriber = new AutoSubscriber(bus, scope.Resolve);
                subscriber.Subscribe(Assembly.GetExecutingAssembly());

                timer.Elapsed += (sender, e) => bus.Publish(new SomeData { Date = DateTime.Now }, "TestExchange", "Key");
                timer.Elapsed += (sender, e) => bus.Publish(new SomeData { Date = DateTime.Now.AddDays(1) }, "TestExchange", "Key2");
                timer.Start();

                Console.WriteLine("Waiting events...");
                Console.Read();
            }
        }
    }
}
