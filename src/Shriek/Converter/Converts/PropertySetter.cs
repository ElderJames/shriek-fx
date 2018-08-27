using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示属性的设置器
    /// </summary>
    internal class PropertySetter
    {
        /// <summary>
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, PropertySetter[]> cached = new ConcurrentCache<Type, PropertySetter[]>();

        /// <summary>
        /// 从类型的属性获取Setter
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static PropertySetter[] GetPropertySetters(Type type)
        {
            return cached.GetOrAdd(type, t => t.GetProperties()
                 .Where(p => p.CanWrite)
                 .Select(p => new PropertySetter(p))
                 .ToArray());
        }

        /// <summary>
        /// Api行为的方法成员调用委托
        /// </summary>
        private readonly Func<object, object[], object> methodInvoker;

        /// <summary>
        /// 获取属性的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取属性的类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 属性的Setter
        /// </summary>
        /// <param name="property">属性</param>
        public PropertySetter(PropertyInfo property)
        {
            this.methodInvoker = MethodReflection.CreateInvoker(property.GetSetMethod());
            this.Name = property.Name;
            this.Type = property.PropertyType;
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">属性的值</param>
        /// <returns></returns>
        public void SetValue(object instance, object value)
        {
            this.methodInvoker.Invoke(instance, new[] { value });
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}