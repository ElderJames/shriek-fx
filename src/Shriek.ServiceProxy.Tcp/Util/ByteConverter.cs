using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Util
{
    /// <summary>
    /// 表示byte类型转换工具类
    /// 提供byte和整型之间的转换
    /// </summary>
    public static class ByteConverter
    {
        /// <summary>
        /// 获取系统字节存储次序
        /// </summary>
        public readonly static Endians Endian = ByteConverter.GetSystemEndian();

        /// <summary>
        /// 系统字节存储次序
        /// </summary>
        /// <returns></returns>
        private static unsafe Endians GetSystemEndian()
        {
            var int32 = 1;
            return *(byte*)&int32 == 1 ? Endians.Little : Endians.Big;
        }

        /// <summary>
        /// 返回由字节数组中指定位置的8个字节转换来的64位有符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static unsafe long ToInt64(byte[] bytes, int startIndex, Endians endian)
        {
            fixed (byte* pbyte = &bytes[startIndex])
            {
                if (endian == Endians.Little)
                {
                    int i1 = (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                    int i2 = (*(pbyte + 4)) | (*(pbyte + 5) << 8) | (*(pbyte + 6) << 16) | (*(pbyte + 7) << 24);
                    return (uint)i1 | ((long)i2 << 32);
                }
                else
                {
                    int i1 = (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                    int i2 = (*(pbyte + 4) << 24) | (*(pbyte + 5) << 16) | (*(pbyte + 6) << 8) | (*(pbyte + 7));
                    return (uint)i2 | ((long)i1 << 32);
                }
            }
        }

        /// <summary>
        /// 返回由字节数组中指定位置的8个字节转换来的64位无符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static ulong ToUInt64(byte[] bytes, int startIndex, Endians endian)
        {
            return (ulong)ToInt64(bytes, startIndex, endian);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的32位有符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public unsafe static int ToInt32(byte[] bytes, int startIndex, Endians endian)
        {
            fixed (byte* pbyte = &bytes[startIndex])
            {
                if (endian == Endians.Little)
                {
                    return (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                }
                else
                {
                    return (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                }
            }
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的32位无符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static uint ToUInt32(byte[] bytes, int startIndex, Endians endian)
        {
            return (uint)ToInt32(bytes, startIndex, endian);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的16位有符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static unsafe short ToInt16(byte[] bytes, int startIndex, Endians endian)
        {
            fixed (byte* pbyte = &bytes[startIndex])
            {
                if (endian == Endians.Little)
                {
                    return (short)((*pbyte) | (*(pbyte + 1) << 8));
                }
                else
                {
                    return (short)((*pbyte << 8) | (*(pbyte + 1)));
                }
            }
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的16位无符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] bytes, int startIndex, Endians endian)
        {
            return (ushort)ToInt16(bytes, startIndex, endian);
        }

        /// <summary>
        /// 返回由64位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(long value, Endians endian)
        {
            byte[] bytes = new byte[8];
            fixed (byte* pbyte = &bytes[0])
            {
                if (endian == Endians.Little)
                {
                    *pbyte = (byte)(value);
                    *(pbyte + 1) = (byte)(value >> 8);
                    *(pbyte + 2) = (byte)(value >> 16);
                    *(pbyte + 3) = (byte)(value >> 24);
                    *(pbyte + 4) = (byte)(value >> 32);
                    *(pbyte + 5) = (byte)(value >> 40);
                    *(pbyte + 6) = (byte)(value >> 48);
                    *(pbyte + 7) = (byte)(value >> 56);
                }
                else
                {
                    *(pbyte + 7) = (byte)(value);
                    *(pbyte + 6) = (byte)(value >> 8);
                    *(pbyte + 5) = (byte)(value >> 16);
                    *(pbyte + 4) = (byte)(value >> 24);
                    *(pbyte + 3) = (byte)(value >> 32);
                    *(pbyte + 2) = (byte)(value >> 40);
                    *(pbyte + 1) = (byte)(value >> 48);
                    *pbyte = (byte)(value >> 56);
                }
            }
            return bytes;
        }

        /// <summary>
        /// 返回由64位无符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(ulong value, Endians endian)
        {
            return ToBytes((long)value, endian);
        }

        /// <summary>
        /// 返回由32位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(int value, Endians endian)
        {
            byte[] bytes = new byte[4];
            fixed (byte* pbyte = &bytes[0])
            {
                if (endian == Endians.Little)
                {
                    *pbyte = (byte)(value);
                    *(pbyte + 1) = (byte)(value >> 8);
                    *(pbyte + 2) = (byte)(value >> 16);
                    *(pbyte + 3) = (byte)(value >> 24);
                }
                else
                {
                    *(pbyte + 3) = (byte)(value);
                    *(pbyte + 2) = (byte)(value >> 8);
                    *(pbyte + 1) = (byte)(value >> 16);
                    *pbyte = (byte)(value >> 24);
                }
            }
            return bytes;
        }

        /// <summary>
        /// 返回由32位无符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(uint value, Endians endian)
        {
            return ToBytes((int)value, endian);
        }

        /// <summary>
        /// 返回由16位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(short value, Endians endian)
        {
            byte[] bytes = new byte[2];
            fixed (byte* pbyte = &bytes[0])
            {
                if (endian == Endians.Little)
                {
                    *pbyte = (byte)(value);
                    *(pbyte + 1) = (byte)(value >> 8);
                }
                else
                {
                    *(pbyte + 1) = (byte)(value);
                    *pbyte = (byte)(value >> 8);
                }
            }
            return bytes;
        }

        /// <summary>
        /// 返回由16位无符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(ushort value, Endians endian)
        {
            return ToBytes((short)value, endian);
        }
    }
}