using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Shriek.DynamicProxy;
using Shriek.ServiceProxy.Abstractions.Internal;
using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Fast;

namespace Shriek.ServiceProxy.Socket
{
    public class SocketClient : FastTcpClient, IApiInterceptor
    {
        private ApiAction ApiAction { get; set; }

        private Type ReturnDataType { get; set; }

        private Type ReturnTaskType { get; set; }

        public SocketClient(EndPoint endPoint)
        {
            this.Connect(endPoint);
        }

        public SocketClient(string hostAndPort)
        {
            var host = hostAndPort.Split(':')[0];
            var port = hostAndPort.Split(':')[1];
            this.Connect(host, int.TryParse(port, out var _port) ? _port : 80);
        }

        public object Intercept(object target, MethodInfo method, object[] parameters)
        {
            this.ApiAction = new ApiAction(method);
            this.ReturnTaskType = method.ReturnType;

            this.ReturnDataType = method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>))
                     ? method.ReturnType.GetGenericArguments().FirstOrDefault()
                     : method.ReturnType;

            if (this.ReturnTaskType.IsGenericType && this.ReturnTaskType.GetGenericTypeDefinition() == typeof(Task<>))
                return this.InvokeApi(this.ReturnDataType, ApiAction.ApiName, parameters).GetTask().CastResult(this.ReturnDataType);
            else
                return this.InvokeApi(this.ReturnDataType, ApiAction.ApiName, parameters).GetResult();
        }
    }
}