using System;
using System.Net.Sockets;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    internal interface IInstanceContextFactory<T> where T : new()
    {
        event Action<T> ServiceInstantiated;

        InstanceContext<T> Create(Socket socket);
    }
}