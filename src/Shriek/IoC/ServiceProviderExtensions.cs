using System;

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