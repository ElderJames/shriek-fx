using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.ServiceProxy.Http
{
    public static class WebApiProxyExtensions
    {
        /// <summary>
        /// 使用WebApi动态代理客户端
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionAction">配置</param>
        public static void UseWebApiProxy(this ShriekOptionBuilder builder, Action<WebApiProxyOptions> optionAction)
        {
            builder.Services.AddWebApiProxy(optionAction);
        }

        /// <summary>
        /// 使用WebApi动态代理客户端
        /// </summary>
        /// <param name="service"></param>
        /// <param name="optionAction"></param>
        public static void AddWebApiProxy(this IServiceCollection service, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            var proxyService = new List<Type>();

            var proxyGeneratorBuilder = new ProxyGeneratorBuilder();

            foreach (var o in option.WebApiProxies)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

                proxyGeneratorBuilder.Configure(config =>
                {
                    config.Interceptors.AddTyped(typeof(HttpApiClient), new object[] { o.BaseUrl ?? option.ProxyHost });
                });

                proxyService.AddRange(types);
            }

            foreach (var type in option.RegisteredServices)
            {
                if (type.Value.IsInterface && type.Value.GetMethods()
                        .SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any())
                {
                    proxyGeneratorBuilder.Configure(config =>
                    {
                        config.Interceptors.AddTyped(typeof(HttpApiClient), new object[] { type.Key ?? option.ProxyHost });
                    });

                    proxyService.Add(type.Value);
                }
            }

            var proxyGenerator = proxyGeneratorBuilder.Build();

            foreach (var type in proxyService)
            {
                var proxy = proxyGenerator.CreateInterfaceProxy(type);
                service.AddSingleton(type, x => proxy);
            }
        }
    }
}