using Newtonsoft.Json;
using RabbitMQ.Client;
using Shriek.Events;
using System;
using System.Text;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly IModel channel;
        private readonly EventBusRabbitMqOptions options;

        public RabbitMqEventBus(IServiceProvider serviceProvider, EventBusRabbitMqOptions options)
        {
            this.channel = options.Channel;
            this.options = options;

            options.ServiceProvider = serviceProvider;
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