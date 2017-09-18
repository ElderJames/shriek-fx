using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Tcp.Buffering;
using Shriek.ServiceProxy.Tcp.Dispatching;
using Shriek.ServiceProxy.Tcp.Protocol;
using Shriek.ServiceProxy.Tcp.Tools;

namespace Shriek.ServiceProxy.Tcp.Server
{
    internal class ServerRequestHandler<T> : AsyncStreamHandler where T : new()
    {
        private readonly IInstanceContextFactory<T> instanceContextFactory;

        private readonly Dictionary<string, ChannelManager> channelManagers;

        private ChannelManager channelManager;

        public ServerRequestHandler(Socket socket,
            Dictionary<string, ChannelManager> channelManagers,
            IInstanceContextFactory<T> instanceContextFactory)
            : base(socket, new DummyBufferManager())
        {
            this.instanceContextFactory = instanceContextFactory;
            this.channelManagers = channelManagers;
        }

        protected override async Task _OnRequestReceived(Message request)
        {
            if (this.channelManager != null)
            {
                await DoHandleRequest(request);
            }
            else
            {
                var contract = request.Contract;
                if (string.IsNullOrEmpty(contract))
                    throw new Exception($"Wrong socket initialization, Request.Contract should not be null or empty");

                try
                {
                    this.channelManager = this.channelManagers[contract];

                    this.Socket.Configure(this.channelManager.Config);

                    this.BufferManager = this.channelManager.BufferManager;

                    await DoHandleRequest(request);
                }
                catch
                {
                    if (this.channelManager != null) throw;
                    var error = $"Wrong socket initialization, contract {contract} is missing";
                    try
                    {
                        var response = new Message(MessageType.Error, request.Id, error);
                        await this.WriteMessage(response);
                    }
                    catch
                    {
                    }
                    throw new Exception(error);
                }
            }
        }

        private async Task DoHandleRequest(Message request)
        {
            var context = this.instanceContextFactory.Create(this.Socket);
            var response = await context.HandleRequest(this.Socket, this.channelManager, request);
            if (response != null)
                await this.WriteMessage(response);
        }
    }
}