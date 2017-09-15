using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    internal class InstanceContextFactory<T> : IInstanceContextFactory<T> where T : new()
    {
        public event Action<T> ServiceInstantiated;

        //Create instace context, no static so we can have two hosts in one application
        private ConcurrentDictionary<Socket, InstanceContext<T>> contexts =
            new ConcurrentDictionary<Socket, InstanceContext<T>>();

        private object _lock = new object();

        private InstanceContext<T> Singleton;

        InstanceContext<T> IInstanceContextFactory<T>.Create(Socket socket)
        {
            InstanceContext<T> result = null;
            switch (InstanceContext<T>.InstanceContextMode)
            {
                case InstanceContextMode.Single:
                    if (Singleton == null)
                    {
                        lock (_lock)
                        {
                            if (Singleton == null)
                                Singleton = new InstanceContext<T>();
                        }
                    }
                    result = Singleton;
                    break;

                case InstanceContextMode.PerCall:
                    result = new InstanceContext<T>();
                    break;

                case InstanceContextMode.PerSession:
                    result = contexts.AddOrUpdate(socket, new InstanceContext<T>(), (s, d) => d);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
}