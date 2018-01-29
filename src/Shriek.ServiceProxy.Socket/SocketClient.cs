using System;
using System.Net;
using System.Reflection;
using Shriek.DynamicProxy;
using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Fast;

namespace Shriek.ServiceProxy.Socket
{
    public class SocketClient : FastTcpClient, IApiInterceptor
    {
        private ApiAction ApiAction { get; set; }

        public SocketClient(int prot)
        {
            this.Connect(IPAddress.Loopback, prot);
        }

        public object Intercept(object target, MethodInfo method, object[] parameters)
        {
            ApiAction = new ApiAction(method);
            return this.InvokeApi(method.ReturnType, ApiAction.ApiName, parameters);
        }
    }
}