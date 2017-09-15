using System;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Communication
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