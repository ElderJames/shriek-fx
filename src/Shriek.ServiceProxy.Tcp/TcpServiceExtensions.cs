using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Tcp.Client;
using Shriek.ServiceProxy.Tcp.Communication;
using Shriek.ServiceProxy.Tcp.Dispatching;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Shriek.ServiceProxy.Tcp
{
    public static class TcpServiceExtensions
    {
        public static void UseTcpServiceProxy(this ShriekOptionBuilder builder, Action<TcpServiceProxyOptions> optionAction)
        {
            var option = new TcpServiceProxyOptions();

            optionAction(option);

            foreach (var o in option.TcpProxys)
            {
                builder.Services.AddDynamicProxy(config =>
                {
                    var channelManager = new ChannelManager(o.Contract, o.Config);

                    config.Interceptors.AddTyped(typeof(TcpServiceClient),
                        o.socket == null
                            ? new object[] { o.Server, o.Port, channelManager, o.Open }
                            : new object[] { o.socket, channelManager, o.Open },
                        x => o.InterfaceType.IsAssignableFrom(x.DeclaringType));
                });
            }
        }

        public class TcpServiceProxyOptions
        {
            public ICollection<TcpProxy> TcpProxys { get; } = new List<TcpProxy>();

            public void AddTcpProxy<TService>(Socket socket, ChannelConfig config, bool open = true)
            {
                CreateProxy<TService>(socket, null, -1, config, open);
            }

            public void AddTcpProxy<TService>(string server, int port, ChannelConfig config, bool open = true)
            {
                CreateProxy<TService>(null, server, port, config, open);
            }

            private void CreateProxy<TService>(Socket socket, string server, int port, ChannelConfig config, bool open)
            {
                TcpProxys.Add(new TcpProxy()
                {
                    socket = socket,
                    Config = config,
                    Open = open,
                    Server = server,
                    Port = port,
                    Contract = ContractDescription<TService>.Create(),
                    InterfaceType = typeof(TService)
                });
            }
        }

        public class TcpProxy
        {
            public Socket socket { get; set; }

            public string Server { get; set; }

            public int Port { get; set; }

            public ChannelConfig Config { get; set; }

            public bool Open { get; set; } = true;

            public ContractDescription Contract { get; set; }

            public Type InterfaceType { get; set; }
        }
    }
}