using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Shriek.Mvc.Internal
{
    internal class ControllerModelConvention : IControllerModelConvention
    {
        public ControllerModelConvention(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        private Type serviceType { get; }

        public void Apply(ControllerModel controller)
        {
            if (!serviceType.IsAssignableFrom(controller.ControllerType)) return;

            var attrs = serviceType.GetCustomAttributes();
            var controllerAttrs = new List<object>();

            foreach (var att in attrs)
            {
                if (att is Shriek.ServiceProxy.Http.RouteAttribute routeAttr)
                {
                    var template = routeAttr.Template;
                    var routeAttribute = Activator.CreateInstance(typeof(RouteAttribute), template);
                    controllerAttrs.Add(routeAttribute);
                }
            }

            if (controllerAttrs.Any())
            {
                controller.Selectors.Clear();
                ModelConventionHelper.AddRange(controller.Selectors, ModelConventionHelper.CreateSelectors(controllerAttrs));
            }
        }
    }
}