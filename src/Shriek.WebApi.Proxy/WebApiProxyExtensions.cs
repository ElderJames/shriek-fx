using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.WebApi.Proxy
{
    public static class WebApiProxyExtensions
    {
        public static void AddWebApiProxy(this IServiceCollection services, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            var webClient = new HttpApiClient();
            foreach (var o in option.ProxyOptions)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x => x.IsInterface);
                foreach (var t in types)
                {
                    services.AddScoped(t, x => webClient.GetHttpApi(t, o.BaseUrl));
                }
            }
        }
    }
}