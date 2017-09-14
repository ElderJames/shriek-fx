using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TcpServiceCore.Buffering
{
    public interface IBufferManager
    {
        byte[] GetFitBuffer(int size);
        void AddBuffer(byte[] buffer);
    }
}
