using System;

namespace Shriek.ServiceProxy.Tcp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceContractAttribute : Attribute
    {
        public Type CallbackContract { get; set; }
    }
}