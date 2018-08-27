using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Shriek.Reflection
{
    /// <summary>
    /// 表示属性
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 获取器
        /// </summary>
        private readonly Method geter;

        /// <summary>
        /// 设置器
        /// </summary>
        private readonly Method seter;

        /// <summary>
        /// 获取属性名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        public PropertyInfo Info { get; private set; }

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public Property(PropertyInfo property)
        {
            this.Name = property.Name;
            this.Info = property;

            var getMethod = property.GetGetMethod();
            if (getMethod != null)
            {
                this.geter = new Method(getMethod);
            }

            var setMethod = property.GetSetMethod();
            if (setMethod != null)
            {
                this.seter = new Method(setMethod);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public object GetValue(object instance)
        {
            if (this.geter == null)
            {
                throw new NotSupportedException();
            }
            return this.geter.Invoke(instance, null);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">值</param>
        /// <exception cref="NotSupportedException"></exception>
        public void SetValue(object instance, object value)
        {
            if (this.seter == null)
            {
                throw new NotSupportedException();
            }
            this.seter.Invoke(instance, value);
        }

        /// <summary>
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, Property[]> cached = new ConcurrentCache<Type, Property[]>();

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Property[] GetProperties(Type type)
        {
            return cached.GetOrAdd(type, t => t.GetProperties().Select(p => new Property(p)).ToArray());
        }
    }
}