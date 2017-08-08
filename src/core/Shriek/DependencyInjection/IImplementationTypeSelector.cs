using System;

namespace Shriek.DependencyInjection
{
    /// <summary>
    /// 类型选择器
    /// </summary>
    public interface IImplementationTypeSelector : IAssemblySelector
    {
        /// <summary>
        /// 添加所有公有或非抽象类到 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>中.
        /// </summary>
        IServiceTypeSelector AddClasses();

        /// <summary>
        /// 添加所有公有或非抽象类到 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>中.
        /// </summary>
        /// <param name="publicOnly">要添加的类型是否是公有的</param>
        IServiceTypeSelector AddClasses(bool publicOnly);

        /// <summary>
        ///添加所有公有或非抽象类，根据 <paramref name="action"/> 进行筛选后的结果到 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>中.
        /// </summary>
        /// <param name="action">过滤函数</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="action"/> argument is <c>null</c>.</exception>
        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action);

        /// <summary>
        /// 添加所有公有或非抽象类，根据 <paramref name="action"/>
        /// 进行筛选后的结果到 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="action">过滤函数</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="action"/> argument is <c>null</c>.</exception>
        /// <param name="publicOnly">要添加的类型是否是公有的</param>
        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly);
    }
}
