using System;
using System.Diagnostics;

namespace Shriek.ServiceProxy.Socket.Util
{
    /// <summary>
    /// 提供二进制数据生成支持
    /// 非线程安全类型
    /// </summary>
    [DebuggerDisplay("Length = {Length}, Endian = {Endian}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class ByteBuilder
    {
        /// <summary>
        /// 容量
        /// </summary>
        private int _capacity;

        /// <summary>
        /// 当前数据
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// 获取数据长度
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// 获取字节存储次序
        /// </summary>
        public Endians Endian { get; private set; }

        /// <summary>
        /// 提供二进制数据读取和操作支持
        /// </summary>
        /// <param name="endian">字节存储次序</param>
        public ByteBuilder(Endians endian)
        {
            this.Endian = endian;
        }

        /// <summary>
        /// 添加一个bool类型
        /// </summary>
        /// <param name="value">值</param>
        public void Add(bool value)
        {
            this.Add(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 添加一个字节
        /// </summary>
        /// <param name="value">字节</param>
        public void Add(byte value)
        {
            this.Add(new byte[] { value });
        }

        /// <summary>
        /// 将16位整数转换为byte数组再添加
        /// </summary>
        /// <param name="value">整数</param>
        public void Add(short value)
        {
            var bytes = ByteConverter.ToBytes(value, this.Endian);
            this.Add(bytes);
        }

        /// <summary>
        /// 将16位整数转换为byte数组再添加
        /// </summary>
        /// <param name="value">整数</param>
        public void Add(ushort value)
        {
            var bytes = ByteConverter.ToBytes(value, this.Endian);
            this.Add(bytes);
        }

        /// <summary>
        /// 将32位整数转换为byte数组再添加
        /// </summary>
        /// <param name="value">整数</param>
        public void Add(int value)
        {
            var bytes = ByteConverter.ToBytes(value, this.Endian);
            this.Add(bytes);
        }

        /// <summary>
        /// 将32位整数转换为byte数组再添加
        /// </summary>
        /// <param name="value">整数</param>
        public void Add(uint value)
        {
            var bytes = ByteConverter.ToBytes(value, this.Endian);
            this.Add(bytes);
        }

        /// <summary>
        /// 将64位整数转换为byte数组再添加
        /// </summary>
        /// <param name="value">整数</param>
        public void Add(long value)
        {
            var bytes = ByteConverter.ToBytes(value, this.Endian);
            this.Add(bytes);
        }

        /// <summary>
        /// 将64位整数转换为byte数组再添加
        /// </summary>
        /// <param name="value">整数</param>
        public void Add(ulong value)
        {
            var bytes = ByteConverter.ToBytes(value, this.Endian);
            this.Add(bytes);
        }

        /// <summary>
        /// 添加指定数据数组
        /// </summary>
        /// <param name="array">数组</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public void Add(byte[] array)
        {
            if (array == null || array.Length == 0)
            {
                return;
            }
            this.Add(array, 0, array.Length);
        }

        /// <summary>
        /// 添加指定数据数组
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="offset">数组的偏移量</param>
        /// <param name="count">字节数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Add(byte[] array, int offset, int count)
        {
            if (array == null || array.Length == 0)
            {
                return;
            }

            if (offset < 0 || offset > array.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "offset值无效");
            }

            if (count < 0 || (offset + count) > array.Length)
            {
                throw new ArgumentOutOfRangeException("count", "count值无效");
            }
            int newLength = this.Length + count;
            this.ExpandCapacity(newLength);

            Buffer.BlockCopy(array, offset, this._buffer, this.Length, count);
            this.Length = newLength;
        }

        /// <summary>
        /// 扩容
        /// </summary>
        /// <param name="newLength">满足的新大小</param>
        private void ExpandCapacity(int newLength)
        {
            if (newLength <= this._capacity)
            {
                return;
            }

            if (this._capacity == 0)
            {
                this._capacity = 64;
            }

            while (newLength > this._capacity)
            {
                this._capacity = this._capacity * 2;
            }

            var newBuffer = new byte[this._capacity];
            if (this.Length > 0)
            {
                Buffer.BlockCopy(this._buffer, 0, newBuffer, 0, this.Length);
            }
            this._buffer = newBuffer;
        }

        /// <summary>
        /// 获取或设置指定位置的字节
        /// </summary>
        /// <param name="index">索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this._buffer[index];
            }
            set
            {
                if (index < 0 || index >= this.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this._buffer[index] = value;
            }
        }

        /// <summary>
        /// 将指定长度的数据复制到目标数组
        /// </summary>
        /// <param name="dstArray">目标数组</param>
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(byte[] dstArray, int count)
        {
            this.CopyTo(dstArray, 0, count);
        }

        /// <summary>
        /// 将指定长度的数据复制到目标数组
        /// </summary>
        /// <param name="dstArray">目标数组</param>
        /// <param name="dstOffset">目标数组偏移量</param>
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(byte[] dstArray, int dstOffset, int count)
        {
            this.CopyTo(0, dstArray, dstOffset, count);
        }

        /// <summary>
        /// 从指定偏移位置将数据复制到目标数组
        /// </summary>
        /// <param name="srcOffset">偏移量</param>
        /// <param name="dstArray">目标数组</param>
        /// <param name="dstOffset">目标数组偏移量</param>
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(int srcOffset, byte[] dstArray, int dstOffset, int count)
        {
            Buffer.BlockCopy(this._buffer, srcOffset, dstArray, dstOffset, count);
        }

        /// <summary>
        /// 转换为byte数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var array = new byte[this.Length];
            this.CopyTo(array, array.Length);
            return array;
        }

        /// <summary>
        /// 转换为ArraySegment类型
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(this._buffer, 0, this.Length);
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private ByteBuilder buidler;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="buidler">查看的对象</param>
            public DebugView(ByteBuilder buidler)
            {
                this.buidler = buidler;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public byte[] Values
            {
                get
                {
                    var array = new byte[buidler.Length];
                    buidler.CopyTo(array, buidler.Length);
                    return array;
                }
            }
        }
    }
}