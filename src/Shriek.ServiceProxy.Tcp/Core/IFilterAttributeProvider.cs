using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 特性过滤器提供者
    /// </summary>
    public interface IFilterAttributeProvider
    {
        /// <summary>
        /// 获取Api行为的过滤器
        /// 不包括全局过滤器
        /// </summary>
        /// <param name="apiAction">Api行为</param>
        /// <returns></returns>
        IEnumerable<IFilter> GetActionFilters(ApiAction apiAction);
    }
}
