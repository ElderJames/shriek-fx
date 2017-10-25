using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.Tcp.Protocol;

namespace Shriek.ServiceProxy.Tcp
{
    public class TcpActionContext : ApiActionContext
    {
        public Message RequestMessage { get; set; }

        public Message ResponseMessage { get; set; }
    }
}