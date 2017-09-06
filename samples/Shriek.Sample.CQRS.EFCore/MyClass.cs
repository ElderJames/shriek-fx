using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Routing;
using Shriek.Samples.Services;
using System.Net.Http;

namespace Shriek.Samples.CQRS.EFCore
{
    public class MyClass<TService> : IControllerModelConvention, IActionModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!typeof(TService).IsAssignableFrom(controller.ControllerType)) return;

            var attrs = typeof(TService).GetCustomAttributes();
            var controllerAttrs = new List<object>();

            foreach (var att in attrs)
            {
                if (att is WebApi.Proxy.RouteAttribute routeAtt)
                {
                    var route = routeAtt.Route;
                    //var ctor = typeof(RouteAttribute).GetConstructors().FirstOrDefault();
                    var routeAttribute = Activator.CreateInstance(typeof(RouteAttribute), route);
                    controllerAttrs.Add(routeAttribute);
                }
            }

            if (controllerAttrs.Any())
            {
                controller.Selectors.Clear();
                AddRange(controller.Selectors, CreateSelectors(controllerAttrs));
            }
        }

        public void Apply(ActionModel action)
        {
            if (!typeof(TService).IsAssignableFrom(action.Controller.ControllerType)) return;

            //TODO:获取Action实现了接口中对应的方法
            var method = typeof(TService).GetMethods().FirstOrDefault(mth => action.ActionMethod.Name == mth.Name);

            var attrs = method.GetCustomAttributes();
            var actionAttrs = new List<object>();

            foreach (var att in attrs)
            {
                if (att is WebApi.Proxy.HttpMethodAttribute routeAtt)
                {
                    var httpMethod = routeAtt.Method;
                    var path = routeAtt.Path;
                    if (httpMethod == HttpMethod.Get)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpGetAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }
                    if (httpMethod == HttpMethod.Post)
                    {
                        var methodAttribute = Activator.CreateInstance(typeof(HttpPostAttribute), path);
                        actionAttrs.Add(methodAttribute);
                    }

                    //TODO:more method...
                }
            }

            if (actionAttrs.Any())
            {
                action.Selectors.Clear();
                AddRange(action.Selectors, CreateSelectors(actionAttrs));
            }
        }

        private ICollection<SelectorModel> CreateSelectors(IList<object> attributes)
        {
            // Route attributes create multiple selector models, we want to split the set of
            // attributes based on these so each selector only has the attributes that affect it.
            //
            // The set of route attributes are split into those that 'define' a route versus those that are
            // 'silent'.
            //
            // We need to define a selector for each attribute that 'defines' a route, and a single selector
            // for all of the ones that don't (if any exist).
            //
            // If the attribute that 'defines' a route is NOT an IActionHttpMethodProvider, then we'll include with
            // it, any IActionHttpMethodProvider that are 'silent' IRouteTemplateProviders. In this case the 'extra'
            // action for silent route providers isn't needed.
            //
            // Ex:
            // [HttpGet]
            // [AcceptVerbs("POST", "PUT")]
            // [HttpPost("Api/Things")]
            // public void DoThing()
            //
            // This will generate 2 selectors:
            // 1. [HttpPost("Api/Things")]
            // 2. [HttpGet], [AcceptVerbs("POST", "PUT")]
            //
            // Another example of this situation is:
            //
            // [Route("api/Products")]
            // [AcceptVerbs("GET", "HEAD")]
            // [HttpPost("api/Products/new")]
            //
            // This will generate 2 selectors:
            // 1. [AcceptVerbs("GET", "HEAD")]
            // 2. [HttpPost]
            //
            // Note that having a route attribute that doesn't define a route template _might_ be an error. We
            // don't have enough context to really know at this point so we just pass it on.
            var routeProviders = new List<IRouteTemplateProvider>();

            var createSelectorForSilentRouteProviders = false;
            foreach (var attribute in attributes)
            {
                if (attribute is IRouteTemplateProvider routeTemplateProvider)
                {
                    if (IsSilentRouteAttribute(routeTemplateProvider))
                    {
                        createSelectorForSilentRouteProviders = true;
                    }
                    else
                    {
                        routeProviders.Add(routeTemplateProvider);
                    }
                }
            }

            foreach (var routeProvider in routeProviders)
            {
                // If we see an attribute like
                // [Route(...)]
                //
                // Then we want to group any attributes like [HttpGet] with it.
                //
                // Basically...
                //
                // [HttpGet]
                // [HttpPost("Products")]
                // public void Foo() { }
                //
                // Is two selectors. And...
                //
                // [HttpGet]
                // [Route("Products")]
                // public void Foo() { }
                //
                // Is one selector.
                if (!(routeProvider is IActionHttpMethodProvider))
                {
                    createSelectorForSilentRouteProviders = false;
                }
            }

            var selectorModels = new List<SelectorModel>();
            if (routeProviders.Count == 0 && !createSelectorForSilentRouteProviders)
            {
                // Simple case, all attributes apply
                selectorModels.Add(CreateSelectorModel(route: null, attributes: attributes));
            }
            else
            {
                // Each of these routeProviders are the ones that actually have routing information on them
                // something like [HttpGet] won't show up here, but [HttpGet("Products")] will.
                foreach (var routeProvider in routeProviders)
                {
                    var filteredAttributes = new List<object>();
                    foreach (var attribute in attributes)
                    {
                        if (ReferenceEquals(attribute, routeProvider))
                        {
                            filteredAttributes.Add(attribute);
                        }
                        else if (InRouteProviders(routeProviders, attribute))
                        {
                            // Exclude other route template providers
                            // Example:
                            // [HttpGet("template")]
                            // [Route("template/{id}")]
                        }
                        else if (
                            routeProvider is IActionHttpMethodProvider &&
                            attribute is IActionHttpMethodProvider)
                        {
                            // Example:
                            // [HttpGet("template")]
                            // [AcceptVerbs("GET", "POST")]
                            //
                            // Exclude other http method providers if this route is an
                            // http method provider.
                        }
                        else
                        {
                            filteredAttributes.Add(attribute);
                        }
                    }

                    selectorModels.Add(CreateSelectorModel(routeProvider, filteredAttributes));
                }

                if (createSelectorForSilentRouteProviders)
                {
                    var filteredAttributes = new List<object>();
                    foreach (var attribute in attributes)
                    {
                        if (!InRouteProviders(routeProviders, attribute))
                        {
                            filteredAttributes.Add(attribute);
                        }
                    }

                    selectorModels.Add(CreateSelectorModel(route: null, attributes: filteredAttributes));
                }
            }

            return selectorModels;
        }

        private static bool InRouteProviders(List<IRouteTemplateProvider> routeProviders, object attribute)
        {
            foreach (var rp in routeProviders)
            {
                if (ReferenceEquals(rp, attribute))
                {
                    return true;
                }
            }

            return false;
        }

        private static SelectorModel CreateSelectorModel(IRouteTemplateProvider route, IList<object> attributes)
        {
            var selectorModel = new SelectorModel();
            if (route != null)
            {
                selectorModel.AttributeRouteModel = new AttributeRouteModel(route);
            }

            AddRange(selectorModel.ActionConstraints, attributes.OfType<IActionConstraintMetadata>());

            // Simple case, all HTTP method attributes apply
            var httpMethods = attributes
                .OfType<IActionHttpMethodProvider>()
                .SelectMany(a => a.HttpMethods)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (httpMethods.Length > 0)
            {
                selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(httpMethods));
            }

            return selectorModel;
        }

        private bool IsIDisposableMethod(MethodInfo methodInfo)
        {
            // Ideally we do not want Dispose method to be exposed as an action. However there are some scenarios where a user
            // might want to expose a method with name "Dispose" (even though they might not be really disposing resources)
            // Example: A controller deriving from MVC's Controller type might wish to have a method with name Dispose,
            // in which case they can use the "new" keyword to hide the base controller's declaration.

            // Find where the method was originally declared
            var baseMethodInfo = methodInfo.GetBaseDefinition();
            var declaringTypeInfo = baseMethodInfo.DeclaringType.GetTypeInfo();

            return
                (typeof(IDisposable).GetTypeInfo().IsAssignableFrom(declaringTypeInfo) &&
                 declaringTypeInfo.GetRuntimeInterfaceMap(typeof(IDisposable)).TargetMethods[0] == baseMethodInfo);
        }

        private bool IsSilentRouteAttribute(IRouteTemplateProvider routeTemplateProvider)
        {
            return
                routeTemplateProvider.Template == null &&
                routeTemplateProvider.Order == null &&
                routeTemplateProvider.Name == null;
        }

        private static void AddRange<T>(IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }
}