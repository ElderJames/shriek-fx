using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpServiceCore.Communication
{
    public interface ICommunicationObject : IDisposable
    {
        event Action<ICommunicationObject, CommunicationState> StateChanged;
        CommunicationState State { get; }
        Task Open();
        Task Close();
        Task Abort();
    }
}
