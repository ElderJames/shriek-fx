using RabbitMQ.Client;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string HostName { get; set; }

        public string ExchangeName { get; set; }

        public string QueueName { get; set; }

        public string RouteKey { get; set; }

        internal IModel Channel { get; set; }

        internal IMessagePublisher MessagePublisher { get; set; }
    }
}