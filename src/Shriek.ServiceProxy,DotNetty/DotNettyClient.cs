using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Shriek.Reflection.DynamicProxy;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.DotNetty.Context;
using Shriek.ServiceProxy.DotNetty.Core;
using Shriek.ServiceProxy.DotNetty.Model;

namespace Shriek.ServiceProxy.DotNetty
{
	public class DotNettyClient : IServiceClient, IInterceptor
	{
		public IJsonFormatter JsonFormatter => throw new NotImplementedException();

		public string RequestHost { get; set; }

		public EndPoint EndsPoint { get; set; }

		public object Intercept(object target, MethodInfo method, object[] @params)
		{
			var httpContext = AspectContext.From(method);

			if (RequestHost == null || string.IsNullOrEmpty(RequestHost))
				if (httpContext.HostAttribute.EndsPoint != null)
					RequestHost = httpContext.HostAttribute.EndsPoint.ToString();
				else
					throw new ArgumentNullException(nameof(method), "未定义任何请求服务器地址,请在注册时传入BaseUrl或在服务契约添加HttpHost标签");

			var actionContext = new SocketApiActionContext()
			{
				HttpApiClient = this,
				RequestMessage = new SocketRequestMessage(),
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
				throw new Exception($"请求远程服务{actionContext.RequestMessage.MethodName ?? RequestHost}异常:{errMsg}", ex);
			}
		}

		public Task SendAsync(ApiActionContext context)
		{
			if (context is SocketApiActionContext socketContext)
			{
				MessageSendHandler handler = RpcServerLoader.Instance.GetMessageSendHandler();
				MessageSendCallBack callBack = handler.SendRequest(socketContext.RequestMessage);
				socketContext.ResponseMessage = callBack.Start();
			}

			return Task.CompletedTask;
		}
	}
}