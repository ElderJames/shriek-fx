using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Tcp.Communication;
using Shriek.ServiceProxy.Tcp.Dispatching;

namespace Shriek.ServiceProxy.Tcp.Server
{
    public class ServiceHost<T> : CommunicationObject where T : new()
    {
        private readonly Type type;
        private readonly TcpListener listener;

        public event Action<T> ServiceInstantiated;

        private readonly IInstanceContextFactory<T> instanceContextFactory = new InstanceContextFactory<T>();

        private readonly Dictionary<string, ChannelManager> channelManagers = new Dictionary<string, ChannelManager>();

        public ServiceHost(int port)
        {
            this.type = typeof(T);

            var endpoint = new IPEndPoint(IPAddress.Any, port);
            this.listener = new TcpListener(endpoint);
        }

        public void AddContract<TContract>(ChannelConfig config)
        {
            var contract = ContractDescription<TContract>.Create();

            contract.ValidateImplementation(this.type.GetTypeInfo());

            var cm = new ChannelManager(contract, config);

            this.channelManagers.Add(contract.ContractName, cm);
        }

        protected override Task OnOpen()
        {
            this.instanceContextFactory.ServiceInstantiated += this.ServiceInstantiated;
            this.listener.Start(3000);
            Task.Run(async () =>
            {
                while (this.State == CommunicationState.Opened)
                {
                    try
                    {
                        var socket = await this.listener.AcceptSocketAsync();
                        var handler = new ServerRequestHandler<T>(socket, this.channelManagers, this.instanceContextFactory);
                        await handler.Open();
                    }
                    catch (Exception ex)
                    {
                        Global.ExceptionHandler?.LogException(ex);
                    }
                }
            });
            return Task.CompletedTask;
        }

        protected override Task OnClose()
        {
            this.listener.Stop();
            return Task.CompletedTask;
        }
    }
}