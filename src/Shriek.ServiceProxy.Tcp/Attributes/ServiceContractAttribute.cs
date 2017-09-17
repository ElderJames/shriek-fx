using System;

namespace Shriek.ServiceProxy.Tcp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ServiceContractAttribute : Attribute
    {
        public Type CallbackContract { get; set; }
    }
}