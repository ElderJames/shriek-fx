using System.Net;
using System.Net.Sockets;

namespace Shriek.ServiceProxy.Tcp
{
    public class ConnectedClient
    {
        public IPAddress ServerIp { get; internal set; }
        public TcpClient Client { get; internal set; }
    }
}