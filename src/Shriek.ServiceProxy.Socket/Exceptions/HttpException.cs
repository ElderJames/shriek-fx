using System;
using System.Net;

namespace Shriek.ServiceProxy.Socket.Exceptions
{
    /// <summary>
    /// 表示http异常
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        /// 获取状态码
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// http异常
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="message">提示消息</param>
        public HttpException(int status, string message)
            : base(message)
        {
            this.Status = status;
        }

        /// <summary>
        /// http异常
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="message">提示消息</param>
        public HttpException(HttpStatusCode status, string message)
            : this((int)status, message)
        {
        }
    }
}