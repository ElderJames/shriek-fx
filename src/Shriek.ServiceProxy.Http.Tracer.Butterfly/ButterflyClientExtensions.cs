using System;
using System.Net.Http;
using Butterfly.Client.AspNetCore;
using Butterfly.Client.Tracing;
using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Http.Tracer.Butterfly
{
    public static class ButterflyClientExtensions
    {
        public static IServiceCollection AddButterflyForShriek(this IServiceCollection services, Action<ButterflyOptions> optionAction)
        {
            services.AddButterfly(optionAction);

            services.AddSingleton<IHttpClient>(p =>
            {
                var handler = p.GetService<HttpTracingHandler>();
                return new HttpClientAdapter(new HttpClient(handler));
            });

            return services;
        }
    }
}