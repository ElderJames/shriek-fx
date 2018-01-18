using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using System;
using System.Linq;
using Shriek.DynamicProxy;

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
                    var proxy = ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, new HttpApiClient(o.BaseUrl ?? option.ProxyHost));
                    service.AddSingleton(type, x => proxy);
                }
            }

            foreach (var type in option.RegisteredServices)
            {
                if (type.Value.IsInterface/* && type.Value.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any()*/)
                {
                    var proxy = ProxyGenerator.CreateInterfaceProxyWithoutTarget(type.Value, new HttpApiClient(type.Key ?? option.ProxyHost));
                    service.AddSingleton(type.Value, x => proxy);
                }
            }

            return service;
        }
    }
}