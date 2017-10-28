using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义Api执行上下文
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// 获取Api行为对象
        /// </summary>
        ApiAction Action { get; }
    }
}
