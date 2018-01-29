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

        public SocketClient(int prot)
        {
            this.Connect(IPAddress.Loopback, prot);
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