using Shriek.Converter.Converts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shriek.Converter
{
    /// <summary>
    /// 提供类型转换
    /// </summary>
    public sealed class Converter
    {
        /// <summary>
        /// 转换器静态实例
        /// </summary>
        private static readonly Converter Instance = new Converter();

        /// <summary>
        /// 支持基础类型、decimal、guid和枚举相互转换以及这些类型的可空类型和数组类型相互转换
        /// 支持字典和DynamicObject转换为对象以及字典和DynamicObject的数组转换为对象数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">值</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static T Cast<T>(object value)
        {
            return Instance.Convert<T>(value);
        }

        /// <summary>
        /// 支持基础类型、decimal、guid和枚举相互转换以及这些类型的可空类型和数组类型相互转换
        /// 支持字典和DynamicObject转换为对象以及字典和DynamicObject的数组转换为对象数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static object Cast(object value, Type targetType)
        {
            return Instance.Convert(value, targetType);
        }

        /// <summary>
        /// 获取转换单元操控对象
        /// </summary>
        public ContertItems Items { get; private set; }

        /// <summary>
        /// 类型转换
        /// </summary>
        public Converter()
        {
            this.Items = new ContertItems()
                .AddLast<NoConvert>()
                .AddLast<NullConvert>()
                .AddLast<PrimitiveContert>()
                .AddLast<NullableConvert>()
                .AddLast<DictionaryConvert>()
                .AddLast<ArrayConvert>()
                .AddLast<DynamicObjectConvert>();
        }

        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>转换后的值</returns>
        public T Convert<T>(object value)
        {
            return (T)this.Convert(value, typeof(T));
        }

        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">要转换的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>转换后的值</returns>
        public object Convert(object value, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            foreach (IConvert item in this.Items)
            {
                if (item.Convert(this, value, targetType, out var result))
                {
                    return result;
                }
            }

            var message = string.Format("不支持将{0}转换为{1}", value, targetType.Name);
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// 表示转换器的转换单元合集
        /// </summary>
        [DebuggerTypeProxy(typeof(DebugView))]
        public class ContertItems : IEnumerable
        {
            /// <summary>
            /// 转换单元列表
            /// </summary>
            private readonly List<IConvert> converts = new List<IConvert>();

            /// <summary>
            /// 插入到指定位置
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="index">索引</param>
            private void Insert<T>(int index) where T : IConvert
            {
                if (this.converts.Any(item => item.GetType() == typeof(T)))
                    return;

                var convert = Activator.CreateInstance<T>();
                this.converts.Insert(index, convert);
            }

            /// <summary>
            /// 添加一个转换单元到最前面
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddFrist<T>() where T : IConvert
            {
                this.Insert<T>(0);
                return this;
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">要被替换掉的转换单元</typeparam>
            /// <typeparam name="TDest">替换后的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddBefore<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                var index = this.converts.FindIndex(item => item.GetType() == typeof(TSource));
                if (index > -1)
                {
                    this.Insert<TDest>(index);
                }
                return this;
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">要被替换掉的转换单元</typeparam>
            /// <typeparam name="TDest">替换后的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddAfter<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                var index = this.converts.FindIndex(item => item.GetType() == typeof(TSource));
                if (index > -1)
                {
                    this.Insert<TDest>(index + 1);
                }
                return this;
            }

            /// <summary>
            /// 添加一个转换单元到末尾
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddLast<T>() where T : IConvert
            {
                if (this.converts.Any(item => item.GetType() == typeof(T)))
                    return this;

                var convert = Activator.CreateInstance<T>();
                this.converts.Add(convert);
                return this;
            }

            /// <summary>
            /// 解绑一个转换单元
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems Remove<T>() where T : IConvert
            {
                var convert = this.converts.FirstOrDefault(item => item.GetType() == typeof(T));
                if (convert != null)
                {
                    this.converts.Remove(convert);
                }
                return this;
            }

            /// <summary>
            /// 替换转换单元
            /// </summary>
            /// <typeparam name="TSource">要被替换掉的转换单元</typeparam>
            /// <typeparam name="TDest">替换后的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems Repace<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                var index = this.converts.FindIndex(item => item.GetType() == typeof(TSource));
                if (index <= -1 || this.converts.Any(item => item.GetType() == typeof(TDest)))
                    return this;

                var convert = Activator.CreateInstance<TDest>();
                this.converts[index] = convert;
                return this;
            }

            /// <summary>
            /// 获取迭代器
            /// </summary>
            /// <returns></returns>
            public IEnumerator GetEnumerator()
            {
                return this.converts.GetEnumerator();
            }

            #region DebugView

            /// <summary>
            /// 调试视图
            /// </summary>
            private class DebugView
            {
                /// <summary>
                /// 查看的对象
                /// </summary>
                private readonly ContertItems view;

                /// <summary>
                /// 调试视图
                /// </summary>
                /// <param name="view">查看的对象</param>
                public DebugView(ContertItems view)
                {
                    this.view = view;
                }

                /// <summary>
                /// 查看的内容
                /// </summary>
                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public IConvert[] Values => view.converts.ToArray();
            }

            #endregion DebugView
        }
    }
}