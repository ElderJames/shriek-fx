using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Shriek.ServiceProxy.Http.ActionAttributes;
using Shriek.ServiceProxy.Abstractions.Attributes;
using HttpDelete = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGet = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpHead = Microsoft.AspNetCore.Mvc.HttpHeadAttribute;
using HttpOptions = Microsoft.AspNetCore.Mvc.HttpOptionsAttribute;
using HttpPost = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPut = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
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
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpGet), path));
                    }
                    else if (httpMethod == HttpMethod.Post)
                    {
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpPost), path));
                    }
                    else if (httpMethod == HttpMethod.Put)
                    {
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpPut), path));
                    }
                    else if (httpMethod == HttpMethod.Delete)
                    {
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpDelete), path));
                    }
                    else if (httpMethod == HttpMethod.Head)
                    {
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpHead), path));
                    }
                    else if (httpMethod == HttpMethod.Options)
                    {
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpOptions), path));
                    }
                }
                if (att is RouteAttribute routeAttr)
                {
                    actionAttrs.Add(Activator.CreateInstance(typeof(Route), routeAttr.Template));
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