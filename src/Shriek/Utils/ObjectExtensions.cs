using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shriek.Utils
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
                if (p.CanRead)
                {
                    var value = p.GetValue(o);
                    if (value != null)
                    {
                        if (p.PropertyType.IsClass && !p.PropertyType.AssemblyQualifiedName.Contains("System"))
                        {
                            map.Add(p.Name, value.ToMap());
                        }
                        else
                            map.Add(p.Name, value);
                    }
                }
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
                if (prop != null && prop.CanWrite)
                {

                    if (prop.PropertyType.IsClass && !prop.PropertyType.AssemblyQualifiedName.Contains("System"))
                    {
                        if (d.Value is Dictionary<string, object> dict)
                            prop.SetValue(md, dict.ToObject(prop.PropertyType));
                    }
                    else
                        prop.SetValue(md, d.Value);
                }

            }
            return md;
        }

        public static object ToObject(this IDictionary<string, object> dic, Type type)
        {
            var md = Activator.CreateInstance(type);

            foreach (var d in dic)
            {
                var prop = md.GetType().GetProperty(d.Key);
                if (prop != null && prop.CanWrite)
                {

                    if (prop.PropertyType.IsClass && !prop.PropertyType.AssemblyQualifiedName.Contains("System"))
                    {
                        if (d.Value is Dictionary<string, object> dict)
                            prop.SetValue(md, dict.ToObject(prop.PropertyType));
                    }
                    else
                        prop.SetValue(md, d.Value);
                }
            }
            return md;
        }
    }
}