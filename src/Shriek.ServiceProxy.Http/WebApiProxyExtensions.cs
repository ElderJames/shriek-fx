using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using System;
using System.Linq;
using System.Net.Http;
using Shriek.Reflection.DynamicProxy;

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
        public static IServiceCollection AddWebApiProxy(this IServiceCollection service, Action<WebApiProxyOptions> optionAction)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();

            var option = new WebApiProxyOptions();
            optionAction(option);

            foreach (var o in option.WebApiProxies)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

                foreach (var type in types)
                {
                    service.AddSingleton(type, x =>
                    {
                        var httpclient = x.GetService<IHttpClient>()
                                         ?? new HttpClientAdapter(new HttpClient(new HttpClientHandler
                                         {
                                             UseProxy = false,
                                         })
                                         {
                                             Timeout = TimeSpan.FromSeconds(10)
                                         });

                        return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, new HttpApiClient(httpclient, o.BaseUrl ?? option.ProxyHost));
                    });
                }
            }

            foreach (var type in option.RegisteredServices)
            {
                if (type.Value.IsInterface)
                {
                    service.AddSingleton(type.Value, x =>
                    {
                        var httpclient = x.GetService<IHttpClient>()
                                         ?? new HttpClientAdapter(new HttpClient(new HttpClientHandler
                                         {
                                             UseProxy = false,
                                         })
                                         {
                                             Timeout = TimeSpan.FromSeconds(10)
                                         });

                        return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type.Value, new HttpApiClient(httpclient, type.Key ?? option.ProxyHost));
                    });
                }
            }

            return service;
        }
    }
}