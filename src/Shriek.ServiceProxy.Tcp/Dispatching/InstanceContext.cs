using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Tcp.Attributes;
using Shriek.ServiceProxy.Tcp.Protocol;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    internal class InstanceContext<T> where T : new()
    {
        public static readonly InstanceContextMode InstanceContextMode;
        private static readonly List<OperationDescription> OperationDispatchers = new List<OperationDescription>();

        public readonly T Service;

        static InstanceContext()
        {
            var type = typeof(T);
            InstanceContextMode = InstanceContextMode.PerCall;

            var serviceBehavior = type.GetTypeInfo().GetCustomAttribute<ServiceBehaviorAttribute>(false);
            if (serviceBehavior != null)
            {
                InstanceContextMode = serviceBehavior.InstanceContextMode;
            }

            GetOperations(type);

            if (OperationDispatchers.Count == 0)
                throw new Exception("No OperationContract found");
        }

        private static void GetOperations(Type type)
        {
            var interfaces = type.GetInterfaces();
            foreach (var intfc in interfaces)
            {
                var intInfo = intfc.GetTypeInfo();
                if (ContractDescription.IsContract(intInfo))
                {
                    var operations = ContractDescription.ValidateContract(intInfo);
                    if (operations != null)
                    {
                        OperationDispatchers.AddRange(operations);
                    }
                    GetOperations(intfc);
                }
            }
        }

        public InstanceContext(T instance)
        {
            this.Service = instance;
        }

        public InstanceContext()
        {
            this.Service = new T();
        }

        private OperationDescription GetOperation(string name)
        {
            return OperationDispatchers.FirstOrDefault(x => x.TypeQualifiedName == name);
        }

        public async Task<Message> HandleRequest(Socket socket, ChannelManager channelManager, Message request)
        {
            var operation = GetOperation(request.Operation);
            var operationContext = new OperationContext(this.Service, channelManager, socket, operation);
            return await operationContext.HandleRequest(request);
        }
    }
}