using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 定义会话包装对象的行为
    /// </summary>
    public interface IWrapper
    {
        /// <summary>
        /// 还原到包装前
        /// </summary>
        /// <returns></returns>
        ISession UnWrap();
    }
}