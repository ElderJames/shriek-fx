using Shriek.ServiceProxy.Tcp.Core;
using System;
using System.Diagnostics;

namespace Shriek.ServiceProxy.Tcp.Fast
{
    /// <summary>
    /// 表示Fast协议Api异常上下文
    /// </summary>
    [DebuggerDisplay("Message = {Exception.Message}")]
    public class ExceptionContext : RequestContext, IExceptionContext
    {
        /// <summary>
        /// 获取异常对象
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 获取或设置异常是否已处理
        /// 设置为true之后中止传递下一个Filter
        /// </summary>
        public bool ExceptionHandled { get; set; }

        /// <summary>
        /// 异常上下文
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="exception">异常</param>
        public ExceptionContext(ActionContext context, Exception exception)
            : base(context.Session, context.Packet, context.AllSessions)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// 异常上下文
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="exception">异常</param>
        public ExceptionContext(RequestContext context, Exception exception)
            : base(context.Session, context.Packet, context.AllSessions)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Exception.Message;
        }
    }
}