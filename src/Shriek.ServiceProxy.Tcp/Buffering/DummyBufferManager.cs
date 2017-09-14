using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TcpServiceCore.Buffering
{
    public class DummyBufferManager : IBufferManager
    {
        public void AddBuffer(byte[] buffer)
        {
            //do nothing
        }

        public byte[] GetFitBuffer(int size)
        {
            return new byte[size];
        }
    }
}
