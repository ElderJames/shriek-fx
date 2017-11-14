using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 全局过滤器管理者的基础类
    /// </summary>
    internal class GlobalFiltersBase : IGlobalFilters
    {
        /// <summary>
        /// 获取过过滤器过滤器
        /// </summary>
        private readonly List<IFilter> fiters = new List<IFilter>();

        /// <summary>
        /// 获取过滤器元素个数
        /// </summary>
        public int Count
        {
            get
            {
                return this.fiters.Count;
            }
        }

        /// <summary>
        /// 获取或设置过滤器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public IFilter this[int index]
        {
            get
            {
                return this.fiters[index];
            }
            set
            {
                this.fiters[index] = value;
            }
        }

        /// <summary>
        /// 添加过滤器并按Order字段排序
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public virtual void Add(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException();
            }

            if (filter.AllowMultiple == false && this.fiters.Any(item => item.GetType() == filter.GetType()))
            {
                throw new ArgumentException(string.Format("类型为{0}过滤器不允许多个实例 ..", filter.GetType().Name));
            }

            this.fiters.Add(filter);
            this.fiters.Sort(new FilterComparer());
        }

        /// <summary>
        /// 移除某类型的过滤器实例
        /// </summary>
        /// <param name="filterType">过滤器的类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Remove(Type filterType)
        {
            if (filterType == null)
            {
                throw new ArgumentNullException();
            }

            for (var i = 0; i < this.fiters.Count; i++)
            {
                if (this.fiters[i].GetType() == filterType)
                {
                    this.fiters.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.fiters.GetEnumerator();
        }

        /// <summary>
        /// 过滤器比较器
        /// </summary>
        private class FilterComparer : IComparer<IFilter>
        {
            /// <summary>
            /// 指示要比较的对象的相对顺序
            /// 值含义小于零x 小于 y。零x 等于 y。大于零x 大于 y
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(IFilter x, IFilter y)
            {
                if (x == null || y == null || x.Order == y.Order)
                {
                    return 0;
                }

                if (x.Order < y.Order)
                {
                    return -1;
                }
                return 1;
            }
        }
    }
}