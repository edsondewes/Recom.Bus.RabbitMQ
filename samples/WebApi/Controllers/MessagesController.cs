using Microsoft.AspNetCore.Mvc;
using Recom.Bus.RabbitMQ;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly IBus bus;

        public MessagesController(IBus bus)
        {
            this.bus = bus;
        }

        [HttpGet("exchange1")]
        public string GetExchange1(string text) => Send(text, "TestExchange1");

        [HttpGet("exchange2")]
        public string GetExchange2(string text) => Send(text, "TestExchange2");

        private string Send(string text, string exchange)
        {
            var data = new SomeData
            {
                Text = $"From {exchange}: {text ?? "Nothing"}"
            };

            bus.Publish(data, exchange, "WebKey");
            return "Message sent. Look at the console";
        }
    }
}
