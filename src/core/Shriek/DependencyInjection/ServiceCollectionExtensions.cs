using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.DependencyInjection
{
    /// <summary>
    /// IoC服务扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 使用特定方法<paramref name="action"/>扫描程序集，并向<paramref name="services"/>批量注册<paramref name="action"/>方法返回的类型.
        /// </summary>
        /// <param name="services">需要被添加注册类型的IoC服务</param>
        /// <param name="action">扫描配置方法</param>
        /// <exception cref="System.ArgumentNullException">参数 <paramref name="services"/>
        /// 或者 <paramref name="action"/> 为 <c>null</c>.</exception>
        public static IServiceCollection Scan(this IServiceCollection services, Action<IAssemblySelector> action)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var selector = new AssemblySelector();

            action(selector);

            ((ISelector)selector).Populate(services);

            return services;
        }
    }
}