using Shriek.ServiceProxy.Tcp.Protocol;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    public sealed class OperationContext
    {
        private static readonly ThreadLocal<OperationContext> _current = new ThreadLocal<OperationContext>();

        private static readonly Type ByteArrayType;

        static OperationContext()
        {
            ByteArrayType = typeof(byte[]);
        }

        public static OperationContext Current => _current.Value;

        public readonly Socket Socket;

        private readonly object service;
        private readonly ChannelManager channelManager;
        private readonly OperationDescription operation;

        internal OperationContext(object service, ChannelManager channelManager, Socket socket, OperationDescription operation)
        {
            this.service = service;
            this.channelManager = channelManager;
            this.Socket = socket;
            this.operation = operation;
            _current.Value = this;
        }

        private async Task<object> Execute(Message request)
        {
            object[] parameters = null;
            var paramTypes = this.operation.ParameterTypes;
            if (paramTypes != null)
            {
                var length = paramTypes.Length;
                parameters = new object[length];
                for (int i = 0; i < length; i++)
                {
                    var pt = paramTypes[i];
                    if (pt == ByteArrayType)
                        parameters[i] = request.Parameters[i];
                    else
                        parameters[i] = Global.Serializer.Deserialize(pt, request.Parameters[i]);
                }
            }
            object result = null;
            if (this.operation.IsVoidTask)
            {
                await (dynamic)this.operation.MethodInfo.Invoke(this.service, parameters);
            }
            else
            {
                result = await (dynamic)this.operation.MethodInfo.Invoke(this.service, parameters);
            }
            return result;
        }

        internal async Task<Message> HandleRequest(Message request)
        {
            Message response = null;

            if (this.operation.IsOneWay)
            {
                await this.Execute(request);
            }
            else
            {
                try
                {
                    var result = await this.Execute(request);
                    if (this.operation.IsVoidTask)
                    {
                        response = new Message(MessageType.Response, request.Id, (byte)1);
                    }
                    else
                    {
                        response = new Message(MessageType.Response, request.Id, result);
                    }
                }
                catch
                {
                    response = new Message(MessageType.Error, request.Id, "Server Error");
                }
            }

            return response;
        }
    }
}