using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Shriek
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 将对象属性转换为key-value对
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToMap(this object o)
        {
            var map = new Dictionary<string, object>();
            foreach (var p in o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead)
                    continue;

                var value = p.GetValue(o);
                if (value == null)
                    continue;

                if (p.PropertyType.IsClass && !p.PropertyType.AssemblyQualifiedName.Contains("System"))
                {
                    map.Add(p.Name, value.ToMap());
                }
                else
                    map.Add(p.Name, value);
            }
            return map;
        }

        /// <summary>
        /// 字典类型转化为对象
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T ToObject<T>(this IDictionary<string, object> dic) where T : new()
        {
            var md = new T();

            foreach (var d in dic)
            {
                var prop = md.GetType().GetProperty(d.Key);
                if (prop == null || !prop.CanWrite)
                    continue;

                if (prop.PropertyType.IsClass && !prop.PropertyType.AssemblyQualifiedName.Contains("System"))
                {
                    if (d.Value is Dictionary<string, object> dict)
                        prop.SetValue(md, dict.ToObject(prop.PropertyType));
                }
                else
                    prop.SetValue(md, d.Value);
            }
            return md;
        }

        public static object ToObject(this IDictionary<string, object> dic, Type type)
        {
            var md = Activator.CreateInstance(type);

            foreach (var d in dic)
            {
                var prop = md.GetType().GetProperty(d.Key);
                if (prop == null || !prop.CanWrite)
                    continue;

                if (prop.PropertyType.IsClass && !prop.PropertyType.AssemblyQualifiedName.Contains("System"))
                {
                    if (d.Value is Dictionary<string, object> dict)
                        prop.SetValue(md, dict.ToObject(prop.PropertyType));
                }
                else
                    prop.SetValue(md, d.Value);
            }
            return md;
        }

        /// <summary>
        /// 获取枚举类型的描述信息
        /// </summary>
        public static string GetDescription(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var name = Enum.GetName(type, enumValue);

            if (name == null) return null;

            var field = type.GetField(name);

            if (!(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute))
            {
                return name;
            }

            return attribute.Description;
        }

        /// <summary>
        /// 获取枚举类型的字段名
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetName(this Enum enumValue)
        {
            return Enum.GetName(enumValue.GetType(), enumValue);
        }

        /// <summary>
        /// 获取枚举的名称(Key)和对应值(Value)的集合 调用方式<code>tyoeof(AnyEnum).GetEnumKeyValuePairs()</code>
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static IEnumerable<(string Name, dynamic Value)> GetEnumKeyValueTuple(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumType)} is not a enum type.");
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            return Enum.GetValues(enumType).Cast<dynamic>().Select(x => (Enum.GetName(enumType, (object)x), Convert.ChangeType(x, underlyingType)));
        }

        public static IDictionary<string, dynamic> GetEnumKeyValue(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumType)} is not a enum type.");
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            return Enum.GetValues(enumType).Cast<dynamic>().ToDictionary(x => Enum.GetName(enumType, (object)x), x => Convert.ChangeType(x, underlyingType));
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}