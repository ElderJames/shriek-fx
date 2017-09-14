using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServiceCore.Dispatching
{
    public enum InstanceContextMode
    {
        PerCall = 0,
        PerSession = 1,
        Single = 2
    }
}
