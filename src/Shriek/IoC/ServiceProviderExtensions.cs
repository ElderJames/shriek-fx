using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.IoC
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider service) where T : class
        {
            return service.GetService(typeof(T)) as T;
        }
    }
}