using System;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 定义发布订阅
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// 订阅
        /// 多次订阅同个频道会覆盖前者的handler
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理者</param>
        void Subscribe(string channel, Action<object> handler);

        /// <summary>
        /// 取消所有订阅
        /// </summary>
        void UnSubscribe();

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel">频道</param>
        /// <returns></returns>
        bool UnSubscribe(string channel);

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        bool Publish(string channel, object data);
    }
}