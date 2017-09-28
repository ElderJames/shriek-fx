using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        public RabbitMqMessagePublisher()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            throw new NotImplementedException();
        }
    }
}