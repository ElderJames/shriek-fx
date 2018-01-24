using System;

namespace Shriek.ServiceProxy.Socket
{
    /// <summary>
    /// 定义服务器插件行为
    /// </summary>
    public interface IPlug
    {
        /// <summary>
        /// 会话连接成功后触发
        /// 如果关闭了会话，将停止传递给下个插件的OnConnected
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        void OnConnected(object sender, IContenxt context);

        /// <summary>
        /// SSL验证完成后触发
        /// 如果起用了SSL，验证通过后才可以往客户端发送数据
        /// 如果关闭了会话，将停止传递给下个插件的OnAuthenticated
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        void OnAuthenticated(object sender, IContenxt context);

        /// <summary>
        /// 收到请求后触发
        /// 如果关闭了会话或清空了数据，将停止传递给下个插件的OnRequested
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        void OnRequested(object sender, IContenxt context);

        /// <summary>
        /// 会话断开后触发
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="context">上下文</param>
        void OnDisconnected(object sender, IContenxt context);

        /// <summary>
        /// 服务异常后触发
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="exception">异常</param>
        void OnException(object sender, Exception exception);
    }
}