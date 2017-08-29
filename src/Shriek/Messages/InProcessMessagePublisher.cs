using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shriek.Messages
{
    public class InProcessMessagePublisher : IMessagePublisher
    {
        private ConcurrentQueue<Message> messageQueue;
        private Task queueTask;

        private IServiceProvider container;

        public InProcessMessagePublisher(IServiceProvider container)
        {
            this.container = container;
            messageQueue = new ConcurrentQueue<Message>();
        }

        public void Dispose()
        {
            queueTask.Dispose();
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            messageQueue.Enqueue(message);
        }

        public void Subscriber(Action<Message> handle)
        {
            queueTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);

                    while (messageQueue.Any())
                    {
                        if (messageQueue.TryDequeue(out Message message))
                        {
                            handle(message);
                        }
                    }
                }
            });
        }
    }
}