using Shriek.ServiceProxy.socket;
using System.Collections;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Socket
{
    /// <summary>
    /// 定义会话管理者的接口
    /// </summary>
    public interface ISessionManager : IEnumerable
    {
        /// <summary>
        /// 获取所有会话的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取并过滤会话的包装对象
        /// </summary>
        /// <typeparam name="TWapper">包装类型</typeparam>
        /// <returns></returns>
        IEnumerable<TWapper> FilterWrappers<TWapper>() where TWapper : class, IWrapper;

        /// <summary>
        /// 获取过滤了协议类型的会话对象
        /// </summary>
        /// <param name="protocol">协议类型</param>
        /// <returns></returns>
        IEnumerable<ISession> FilterProtocol(Protocol protocol);
    }
}