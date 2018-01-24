using System;
using System.Text;

namespace Shriek.ServiceProxy.Socket.Core.Internal
{
    /// <summary>
    /// 默认提供的二进制序列化工具
    /// </summary>
    internal sealed class DefaultSerializer : ISerializer
    {
        /// <summary>
        /// json序列化
        /// </summary>
        private readonly DefaultDynamicJsonSerializer jsonSerializer = new DefaultDynamicJsonSerializer();

        /// <summary>
        /// 序列化为二进制
        /// </summary>
        /// <param name="model">实体</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public byte[] Serialize(object model)
        {
            var json = this.jsonSerializer.Serialize(model);
            return json == null ? null : Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// 反序列化为实体
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="type">实体类型</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public object Deserialize(byte[] bytes, Type type)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(bytes);
            return this.jsonSerializer.Deserialize(json, type);
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "JavaScriptSerializer";
        }
    }
}