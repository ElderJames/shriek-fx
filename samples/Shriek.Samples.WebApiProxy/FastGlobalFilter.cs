using Shriek.ServiceProxy.Socket.Fast;
using Shriek.ServiceProxy.Socket.Fast.Context;

namespace Shriek.Samples.WebApiProxy
{
    /// <summary>
    /// fast协议全局过滤器
    /// </summary>
    public class FastGlobalFilter : FastFilterAttribute
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            // 标记已处理
            filterContext.ExceptionHandled = true;
        }
    }
}