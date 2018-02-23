using System;
using RabbitMQ.Client;
using _ExchangeType = RabbitMQ.Client.ExchangeType;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string HostName { get; set; }

        public string ExchangeType { get; set; } = _ExchangeType.Topic;

        public string ExchangeName { get; set; }

        public string QueueName { get; set; }

        public string RouteKey { get; set; }

        internal IModel Channel { get; set; }

        internal IServiceProvider ServiceProvider { get; set; }
    }
}