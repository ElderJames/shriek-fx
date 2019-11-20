using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shriek
{
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Converts all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns></returns>
        public static TResult[] ConvertAll<T, TResult>(this T[] items, Converter<T, TResult> transformation)
        {
            return Array.ConvertAll(items, transformation);
        }

        /// <summary>
        ///     Finds the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static T Find<T>(this T[] items, Predicate<T> predicate)
        {
            return Array.Find(items, predicate);
        }

        /// <summary>
        ///     Finds all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] items, Predicate<T> predicate)
        {
            return Array.FindAll(items, predicate);
        }

        /// <summary>
        ///     Fors the each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                return;
            }

            foreach (T obj in items)
            {
                action(obj);
            }
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> func)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count(); i++)
            {
                await func(items.ElementAt(i));
            }
        }

        public static ParallelLoopResult ForEachParallel<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return Parallel.ForEach(items, action);
        }

        /// <summary>
        ///     Checks whether or not collection is null or empty. Assumes colleciton can be safely enumerated multiple times.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <returns>
        ///     <c>true</c> if [is null or empty] [the specified this]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this IEnumerable @this)
        {
            if (@this != null)
            {
                return !@this.GetEnumerator().MoveNext();
            }

            return true;
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, IEnumerable<T> list)
        {
            return list.Contains(item);
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
            where T : class
        {
            return source.Distinct(new DynamicEqualityComparer<T>(comparer));
        }

        public static IEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, TKey, int> comparer)
            where T : class
            where TKey : class
        {
            return source.OrderBy(keySelector, new DynamicComparer<TKey>(comparer));
        }

        public static IEnumerable<T> OrderByDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, TKey, int> comparer)
            where T : class
            where TKey : class
        {
            return source.OrderByDescending(keySelector, new DynamicComparer<TKey>(comparer));
        }

        private sealed class DynamicEqualityComparer<T> : IEqualityComparer<T>
            where T : class
        {
            private readonly Func<T, T, bool> func;

            public DynamicEqualityComparer(Func<T, T, bool> func)
            {
                this.func = func;
            }

            public bool Equals(T x, T y)
            {
                return this.func(x, y);
            }

            public int GetHashCode(T obj)
            {
                return 0;
            }
        }

        private sealed class DynamicComparer<T> : IComparer<T> where T : class
        {
            private readonly Func<T, T, int> func;

            public DynamicComparer(Func<T, T, int> func)
            {
                this.func = func;
            }

            public int Compare(T x, T y)
            {
                return this.func(x, y);
            }
        }
    }
}