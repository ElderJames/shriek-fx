using System;

namespace Shriek.ServiceProxy.Tcp.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OperationContractAttribute : Attribute
    {
        public bool IsOneWay { get; set; }
    }
}