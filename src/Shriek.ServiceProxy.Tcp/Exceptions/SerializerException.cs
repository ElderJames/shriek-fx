using System;

namespace Shriek.ServiceProxy.Tcp.Exceptions
{
    /// <summary>
    /// 表示序列化或反序列化过程中产生的异常
    /// </summary>
    public class SerializerException : Exception
    {
        /// <summary>
        /// 表示序列化或反序列化过程中产生的异常
        /// </summary>
        /// <param name="inner">异常内部</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializerException(Exception inner)
            : base("序列化或反序列化异常", inner)
        {
        }
    }
}