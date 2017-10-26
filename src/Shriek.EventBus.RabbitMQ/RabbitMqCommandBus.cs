using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shriek.Commands;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqCommandBus : ICommandBus, IDisposable
    {
        private readonly IModel channel;
        private readonly CommandBusRabbitMqOptions options;

        public RabbitMqCommandBus(IMessagePublisher messagePublisher, CommandBusRabbitMqOptions options)
        {
            this.channel = options.Channel;
            this.options = options;

            options.MessagePublisher = messagePublisher;
        }

        public void Dispose()
        {
            channel.Dispose();
        }

        public void Send<TCommand>(TCommand command) where TCommand : Command
        {
            if (command == null)
                return;

            var msg = JsonConvert.SerializeObject(command);
            var sendBytes = Encoding.UTF8.GetBytes(msg);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(options.ExchangeName, options.RouteKey, properties, sendBytes);
        }
    }
}