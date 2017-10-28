using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示字节存储次序枚举
    /// </summary>
    public enum Endians
    {
        /// <summary>
        /// 高位在前
        /// </summary>
        Big,
        /// <summary>
        /// 低位在前
        /// </summary>
        Little
    }
}
