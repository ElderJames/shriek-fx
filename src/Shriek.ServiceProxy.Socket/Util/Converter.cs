using Shriek.ServiceProxy.Socket.Util.Converts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shriek.ServiceProxy.Socket.Util
{
    /// <summary>
    /// 提供丰富的类型转换功能
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
            return Converter.Instance.Convert<T>(value);
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
            return Converter.Instance.Convert(value, targetType);
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
            this.Items = new ContertItems(this)
                .AddLast<NoConvert>()
                .AddLast<NullConvert>()
                .AddLast<SimpleContert>()
                .AddLast<NullableConvert>()
                .AddLast<DictionaryConvert>()
                .AddLast<ArrayConvert>()
                .AddLast<ListConvert>()
                .AddLast<JTokenConvert>()
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
            return this.Items.First.Convert(value, targetType);
        }

        /// <summary>
        /// 表示转换器的转换单元合集
        /// </summary>
        [DebuggerTypeProxy(typeof(DebugView))]
        public class ContertItems
        {
            /// <summary>
            /// 转换器实例
            /// </summary>
            private readonly Converter converter;

            /// <summary>
            /// 转换单元列表
            /// </summary>
            private readonly LinkedList<IConvert> linkedList = new LinkedList<IConvert>();

            /// <summary>
            /// 获取第一个转换单元
            /// </summary>
            internal IConvert First
            {
                get
                {
                    return this.linkedList.First.Value;
                }
            }

            /// <summary>
            /// 转换单元合集
            /// </summary>
            /// <param name="converter">转换器实例</param>
            public ContertItems(Converter converter)
            {
                this.converter = converter;
                this.linkedList.AddLast(new NotSupportedConvert());
            }

            /// <summary>
            /// 通过类型查找节点
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns></returns>
            private LinkedListNode<IConvert> FindNode<T>() where T : IConvert
            {
                var node = this.linkedList.First;
                while (node != null)
                {
                    if (node.Value.GetType() == typeof(T))
                    {
                        return node;
                    }
                    node = node.Next;
                }
                return null;
            }

            /// <summary>
            /// 是否已存在T类型的转换单元
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            private bool ExistConvert<T>() where T : IConvert
            {
                return this.FindNode<T>() != null;
            }

            /// <summary>
            /// 初始化各转换单元
            /// </summary>
            /// <returns></returns>
            private ContertItems ReInitItems()
            {
                var node = this.linkedList.First;
                while (node.Next != null)
                {
                    node.Value.NextConvert = node.Next.Value;
                    node.Value.Converter = this.converter;
                    node = node.Next;
                }
                return this;
            }

            /// <summary>
            /// 添加一个转换单元到最前面
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddFrist<T>() where T : IConvert
            {
                if (this.ExistConvert<T>() == false)
                {
                    var convert = Activator.CreateInstance<T>();
                    this.linkedList.AddFirst(convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">已存在的转换单元</typeparam>
            /// <typeparam name="TDest">新加入的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddBefore<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                var node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    var convert = Activator.CreateInstance<TDest>();
                    this.linkedList.AddBefore(node, convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">已存在的转换单元</typeparam>
            /// <typeparam name="TDest">新加入的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddAfter<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                var node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    var convert = Activator.CreateInstance<TDest>();
                    this.linkedList.AddAfter(node, convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加一个转换单元到末尾
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddLast<T>() where T : IConvert
            {
                return this.AddBefore<NotSupportedConvert, T>();
            }

            /// <summary>
            /// 解绑一个转换单元
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems Remove<T>() where T : IConvert
            {
                var node = this.FindNode<T>();
                if (node != null)
                {
                    this.linkedList.Remove(node);
                }
                return this.ReInitItems();
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
                var node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    var convert = Activator.CreateInstance<TDest>();
                    node.Value = convert;
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 清除所有转换单元
            /// </summary>
            public void Clear()
            {
                this.linkedList.Clear();
                this.linkedList.AddLast(new NotSupportedConvert());
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
                private ContertItems view;

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
                public IConvert[] Values
                {
                    get
                    {
                        return view.linkedList.Where(item => item.GetType() != typeof(NotSupportedConvert)).ToArray();
                    }
                }
            }

            #endregion DebugView
        }
    }
}