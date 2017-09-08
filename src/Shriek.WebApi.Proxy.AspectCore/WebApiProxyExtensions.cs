using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Shriek.WebApi.Proxy.AspectCore
{
    public static class WebApiProxyExtensions
    {
        public static void AddWebApiProxy(this IServiceCollection services, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            foreach (var o in option.WebApiProxies)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());
                foreach (var t in types)
                {
                    services.AddScoped(t, x => ServiceAdapter.GetHttpApi(t, o.BaseUrl));
                }
            }
        }
    }
}