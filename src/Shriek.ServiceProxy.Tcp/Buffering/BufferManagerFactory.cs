using System.Collections.Concurrent;

namespace Shriek.ServiceProxy.Tcp.Buffering
{
    public class BufferManagerFactory
    {
        private ConcurrentDictionary<string, IBufferManager> _bufferManagers =
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