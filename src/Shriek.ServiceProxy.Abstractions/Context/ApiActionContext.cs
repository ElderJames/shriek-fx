using Shriek.ServiceProxy.Abstractions.Attributes;

namespace Shriek.ServiceProxy.Abstractions.Context
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public abstract class ApiActionContext
    {
        /// <summary>
        /// 获取关联的HttpApiClient
        /// </summary>
        public IServiceClient HttpApiClient { get; set; }

        /// <summary>
        /// 中间路由模版
        /// </summary>
        public RouteAttribute[] RouteAttributes { get; set; }

        /// <summary>
        /// 获取关联的ApiReturnAttribute
        /// </summary>
        public ApiReturnAttribute ApiReturnAttribute { get; set; }

        /// <summary>
        /// 获取ApiActionFilterAttribute
        /// </summary>
        public ApiActionFilterAttribute[] ApiActionFilterAttributes { get; set; }

        /// <summary>
        /// 获取关联的ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; set; }
    }
}