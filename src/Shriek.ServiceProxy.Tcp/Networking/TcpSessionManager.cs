using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示Tcp会话对象管理器
    /// 线程安全类型
    /// </summary>   
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SessionCollectionDebugView))]
    internal class TcpSessionManager : ISessionManager, IEnumerable<TcpSessionBase>, IDisposable
    {
        /// <summary>
        /// 已释放的会话
        /// </summary>
        private readonly ConcurrentQueue<TcpSessionBase> freeSessions = new ConcurrentQueue<TcpSessionBase>();

        /// <summary>
        /// 工作中的会话
        /// </summary>
        private readonly ConcurrentDictionary<Guid, TcpSessionBase> workSessions = new ConcurrentDictionary<Guid, TcpSessionBase>();
        
        /// <summary>
        /// 获取元素数量 
        /// </summary>
        public int Count
        {
            get
            {
                return this.workSessions.Count;
            }
        }

        /// <summary>
        /// 申请一个会话
        /// </summary>
        /// <param name="cer">服务器证书</param>
        /// <returns></returns>
        public TcpSessionBase Alloc(X509Certificate cer)
        {
            TcpSessionBase session;
            if (this.freeSessions.TryDequeue(out session) == true)
            {
                return session;
            }

            if (cer == null)
            {
                return new IocpTcpSession();
            }
            else
            {
                return new SslTcpSession(cer);
            }
        }

        /// <summary>
        /// 添加一个会话
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <returns></returns>
        public bool Add(TcpSessionBase session)
        {
            return this.workSessions.TryAdd(session.ID, session);
        }

        /// <summary>
        /// 移除一个会话    
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <returns></returns>
        public bool Remove(TcpSessionBase session)
        {
            if (session == null)
            {
                return false;
            }

            if (this.workSessions.TryRemove(session.ID, out session) == true)
            {
                session.Shutdown();
                this.freeSessions.Enqueue(session);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取会话的包装对象
        /// </summary>
        /// <typeparam name="TWapper">包装类型</typeparam>
        /// <returns></returns>
        IEnumerable<TWapper> ISessionManager.FilterWrappers<TWapper>()
        {
            return this.Select(item => item.Wrapper).OfType<TWapper>();
        }

        /// <summary>
        /// 获取过滤了协议类型的会话对象
        /// </summary>
        /// <param name="protocol">协议类型</param>
        /// <returns></returns>
        IEnumerable<ISession> ISessionManager.FilterProtocol(Protocol protocol)
        {
            return this.Where(item => item.Protocol == protocol);
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TcpSessionBase> GetEnumerator()
        {
            return this.workSessions.Values.GetEnumerator();
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.workSessions.Values.GetEnumerator();
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var item in this)
            {
                item.Dispose();
            }
            this.workSessions.Clear();

            TcpSessionBase session;
            while (this.freeSessions.TryDequeue(out session))
            {
                session.Dispose();
            }
        }


        /// <summary>
        /// 调试视图
        /// </summary>
        private class SessionCollectionDebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private TcpSessionManager view;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="view">查看的对象</param>
            public SessionCollectionDebugView(TcpSessionManager view)
            {
                this.view = view;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public TcpSessionBase[] Values
            {
                get
                {
                    return this.view.ToArray();
                }
            }
        }
    }

}
