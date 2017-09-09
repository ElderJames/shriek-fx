using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 表示将回复的结果作HttpResponseMessage或byte[]或string处理
    /// 如果使用其它类型作接收结果，将引发NotSupportedException
    /// 此特性不需要显示声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public sealed class DefaultReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task<object> GetTaskResult(ApiActionContext context)
        {
            if (context.ResponseMessage.Content.Headers.ContentType.MediaType == "application/json" ||
                context.ResponseMessage.Content.Headers.ContentType.MediaType == "application/xml")
                return null;

            var response = context.ResponseMessage;
            var returnType = context.ApiActionDescriptor.ReturnDataType;

            if (returnType == typeof(HttpResponseMessage))
            {
                return response;
            }

            if (returnType == typeof(byte[]))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            if (returnType == typeof(string))
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}