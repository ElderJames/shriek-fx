using System;
using System.Diagnostics;

namespace Shriek.ServiceProxy.Socket.Exceptions
{
    /// <summary>
    /// 表示Api不存在引发的异常
    /// </summary>
    [DebuggerDisplay("Message = {Message}")]
    public class ApiNotExistException : Exception
    {
        /// <summary>
        /// 获取Api名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Api不存在引发的异常
        /// </summary>
        /// <param name="name">Api名称</param>
        public ApiNotExistException(string name)
            : base(string.Format("请求的{0}不存在", name))
        {
            this.Name = name;
        }
    }
}