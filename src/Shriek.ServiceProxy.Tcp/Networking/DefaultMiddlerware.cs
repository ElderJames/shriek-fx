using Shriek.ServiceProxy.Tcp.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示默认的最后一个中间件
    /// </summary>
    internal class DefaultMiddlerware : IMiddleware
    {
        /// <summary>
        /// 数据包长度超过这个值 且还无法解析出协议的连接
        /// 将要被关闭
        /// </summary>
        private readonly static int MaxProtocolLength = 4096;

        /// <summary>
        /// 下一个中间件
        /// </summary>
        public IMiddleware Next { set; private get; }

        /// <summary>
        /// 执行中间件          
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task Invoke(IContenxt context)
        {
            if (context.Session.Protocol == Protocol.None)
            {
                if (context.StreamReader.Length > MaxProtocolLength)
                {
                    context.StreamReader.Clear();
                    context.Session.Close();
                }
            }
            return TaskExtend.CompletedTask;
        }
    }
}
