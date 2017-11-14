using System;

namespace Shriek.ServiceProxy.Tcp.Exceptions
{
    /// <summary>
    /// 表示Api执行异常
    /// </summary>
    public class ApiExecuteException : Exception
    {
        /// <summary>
        /// Api执行异常
        /// </summary>
        /// <param name="innerException">内部异常</param>
        public ApiExecuteException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.InnerException.ToString();
        }
    }
}