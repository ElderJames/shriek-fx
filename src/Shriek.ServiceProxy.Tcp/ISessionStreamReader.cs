using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 定义会话收到的数据流读取接口
    /// </summary>
    public interface ISessionStreamReader
    {
        /// <summary>
        /// 获取同步锁对象
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// 获取用字节表示的流长度
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 获取或设置流中的当前位置
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// 获取指定位置的字节
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        byte this[int index] { get; }

        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前推进一个字节
        /// </summary>
        /// <returns></returns>
        bool ReadBoolean();

        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前推进一个字节，如果已到达流的末尾，则返回 -1
        /// </summary>
        /// <returns></returns>
        byte ReadByte();

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其Int16的Endians.Big表示类型
        /// </summary>
        /// <returns></returns>
        short ReadInt16();

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其Int16表示类型
        /// </summary>
        /// <param name="endian">字节序</param>
        /// <returns></returns>
        short ReadInt16(Endians endian);

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其UInt16的Endians.Big表示类型
        /// </summary>
        /// <returns></returns>
        ushort ReadUInt16();

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其UInt16表示类型
        /// </summary>
        /// <param name="endian">字节序</param>
        /// <returns></returns>
        ushort ReadUInt16(Endians endian);

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其Int32的Endians.Big表示类型
        /// </summary>
        /// <returns></returns>
        int ReadInt32();

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其Int32表示类型
        /// </summary>
        /// <param name="endian">字节序</param>
        /// <returns></returns>
        int ReadInt32(Endians endian);

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其UInt32的Endians.Big表示类型
        /// </summary>
        /// <returns></returns>
        uint ReadUInt32();

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其UInt32表示类型
        /// </summary>
        /// <param name="endian">字节序</param>
        /// <returns></returns>
        uint ReadUInt32(Endians endian);

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其Int64的Endians.Big表示类型
        /// </summary>
        /// <returns></returns>
        long ReadInt64();

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其Int64表示类型
        /// </summary>
        /// <param name="endian">字节序</param>
        /// <returns></returns>
        long ReadInt64(Endians endian);

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其UInt64的Endians.Big表示类型
        /// </summary>
        /// <returns></returns>
        ulong ReadUInt64();

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其UInt64表示类型
        /// </summary>
        /// <param name="endian">字节序</param>
        /// <returns></returns>
        ulong ReadUInt64(Endians endian);

        /// <summary>
        /// 从流中读取到末尾的字节，并将流内的位置向前推进相同的字节
        /// </summary>
        /// <returns></returns>
        byte[] ReadArray();

        /// <summary>
        /// 从流中读取count字节，并将流内的位置向前推进count字节
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        byte[] ReadArray(int count);

        /// <summary>
        /// 从流中读取Position到末尾的所有字节，并将流内的位置推到末尾
        /// 返回以指定编码转换的字符串
        /// </summary>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        string ReadString(Encoding encode);

        /// <summary>
        /// 从流中读取count字节，并将流内的位置向前推进count字节
        /// 返回以指定编码转换的字符串
        /// </summary>
        /// <param name="encode">编码</param>
        /// <param name="count">字节数</param>
        /// <returns></returns>
        string ReadString(Encoding encode, int count);

        /// <summary>
        /// 清空流的所有数据
        /// </summary>
        void Clear();

        /// <summary>
        /// 从开始位置清除数据
        /// </summary>
        /// <param name="count">清除的字节数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void Clear(int count);

        /// <summary>
        /// 从Position位置开始查找第一个匹配的值
        /// 返回相对于Position的偏移量
        /// </summary>
        /// <param name="bin">要匹配的数据</param>
        /// <returns></returns>
        int IndexOf(byte[] bin);

        /// <summary>
        /// 从Position位置开始查找第一个匹配的值
        /// 返回相对于Position的偏移量
        /// </summary>
        /// <param name="b">要匹配的数据</param>
        int IndexOf(byte b);

        /// <summary>
        /// 从Position位置开始
        /// 是否开始包含bin
        /// </summary>
        /// <param name="bin">要匹配的数据</param>
        /// <returns></returns>
        bool StartWith(byte[] bin);
    }
}