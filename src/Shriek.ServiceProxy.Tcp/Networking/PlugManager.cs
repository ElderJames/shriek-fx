using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示插件管理器
    /// </summary>
    internal class PlugManager
    {
        /// <summary>
        /// 所有插件
        /// </summary>
        private readonly List<IPlug> plugs = new List<IPlug>();

        /// <summary>
        /// 使用插件
        /// </summary>
        /// <param name="plug">插件</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Use(IPlug plug)
        {
            if (plug == null)
            {
                throw new ArgumentNullException();
            }
            this.plugs.Add(plug);
        }

        /// <summary>
        /// 清除所有插件
        /// </summary>
        public void Clear()
        {
            this.plugs.Clear();
        }

        /// <summary>
        /// 会话连接成功后触发
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool RaiseConnected(object sender, IContenxt context)
        {
            foreach (var item in this.plugs)
            {
                if (context.Session.IsConnected == false)
                {
                    return false;
                }
                item.OnConnected(sender, context);
            }
            return true;
        }

        /// <summary>
        /// SSL验证后触发
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool RaiseAuthenticated(object sender, IContenxt context)
        {
            foreach (var item in this.plugs)
            {
                if (context.Session.IsConnected == false)
                {
                    return false;
                }
                item.OnAuthenticated(sender, context);
            }
            return true;
        }

        /// <summary>
        /// 收到请求后触发
        /// 此方法在先于协议中间件的Invoke方法调用
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool RaiseRequested(object sender, IContenxt context)
        {
            foreach (var item in this.plugs)
            {
                if (context.Session.IsConnected == false || context.StreamReader.Length == 0)
                {
                    return false;
                }
                item.OnRequested(sender, context);
            }
            return true;
        }

        /// <summary>
        /// 会话断开后触发
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        public void RaiseDisconnected(object sender, IContenxt context)
        {
            foreach (var item in this.plugs)
            {
                item.OnDisconnected(sender, context);
            }
        }

        /// <summary>
        /// 服务异常后触发
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="exception">异常</param>
        public void RaiseException(object sender, Exception exception)
        {
            foreach (var item in this.plugs)
            {
                item.OnException(sender, exception);
            }
        }
    }
}