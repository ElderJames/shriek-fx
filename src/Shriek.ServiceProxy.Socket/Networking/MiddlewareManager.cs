using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Socket.Networking
{
    /// <summary>
    /// 表示中间件管理器
    /// </summary>
    internal class MiddlewareManager
    {
        /// <summary>
        /// 所有中间件
        /// </summary>
        private readonly LinkedList<IMiddleware> middlewares = new LinkedList<IMiddleware>();

        /// <summary>
        /// Tcp中间件管理器
        /// </summary>
        public MiddlewareManager()
        {
            this.middlewares.AddLast(new DefaultMiddlerware());
        }

        /// <summary>
        /// 使用协议中间件
        /// </summary>
        /// <param name="middleware">协议中间件</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Use(IMiddleware middleware)
        {
            if (middleware == null)
            {
                throw new ArgumentNullException();
            }

            this.middlewares.AddBefore(this.middlewares.Last, middleware);
            var node = this.middlewares.First;
            while (node.Next != null)
            {
                node.Value.Next = node.Next.Value;
                node = node.Next;
            }
        }

        /// <summary>
        /// 清除所有协议中间件
        /// </summary>
        public void Clear()
        {
            this.middlewares.Clear();
        }

        /// <summary>
        /// 触发执行中间件
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task RaiseInvoke(IContenxt context)
        {
            return this.middlewares.First.Value.Invoke(context);
        }
    }
}