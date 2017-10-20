using Shriek.ServiceProxy.Http.Contexts;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Abstractions
{
    /// <summary>
    /// 表示http请求方法描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpMethodAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取请求方法
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// 获取请求相对路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">请求相对路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpMethodAttribute(HttpMethod method, string path)
        {
            //if (string.IsNullOrEmpty(path))
            //{
            //    throw new ArgumentNullException();
            //}
            this.Method = method;
            this.Path = path.TrimEnd('&');
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            if (context is HttpApiActionContext httpApiActionContext)
            {
                httpApiActionContext.RequestMessage.Method = this.Method;
                var routes = context.RouteAttributes.Where((x, i) => string.IsNullOrEmpty(this.Path) || i < 1)
                    .Select(x => x.Template.Trim('/'));
                httpApiActionContext.RequestMessage.RequestUri = new Uri(context.HttpApiClient.RequestHost,
                    string.Join("/", routes) + '/' + this.Path.Trim('/'));
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", this.Method, this.Path);
        }
    }
}