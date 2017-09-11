using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.Messages
{
    public class InProcessMessagePublisher : IMessagePublisher
    {
        private Task queueTask;

        private IServiceProvider container;

        public InProcessMessagePublisher(IServiceProvider container)
        {
            this.container = container;
        }

        public void Dispose()
        {
            if (queueTask != null && queueTask.IsCompleted)
                queueTask?.Dispose();
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            var subscribers = container.GetServices(typeof(IMessageSubscriber<>).MakeGenericType(message.GetType()));
            if (!subscribers.Any()) return;

            foreach (var sub in subscribers)
            {
                ((dynamic)sub).Execute((dynamic)message);
            }
        }
    }
}