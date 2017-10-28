using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示用户附加数据
    /// </summary>   
    internal class DefaultTag : ConcurrentDictionary<string, object>, ITag
    {
        /// <summary>
        /// 获取或设置唯一标识符
        /// </summary>
        string ITag.ID { get; set; }

        /// <summary>
        /// 表示用户附加数据
        /// </summary>
        public DefaultTag()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void ITag.Set(string key, object value)
        {
            base.AddOrUpdate(key, value, (k, v) => value);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        TagItem ITag.Get(string key)
        {
            object value;
            base.TryGetValue(key, out value);
            return new TagItem(value);
        }

        /// <summary>
        /// 获取值
        /// 如果指定的键存在则返回键的值
        /// 如果指定的键不存在，则将添加并返回valueFactory的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valueFactory">值</param>
        TagItem ITag.Get(string key, Func<object> valueFactory)
        {
            var value = base.GetOrAdd(key, (k) => valueFactory());
            return new TagItem(value);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ITag.Remove(string key)
        {
            object value;
            return base.TryRemove(key, out value);
        }
    }
}
