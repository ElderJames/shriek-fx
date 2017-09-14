using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TcpServiceCore.Buffering
{
    public class BufferManager : IBufferManager
    {
        public readonly int MaxBufferSize;
        public readonly int MaxBufferPoolSize;

        const int MIN_BUFFER_SIZE = 64000;
        static int Base2Index;

        static BufferManager()
        {
            Base2Index = (int)Math.Log(MIN_BUFFER_SIZE, 2);
        }

        List<BufferPool> pools = new List<BufferPool>();
        
        public BufferManager(int maxBufferSize, int maxBufferPoolSize)
        {
            var bsn = nameof(maxBufferSize);
            var psn = nameof(maxBufferPoolSize);

            if (maxBufferSize <= MIN_BUFFER_SIZE)
                throw new Exception($"{bsn} must be positive greater than {MIN_BUFFER_SIZE}");
            if (maxBufferPoolSize <= MIN_BUFFER_SIZE)
                throw new Exception($"{psn} must be positive greater than {MIN_BUFFER_SIZE}");
            if (maxBufferPoolSize < maxBufferSize)
                throw new Exception($"{psn} can not be less than {bsn}");

            this.MaxBufferSize = maxBufferSize;
            this.MaxBufferPoolSize = maxBufferPoolSize;

            var poolSize = MIN_BUFFER_SIZE / 2;
            do
            {
                poolSize = Math.Min(poolSize * 2, this.MaxBufferPoolSize);
                pools.Add(new BufferPool(poolSize, this.MaxBufferPoolSize));
            } while (poolSize < this.MaxBufferPoolSize);
        }

        public byte[] GetFitBuffer(int size)
        {
            if (size > this.MaxBufferSize)
                throw new Exception($"Received message is too big, max buffer size is {this.MaxBufferSize}");

            if (size < MIN_BUFFER_SIZE)
                return new byte[size];

            var fitPoolIndex = (int)Math.Ceiling(Math.Log(size, 2)) - Base2Index;

            return this.pools[fitPoolIndex].GetBuffer();
        }

        public void AddBuffer(byte[] buffer)
        {
            if (buffer == null)
                return;

            var length = buffer.Length;

            if (length < MIN_BUFFER_SIZE)
                return;

            var fitPoolIndex = (int)Math.Ceiling(Math.Log(length, 2)) - Base2Index;

            this.pools[fitPoolIndex].AddBuffer(buffer);
        }
    }
}
