using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Shriek.Reflection
{
    /// <summary>
    /// 提供类型扩展
    /// </summary>
    public static partial class TypeExtend
    {
        /// <summary>
        /// 类型信息缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, InfoEx> cached = new ConcurrentCache<Type, InfoEx>();

        /// <summary>
        /// 是否为简单类型 指基础类型、guid以及对应的可空类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsSimple(this Type type)
        {
            return type.GetInfoEx().IsSimple;
        }

        /// <summary>
        /// 是否为数组或ListOf(T)类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsArrayOrList(this Type type)
        {
            return type.GetInfoEx().IsArrayOrList;
        }

        /// <summary>
        /// 是否为复杂类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsComplexClass(this Type type)
        {
            return type.GetInfoEx().IsComplexClass;
        }

        /// <summary>
        /// 获取类型信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static InfoEx GetInfoEx(this Type type)
        {
            return cached.GetOrAdd(type, (t) => new InfoEx(t));
        }

        /// <summary>
        /// 表示类型信息
        /// </summary>
        private class InfoEx
        {
            /// <summary>
            /// 是否为简单类型
            /// </summary>
            public bool IsSimple { get; private set; }

            /// <summary>
            /// 是否为数组或ListOf(T)类型
            /// </summary>
            public bool IsArrayOrList { get; private set; }

            /// <summary>
            /// 是否为复杂类型
            /// </summary>
            public bool IsComplexClass { get; private set; }

            /// <summary>
            /// 类型信息
            /// </summary>
            /// <param name="type">类型</param>
            public InfoEx(Type type)
            {
                this.IsSimple = IsSimpleType(type);
                this.IsArrayOrList = IsArrayOrListType(type);
                this.IsComplexClass = type.IsClass && !this.IsSimple && !this.IsArrayOrList;
            }

            /// <summary>
            /// 类型简单类型
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns></returns>
            private static bool IsSimpleType(Type type)
            {
                if (typeof(IConvertible).IsAssignableFrom(type) == true)
                {
                    return true;
                }

                if (typeof(Guid) == type)
                {
                    return true;
                }

                var underlyingtype = Nullable.GetUnderlyingType(type);
                if (underlyingtype != null)
                {
                    return IsSimpleType(underlyingtype);
                }
                return false;
            }

            /// <summary>
            /// 是否为数组或ListOf(T)类型
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns></returns>
            private static bool IsArrayOrListType(Type type)
            {
                if (type.IsArray == true)
                {
                    return true;
                }

                if (type.IsGenericType == false)
                {
                    return false;
                }

                var defindtionType = type.GetGenericTypeDefinition();
                return defindtionType == typeof(List<>);
            }
        }
    }
}