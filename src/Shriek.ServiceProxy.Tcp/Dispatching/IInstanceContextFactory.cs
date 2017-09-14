using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServiceCore.Dispatching
{
    interface IInstanceContextFactory<T> where T: new()
    {
        event Action<T> ServiceInstantiated;
        InstanceContext<T> Create(Socket socket);
    }
}
