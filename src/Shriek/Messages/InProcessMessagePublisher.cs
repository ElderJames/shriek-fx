using Microsoft.Extensions.DependencyInjection;
using System;

namespace Shriek.Messages
{
    public class InProcessMessagePublisher : IMessagePublisher
    {
        private IServiceProvider container;

        public InProcessMessagePublisher(IServiceProvider container)
        {
            this.container = container;
        }

        public void Dispose()
        {
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            var subscribers = container.GetServices(typeof(IMessageSubscriber<>).MakeGenericType(message.GetType()));

            foreach (var sub in subscribers)
            {
                ((dynamic)sub).Execute((dynamic)message);
            }
        }
    }
}