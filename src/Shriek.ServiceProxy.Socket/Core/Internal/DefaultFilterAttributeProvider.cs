using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.ServiceProxy.Socket.Core.Internal
{
    /// <summary>
    /// 默认提供的Api行为特性过滤器提供者
    /// </summary>
    internal class DefaultFilterAttributeProvider : IFilterAttributeProvider
    {
        /// <summary>
        /// ApiAction的过滤器缓存
        /// </summary>
        private static readonly ConcurrentDictionary<ApiAction, IFilter[]> filtersCached = new ConcurrentDictionary<ApiAction, IFilter[]>(new ApiActionComparer());

        /// <summary>
        /// 获取Api行为的特性过滤器
        /// </summary>
        /// <param name="apiAction">Api行为</param>
        /// <returns></returns>
        public IEnumerable<IFilter> GetActionFilters(ApiAction apiAction)
        {
            return this.GetActionFilters(apiAction, cached: true);
        }

        /// <summary>
        /// 获取Api行为的特性过滤器
        /// </summary>
        /// <param name="apiAction">Api行为</param>
        /// <param name="cached">是否使用缓存</param>
        /// <returns></returns>
        public IFilter[] GetActionFilters(ApiAction apiAction, bool cached)
        {
            if (cached == true)
            {
                return filtersCached.GetOrAdd(apiAction, (action) => this.GetActionFiltersNoCached(action));
            }
            return this.GetActionFiltersNoCached(apiAction);
        }

        /// <summary>
        /// 获取Api行为的特性过滤器
        /// </summary>
        /// <param name="apiAction">Api行为</param>
        /// <returns></returns>
        private IFilter[] GetActionFiltersNoCached(ApiAction apiAction)
        {
            var paramtersFilters = apiAction.GetParametersFilterAttributes();
            var methodFilters = apiAction.GetMethodFilterAttributes();
            var classFilters = apiAction.GetClassFilterAttributes();

            var allFilters = paramtersFilters
                .Concat(methodFilters)
                .Concat(classFilters)
                .Distinct(new FilterAttributeComparer())
                .OrderBy(f => f.Order)
                .ToArray();

            return allFilters;
        }

        /// <summary>
        /// 获取Api行为的特性过滤器
        /// </summary>
        /// <param name="apiAction">Api行为</param>
        /// <returns></returns>
        [Obsolete("GetActionFiltersNoCached", true)]
        private IFilter[] GetActionFiltersNoCachedOld(ApiAction apiAction)
        {
            var paramtersFilters = apiAction.GetParametersFilterAttributes();
            var methodFilters = apiAction.GetMethodFilterAttributes();
            var classFilters = apiAction.GetClassFilterAttributes()
                .Where(cf => cf.AllowMultiple || methodFilters.Any(mf => mf.TypeId == cf.TypeId) == false);

            var allFilters = paramtersFilters.Concat(methodFilters).Concat(classFilters).OrderBy(f => f.Order).ToArray();
            return allFilters;
        }

        /// <summary>
        /// FilterAttribute比较器
        /// </summary>
        private class FilterAttributeComparer : IEqualityComparer<FilterAttribute>
        {
            public bool Equals(FilterAttribute x, FilterAttribute y)
            {
                // 如果其中一个不允许重复，返回true将y过滤
                return x.AllowMultiple == false || y.AllowMultiple == false;
            }

            public int GetHashCode(FilterAttribute obj)
            {
                return obj.TypeId.GetHashCode();
            }
        }

        /// <summary>
        /// ApiAction比较器
        /// </summary>
        private class ApiActionComparer : IEqualityComparer<ApiAction>
        {
            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(ApiAction x, ApiAction y)
            {
                return x.Method.Equals(y.Method);
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(ApiAction obj)
            {
                return obj.Method.GetHashCode();
            }
        }
    }
}