using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TcpServiceCore.Protocol
{
    enum MessageType : byte
    {
        Request,
        Response,
        Error
    }
}
