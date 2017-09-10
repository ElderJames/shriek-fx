using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Http.Server.Internal;
using Shriek.Utils;

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
            mvcBuilder.Services.AddMvcCore().AddWebApiProxy();
            return mvcBuilder;
        }

        public static IMvcCoreBuilder AddWebApiProxy(this IMvcCoreBuilder mvcBuilder)
        {
            var interfaceTypes = Reflection.CurrentAssembiles.SelectMany(x => x.GetTypes())
                .Where(x => x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

            foreach (var t in interfaceTypes)
            {
                mvcBuilder.AddMvcOptions(opt =>
                {
                    opt.Conventions.Add(new ControllerModelConvention(t));
                    opt.Conventions.Add(new ActionModelConvention(t));
                    opt.Conventions.Add(new ParameterModelConvention(t));
                });
            }

            return mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                var featureProvider = new ServiceControllerFeatureProvider(interfaceTypes);
                manager.FeatureProviders.Add(featureProvider);
                mvcBuilder.Services.AddSingleton<IApplicationFeatureProvider<ControllerFeature>>(featureProvider);
            }); ;
        }
    }
}