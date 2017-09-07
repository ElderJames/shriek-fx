using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shriek.WebApi.Proxy;

namespace Shriek.Mvc.Internal
{
    internal class ParameterModelConvention<TService> : IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (!typeof(TService).IsAssignableFrom(parameter.Action.Controller.ControllerType)) return;

            var actionParams = parameter.Action.ActionMethod.GetParameters();

            var method = typeof(TService).GetMethods().FirstOrDefault(mth =>
            {
                var mthParams = mth.GetParameters();
                return parameter.Action.ActionMethod.Name == mth.Name
                       && actionParams.Length == mthParams.Length
                       && actionParams.Any(x => mthParams.Any(o => x.Name == o.Name && x.GetType() == o.GetType()));
            });

            var theParam = method.GetParameters().FirstOrDefault(x => x.GetType() == parameter.ParameterInfo.GetType());

            if (theParam == null) return;

            var attrs = theParam.GetCustomAttributes();
            var paramAttrs = attrs.OfType<JsonContentAttribute>().Select(att => Activator.CreateInstance(typeof(FromBodyAttribute)));

            if (!paramAttrs.Any()) return;

            parameter.BindingInfo = BindingInfo.GetBindingInfo(paramAttrs);
        }
    }
}