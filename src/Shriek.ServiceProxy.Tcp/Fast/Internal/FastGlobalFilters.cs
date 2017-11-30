using Shriek.ServiceProxy.Tcp.Core;
using System;

namespace Shriek.ServiceProxy.Tcp.Fast
{
    /// <summary>
    /// Fast协议的全局过滤器提供者
    /// </summary>
    internal class FastGlobalFilters : GlobalFiltersBase
    {
        /// <summary>
        /// 添加过滤器
        /// </summary>
        /// <param name="filter"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override void Add(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException();
            }

            var fastFilter = filter as FastFilterAttribute;
            if (fastFilter == null)
            {
                throw new ArgumentException("过滤器的类型要继承于" + typeof(FastFilterAttribute).Name);
            }
            base.Add(filter);
        }
    }
}