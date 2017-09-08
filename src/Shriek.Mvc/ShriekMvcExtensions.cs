using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Mvc.Internal;
using Shriek.WebApi.Proxy.AspectCore;

namespace Shriek.Mvc
{
    public static class ShriekMvcExtensions
    {
        public static IMvcBuilder UseWebApiProxy(this IMvcBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();

            optionAction(option);
            IEnumerable<Type> interfaceTypes = new List<Type>();

            foreach (var proxy in option.WebApiProxies)
            {
                interfaceTypes = proxy.GetType().Assembly.GetTypes().Where(x =>
                   x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

                foreach (var t in interfaceTypes)
                {
                    mvcBuilder.AddMvcOptions(opt =>
                    {
                        opt.Conventions.Add(new ControllerModelConvention(t));
                        opt.Conventions.Add(new ActionModelConvention(t));
                        opt.Conventions.Add(new ParameterModelConvention(t));
                    });
                }
            }

            return mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                var featureProvider = new ServiceControllerFeatureProvider(interfaceTypes);
                manager.FeatureProviders.Add(featureProvider);
                mvcBuilder.Services.AddSingleton<IApplicationFeatureProvider<ControllerFeature>>(featureProvider);
            }); ;
        }

        public static IMvcCoreBuilder UseWebApiProxy(this IMvcCoreBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();

            optionAction(option);
            IEnumerable<Type> interfaceTypes = new List<Type>();

            foreach (var proxy in option.WebApiProxies)
            {
                interfaceTypes = proxy.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());

                foreach (var t in interfaceTypes)
                {
                    mvcBuilder.AddMvcOptions(opt =>
                    {
                        opt.Conventions.Add(new ControllerModelConvention(t));
                        opt.Conventions.Add(new ActionModelConvention(t));
                        opt.Conventions.Add(new ParameterModelConvention(t));
                    });
                }
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