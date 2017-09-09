using System.Net.Http;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public class ApiActionContext
    {
        /// <summary>
        /// 获取关联的HttpApiClient
        /// </summary>
        public HttpApiClient HttpApiClient { get; internal set; }

        /// <summary>
        /// 中间路由模版
        /// </summary>
        public RouteAttribute[] RouteAttributes { get; internal set; }

        /// <summary>
        /// 获取关联的ApiReturnAttribute
        /// </summary>
        public ApiReturnAttribute ApiReturnAttribute { get; internal set; }

        /// <summary>
        /// 获取ApiActionFilterAttribute
        /// </summary>
        public ApiActionFilterAttribute[] ApiActionFilterAttributes { get; internal set; }

        /// <summary>
        /// 获取关联的ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; internal set; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpRequestMessage RequestMessage { get; internal set; }

        /// <summary>
        /// 获取关联的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; internal set; }
    }
}