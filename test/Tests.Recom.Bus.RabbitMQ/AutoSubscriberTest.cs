using System;
using System.Reflection;
using System.Text;
using Moq;
using Newtonsoft.Json;
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
            var subscriber = new AutoSubscriber(null, null);
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
            var bus = new Mock<IBus>();
            bus.Setup(b => b.Subscribe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new EventingBasicConsumer(null));

            var serviceProvider = new Mock<IServiceProvider>();

            var subscriber = new AutoSubscriber(bus.Object, serviceProvider.Object);
            subscriber.Subscribe(GetType().GetTypeInfo().Assembly);

            bus.Verify(b => b.Subscribe("TestExchange", "Service.Event", "Service1Queue"), Times.Once);
            bus.Verify(b => b.Subscribe("TestExchange", "Key.*", "Service2Queue"), Times.Once);
        }

        [Fact]
        public void WhenAMessageIsDeliveredShouldCallWithDeserializedMessage()
        {
            var consumer = new EventingBasicConsumer(null);

            var bus = new Mock<IBus>();
            bus.Setup(b => b.Subscribe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(consumer);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(this);

            var subscriber = new AutoSubscriber(bus.Object, serviceProvider.Object);
            subscriber.Subscribe(new[] { GetType().GetMethod("HelperMethod") });

            var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("hello world"));
            consumer.HandleBasicDeliver(string.Empty, 0, false, string.Empty, string.Empty, null, message);

            Assert.True(helperHandled);
        }

        [RabbitSubscription(Exchange = "Exchange", Queue = "Queue", RoutingKey = "Key")]
        public void HelperMethod(string message)
        {
            Assert.Equal("hello world", message);
            helperHandled = true;
        }
    }
}
