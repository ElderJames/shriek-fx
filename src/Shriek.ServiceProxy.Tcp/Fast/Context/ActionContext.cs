using Shriek.ServiceProxy.Tcp.Core;
using Shriek.ServiceProxy.Tcp.Exceptions;
using System.Diagnostics;

namespace Shriek.ServiceProxy.Tcp.Fast
{
    /// <summary>
    /// 表示Fast协议的Api执行上下文
    /// </summary>
    [DebuggerDisplay("Action = {Action}")]
    public class ActionContext : RequestContext, IActionContext
    {
        /// <summary>
        /// 获取Api行为对象
        /// </summary>
        public ApiAction Action { get; private set; }

        /// <summary>
        /// 获取或设置结果
        /// 当设置了Result值，执行将终止并将Result发送到客户端
        /// </summary>
        public RemoteException Result { get; set; }

        /// <summary>
        /// Api行为上下文
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="action">Api行为</param>
        public ActionContext(RequestContext context, ApiAction action)
            : base(context.Session, context.Packet, context.AllSessions)
        {
            this.Action = action;
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Action.ToString();
        }
    }
}