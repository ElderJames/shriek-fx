using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.Http.Contexts;
using System;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http.ReturnAttributes
{
    /// <summary>
    /// 表示将回复的Json结果作反序化为指定类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class JsonReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task<object> GetTaskResult(ApiActionContext context)
        {
            if (!(context is HttpApiActionContext httpContext)) return Task.CompletedTask;

            if (httpContext.ResponseMessage.Content.Headers.ContentType.MediaType != "application/json")
                return null;

            var response = httpContext.ResponseMessage.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var dataType = context.ApiActionDescriptor.ReturnDataType;
            var result = context.HttpApiClient.JsonFormatter.Deserialize(json, dataType);
            return result;
        }
    }
}