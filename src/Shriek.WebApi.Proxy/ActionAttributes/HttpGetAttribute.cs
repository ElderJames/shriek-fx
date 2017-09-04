using System;
using System.Net.Http;

namespace Shriek.WebApi.Proxy
{
    /// <summary>
    /// 表示Get请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpGetAttribute(string path)
            : base(HttpMethod.Get, path)
        {
        }
    }
}