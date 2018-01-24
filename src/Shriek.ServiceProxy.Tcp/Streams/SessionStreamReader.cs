using Shriek.ServiceProxy.Socket.Util;
using System;
using System.Diagnostics;
using System.Text;

namespace Shriek.ServiceProxy.Socket.Streams
{
    /// <summary>
    /// 提供对内存流读取
    /// 非线程安全类型
    /// </summary>
    [DebuggerDisplay("Position = {Position}, Length = {Length}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class SessionStreamReader : ISessionStreamReader
    {
        /// <summary>
        /// 获取所读取的数据流对象
        /// </summary>
        public SessionStream Stream { get; private set; }

        /// <summary>
        /// 获取同步锁对象
        /// </summary>
        public object SyncRoot { get; private set; }

        /// <summary>
        /// 获取用字节表示的流长度
        /// </summary>
        public int Length
        {
            get
            {
                return (int)this.Stream.Length;
            }
        }

        /// <summary>
        /// 获取或设置流中的当前位置
        /// </summary>
        public int Position
        {
            get
            {
                return (int)this.Stream.Position;
            }
            set
            {
                this.Stream.Position = value;
            }
        }

        /// <summary>
        /// 获取指定位置的字节
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
                return this.Stream.GetBuffer()[index];
            }
        }

        /// <summary>
        /// 对内存流读取
        /// </summary>
        /// <param name="stream">会话数据流</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SessionStreamReader(SessionStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }
            this.Stream = stream;
            this.SyncRoot = new object();
        }

        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前推进一个字节
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return this.Stream.ReadByte() != 0;
        }

        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前推进一个字节，如果已到达流的末尾，则返回 -1
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public byte ReadByte()
        {
            return (byte)this.Stream.ReadByte();
        }

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其Int16表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public short ReadInt16()
        {
            return this.ReadInt16(Endians.Big);
        }

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其Int16表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public short ReadInt16(Endians endian)
        {
            var range = this.ReadArraySegment(sizeof(short));
            return ByteConverter.ToInt16(range.Array, range.Offset, endian);
        }

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其UInt16表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ushort ReadUInt16()
        {
            return this.ReadUInt16(Endians.Big);
        }

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其UInt16表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ushort ReadUInt16(Endians endian)
        {
            var range = this.ReadArraySegment(sizeof(ushort));
            return ByteConverter.ToUInt16(range.Array, range.Offset, endian);
        }

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其Int32表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public int ReadInt32()
        {
            return this.ReadInt32(Endians.Big);
        }

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其Int32表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public int ReadInt32(Endians endian)
        {
            var range = this.ReadArraySegment(sizeof(int));
            return ByteConverter.ToInt32(range.Array, range.Offset, endian);
        }

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其UInt32表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            return this.ReadUInt32(Endians.Big);
        }

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其UInt32表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadUInt32(Endians endian)
        {
            var range = this.ReadArraySegment(sizeof(uint));
            return ByteConverter.ToUInt32(range.Array, range.Offset, endian);
        }

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其Int64表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public long ReadInt64()
        {
            return this.ReadInt64(Endians.Big);
        }

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其Int64表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public long ReadInt64(Endians endian)
        {
            var range = this.ReadArraySegment(sizeof(long));
            return ByteConverter.ToInt64(range.Array, range.Offset, endian);
        }

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其UInt64表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            return this.ReadUInt64(Endians.Big);
        }

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其UInt64表示类型
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong ReadUInt64(Endians endian)
        {
            var range = this.ReadArraySegment(sizeof(ulong));
            return ByteConverter.ToUInt64(range.Array, range.Offset, endian);
        }

        /// <summary>
        /// 从流中读取到末尾的字节，并将流内的位置向前推进相同的字节
        /// </summary>
        /// <returns></returns>
        public byte[] ReadArray()
        {
            return this.ReadArray((this.Length - this.Position));
        }

        /// <summary>
        /// 从流中读取count字节，并将流内的位置向前推进count字节
        /// </summary>
        /// <param name="count">要读取的字节数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public byte[] ReadArray(int count)
        {
            var range = this.ReadArraySegment(count);
            var bytes = new byte[count];

            Buffer.BlockCopy(range.Array, range.Offset, bytes, 0, count);
            return bytes;
        }

        /// <summary>
        /// 从流中读取Position到末尾的所有字节，并将流内的位置推到末尾
        /// 返回以指定编码转换的字符串
        /// </summary>
        /// <param name="encode">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string ReadString(Encoding encode)
        {
            return this.ReadString(encode, this.Length - this.Position);
        }

        /// <summary>
        /// 从流中读取count字节，并将流内的位置向前推进count字节
        /// 返回以指定编码转换的字符串
        /// </summary>
        /// <param name="count">字节数</param>
        /// <param name="encode">编码</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string ReadString(Encoding encode, int count)
        {
            if (encode == null)
            {
                throw new ArgumentNullException();
            }

            var range = this.ReadArraySegment(count);
            return encode.GetString(range.Array, range.Offset, range.Count);
        }

        /// <summary>
        /// 从流中读取count字节的范围标记
        /// 并将流内的位置向前推进count个字节
        /// </summary>
        /// <param name="count">字节数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private ArraySegment<byte> ReadArraySegment(int count)
        {
            var range = new ArraySegment<byte>(this.Stream.GetBuffer(), this.Position, count);
            this.Position = this.Position + count;
            return range;
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            this.Position = 0;
            this.Stream.SetLength(0L);
        }

        /// <summary>
        /// 从开始位置清除数据
        /// </summary>
        /// <param name="count">清除的字节数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Clear(int count)
        {
            if (count < 0 || count > this.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            var length = this.Length - count;
            if (length > 0)
            {
                this.CopyTo(count, this.Stream.GetBuffer(), 0, length);
            }

            this.Position = 0;
            this.Stream.SetLength(length);
        }

        /// <summary>
        /// 从开始位置将指定长度的数据复制到目标数组
        /// </summary>
        /// <param name="dstArray">目标数组</param>
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(byte[] dstArray, int count)
        {
            this.CopyTo(0, dstArray, 0, count);
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
            Buffer.BlockCopy(this.Stream.GetBuffer(), srcOffset, dstArray, dstOffset, count);
        }

        /// <summary>
        /// 从Position位置开始查找第一个匹配的值
        /// 返回相对于Position的偏移量
        /// </summary>
        /// <param name="bin">要匹配的数据</param>
        /// <returns></returns>
        public unsafe int IndexOf(byte[] bin)
        {
            if (bin == null || bin.Length == 0)
            {
                return -1;
            }

            if (this.Position + bin.Length > this.Length)
            {
                return -1;
            }

            var count = this.Length - bin.Length - this.Position;
            fixed (byte* pBytes = &this.Stream.GetBuffer()[this.Position], pBin = &bin[0])
            {
                for (var i = 0; i <= count; i++)
                {
                    if (this.StartWith(pBytes + i, pBin, bin.Length) == true)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 从Position位置开始查找第一个匹配的值
        /// 返回相对于Position的偏移量
        /// </summary>
        /// <param name="b">要匹配的数据</param>
        public unsafe int IndexOf(byte b)
        {
            fixed (byte* pBytes = &this.Stream.GetBuffer()[this.Position])
            {
                var length = this.Length - this.Position;
                for (var i = 0; i < length; i++)
                {
                    if (*(pBytes + i) == b)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 从Position位置开始
        /// 是否开始包含bin
        /// </summary>
        /// <param name="bin">要匹配的数据</param>
        /// <returns></returns>
        public unsafe bool StartWith(byte[] bin)
        {
            if (bin == null || bin.Length == 0)
            {
                return false;
            }

            if (this.Position + bin.Length > this.Length)
            {
                return false;
            }

            fixed (byte* pBytes = &this.Stream.GetBuffer()[this.Position], pBin = &bin[0])
            {
                return this.StartWith(pBytes, pBin, bin.Length);
            }
        }

        /// <summary>
        /// 是否开始包含pBin
        /// </summary>
        /// <param name="pBytes">数据源</param>
        /// <param name="pBin">要匹配的数据</param>
        /// <param name="length">匹配长度</param>
        /// <returns></returns>
        private unsafe bool StartWith(byte* pBytes, byte* pBin, int length)
        {
            for (var i = 0; i < length; i++)
            {
                if (*(pBytes + i) != *(pBin + i))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private SessionStreamReader view;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="view">查看的对象</param>
            public DebugView(SessionStreamReader view)
            {
                this.view = view;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public byte[] Values
            {
                get
                {
                    var array = new byte[view.Length];
                    view.CopyTo(array, view.Length);
                    return array;
                }
            }
        }
    }
}