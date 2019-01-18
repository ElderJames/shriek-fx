using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Shriek
{
    /// <summary>
    /// 表示线程安全的内存缓存
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public class ConcurrentCache<TKey, TValue>
    {
        /// <summary>
        /// 线程安全字典
        /// </summary>
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> dictionary;

        /// <summary>
        /// 线程安全的内存缓存
        /// </summary>
        public ConcurrentCache()
        {
            this.dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        /// <summary>
        /// 线程安全的内存缓存
        /// </summary>
        /// <param name="comparer">键的比较器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConcurrentCache(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>(comparer);
        }

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valueFactory">生成缓存内容的委托</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return this.dictionary
                .GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication))
                .Value;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            return this.dictionary
                .GetOrAdd(key, k => new Lazy<TValue>(() => value, LazyThreadSafetyMode.ExecutionAndPublication))
                .Value;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.dictionary.TryGetValue(key, out Lazy<TValue> lazyvalue))
            {
                value = lazyvalue.Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (dictionary.ContainsKey(key))
                    return dictionary[key].Value;

                return default(TValue);
            }
            set
            {
                dictionary.AddOrUpdate(key,
                    k => new Lazy<TValue>(() => value, LazyThreadSafetyMode.ExecutionAndPublication),
                (k, v) => new Lazy<TValue>(() => value, LazyThreadSafetyMode.ExecutionAndPublication));
            }
        }

        public void AddOrUpdate(TKey key, TValue value, Func<TKey, TValue, TValue> action)
        {
            dictionary.AddOrUpdate(key, new Lazy<TValue>(() => value, LazyThreadSafetyMode.ExecutionAndPublication),
                (k, v) => new Lazy<TValue>(() => action(k, v.Value), LazyThreadSafetyMode.ExecutionAndPublication));
        }
    }
}