using System.Threading.Tasks;

namespace Tests.Recom.Bus.RabbitMQ.Assets
{
    public class Service3Rabbit
    {
        public async Task Method(string text)
        {
            await Task.Delay(1);
        }
    }
}
