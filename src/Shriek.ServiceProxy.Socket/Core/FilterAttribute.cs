using Shriek.ServiceProxy.Socket.Tasks;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Shriek.ServiceProxy.Socket.Core
{
    /// <summary>
    /// 表示Api行为过滤器基础特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class FilterAttribute : Attribute, IFilter
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> multiuseAttributeCache = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// 执行顺序
        /// 最小的值优先执行
        /// </summary>
        public int Order { get; protected set; }

        /// <summary>
        /// 获取是否允许多个实例存在
        /// </summary>
        public bool AllowMultiple
        {
            get
            {
                return IsAllowMultiple(this.GetType());
            }
        }

        /// <summary>
        /// 表示服务或Api行为过滤器基础特性
        /// </summary>
        public FilterAttribute()
        {
        }

        /// <summary>
        /// 获取特性是否允许多个实例
        /// </summary>
        /// <param name="attributeType">特性类型</param>
        /// <returns></returns>
        private static bool IsAllowMultiple(Type attributeType)
        {
            return multiuseAttributeCache.GetOrAdd(attributeType, type => type
                .GetCustomAttributes(typeof(AttributeUsageAttribute), true)
                .Cast<AttributeUsageAttribute>()
                .First()
                .AllowMultiple);
        }

        /// <summary>
        /// 在执行Api行为前触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        /// <returns></returns>
        void IFilter.OnExecuting(IActionContext filterContext)
        {
            Dispatcher.Wait(() => this.OnExecuting(filterContext));
        }

        /// <summary>
        /// 在执行Api行为后触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        void IFilter.OnExecuted(IActionContext filterContext)
        {
            Dispatcher.Wait(() => this.OnExecuted(filterContext));
        }

        /// <summary>
        /// 在Api执行中异常时触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        void IFilter.OnException(IExceptionContext filterContext)
        {
            Dispatcher.Wait(() => this.OnException(filterContext));
        }

        /// <summary>
        /// 在执行Api行为前触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        /// <returns></returns>
        protected abstract void OnExecuting(IActionContext filterContext);

        /// <summary>
        /// 在执行Api行为后触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected abstract void OnExecuted(IActionContext filterContext);

        /// <summary>
        /// 在Api执行中异常时触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected abstract void OnException(IExceptionContext filterContext);
    }
}