using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Shriek.ServiceProxy.Socket.Networking
{
    /// <summary>
    /// 提供设置SocketAsyncEventArgs缓冲区或申请缓冲区
    /// </summary>
    internal static class BufferPool
    {
        /// <summary>
        /// 获取SocketAsyncEventArgs的缓存区大小(8k)
        /// </summary>
        public static readonly int BlockSize = 8 * 1024;

        /// <summary>
        /// 获取每个缓冲区SocketAsyncEventArgs的数量(256)
        /// </summary>
        public static readonly int BlockCount = 256;

        /// <summary>
        /// 同步锁
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// 缓冲区块列表
        /// </summary>
        private static readonly LinkedList<BufferSlab> slabs = new LinkedList<BufferSlab>();

        /// <summary>
        /// 获取可以分配内存的slab
        /// </summary>
        /// <returns></returns>
        private static BufferSlab GetAvailableSlab()
        {
            var slabNode = slabs.Last;
            if (slabNode == null || slabNode.Value.CanAlloc == false)
            {
                var slab = new BufferSlab(BlockSize, BlockCount);
                return slabs.AddLast(slab).Value;
            }
            else
            {
                return slabNode.Value;
            }
        }

        /// <summary>
        /// 分配一个缓冲区
        /// </summary>
        /// <returns></returns>
        public static ArraySegment<byte> AllocBuffer()
        {
            lock (syncRoot)
            {
                return BufferPool.GetAvailableSlab().AllocBuffer();
            }
        }

        /// <summary>
        /// 设置SocketAsyncEventArgs缓存区
        /// </summary>
        /// <param name="arg">SocketAsyncEventArgs对象</param>
        public static void SetBuffer(SocketAsyncEventArgs arg)
        {
            lock (syncRoot)
            {
                var buffer = BufferPool.AllocBuffer();
                arg.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
            }
        }

        /// <summary>
        /// 表示缓冲区数据块
        /// </summary>
        private class BufferSlab
        {
            /// <summary>
            /// 缓冲区大小
            /// </summary>
            private readonly int blockSize;

            /// <summary>
            /// 数据内容
            /// </summary>
            private readonly byte[] buffer;

            /// <summary>
            /// 有效数据的位置
            /// </summary>
            private int position = 0;

            public bool CanAlloc
            {
                get
                {
                    return this.position < this.buffer.Length;
                }
            }

            /// <summary>
            /// 缓冲区数据块
            /// </summary>
            /// <param name="blockSize">缓冲区大小</param>
            /// <param name="blockCount">缓冲区数量</param>
            public BufferSlab(int blockSize, int blockCount)
            {
                this.blockSize = blockSize;
                this.buffer = new byte[blockSize * blockCount];
            }

            /// <summary>
            /// 分配一个缓冲区
            /// </summary>
            /// <returns></returns>
            public ArraySegment<byte> AllocBuffer()
            {
                if (this.CanAlloc == false)
                {
                    throw new Exception("Slab is full..");
                }
                var byteRange = new ArraySegment<byte>(this.buffer, this.position, this.blockSize);
                this.position = this.position + this.blockSize;
                return byteRange;
            }
        }
    }
}