using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Tcp.Protocol;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    public sealed class OperationContext
    {
        private static ThreadLocal<OperationContext> _Current = new ThreadLocal<OperationContext>();

        private static readonly Type ByteArrayType;

        static OperationContext()
        {
            ByteArrayType = typeof(byte[]);
        }

        public static OperationContext Current
        {
            get { return _Current.Value; }
        }

        public readonly Socket Socket;

        private readonly object Service;
        private readonly ChannelManager ChannelManager;
        private readonly OperationDescription Operation;

        internal OperationContext(object service, ChannelManager channelManager, Socket socket, OperationDescription operation)
        {
            this.Service = service;
            this.ChannelManager = channelManager;
            this.Socket = socket;
            this.Operation = operation;
            _Current.Value = this;
        }

        private async Task<object> Execute(Message request)
        {
            object[] parameters = null;
            var paramTypes = this.Operation.ParameterTypes;
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
            if (this.Operation.IsVoidTask)
            {
                await (dynamic)this.Operation.MethodInfo.Invoke(this.Service, parameters);
            }
            else
            {
                result = await (dynamic)this.Operation.MethodInfo.Invoke(this.Service, parameters);
            }
            return result;
        }

        internal async Task<Message> HandleRequest(Message request)
        {
            Message response = null;

            if (this.Operation.IsOneWay)
            {
                await this.Execute(request);
            }
            else
            {
                try
                {
                    var result = await this.Execute(request);
                    if (this.Operation.IsVoidTask)
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