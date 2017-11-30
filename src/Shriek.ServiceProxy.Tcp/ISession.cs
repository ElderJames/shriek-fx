using System;
using System.Net;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 定义会话的接口
    /// </summary>
    public interface ISession : ISubscriber, IDisposable
    {
        /// <summary>
        /// 获取用户数据字典
        /// </summary>
        ITag Tag { get; }

        /// <summary>
        /// 获取本机终结点
        /// </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 获取是否已连接到远程端
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 获取会话是否提供SSL/TLS安全
        /// </summary>
        bool IsSecurity { get; }

        /// <summary>
        /// 获取会话的协议名
        /// </summary>
        Protocol Protocol { get; }

        /// <summary>
        /// 获取会话的包装对象
        /// 该对象一般为会话对协议操作的包装
        /// </summary>
        IWrapper Wrapper { get; }

        /// <summary>
        /// 设置会话的协议名和会话包装对象
        /// </summary>
        /// <param name="protocol">协议</param>
        /// <param name="wrapper">会话的包装对象</param>
        void SetProtocolWrapper(Protocol protocol, IWrapper wrapper);

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        int Send(byte[] buffer);

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="byteRange">数据范围</param>
        /// <returns></returns>
        int Send(ArraySegment<byte> byteRange);

        /// <summary>
        /// 断开和远程端的连接
        /// </summary>
        void Close();
    }
}