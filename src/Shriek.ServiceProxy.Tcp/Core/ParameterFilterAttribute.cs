using System;

namespace Shriek.ServiceProxy.Socket.Core
{
    /// <summary>
    /// 表示Api行为的参数过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public abstract class ParameterFilterAttribute : FilterAttribute
    {
        /// <summary>
        /// 参数索引
        /// </summary>
        private int index;

        /// <summary>
        /// 绑定参数
        /// </summary>
        /// <param name="index">参数索引</param>
        internal ParameterFilterAttribute InitWith(int index)
        {
            this.index = index;
            return this;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="filterContext"></param>
        protected sealed override void OnExecuting(IActionContext filterContext)
        {
            this.OnExecuting(filterContext.Action, filterContext.Action.Parameters[this.index]);
        }

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="filterContext"></param>
        protected sealed override void OnExecuted(IActionContext filterContext)
        {
        }

        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="filterContext"></param>
        protected sealed override void OnException(IExceptionContext filterContext)
        {
        }

        /// <summary>
        /// Api执行之前
        /// 在此检测parameter的输入合理性
        /// 不合理可以抛出异常
        /// </summary>
        /// <param name="action">关联的Api行为</param>
        /// <param name="parameter">参数信息</param>
        public abstract void OnExecuting(ApiAction action, ApiParameter parameter);
    }
}