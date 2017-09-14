using TcpServiceCore.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServiceCore.Buffering;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace TcpServiceCore.Protocol
{
    class AsyncStreamHandler : StreamHandler
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
