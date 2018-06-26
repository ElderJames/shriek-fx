using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Shriek.ServiceProxy.Http.Server.RouteAnalyzer.Impl
{
    internal class RouteAnalyzer : IRouteAnalyzer
    {
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        public RouteAnalyzer(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public IEnumerable<RouteInformation> GetAllRouteInformations()
        {
            var ret = new List<RouteInformation>();

            var descriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (var action in descriptors)
            {
                var info = new RouteInformation();

                if (action.ActionConstraints != null)
                {
                    var httpMethod = action.ActionConstraints.Where(x => x is HttpMethodActionConstraint).OfType<HttpMethodActionConstraint>();
                    info.HttpMethod = string.Join("|", httpMethod.SelectMany(x => x.HttpMethods));
                }

                if (string.IsNullOrEmpty(info.HttpMethod))
                    info.HttpMethod = "Get";

                // Area
                if (action.RouteValues.ContainsKey("area"))
                {
                    info.Area = action.RouteValues["area"];
                }

                // Path and Invocation of Razor Pages
                //if (action is PageActionDescriptor page)
                //{
                //    info.Path = page.ViewEnginePath;
                //    info.Invocation = page.RelativePath;
                //}

                // Path of Route Attribute
                if (action.AttributeRouteInfo != null)
                {
                    info.Path = $"/{action.AttributeRouteInfo.Template}";
                }

                // Path and Invocation of Controller/Action
                if (action is ControllerActionDescriptor ca)
                {
                    if (string.IsNullOrEmpty(info.Path))
                    {
                        info.Path = $"/{ca.ControllerName}/{ca.ActionName}";
                    }
                    info.Invocation = $"{ca.ControllerTypeInfo.Name}.{ca.ActionName}";
                }

                // Special controller path
                if (info.Path == "/RouteAnalyzer/ShowAllRoutes")
                {
                    continue;
                    //info.Path = RouteAnalyzerRouteBuilderExtensions.RouteAnalyzerUrlPath;
                }

                // Additional information of invocation
                info.Invocation += $" ({action.DisplayName})";

                // Generating List
                ret.Add(info);
            }

            // Result
            return ret;
        }
    }
}