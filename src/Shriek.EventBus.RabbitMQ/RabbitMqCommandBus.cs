using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shriek.Commands;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqCommandBus : ICommandBus, IDisposable
    {
        private IModel channel;
        private CommandBusRabbitMqOptions options;

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

            channel.BasicPublish(options.ExchangeName, options.RouteKey, null, sendBytes);
        }
    }
}