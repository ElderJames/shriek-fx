using AspectCore.DynamicProxy;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using AspectCore.Configuration;

namespace Shriek.WebApi.Proxy.AspectCore
{
    /// <summary>
    /// 表示web api请求客户端
    /// </summary>
    public class HttpApiClient : InterceptorAttribute, IDisposable
    {
        /// <summary>
        /// 获取或设置http客户端
        /// </summary>
        public IHttpClient HttpClient => _httpClient;

        private IHttpClient _httpClient;

        public Uri RequestHost { get; }

        /// <summary>
        /// 获取或设置json解析工具
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        public HttpApiClient(string host)
        {
            RequestHost = new Uri(host);
            if (_httpClient == null)
                _httpClient = new HttpClientAdapter(new HttpClient() { BaseAddress = RequestHost });
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        /// <param name="httpClient">关联的http客户端</param>
        public HttpApiClient(HttpClient httpClient)
        {
            RequestHost = httpClient.BaseAddress;
            if (_httpClient == null)
                _httpClient = new HttpClientAdapter(httpClient ?? new HttpClient());
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose()
        {
            // this.HttpClient.Dispose();
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var _context = CastleContext.From(context);
            var actionContext = new ApiActionContext
            {
                HttpApiClient = this,
                RequestMessage = new HttpRequestMessage(),
                RouteAttributes = _context.RouteAttributes,
                ApiReturnAttribute = _context.ApiReturnAttribute,
                ApiActionFilterAttributes = _context.ApiActionFilterAttributes,
                ApiActionDescriptor = _context.ApiActionDescriptor.Clone() as ApiActionDescriptor
            };

            var parameters = actionContext.ApiActionDescriptor.Parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Value = context.Parameters[i];
            }

            var apiAction = _context.ApiActionDescriptor;

            await next(context);

            context.ReturnValue = apiAction.Execute(actionContext);
        }
    }
}