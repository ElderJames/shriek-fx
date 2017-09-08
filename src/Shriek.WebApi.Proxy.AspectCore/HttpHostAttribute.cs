using System;

namespace Shriek.WebApi.Proxy.AspectCore
{
    /// <summary>
    /// 表示请求服务根路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public sealed class HttpHostAttribute : Attribute
    {
        /// <summary>
        /// 获取根路径
        /// </summary>
        public Uri Host { get; }

        /// <summary>
        /// 请求服务的根路径
        /// </summary>
        /// <param name="host">根路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public HttpHostAttribute(string host)
        {
            if (!string.IsNullOrEmpty(host))
                this.Host = new Uri(host);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Host.ToString();
        }
    }
}