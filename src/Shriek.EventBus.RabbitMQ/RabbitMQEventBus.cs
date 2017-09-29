using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shriek.Events;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private IModel channel;
        private EventBusRabbitMqOptions options;

        public RabbitMqEventBus(IMessagePublisher messagePublisher, EventBusRabbitMqOptions options)
        {
            this.channel = options.Channel;
            this.options = options;

            options.MessagePublisher = messagePublisher;
        }

        public void Dispose()
        {
            channel.Dispose();
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null)
                return;

            var msg = JsonConvert.SerializeObject(@event);
            var sendBytes = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(options.ExchangeName, options.RouteKey, null, sendBytes);
        }
    }
}