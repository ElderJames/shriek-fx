using Shriek.ServiceProxy.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Shriek.ServiceProxy.Http.Contexts;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 表示将回复的Xml结果作反序化为指定类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class XmlReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task<object> GetTaskResult(ApiActionContext context)
        {
            if (!(context is HttpApiActionContext httpContext)) return Task.CompletedTask;

            if (httpContext.ResponseMessage.Content.Headers.ContentType.MediaType != "application/xml")
                return null;

            var response = httpContext.ResponseMessage;
            var dataType = context.ApiActionDescriptor.ReturnDataType;
            var xmlSerializer = new XmlSerializer(dataType);

            using (var stream = new MemoryStream())
            {
                await response.Content.CopyToAsync(stream);
                stream.Position = 0;
                return xmlSerializer.Deserialize(stream);
            }
        }
    }
}