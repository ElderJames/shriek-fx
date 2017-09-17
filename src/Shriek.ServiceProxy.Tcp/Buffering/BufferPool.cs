using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Tcp.Buffering
{
    public class BufferPool
    {
        private Queue<byte[]> buffers = new Queue<byte[]>();

        public readonly int BufferSize;
        public readonly int PoolSize;
        public readonly int MaxBuffersCount;

        public BufferPool(int bufferSize, int poolSize)
        {
            var bsn = nameof(bufferSize);
            var psn = nameof(poolSize);
            if (bufferSize <= 0)
                throw new Exception($"{bsn} must be positive greater than 0");
            if (poolSize <= 0)
                throw new Exception($"{psn} must be positive greater than 0");
            if (poolSize < bufferSize)
                throw new Exception($"{psn} can not be less than {bsn}");

            this.BufferSize = bufferSize;
            this.PoolSize = poolSize;
            this.MaxBuffersCount = (int)Math.Floor((double)this.PoolSize / this.BufferSize);
        }

        public byte[] GetBuffer()
        {
            byte[] buffer = null;
            lock (this.buffers)
            {
                if (this.buffers.Count == 0)
                {
                    buffer = new byte[this.BufferSize];
                    this.buffers.Enqueue(buffer);
                }
                else
                {
                    buffer = this.buffers.Dequeue();
                }
            }
            return buffer;
        }

        public void AddBuffer(byte[] buffer)
        {
            if (buffer == null || buffer.Length != this.BufferSize)
                return;

            if (this.buffers.Count < this.MaxBuffersCount)
            {
                lock (this.buffers)
                {
                    if (this.buffers.Count < this.MaxBuffersCount)
                    {
                        this.buffers.Enqueue(buffer);
                    }
                }
            }
        }
    }
}