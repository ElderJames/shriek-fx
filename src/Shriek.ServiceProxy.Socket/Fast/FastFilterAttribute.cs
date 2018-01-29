using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Fast.Context;

namespace Shriek.ServiceProxy.Socket.Fast
{
    /// <summary>
    /// 表示Fast协议过滤器
    /// </summary>
    public abstract class FastFilterAttribute : FilterAttribute
    {
        /// <summary>
        /// 在执行Api行为前触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        /// <returns></returns>
        protected sealed override void OnExecuting(IActionContext filterContext)
        {
            this.OnExecuting(filterContext as ActionContext);
        }

        /// <summary>
        /// 在执行Api行为后触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected sealed override void OnExecuted(IActionContext filterContext)
        {
            this.OnExecuted(filterContext as ActionContext);
        }

        /// <summary>
        /// 在Api执行中异常时触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected sealed override void OnException(IExceptionContext filterContext)
        {
            this.OnException(filterContext as ExceptionContext);
        }

        /// <summary>
        /// 在执行Api行为前触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        /// <returns></returns>
        protected virtual void OnExecuting(ActionContext filterContext)
        {
        }

        /// <summary>
        /// 在执行Api行为后触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected virtual void OnExecuted(ActionContext filterContext)
        {
        }

        /// <summary>
        /// 在Api执行中异常时触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected virtual void OnException(ExceptionContext filterContext)
        {
        }
    }
}