using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http.Server.Internal;
using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Shriek.ServiceProxy.Http.Server.RouteAnalyzer;
using Shriek.ServiceProxy.Http.Server.RouteAnalyzer.Impl;

namespace Shriek.ServiceProxy.Http.Server
{
    public static class ShriekServerExtensions
    {
        public static IServiceCollection AddWebApiProxyServer(this IServiceCollection services, Action<WebApiProxyOptions> optionAction)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();
            services.AddMvcCore().AddWebApiProxyServer(optionAction);
            return services;
        }

        public static IMvcBuilder AddWebApiProxyServer(this IMvcBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();
            mvcBuilder.Services.AddMvcCore().AddWebApiProxyServer(optionAction);
            return mvcBuilder;
        }

        public static IMvcBuilder AddWebApiProxyServer(this IMvcBuilder mvcBuilder)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();
            mvcBuilder.Services.AddMvcCore().AddWebApiProxyServer();
            return mvcBuilder;
        }

        public static IMvcCoreBuilder AddWebApiProxyServer(this IMvcCoreBuilder mvcBuilder)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();
            var webApiProxyTypes = AppDomain.CurrentDomain.GetAllTypes()
                .Where(x => !x.IsAbstract && typeof(WebApiProxy).IsAssignableFrom(x));

            mvcBuilder.AddWebApiProxyServer(opt =>
            {
                foreach (var type in webApiProxyTypes)
                {
                    opt.AddWebApiProxy(type, "");
                }
            });

            return mvcBuilder;
        }

        public static IMvcCoreBuilder AddWebApiProxyServer(this IMvcCoreBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            foreach (var inf in option.WebApiProxies.SelectMany(x => x.GetType().Assembly.GetTypes().Where(t => t.IsInterface)))
            {
                option.RegisteredServices.Add(new KeyValuePair<string, Type>("", inf));
            }

            return mvcBuilder.ConfigureInterfaces(option);
        }

        private static IMvcCoreBuilder ConfigureInterfaces(this IMvcCoreBuilder mvcBuilder, WebApiProxyOptions options)
        {
            foreach (var t in options.RegisteredServices.Select(x => x.Value))
            {
                mvcBuilder.AddMvcOptions(opt =>
                {
                    opt.Conventions.Add(new ControllerModelConvention(t));
                    opt.Conventions.Add(new ActionModelConvention(t));
                    opt.Conventions.Add(new ParameterModelConvention(t));
                });
            }

            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                var featureProvider = new ServiceControllerFeatureProvider(options.RegisteredServices.Select(x => x.Value));

                manager.FeatureProviders.Remove(manager.FeatureProviders.FirstOrDefault(x => x.GetType() == typeof(ControllerFeatureProvider)));
                manager.FeatureProviders.Add(featureProvider);
            });

            return mvcBuilder;
        }

        public static IServiceCollection AddRouteAnalyzer(this IServiceCollection services)
        {
            services.AddSingleton<IRouteAnalyzer, RouteAnalyzer.Impl.RouteAnalyzer>();
            return services;
        }

        public static IRouteBuilder MapRouteAnalyzer(this IRouteBuilder routes, string routeAnalyzerUrlPath)
        {
            routes.Routes.Add(new Router(routes.DefaultHandler, routeAnalyzerUrlPath));
            return routes;
        }
    }
}