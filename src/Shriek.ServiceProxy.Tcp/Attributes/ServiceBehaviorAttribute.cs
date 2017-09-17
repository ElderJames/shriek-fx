using System;
using Shriek.ServiceProxy.Tcp.Dispatching;

namespace Shriek.ServiceProxy.Tcp.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceBehaviorAttribute : Attribute
    {
        public InstanceContextMode InstanceContextMode { get; set; }

        public ServiceBehaviorAttribute()
        {
            this.InstanceContextMode = InstanceContextMode.PerSession;
        }
    }
}