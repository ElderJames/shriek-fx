using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Shriek.ServiceProxy.Abstractions;
using HttpGet = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPost = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPut = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using HttpDelete = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpHead = Microsoft.AspNetCore.Mvc.HttpHeadAttribute;
using HttpOptions = Microsoft.AspNetCore.Mvc.HttpOptionsAttribute;
using Route = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Shriek.ServiceProxy.Http.Server.Internal
{
    internal class ActionModelConvention : IActionModelConvention
    {
        public ActionModelConvention(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        private Type serviceType { get; }

        public void Apply(ActionModel action)
        {
            if (!serviceType.IsAssignableFrom(action.Controller.ControllerType)) return;

            var actionParams = action.ActionMethod.GetParameters();

            var method = serviceType.GetMethods().FirstOrDefault(mth =>
            {
                var mthParams = mth.GetParameters();
                return action.ActionMethod.Name == mth.Name
                       && actionParams.Length == mthParams.Length
                       && actionParams.Any(x => mthParams.Where(o => x.Name == o.Name).Any(o => x.GetType() == o.GetType()));
            });

            var attrs = method.GetCustomAttributes();
            var actionAttrs = new List<object>();

            foreach (var att in attrs)
            {
                if (att is HttpMethodAttribute methodAttr)
                {
                    var httpMethod = methodAttr.Method;
                    var path = methodAttr.Path;

                    if (httpMethod == HttpMethod.Get)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpGet), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Post)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpPost), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Put)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpPut), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Delete)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpDelete), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Head)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpHead), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    else if (httpMethod == HttpMethod.Options)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpOptions), path);
                        actionAttrs.Add(methodAttribute);
                    }
                }
                if (att is RouteAttribute routeAttr)
                {
                    var template = routeAttr.Template;
                    var routeAttribute = Activator.CreateInstance(typeof(Route), template);
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