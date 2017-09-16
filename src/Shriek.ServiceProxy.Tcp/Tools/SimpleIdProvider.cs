using System;
using System.Collections.Concurrent;

namespace Shriek.ServiceProxy.Tcp.Tools
{
    internal class SimpleIdProvider : IMsgIdProvider
    {
        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

        public SimpleIdProvider()
        {
            for (int i = 0; i < 1000; i++)
            {
                queue.Enqueue(i.ToString());
            }
        }

        public string NewId()
        {
            string next;
            if (queue.TryDequeue(out next) == false)
            {
                next = NewId();
            }
            else
            {
                queue.Enqueue(next);
            }
            return next;
        }
    }
}