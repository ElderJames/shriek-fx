using System;

namespace Shriek.ServiceProxy.Socket.Core
{
    /// <summary>
    /// 定义对象的化进制序列化与反序列化的接口
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 序列化为二进制
        /// 异常时抛出SerializerException
        /// </summary>
        /// <param name="model">实体</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        byte[] Serialize(object model);

        /// <summary>
        /// 反序列化为实体
        /// 异常时抛出SerializerException
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="type">实体类型</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        object Deserialize(byte[] bytes, Type type);
    }
}