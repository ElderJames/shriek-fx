using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.WebApi.Proxy
{
    public static class WebApiProxyExtensions
    {
        public static IShriekBuilder AddWebApiProxy(this IShriekBuilder builder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            var webClient = new HttpApiClient();
            foreach (var o in option.WebApiProxies)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());
                foreach (var t in types)
                {
                    builder.Services.AddScoped(t, x => webClient.GetHttpApi(t, o.BaseUrl));
                }
            }

            return builder;
        }
    }
}