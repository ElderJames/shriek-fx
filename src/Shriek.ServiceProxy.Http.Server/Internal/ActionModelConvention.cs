using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.ActionAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Shriek.ServiceProxy.Abstractions;
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

            var method = serviceType.GetMethods().FirstOrDefault(mth => action.ActionMethod.Name == mth.Name && !actionParams.Except(mth.GetParameters(), new ModelConventionHelper.ParameterInfoEqualityComparer()).Any());

            if (method == null) return;

            var attrs = method.GetCustomAttributes();
            var actionAttrs = new List<object>();

            if (!attrs.Any(x => x is HttpMethodAttribute || x is RouteAttribute))
                actionAttrs.Add(new HttpPost(method.GetPath()));
            else
                foreach (var att in attrs)
                {
                    switch (att)
                    {
                        case HttpMethodAttribute methodAttr:
                            var httpMethod = methodAttr.Method;
                            var path = methodAttr.Path;

                            if (httpMethod == HttpMethod.Get)
                            {
                                actionAttrs.Add(new HttpGet(path));
                            }
                            else if (httpMethod == HttpMethod.Post)
                            {
                                actionAttrs.Add(new HttpPost(path));
                            }
                            else if (httpMethod == HttpMethod.Put)
                            {
                                actionAttrs.Add(new HttpPut(path));
                            }
                            else if (httpMethod == HttpMethod.Delete)
                            {
                                actionAttrs.Add(new HttpDelete(path));
                            }
                            else if (httpMethod == HttpMethod.Head)
                            {
                                actionAttrs.Add(new HttpHead(path));
                            }
                            else if (httpMethod == HttpMethod.Options)
                            {
                                actionAttrs.Add(new HttpOptions(path));
                            }
                            break;

                        case RouteAttribute routeAttr:
                            actionAttrs.Add(new Route(routeAttr.Template));
                            break;
                    }
                }

            action.Selectors.Clear();
            ModelConventionHelper.AddRange(action.Selectors, ModelConventionHelper.CreateSelectors(actionAttrs));
        }
    }
}