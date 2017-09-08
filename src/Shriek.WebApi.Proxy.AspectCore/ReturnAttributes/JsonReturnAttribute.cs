using System;
using System.Threading.Tasks;

namespace Shriek.WebApi.Proxy.AspectCore
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
            if (context.ResponseMessage.Content.Headers.ContentType.MediaType != "application/json")
                return null;

            var response = context.ResponseMessage.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var dataType = context.ApiActionDescriptor.ReturnDataType;
            var result = context.HttpApiClient.JsonFormatter.Deserialize(json, dataType);
            return result;
        }
    }
}