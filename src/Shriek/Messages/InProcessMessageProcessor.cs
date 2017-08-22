using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shriek.Exceptions;

namespace Shriek.Messages
{
    public class InProcessMessageProcessor : IMessageProcessor
    {
        private ConcurrentQueue<Message> messageQueue;
        private ConcurrentQueue<Message> retryQueue;
        private Task queueTask;

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
            messageQueue = new ConcurrentQueue<Message>();
            retryQueue = new ConcurrentQueue<Message>();
            queueTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);

                    while (messageQueue.Any())
                    {
                        if (messageQueue.TryDequeue(out Message message))
                        {
                            try
                            {
                                handle(message);
                            }
                            catch (DomainException ex)
                            {
                            }
                            catch (Exception ex)
                            {
                                //TODO:日志
                                //retryQueue.Enqueue(message);
                                throw;
                            }
                        }
                    }

                    for (var i = 0; i < retryQueue.Count; i++)
                    {
                        try
                        {
                            if (retryQueue.TryPeek(out Message message))
                            {
                                handle(message);
                                retryQueue.TryDequeue(out message);
                            }
                        }
                        catch
                        {
                            //TODO:日志
                        }
                    }
                }
            });
        }
    }
}