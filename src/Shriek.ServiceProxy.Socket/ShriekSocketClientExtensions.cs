using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Shriek.DynamicProxy;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Socket
{
    public static class ShriekSocketClientExtensions
    {
        public static void UseWebApiProxy(this ShriekOptionBuilder builder, Action<WebApiProxyOptions> optionAction)
        {
            builder.Services.AddSocketProxy(optionAction);
        }

        /// <summary>
        /// 使用WebApi动态代理客户端
        /// </summary>
        /// <param name="service"></param>
        /// <param name="optionAction"></param>
        public static IServiceCollection AddSocketProxy(this IServiceCollection service, Action<WebApiProxyOptions> optionAction)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();

            var option = new WebApiProxyOptions();
            optionAction(option);

            foreach (var type in option.RegisteredServices)
            {
                if (type.Value.IsInterface)
                {
                    var proxy = ProxyGenerator.CreateInterfaceProxyWithoutTarget(type.Value, option.EndPoint == null ? new SocketClient(option.ProxyHost) : new SocketClient(option.EndPoint));
                    service.AddSingleton(type.Value, x => proxy);
                }
            }

            return service;
        }
    }
}