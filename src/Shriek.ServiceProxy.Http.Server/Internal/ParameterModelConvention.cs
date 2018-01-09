using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shriek.ServiceProxy.Http.ParameterAttributes;
using System;
using System.Linq;
using System.Reflection;
using HttpGetAttribute = Shriek.ServiceProxy.Http.ActionAttributes.HttpGetAttribute;

namespace Shriek.ServiceProxy.Http.Server.Internal
{
    internal class ParameterModelConvention : IParameterModelConvention
    {
        public ParameterModelConvention(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        private Type serviceType { get; }

        public void Apply(ParameterModel parameter)
        {
            if (!serviceType.IsAssignableFrom(parameter.Action.Controller.ControllerType)) return;

            var actionParams = parameter.Action.ActionMethod.GetParameters();

            var method = serviceType.GetMethods().FirstOrDefault(mth =>
            {
                var mthParams = mth.GetParameters();
                return parameter.Action.ActionMethod.Name == mth.Name
                       && actionParams.Length == mthParams.Length
                       && actionParams.Any(x => mthParams.Where(o => x.Name == o.Name).Any(o => x.GetType() == o.GetType()));
            });

            if (method == null) return;

            var theParam = method.GetParameters().FirstOrDefault(x => x.ParameterType == parameter.ParameterInfo.ParameterType);
            var isGetMethod = method.GetCustomAttribute<HttpGetAttribute>(true) != null;

            if (theParam == null) return;

            var paramAttrs = theParam.GetCustomAttributes().OfType<HttpContentAttribute>().Select(att => att is FormContentAttribute ? (IBindingSourceMetadata)new FromQueryAttribute() : new FromBodyAttribute());

            if (!paramAttrs.Any())
            {
                //默认配置 uri参数,或者是uri参数数组且为Get方法， 从Query取
                if (!theParam.ParameterType.IsUriParameterType() && !isGetMethod)
                    paramAttrs = new[] { new FromBodyAttribute() };
                else
                    return;
            }

            parameter.BindingInfo = BindingInfo.GetBindingInfo(paramAttrs);
        }
    }
}