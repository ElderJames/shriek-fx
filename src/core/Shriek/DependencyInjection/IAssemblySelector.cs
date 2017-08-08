using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shriek.DependencyInjection
{
    /// <summary>
    /// 程序集选择器
    /// </summary>
    public interface IAssemblySelector : IFluentInterface
    {
        /// <summary>
        /// 将从类型 <typeparamref name="T"/> 所在程序集扫描类型.
        /// </summary>
        /// <typeparam name="T">需要扫描的程序集中的一个类型。</typeparam>
        IImplementationTypeSelector FromAssemblyOf<T>();

        /// <summary>
        /// 将扫描参数 <paramref name="types"/> 里的每个 <see cref="Type"/> 类型所在的程序集中的类型。
        /// </summary>
        /// <param name="types">类型数组，将扫描他们所在的程序集。</param>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="types"/> 的值为 <c>null</c>.</exception>
        IImplementationTypeSelector FromAssembliesOf(params Type[] types);

        /// <summary>
        /// 将扫描参数 <paramref name="types"/> 里的每个<see cref="Type"/> 类型所在的程序集中的类型。
        /// </summary>
        /// <param name="types">类型列表，将扫描他们所在的程序集。</param>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="types"/> 的值为 <c>null</c>.</exception>
        IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types);

        /// <summary>
        /// 将在 <paramref name="assemblies"/> 中的每个 <see cref="Assembly"/> 中扫描类型。
        /// </summary>
        /// <param name="assemblies">需要扫描的程序集</param>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="assemblies"/> 的值为 <c>null</c>.</exception>
        IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies);

        /// <summary>
        /// 将从 <paramref name="assemblies"/> 中的每个 <see cref="Assembly"/> 中扫描类型。
        /// </summary>
        /// <param name="assemblies">需要扫描的程序集</param>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="assemblies"/> 的值为 <c>null</c>.</exception>
        IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies);
    }
}