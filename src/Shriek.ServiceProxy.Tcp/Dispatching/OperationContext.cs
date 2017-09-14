using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TcpServiceCore.Client;
using TcpServiceCore.Protocol;

namespace TcpServiceCore.Dispatching
{
    public sealed class OperationContext
    {
        static ThreadLocal<OperationContext> _Current = new ThreadLocal<OperationContext>();

        static readonly Type ByteArrayType;

        static OperationContext()
        {
            ByteArrayType = typeof(byte[]);
        }

        public static OperationContext Current
        {
            get { return _Current.Value; }
        }

        public readonly Socket Socket;

        readonly object Service;
        readonly ChannelManager ChannelManager;
        readonly OperationDescription Operation;

        internal OperationContext(object service, ChannelManager channelManager, Socket socket, OperationDescription operation)
        {
            this.Service = service;
            this.ChannelManager = channelManager;
            this.Socket = socket;
            this.Operation = operation;
            _Current.Value = this;
        }

        public async Task<T> CreateCallbackChannel<T>()
        {
            if (typeof(T) != this.ChannelManager.Contract.CallbackType)
            {
                throw new Exception($"{this.ChannelManager.Contract.ContractType.FullName} does not define " +
                    $"callback of type {typeof(T).FullName}");
            }
            return await ChannelFactory<T>.CreateProxy(this.Socket, this.ChannelManager.Config, false);
        }

        async Task<object> Execute(Message request)
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
                await(dynamic)this.Operation.MethodInfo.Invoke(this.Service, parameters);
            }
            else
            {
                result = await(dynamic)this.Operation.MethodInfo.Invoke(this.Service, parameters);
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
