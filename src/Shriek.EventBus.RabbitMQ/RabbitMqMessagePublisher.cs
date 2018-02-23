using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IModel channel;
        private readonly RabbitMqOptions options;

        public RabbitMqMessagePublisher(RabbitMqOptions options)
        {
            this.channel = options.Channel;
            this.options = options;
        }

        public void Dispose()
        {
            channel.Dispose();
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            if (message == null)
                return;

            var msg = JsonConvert.SerializeObject(message);
            var sendBytes = Encoding.UTF8.GetBytes(msg);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(options.ExchangeName, options.RouteKey, properties, sendBytes);
        }
    }
}