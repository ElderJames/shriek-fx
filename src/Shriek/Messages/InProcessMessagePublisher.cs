using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.Messages
{
    public class InProcessMessagePublisher : IMessagePublisher
    {
        private readonly IServiceProvider container;

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