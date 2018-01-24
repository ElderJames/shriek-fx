using System;
using System.Diagnostics;

namespace Shriek.ServiceProxy.Socket.Exceptions
{
    /// <summary>
    /// 表示依赖反转异常
    /// </summary>
    [DebuggerDisplay("Message = {Message}")]
    public class ResolveException : Exception
    {
        /// <summary>
        /// 依赖反转异常
        /// </summary>
        /// <param name="type">要反转的类型</param>
        /// <param name="innerException">内部异常</param>
        public ResolveException(Type type, Exception innerException)
            : base(string.Format("无法获取类型{0}的实例", type.Name), innerException)
        {
        }
    }
}