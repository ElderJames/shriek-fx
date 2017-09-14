using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.TcpClient;

namespace Shriek.ServiceProxy.Tcp
{
    public class TcpActionContext : ApiActionContext
    {
        public TcpMessage RequestMessage { get; set; }

        public TcpMessage ResponseMessage { get; set; }
    }
}