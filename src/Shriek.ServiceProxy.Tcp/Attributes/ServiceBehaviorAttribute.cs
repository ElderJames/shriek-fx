using TcpServiceCore.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServiceCore.Attributes
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
