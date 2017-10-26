using System.Net.Http;
using Shriek.ServiceProxy.Abstractions.Context;

namespace Shriek.ServiceProxy.Http.Contexts
{
    internal class HttpApiActionContext : ApiActionContext
    {
        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpRequestMessage RequestMessage { get; set; }

        /// <summary>
        /// 获取关联的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; set; }
    }
}