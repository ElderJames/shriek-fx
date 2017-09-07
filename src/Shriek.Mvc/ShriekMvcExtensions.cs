using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Mvc.Internal;
using Shriek.WebApi.Proxy;

namespace Shriek.Mvc
{
    public static class ShriekMvcExtensions
    {
        public static IMvcBuilder UseWebApiProxy(this IMvcBuilder mvcBuilder, Action<WebApiProxyOptions> optionAction)
        {
            var option = new WebApiProxyOptions();

            optionAction(option);

            foreach (var o in option.WebApiProxies)
            {
                var types = o.GetType().Assembly.GetTypes().Where(x =>
                    x.IsInterface && x.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(ApiActionAttribute), true)).Any());
                foreach (var t in types)
                {
                    mvcBuilder.AddMvcOptions(opt =>
                    {
                        opt.Conventions.Add(new ControllerModelConvention(t));
                        opt.Conventions.Add(new ActionModelConvention(t));
                        opt.Conventions.Add(new ParameterModelConvention(t));
                    });
                }
            }

            return mvcBuilder;
        }
    }
}