using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TcpServiceCore.Buffering
{
    public class BufferManagerFactory
    {
        ConcurrentDictionary<string, IBufferManager> _bufferManagers = 
            new ConcurrentDictionary<string, IBufferManager>();

        public virtual IBufferManager CreateBufferManager(string contract, int maxBufferSize, int maxBufferPoolSize)
        {
            if (_bufferManagers.Keys.Contains(contract))
            {
                return _bufferManagers[contract];
            }
            var result = new BufferManager(maxBufferSize, maxBufferPoolSize);
            _bufferManagers.AddOrUpdate(contract, result, (c, m) => m);
            return result;
        }
    }
}
