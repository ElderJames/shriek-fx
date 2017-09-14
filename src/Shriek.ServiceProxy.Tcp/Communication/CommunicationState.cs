using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServiceCore.Communication
{
    public enum CommunicationState
    {
        Created,
        Openning,
        Opened,
        Closing,
        Closed,
        Faulted
    }
}
