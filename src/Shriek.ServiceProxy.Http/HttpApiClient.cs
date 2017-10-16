using AspectCore.DynamicProxy;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http.Contexts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 表示web api请求客户端
    /// </summary>
    public class HttpApiClient : AbstractInterceptorAttribute, IServiceClient, IDisposable
    {
        /// <summary>
        /// 静态httpClient
        /// </summary>
        private static IHttpClient _client;

        /// <summary>
        /// 获取或设置http客户端
        /// </summary>
        public IHttpClient HttpClient => _client;

        /// <summary>
        /// 请求服务器地址
        /// </summary>
        public Uri RequestHost { get; private set; }

        /// <summary>
        /// 获取或设置json解析工具
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        /// <param name="baseUrl"></param>
        public HttpApiClient(string baseUrl)
        {
            if (!string.IsNullOrEmpty(baseUrl))
                RequestHost = new Uri(baseUrl);
            if (_client == null)
                _client = new HttpClientAdapter(new HttpClient());
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        public HttpApiClient()
        {
            if (_client == null)
                _client = new HttpClientAdapter(new HttpClient());
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        /// <param name="httpClient">关联的http客户端</param>
        public HttpApiClient(HttpClient httpClient)
        {
            RequestHost = httpClient.BaseAddress;
            if (_client == null)
                _client = new HttpClientAdapter(httpClient);
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
            var _context = AspectCoreContext.From(context);

            if (RequestHost == null || string.IsNullOrEmpty(RequestHost.OriginalString))
                if (_context.HostAttribute.Host != null && !string.IsNullOrEmpty(_context.HostAttribute.Host.OriginalString))
                    RequestHost = _context.HostAttribute.Host;
                else
                    throw new ArgumentNullException("BaseUrl or HttpHost attribute", "未定义任何请求服务器地址,请在注册时传入BaseUrl或在服务契约添加HttpHost标签");

            var actionContext = new HttpApiActionContext
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

        public async Task SendAsync(ApiActionContext context)
        {
            if (context is HttpApiActionContext httpContext)
            {
                httpContext.ResponseMessage = await this.HttpClient.SendAsync(httpContext.RequestMessage);

                if (!httpContext.ResponseMessage.IsSuccessStatusCode)
                    throw new HttpRequestException(httpContext.ResponseMessage.ReasonPhrase);
            }
        }
    }
}