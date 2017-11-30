using Shriek.ServiceProxy.Tcp.Core;
using Shriek.ServiceProxy.Tcp.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Fast
{
    /// <summary>
    /// 表示Fast协议的封包
    /// </summary>
    [DebuggerDisplay("ApiName = {ApiName}")]
    public sealed class FastPacket
    {
        /// <summary>
        /// 获取fast协议封包标记
        /// 字符表示为$
        /// </summary>
        public static readonly byte Mark = 36;

        /// <summary>
        /// 获取封包的字节长度
        /// </summary>
        public int TotalBytes { get; private set; }

        /// <summary>
        /// 获取api名称长度
        /// </summary>
        public byte ApiNameLength { get; private set; }

        /// <summary>
        /// 获取api名称
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// 获取封包的唯一标识
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// 获取是否为客户端的封包
        /// </summary>
        public bool IsFromClient { get; private set; }

        /// <summary>
        /// 获取或设置是否异常数据
        /// </summary>
        public bool IsException { get; set; }

        /// <summary>
        /// 获取或设置数据体的数据
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// 通讯协议的封包
        /// </summary>
        /// <param name="api">api名称</param>
        /// <param name="id">标识符</param>
        /// <param name="fromClient">是否为客户端的封包</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FastPacket(string api, long id, bool fromClient)
        {
            if (string.IsNullOrEmpty(api))
            {
                throw new ArgumentNullException("api");
            }
            this.ApiName = api;
            this.Id = id;
            this.IsFromClient = fromClient;
        }

        /// <summary>
        /// 将参数序列化并写入为Body
        /// </summary>
        /// <param name="serializer">序列化工具</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="SerializerException"></exception>
        public void SetBodyParameters(ISerializer serializer, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return;
            }
            var builder = new ByteBuilder(Endians.Big);
            foreach (var item in parameters)
            {
                // 序列化参数为二进制内容
                var paramBytes = serializer.Serialize(item);
                // 添加参数内容长度
                builder.Add(paramBytes == null ? 0 : paramBytes.Length);
                // 添加参数内容
                builder.Add(paramBytes);
            }
            this.Body = builder.ToArray();
        }

        /// <summary>
        /// 将Body的数据解析为参数
        /// </summary>
        /// <returns></returns>
        public IList<byte[]> GetBodyParameters()
        {
            var parameterList = new List<byte[]>();

            if (this.Body == null || this.Body.Length < 4)
            {
                return parameterList;
            }

            var index = 0;
            while (index < this.Body.Length)
            {
                // 参数长度
                var length = ByteConverter.ToInt32(this.Body, index, Endians.Big);
                index = index + 4;
                var paramBytes = new byte[length];
                // 复制出参数的数据
                Buffer.BlockCopy(this.Body, index, paramBytes, 0, length);
                index = index + length;
                parameterList.Add(paramBytes);
            }

            return parameterList;
        }

        /// <summary>
        /// 转换为ArraySegment
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> ToArraySegment()
        {
            var apiNameBytes = Encoding.UTF8.GetBytes(this.ApiName);
            var headLength = apiNameBytes.Length + 16;
            this.TotalBytes = this.Body == null ? headLength : headLength + this.Body.Length;

            this.ApiNameLength = (byte)apiNameBytes.Length;
            var builder = new ByteBuilder(Endians.Big);
            builder.Add(FastPacket.Mark);
            builder.Add(this.TotalBytes);
            builder.Add(this.ApiNameLength);
            builder.Add(apiNameBytes);
            builder.Add(this.Id);
            builder.Add(this.IsFromClient);
            builder.Add(this.IsException);
            builder.Add(this.Body);
            return builder.ToArraySegment();
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ApiName;
        }

        /// <summary>
        /// 解析一个数据包
        /// 不足一个封包时返回null
        /// </summary>
        /// <param name="streamReader">数据读取器</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public static bool Parse(ISessionStreamReader streamReader, out FastPacket packet)
        {
            packet = null;
            const int packetMinSize = 16;

            if (streamReader.Length < packetMinSize || streamReader[0] != FastPacket.Mark)
            {
                return false;
            }

            streamReader.Position = 1;
            var totalBytes = streamReader.ReadInt32();
            if (totalBytes < packetMinSize)
            {
                return false;
            }

            // 数据包未接收完整
            if (streamReader.Length < totalBytes)
            {
                return true;
            }

            // api名称数据长度
            var apiNameLength = streamReader.ReadByte();
            if (totalBytes < apiNameLength + packetMinSize)
            {
                return false;
            }

            // api名称数据
            var apiNameBytes = streamReader.ReadArray(apiNameLength);
            // 标识符
            var id = streamReader.ReadInt64();
            // 是否为客户端封包
            var isFromClient = streamReader.ReadBoolean();
            // 是否异常
            var isException = streamReader.ReadBoolean();
            // 实体数据
            var body = streamReader.ReadArray(totalBytes - streamReader.Position);

            // 清空本条数据
            streamReader.Clear(totalBytes);

            var apiName = Encoding.UTF8.GetString(apiNameBytes);
            packet = new FastPacket(apiName, id, isFromClient)
            {
                TotalBytes = totalBytes,
                ApiNameLength = apiNameLength,
                IsException = isException,
                Body = body
            };
            return true;
        }
    }
}