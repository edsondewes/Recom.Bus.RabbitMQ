using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public string Get(string text)
        {
            var data = new SomeData
            {
                Text = text ?? "Nothing"
            };

            bus.Publish(data, "TestExchange", "WebKey");

            return "Message sent. Look at the console";
        }
    }
}
