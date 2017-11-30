using Shriek.ServiceProxy.Tcp.Core;
using System.Net;

namespace Shriek.ServiceProxy.Tcp.Fast
{
    /// <summary>
    /// 表示fast协议的会话对象
    /// </summary>
    public sealed class FastSession : IWrapper
    {
        /// <summary>
        /// 会话对象
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// 中间件实例
        /// </summary>
        private readonly FastMiddleware middleware;

        /// <summary>
        /// 获取用户数据字典
        /// </summary>
        public ITag Tag
        {
            get
            {
                return this.session.Tag;
            }
        }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get
            {
                return this.session.RemoteEndPoint;
            }
        }

        /// <summary>
        /// 获取本机终结点
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get
            {
                return this.session.LocalEndPoint;
            }
        }

        /// <summary>
        /// 获取是否已连接到远程端
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.session.IsConnected;
            }
        }

        /// <summary>
        /// fast协议的会话对象
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="middleware">中间件实例</param>
        public FastSession(ISession session, FastMiddleware middleware)
        {
            this.session = session;
            this.middleware = middleware;
        }

        /// <summary>
        /// 断开和远程端的连接
        /// </summary>
        public void Close()
        {
            this.session.Close();
        }

        /// <summary>
        /// 调用远程端实现的Api
        /// </summary>
        /// <param name="api">数据包Api名</param>
        /// <param name="parameters">参数列表</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="SerializerException"></exception>
        public void InvokeApi(string api, params object[] parameters)
        {
            var id = this.middleware.PacketIdProvider.NewId();
            var packet = new FastPacket(api, id, false);
            packet.SetBodyParameters(this.middleware.Serializer, parameters);
            this.session.Send(packet.ToArraySegment());
        }

        /// <summary>
        /// 调用远程端实现的Api
        /// 并返回结果数据任务
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="api">数据包Api名</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="SerializerException"></exception>
        /// <returns>远程数据任务</returns>
        public ApiResult<T> InvokeApi<T>(string api, params object[] parameters)
        {
            var id = this.middleware.PacketIdProvider.NewId();
            var packet = new FastPacket(api, id, false);
            packet.SetBodyParameters(this.middleware.Serializer, parameters);
            return Common.InvokeApi<T>(this.session, this.middleware.TaskSetterTable, this.middleware.Serializer, packet, this.middleware.TimeOut);
        }

        /// <summary>
        /// 还原到包装前
        /// </summary>
        /// <returns></returns>
        public ISession UnWrap()
        {
            return this.session;
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.session.ToString();
        }
    }
}