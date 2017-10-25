using System;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.Tcp.Protocol;

namespace Shriek.ServiceProxy.Tcp
{
    public class JsonAttribute : ApiReturnAttribute
    {
        public override Task<object> GetTaskResult(ApiActionContext context)
        {
            if (!(context is TcpActionContext tcpContext)) return Task.FromResult<object>(0);

            if (tcpContext.ResponseMessage.MessageType == MessageType.Error)
                throw new Exception(Global.Serializer.Deserialize<string>(tcpContext.ResponseMessage.Parameters[0]));
            var result = Global.Serializer.Deserialize(tcpContext.ApiActionDescriptor.ReturnDataType, tcpContext.ResponseMessage.Parameters[0]);

            return Task.FromResult(result);
        }
    }
}