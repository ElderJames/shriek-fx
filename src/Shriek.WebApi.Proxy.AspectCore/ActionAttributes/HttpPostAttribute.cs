using System;
using System.Net.Http;

namespace Shriek.WebApi.Proxy.AspectCore
{
    /// <summary>
    /// 表示Post请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpPostAttribute(string path = "")
            : base(HttpMethod.Post, path)
        {
        }
    }
}