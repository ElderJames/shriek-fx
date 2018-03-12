using System;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IModel channel;
        private readonly RabbitMqOptions options;

        public RabbitMqMessagePublisher(IServiceProvider serviceProvider, RabbitMqOptions options)
        {
            options.ServiceProvider = serviceProvider;

            this.channel = options.Publisher;
            this.options = options;
        }

        public void Dispose()
        {
            //channel.Dispose();
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            if (message == null)
                return;

            var consumer = new EventingBasicConsumer(channel);

            var properties = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = channel.QueueDeclare().QueueName;
            var respQueue = new BlockingCollection<string>();

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            var msg = JsonConvert.SerializeObject(message);
            var sendBytes = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(options.ExchangeName, options.RouteKey, properties, sendBytes);

            channel.BasicConsume(consumer, properties.ReplyTo, true);

            var reply = respQueue.Take();

            if (!string.IsNullOrEmpty(reply))
                throw new Exception(reply);
        }
    }
}