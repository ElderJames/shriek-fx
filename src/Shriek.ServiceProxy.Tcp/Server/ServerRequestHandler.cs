using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TcpServiceCore.Buffering;
using TcpServiceCore.Communication;
using TcpServiceCore.Dispatching;
using TcpServiceCore.Protocol;
using TcpServiceCore.Tools;

namespace TcpServiceCore.Server
{
    class ServerRequestHandler<T> : AsyncStreamHandler where T : new()
    {
        IInstanceContextFactory<T> instanceContextFactory;

        Dictionary<string, ChannelManager> channelManagers = new Dictionary<string, ChannelManager>();

        ChannelManager channelManager;

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
                    if (this.channelManager == null)
                    {
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
                    throw;
                }
            }
        }

        async Task DoHandleRequest(Message request)
        {
            var context = this.instanceContextFactory.Create(this.Socket);
            var response = await context.HandleRequest(this.Socket, this.channelManager, request);
            if (response != null)
                await this.WriteMessage(response);
        }
    }
}
