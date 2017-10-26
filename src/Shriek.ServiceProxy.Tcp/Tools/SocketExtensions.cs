using System.Net.Sockets;
using Shriek.ServiceProxy.Tcp.Communication;

namespace Shriek.ServiceProxy.Tcp.Tools
{
    public static class SocketExtensions
    {
        public static void Configure(this TcpClient client, ChannelConfig channelConfig)
        {
            client.Client.Configure(channelConfig);
        }

        public static void Configure(this Socket socket, ChannelConfig channelConfig)
        {
            socket.ReceiveTimeout = (int)channelConfig.ReceiveTimeout.TotalMilliseconds;
            socket.SendTimeout = (int)channelConfig.SendTimeout.TotalMilliseconds;
            socket.NoDelay = channelConfig.NoDelay;
            socket.ReceiveBufferSize = channelConfig.ReceiveBufferSize;
            socket.SendBufferSize = channelConfig.SendBufferSize;
            socket.LingerState.Enabled = false;
        }
    }
}