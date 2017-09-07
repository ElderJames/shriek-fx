using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Shriek.Mvc.Internal
{
    internal class ActionModelConvention<TService> : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (!typeof(TService).IsAssignableFrom(action.Controller.ControllerType)) return;

            var actionParams = action.ActionMethod.GetParameters();

            var method = typeof(TService).GetMethods().FirstOrDefault(mth =>
            {
                var mthParams = mth.GetParameters();
                return action.ActionMethod.Name == mth.Name
                       && actionParams.Length == mthParams.Length
                       && actionParams.Any(x => mthParams.Any(o => x.Name == o.Name && x.GetType() == o.GetType()));
            });

            var attrs = method.GetCustomAttributes();
            var actionAttrs = new List<object>();

            foreach (var att in attrs)
            {
                if (att is WebApi.Proxy.HttpMethodAttribute methodAttr)
                {
                    var httpMethod = methodAttr.Method;
                    var path = methodAttr.Path;

                    if (httpMethod == HttpMethod.Get)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpGetAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Post)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpPostAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Put)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpPutAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Delete)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpDeleteAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Head)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpHeadAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Options)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpOptionsAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                }
                if (att is WebApi.Proxy.RouteAttribute routeAttr)
                {
                    var template = routeAttr.Template;
                    var routeAttribute = Activator.CreateInstance(typeof(RouteAttribute), template);
                    actionAttrs.Add(routeAttribute);
                }
            }

            if (actionAttrs.Any())
            {
                action.Selectors.Clear();
                ModelConventionHelper.AddRange(action.Selectors, ModelConventionHelper.CreateSelectors(actionAttrs));
            }
        }
    }
}