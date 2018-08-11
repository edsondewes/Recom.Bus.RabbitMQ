using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MessagePack;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Recom.Bus.RabbitMQ;
using Tests.Recom.Bus.RabbitMQ.Assets;
using Xunit;

namespace Tests.Recom.Bus.RabbitMQ
{
    public class AutoSubscriberTest
    {
        private bool helperHandled = false;

        [Fact]
        public void ListSubscriptionMethodsShouldFindRabbitServices()
        {
            var subscriber = new AutoSubscriber((type) => null);
            var methods = subscriber.ListSubscriptionMethods(GetType().GetTypeInfo().Assembly);

            Assert.Collection(methods,
                m =>
                {
                    Assert.Equal("Execute", m.Name);
                    Assert.Equal(typeof(Service1), m.DeclaringType);
                },
                m =>
                {
                    Assert.Equal("Method", m.Name);
                    Assert.Equal(typeof(Service2), m.DeclaringType);
                });
        }

        [Fact]
        public void SubscribeShouldRegisterAllServices()
        {
            var bus = new Mock<IBus<Message>>();
            bus.Setup(b => b.Subscribe(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<Action<Message>>()));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(IBus<Message>)))
                .Returns(bus.Object);

            serviceProvider.Setup(s => s.GetService(typeof(Service1))).Returns(new Service1());
            serviceProvider.Setup(s => s.GetService(typeof(Service2))).Returns(new Service2());

            var subscriber = new AutoSubscriber(serviceProvider.Object);
            subscriber.Subscribe(GetType().GetTypeInfo().Assembly);

            bus.Verify(b => b.Subscribe("TestExchange", new[] { "Service.Event", "Service.OtherEvent" }, "Service1Queue", It.IsAny<Action<Message>>()), Times.Once);
            bus.Verify(b => b.Subscribe("OtherExchange", new[] { "Other.Event" }, "Service1Queue2", It.IsAny<Action<Message>>()), Times.Once);
            bus.Verify(b => b.Subscribe("TestExchange", new[] { "Key.*" }, "Service2Queue", It.IsAny<Action<Message>>()), Times.Once);
        }

        [Fact]
        public void WhenAMessageIsDeliveredShouldCallWithDeserializedMessage()
        {
            var consumer = new EventingBasicConsumer(null);

            var rabbitConsumer = new Mock<IConsumerManager>();
            rabbitConsumer.Setup(c => c.CreateEventingConsumer(It.IsAny<IModel>())).Returns(consumer);

            var rabbitChannel = new Mock<IModel>();
            rabbitChannel.Setup(c => c.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                .Returns(new QueueDeclareOk("fakeQueue", 0, 0));

            var rabbitConnection = new Mock<IConnection>();
            rabbitConnection.Setup(c => c.CreateModel()).Returns(rabbitChannel.Object);

            var eventManager = new EventManager<string>(new ConfigRabbitMQ(), rabbitConnection.Object, rabbitConsumer.Object);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(AutoSubscriberTest))).Returns(this);
            serviceProvider.Setup(s => s.GetService(typeof(IBus<string>)))
                .Returns(eventManager);
            
            var subscriber = new AutoSubscriber(serviceProvider.Object);
            subscriber.Subscribe(new[]
            {
                GetType().GetMethod("HelperMethod", BindingFlags.NonPublic | BindingFlags.Instance)
            });

            var message = MessagePackSerializer.Serialize("hello world", MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            consumer.HandleBasicDeliver(string.Empty, 0, false, string.Empty, string.Empty, null, message);

            Assert.True(helperHandled);
        }

        [RabbitSubscription("Exchange", "Queue", "Key")]
        private void HelperMethod(string message)
        {
            Assert.Equal("hello world", message);
            helperHandled = true;
        }
    }
}
