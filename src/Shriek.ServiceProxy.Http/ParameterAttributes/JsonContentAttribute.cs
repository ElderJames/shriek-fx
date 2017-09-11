using Shriek.ServiceProxy.Abstractions;
using System;
using System.Net.Http;
using System.Text;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 表示将参数体作为application/json请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JsonContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var json = parameter.Value == null ? null : context.HttpApiClient.JsonFormatter.Serialize(parameter.Value);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}