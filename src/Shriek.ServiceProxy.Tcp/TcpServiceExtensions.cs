using System;
using System.Collections.Generic;
using System.Net.Sockets;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Tcp.Client;
using Shriek.ServiceProxy.Tcp.Communication;
using Shriek.ServiceProxy.Tcp.Dispatching;

namespace Shriek.ServiceProxy.Tcp
{
    public static class TcpServiceExtensions
    {
        public static IShriekBuilder AddTcpServiceProxy(this IShriekBuilder builder, Action<TcpServiceProxyOptions> optionAction)
        {
            var option = new TcpServiceProxyOptions();

            optionAction(option);

            foreach (var o in option.TcpProxys)
            {
                builder.Services.AddDynamicProxy(config =>
                {
                    var channelManager = new ChannelManager(o.Contract, o.config);

                    config.Interceptors.AddTyped(typeof(InnerProxy),
                        o.socket == null
                            ? new object[] { o.server, o.port, channelManager, o.open }
                            : new object[] { o.socket, channelManager, o.open },
                        x => o.InterfaceType.IsAssignableFrom(x.DeclaringType));
                });
            }

            return builder;
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
                    config = config,
                    open = open,
                    server = server,
                    port = port,
                    Contract = ContractDescription<TService>.Create(),
                    InterfaceType = typeof(TService)
                });
            }
        }

        public class TcpProxy
        {
            public Socket socket { get; set; }

            public string server { get; set; }

            public int port { get; set; }

            public ChannelConfig config { get; set; }

            public bool open { get; set; } = true;

            public ContractDescription Contract { get; set; }

            public Type InterfaceType { get; set; }
        }
    }
}