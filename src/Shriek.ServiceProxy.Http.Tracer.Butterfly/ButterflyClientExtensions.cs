using System;
using System.Net.Http;
using Butterfly.Client.AspNetCore;
using Butterfly.Client.Tracing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Http.Tracer.Butterfly
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddButterflyForShriek(this IServiceCollection services, Action<ButterflyOptions> optionAction)
        {
            services.AddButterfly(optionAction);

            services.AddSingleton<IServiceTracerProvider, ServiceTracerProvider>();

            services.AddSingleton<IHttpClient>(p =>
            {
                var handler = p.GetService<HttpTracingHandler>();
                return new HttpClientAdapter(new HttpClient(handler));
            });

            return services;
        }

        public static IServiceCollection AddButterflyForShriekConsole(this IServiceCollection services, Action<ButterflyOptions> optionAction)
        {
            var option = new ButterflyOptions();
            optionAction(option);

            services.AddButterfly(optionAction);

            services.AddSingleton<ILoggerFactory>(new LoggerFactory());
            services.AddSingleton<IOptions<ButterflyOptions>>(new OptionsWrapper<ButterflyOptions>(option));
            services.AddSingleton<IServiceTracerProvider, ConsoleServiceTracerProvider>();

            services.AddSingleton<IHttpClient>(p =>
            {
                var handler = p.GetService<HttpTracingHandler>();
                return new HttpClientAdapter(new HttpClient(handler));
            });

            return services;
        }
    }
}