using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义Api异常上下文
    /// </summary>
    public interface IExceptionContext
    {
        /// <summary>
        /// 获取异常对象
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// 获取或设置异常是否已处理
        /// 设置为true之后中止传递下一个Filter
        /// </summary>
        bool ExceptionHandled { get; set; }
    }
}
