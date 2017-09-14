using System;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Tcp
{
    public class JsonAttribute : ApiReturnAttribute
    {
        public override Task<object> GetTaskResult(ApiActionContext context)
        {
            if (!(context is TcpActionContext tcpContext)) return Task.FromResult<object>(0);

            var dataType = context.ApiActionDescriptor.ReturnDataType;

            return Task.FromResult(context.HttpApiClient.JsonFormatter.Deserialize(tcpContext.ResponseMessage.MessageString, dataType));
        }
    }
}