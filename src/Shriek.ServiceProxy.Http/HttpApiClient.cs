using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.Abstractions.Internal;
using Shriek.ServiceProxy.Http.Contexts;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using Shriek.Reflection.DynamicProxy;

namespace Shriek.ServiceProxy.Http
{
	/// <summary>
	/// 表示web api请求客户端
	/// </summary>
	public class HttpApiClient : IInterceptor, IServiceClient, IDisposable
	{
		/// <summary>
		/// 静态httpClient
		/// </summary>
		private readonly IHttpClient _client;

		/// <summary>
		/// 获取或设置http客户端
		/// </summary>
		public IHttpClient HttpClient => _client;

		/// <summary>
		/// 请求服务器地址
		/// </summary>
		public string RequestHost { get; private set; }

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
				RequestHost = baseUrl;

			if (_client == null)
			{
				var httpClient = new HttpClient(new HttpClientHandler
				{
					UseProxy = false,
				})
				{
					Timeout = TimeSpan.FromSeconds(10)
				};

				httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

				_client = new HttpClientAdapter(httpClient);
			}
			this.JsonFormatter = new DefaultJsonFormatter();
		}

		/// <summary>
		/// web api请求客户端
		/// </summary>
		public HttpApiClient()
		{
			if (_client == null)
			{
				var httpClient = new HttpClient(new HttpClientHandler
				{
					UseProxy = false,
				})
				{
					Timeout = TimeSpan.FromSeconds(10)
				};
				httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

				_client = new HttpClientAdapter(httpClient);
			}
			this.JsonFormatter = new DefaultJsonFormatter();
		}

		/// <summary>
		/// web api请求客户端
		/// </summary>
		/// <param name="httpClient">关联的http客户端</param>
		/// <param name="baseUrl"></param>
		public HttpApiClient(IHttpClient httpClient, string baseUrl)
		{
			if (!string.IsNullOrEmpty(baseUrl))
				RequestHost = baseUrl;

			if (_client == null)
			{
				//httpClient.Timeout = TimeSpan.FromSeconds(10);
				//httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
				_client = httpClient;
			}
			this.JsonFormatter = new DefaultJsonFormatter();
		}

		/// <summary>
		/// 释放相关资源
		/// </summary>
		public void Dispose()
		{
			// this.HttpClient.Dispose();
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

		public object Intercept(object target, MethodInfo method, object[] @params)
		{
			var httpContext = AspectContext.From(method);

			if (RequestHost == null || string.IsNullOrEmpty(RequestHost))
				if (httpContext.HostAttribute.Host != null && !string.IsNullOrEmpty(httpContext.HostAttribute.Host.OriginalString))
					RequestHost = httpContext.HostAttribute.Host.OriginalString;
				else
					throw new ArgumentNullException(nameof(method), "未定义任何请求服务器地址,请在注册时传入BaseUrl或在服务契约添加HttpHost标签");

			var actionContext = new HttpApiActionContext
			{
				HttpApiClient = this,
				RequestMessage = new HttpRequestMessage(),
				RouteAttributes = httpContext.RouteAttributes,
				ApiReturnAttribute = httpContext.ApiReturnAttribute,
				ApiActionFilterAttributes = httpContext.ApiActionFilterAttributes,
				ApiActionDescriptor = httpContext.ApiActionDescriptor.Clone() as ApiActionDescriptor
			};

			var parameters = actionContext.ApiActionDescriptor.Parameters;
			for (var i = 0; i < parameters.Length; i++)
			{
				parameters[i].Value = @params[i];
			}

			var apiAction = httpContext.ApiActionDescriptor;

			try
			{
				return apiAction.Execute(actionContext);
			}
			catch (Exception ex)
			{
				var errMsg = ex.Message;

				while (ex.InnerException != null)
				{
					errMsg += "--->" + ex.InnerException.Message;
					ex = ex.InnerException;
				}
				throw new Exception($"请求远程服务[{actionContext.RequestMessage?.Method.Method}]{actionContext.RequestMessage?.RequestUri.ToString() ?? RequestHost}异常:{errMsg}", ex);
			}
		}
	}
}