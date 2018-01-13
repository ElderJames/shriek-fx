using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http.Server.Internal;
using System;
using System.Linq;

namespace Shriek.ServiceProxy.Http.Server
{
    public static class ShriekServerExtensions
    {
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

            foreach (var o in option.WebApiProxies)
            {
                var interfaceTypes = o.GetType().Assembly.GetTypes()
                    .Where(x => x.IsInterface);

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
                    manager.FeatureProviders.Remove(manager.FeatureProviders.FirstOrDefault(x => x.GetType() == typeof(ControllerFeatureProvider)));
                    manager.FeatureProviders.Add(featureProvider);
                });
            }

            return mvcBuilder;
        }
    }
}