using Shriek.ServiceProxy.Tcp.Streams;
using Shriek.ServiceProxy.Tcp.Tasks;
using Shriek.ServiceProxy.Tcp.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 接收完成委托
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    internal delegate Task ReceiveCompletedHandler(TcpSessionBase sender);

    /// <summary>
    /// 断开委托
    /// </summary>
    /// <param name="sender"></param>
    internal delegate void DisconnectHandler(TcpSessionBase sender);

    /// <summary>
    /// 关闭时委托
    /// </summary>
    /// <param name="sender"></param>
    internal delegate void CloseHandler(TcpSessionBase sender);

    /// <summary>
    /// Tcp会话抽象类
    /// </summary>
    internal abstract class TcpSessionBase : ISession, IDisposable
    {
        /// <summary>
        /// 是否已关闭
        /// </summary>
        private bool socketClosed = true;

        /// <summary>
        /// socket同步锁
        /// </summary>
        private object socketRoot = new object();

        /// <summary>
        /// 获取绑定的Socket对象
        /// </summary>
        protected Socket Socket { get; private set; }

        /// <summary>
        /// 处理和分析收到的数据的委托
        /// </summary>
        public ReceiveCompletedHandler ReceiveCompletedHandler;

        /// <summary>
        /// 连接断开委托
        /// </summary>
        public DisconnectHandler DisconnectHandler;

        /// <summary>
        /// 关闭时的委托
        /// </summary>
        public CloseHandler CloseHandler;

        /// <summary>
        /// 获取是否已连接到远程端
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.socketClosed == false && this.Socket != null && this.Socket.Connected;
            }
        }

        /// <summary>
        /// 获取会话的唯一标识符
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// 获取会话是否提供SSL/TLS安全
        /// </summary>
        public abstract bool IsSecurity { get; }

        /// <summary>
        /// 获取用户附加数据
        /// </summary>
        public ITag Tag { get; private set; }

        /// <summary>
        /// 获取会话的协议
        /// </summary>
        public Protocol Protocol { get; private set; }

        /// <summary>
        /// 获取会话的包装对象
        /// </summary>
        public IWrapper Wrapper { get; private set; }

        /// <summary>
        /// 获取接收到数据读取器
        /// </summary>
        public SessionStreamReader StreamReader { get; private set; }

        /// <summary>
        /// 获取本机终结点
        /// </summary>
        public EndPoint LocalEndPoint { get; private set; }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// 表示会话对象
        /// </summary>
        public TcpSessionBase()
        {
            this.ID = Guid.NewGuid();
            this.Tag = new DefaultTag();
            this.StreamReader = new SessionStreamReader(new SessionStream());
        }

        /// <summary>
        /// 设置关联一个Socket实例
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void SetSocket(Socket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException();
            }
            this.Socket = socket;
            this.socketClosed = false;

            this.StreamReader.Clear();
            this.Tag.ID = null;
            ((IDictionary)this.Tag).Clear();
            this.UnSubscribe();
            this.SetProtocolWrapper(Protocol.None, null);
            this.LocalEndPoint = socket.LocalEndPoint;
            this.RemoteEndPoint = socket.RemoteEndPoint;
        }

        /// <summary>
        /// SSL验证
        /// </summary>
        public virtual void Authenticate()
        {
        }

        /// <summary>
        /// 异步SSL验证
        /// </summary>
        /// <returns></returns>
        public virtual Task AuthenticateAsync()
        {
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 开始异步循环接收数据
        /// </summary>
        public async void LoopReceiveAsync()
        {
            while (this.IsConnected)
            {
                if (await this.ReceiveAsync() == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 异步接收数据
        /// 将接收结果写入StreamReader
        /// 如果返回false表示接收异常
        /// </summary>
        /// <returns></returns>
        protected abstract Task<bool> ReceiveAsync();

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="byteRange">数据范围</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public virtual int Send(ArraySegment<byte> byteRange)
        {
            if (byteRange.Array == null)
            {
                throw new ArgumentNullException();
            }

            if (this.IsConnected == false)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }

            return this.Socket.Send(byteRange.Array, byteRange.Offset, byteRange.Count, SocketFlags.None);
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public virtual int Send(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            if (this.IsConnected == false)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }
            return this.Socket.Send(buffer);
        }

        /// <summary>
        /// 关闭会话的发送与接收
        /// </summary>
        /// <returns></returns>
        public virtual bool Shutdown()
        {
            lock (this.socketRoot)
            {
                if (this.socketClosed == true)
                {
                    return false;
                }

                try
                {
                    this.Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                }
                finally
                {
                    this.Socket.Dispose();
                    this.socketClosed = true;
                }
                return true;
            }
        }

        /// <summary>
        /// 断开和远程端的连接
        /// </summary>
        void ISession.Close()
        {
            if (this.Shutdown() && this.CloseHandler != null)
            {
                this.CloseHandler(this);
            }
        }

        /// <summary>
        /// 设置协议和会话包装对象
        /// </summary>
        /// <param name="protocol">协议</param>
        /// <param name="wrapper">会话的包装对象</param>
        public void SetProtocolWrapper(Protocol protocol, IWrapper wrapper)
        {
            this.Protocol = protocol;
            this.Wrapper = wrapper;
        }

        /// <summary>
        /// 设置会话的心跳包
        /// </summary>
        /// <param name="keepAlivePeriod">时间间隔</param>
        /// <returns></returns>
        public bool SetKeepAlive(TimeSpan keepAlivePeriod)
        {
            if (keepAlivePeriod == TimeSpan.Zero)
            {
                return false;
            }

            if (this.Socket == null)
            {
                return false;
            }

            var period = (int)keepAlivePeriod.TotalMilliseconds;
            return this.TrySetKeepAlive(Socket, period, period);
        }

        /// <summary>
        /// 设置会话的心跳包
        /// </summary>
        /// <param name="socket">客户端</param>
        /// <param name="dueTime">延迟的时间量（以毫秒为单位）</param>
        /// <param name="period">时间间隔（以毫秒为单位）</param>
        /// <returns></returns>
        private bool TrySetKeepAlive(Socket socket, int dueTime, int period)
        {
            var inOptionValue = new byte[12];
            var outOptionValue = new byte[12];

            ByteConverter.ToBytes(1, ByteConverter.Endian).CopyTo(inOptionValue, 0);
            ByteConverter.ToBytes(dueTime, ByteConverter.Endian).CopyTo(inOptionValue, 4);
            ByteConverter.ToBytes(period, ByteConverter.Endian).CopyTo(inOptionValue, 8);

            try
            {
                socket.IOControl(IOControlCode.KeepAliveValues, inOptionValue, outOptionValue);
                return true;
            }
            catch (NotSupportedException)
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                return true;
            }
            catch (NotImplementedException)
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.RemoteEndPoint == null ? string.Empty : this.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as TcpSessionBase;
            return other != null && other.ID == this.ID;
        }

        #region ISubscriber

        /// <summary>
        /// 订阅表
        /// </summary>
        private readonly Dictionary<string, Action<object>> subscribeTable = new Dictionary<string, Action<object>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 订阅
        /// 多次订阅同个频道会覆盖前者的handler
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理者</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Subscribe(string channel, Action<object> handler)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException("channel");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.subscribeTable[channel] = handler;
        }

        /// <summary>
        /// 取消所有订阅
        /// </summary>
        public void UnSubscribe()
        {
            this.subscribeTable.Clear();
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel">频道</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public bool UnSubscribe(string channel)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException();
            }
            return this.subscribeTable.Remove(channel);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="data">数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public bool Publish(string channel, object data)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException("channel");
            }

            Action<object> action;
            if (this.subscribeTable.TryGetValue(channel, out action))
            {
                action(data);
                return true;
            }
            return false;
        }

        #endregion ISubscriber

        #region IDisposable

        /// <summary>
        /// 获取是否已释放
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 关闭和释放所有相关资源
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed == false)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            this.IsDisposed = true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TcpSessionBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            this.Shutdown();
            this.StreamReader.Stream.Dispose();

            if (disposing)
            {
                this.socketRoot = null;
                this.Socket = null;
                this.Tag = null;
                this.StreamReader = null;
                this.CloseHandler = null;
                this.DisconnectHandler = null;
                this.ReceiveCompletedHandler = null;
            }
        }

        #endregion IDisposable
    }
}