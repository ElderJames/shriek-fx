using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Shriek.Mvc.Internal
{
    public class ServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        private IEnumerable<Type> ServiceTypes;

        public ServiceControllerFeatureProvider(IEnumerable<Type> ServiceTypes)
        {
            this.ServiceTypes = ServiceTypes;
        }

        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo)
                || ServiceTypes.Any(o => typeInfo.GetInterface(o.Name) == o);

            return isController;
        }
    }
}