using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using System;
using System.Linq;

namespace Shriek.ServiceProxy.Http
{
    public static class WebApiProxyExtensions
    {
        public static void AddWebApiProxy(this ShriekOptionBuilder builder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            foreach (var o in option.WebApiProxies)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

                builder.Services.AddDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped(typeof(HttpApiClient), new object[] { o.BaseUrl }, method => types.Any(t => t.IsAssignableFrom(method.DeclaringType)));
                });
            }
        }
    }
}