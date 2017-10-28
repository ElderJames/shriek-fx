using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义滤过器的接口
    /// </summary>
    public interface IFilter 
    {
        /// <summary>
        /// 执行顺序
        /// 最小的值优先执行
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 获取是否允许多个实例存在
        /// </summary>
        bool AllowMultiple { get; }


        /// <summary>
        /// 在执行Api行为前触发       
        /// </summary>
        /// <param name="filterContext">上下文</param>       
        /// <returns></returns>
        void OnExecuting(IActionContext filterContext);

        /// <summary>
        /// 在执行Api行为后触发
        /// </summary>
        /// <param name="filterContext">上下文</param>      
        void OnExecuted(IActionContext filterContext);

        /// <summary>
        /// 在Api执行中异常时触发
        /// </summary>
        /// <param name="filterContext">上下文</param>
        void OnException(IExceptionContext filterContext);
    }
}
