using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Tcp.Protocol;

namespace Shriek.ServiceProxy.Tcp
{
    public class TcpActionContext : ApiActionContext
    {
        public Message RequestMessage { get; set; }

        public Message ResponseMessage { get; set; }
    }
}