using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shriek.DependencyInjection
{
    /// <summary>
    /// 服务选择器，用于确定服务的类型、接口、特性等。
    /// </summary>
    public interface IServiceTypeSelector : IImplementationTypeSelector
    {
        /// <summary>
        /// 注册每个类型与自身的类型映射.
        /// </summary>
        ILifetimeSelector AsSelf();

        /// <summary>
        /// 注册每个类型与 <typeparamref name="T"/>类型映射.
        /// </summary>
        /// <typeparam name="T">类型（一般为接口）</typeparam>
        ILifetimeSelector As<T>();

        /// <summary>
        /// 注册多对多的类型映射关系根据 <paramref name="types" />.
        /// </summary>
        /// <param name="types">要匹配的类型列表</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(params Type[] types);

        /// <summary>
        /// 注册多对多的类型映射关系根据 <paramref name="types" />.
        /// </summary>
        /// <param name="types">要匹配的类型列表</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(IEnumerable<Type> types);

        /// <summary>
        /// 注册每个类型与自身的接口类型映射.
        /// </summary>
        ILifetimeSelector AsImplementedInterfaces();

        /// <summary>
        /// 注册多对多的类型映射关系根据 <paramref name="selector"/> function，确定自定义的映射关系.
        /// </summary>
        /// <param name="selector">选择器代理用于确定类与继承类或接口的关系映射</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="selector"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector);

        /// <summary>
        /// Registers the type with the first found matching interface name.  (e.g. ClassName is matched to IClassName)
        /// </summary>
        ILifetimeSelector AsMatchingInterface();

        /// <summary>
        /// Registers the type with the first found matching interface name.  (e.g. ClassName is matched to IClassName) 
        /// </summary>
        /// <param name="action">Filter for matching the Type to an implementing interface</param>
        ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action);

        /// <summary>
        /// 根据<see cref="ServiceDescriptorAttribute"/>注册类型映射.
        /// </summary>
        IImplementationTypeSelector UsingAttributes();
    }
}
