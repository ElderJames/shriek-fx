using Shriek.ServiceProxy.Tcp.Buffering;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Protocol
{
    internal class AsyncStreamHandler : StreamHandler
    {
        public AsyncStreamHandler(Socket socket, IBufferManager bufferManager)
            : base(socket, bufferManager)
        {
        }

        protected override async Task OnOpen()
        {
            await base.OnOpen();
        }

        protected override Task<int> _Read(ArraySegment<byte> buffer)
        {
            return this.Socket.ReceiveAsync(buffer, SocketFlags.None);
        }

        protected override Task _Write(ArraySegment<byte> buffer)
        {
            return this.Socket.SendAsync(buffer, SocketFlags.None);
        }
    }
}