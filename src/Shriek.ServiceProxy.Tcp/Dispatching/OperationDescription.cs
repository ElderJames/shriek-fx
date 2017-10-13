using Shriek.ServiceProxy.Tcp.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    internal class OperationDescription
    {
        public readonly string Name;
        public readonly string TypeQualifiedName;
        public readonly bool IsReturnTypeGeneric;
        public readonly bool IsVoidTask;
        public readonly bool IsOperation;
        public readonly bool IsOneWay;
        public readonly bool IsAwaitable;

        public readonly MethodInfo MethodInfo;
        public readonly Type ReturnType;
        public readonly Type[] ParameterTypes;

        public OperationDescription(MethodInfo methodInfo)
        {
            this.MethodInfo = methodInfo;
            this.ReturnType = this.MethodInfo.ReturnType;
            this.Name = methodInfo.Name;
            this.TypeQualifiedName = $"{methodInfo.DeclaringType.Name}.{methodInfo.Name}";

            this.IsReturnTypeGeneric = this.ReturnType.GetTypeInfo().IsGenericType;
            this.IsVoidTask = this.MethodInfo.ReturnType == typeof(Task);
            this.IsAwaitable = IsVoidTask ||
                (this.IsReturnTypeGeneric && this.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
            this.ParameterTypes = this.MethodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            var attr = this.MethodInfo.GetCustomAttribute<OperationContractAttribute>();
            if (attr != null)
            {
                this.IsOperation = true;
                this.IsOneWay = attr.IsOneWay;
            }
        }

        public void ValidateOperationContract()
        {
            if (!this.IsOperation)
                throw new Exception($"{this.Name} is not attributed with {nameof(OperationContractAttribute)}");
            if (!this.IsAwaitable)
                throw new Exception($"{this.Name} is Operation Contract, it must return Task or Task<T>");
            if (this.IsOneWay && !this.IsVoidTask)
                throw new Exception($"{this.Name} is One Way Operation Contract, it must return Task");
        }
    }
}