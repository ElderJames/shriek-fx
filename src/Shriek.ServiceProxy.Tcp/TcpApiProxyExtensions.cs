using System;
using System.Collections.Generic;
using System.Net.Sockets;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using TcpServiceCore.Client;
using TcpServiceCore.Communication;
using TcpServiceCore.Dispatching;

namespace Shriek.ServiceProxy.Tcp
{
    public static class TcpApiProxyExtensions
    {
        public static IShriekBuilder AddServiceProxy(this IShriekBuilder builder, Action<TcpOption> optionAction)
        {
            TcpOption option = new TcpOption();

            optionAction(option);

            foreach (var o in option.ServiceOptions)
            {
                var channelManager = new ChannelManager(o.Contract, o.Config);

                builder.Services.AddDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<InnerProxy>(o.socket == null
                        ? new object[] { o.server, o.port, channelManager, o.open }
                        : new object[] { o.socket, channelManager, o.open });
                });
            }

            return builder;
        }
    }

    public class TcpOption
    {
        public ICollection<ServiceOption> ServiceOptions = new List<ServiceOption>();

        public void AddTcpProxy<TService>(Socket socket, string server, int port, ChannelConfig config, bool open)
        {
            ServiceOptions.Add(new ServiceOption()
            {
                ImplementingType = typeof(InnerProxy),
                Contract = ContractDescription<TService>.Create(),
                ProxyType = typeof(TService),
                socket = socket,
                Config = config,
                server = server,
                port = port,
                open = open
            });
        }

        public void AddTcpProxy<TService>(string server, int port, ChannelConfig config, bool open)
        {
            AddTcpProxy<TService>(null, server, port, config, open);
        }
    }

    public class ServiceOption
    {
        public ContractDescription Contract { get; set; }

        public Type ImplementingType { get; set; }

        public Type ProxyType { get; set; }

        public Socket socket { get; set; }

        public ChannelConfig Config { get; set; }

        public string server { get; set; }

        public int port { get; set; }

        public bool open { get; set; }
    }
}