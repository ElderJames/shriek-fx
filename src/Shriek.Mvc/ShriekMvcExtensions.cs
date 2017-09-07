using Microsoft.Extensions.DependencyInjection;
using Shriek.Mvc.Internal;

namespace Shriek.Mvc
{
    public static class ShriekMvcExtensions
    {
        public static IMvcBuilder UseWebApiProxy<TService>(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddMvcOptions(option =>
             {
                 option.Conventions.Add(new ControllerModelConvention<TService>());
                 option.Conventions.Add(new ActionModelConvention<TService>());
                 option.Conventions.Add(new ParameterModelConvention<TService>());
             });
        }
    }
}