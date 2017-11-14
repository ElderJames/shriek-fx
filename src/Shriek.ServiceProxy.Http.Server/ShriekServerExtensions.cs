using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.Server.Internal;
using System;
using System.Linq;

namespace Shriek.ServiceProxy.Http.Server
{
    public static class ShriekServerExtensions
    {
        public static IWebHostBuilder UseShriekStartup(this IWebHostBuilder builder, params string[] urls)
        {
            return builder.UseStartup<MvcStartup>().UseUrls(urls);
        }

        public static IWebHostBuilder UseShriekStartup<TStartup>(this IWebHostBuilder builder, params string[] urls) where TStartup : MvcStartup
        {
            return builder.UseStartup<TStartup>().UseUrls(urls);
        }

        public static IMvcBuilder AddWebApiProxy(this IMvcBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            mvcBuilder.Services.AddMvcCore().AddWebApiProxy(optionAction);
            return mvcBuilder;
        }

        public static IMvcBuilder AddWebApiProxy(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddMvcCore().AddWebApiProxy();
            return mvcBuilder;
        }

        public static IMvcCoreBuilder AddWebApiProxy(this IMvcCoreBuilder mvcBuilder)
        {
            var webApiProxyTypes = AppDomain.CurrentDomain.GetExcutingAssembiles().SelectMany(x => x.GetTypes())
                .Where(x => !x.IsAbstract && typeof(WebApiProxy).IsAssignableFrom(x));

            mvcBuilder.AddWebApiProxy(opt =>
            {
                foreach (var type in webApiProxyTypes)
                {
                    opt.AddWebApiProxy(type, "");
                }
            });

            return mvcBuilder;
        }

        public static IMvcCoreBuilder AddWebApiProxy(this IMvcCoreBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();
            optionAction(option);

            foreach (var o in option.WebApiProxies)
            {
                var interfaceTypes = o.GetType().Assembly.GetTypes()
                    .Where(x => x.IsInterface && x.GetMethods()
                                    .SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

                foreach (var t in interfaceTypes)
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
                    var featureProvider = new ServiceControllerFeatureProvider(interfaceTypes);
                    manager.FeatureProviders.Remove(
                        manager.FeatureProviders.FirstOrDefault(x => x.GetType() == typeof(ControllerFeatureProvider)));
                    manager.FeatureProviders.Add(featureProvider);
                });
                ;
            }

            return mvcBuilder;
        }
    }
}