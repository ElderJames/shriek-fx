using System;
using System.Net.Http;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.Http.Contexts;

namespace Shriek.ServiceProxy.Http.ParameterAttributes
{
    /// <summary>
    /// 表示参数为HttpContent或派生类型的特性
    /// 此特性不需要显示声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class HttpContentAttribute : ApiParameterAttribute
    {
        public abstract string MediaType { get; }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public sealed override async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (!(context is HttpApiActionContext httpContext)) return;

            if (httpContext.RequestMessage.Method == HttpMethod.Get)
            {
                return;
            }

            var httpContent = this.GetHttpContent(context, parameter);
            httpContext.RequestMessage.Content = httpContent;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected abstract HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter);
    }
}