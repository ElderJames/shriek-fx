using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;
using System.Text;
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
                md.GetType().GetProperty(d.Key).SetValue(md, d.Value);
            }
            return md;
        }

        public static object ToObject(this IDictionary<string, object> dic, Type type)
        {
            var assembly = Assembly.GetAssembly(type);

            var md = assembly.CreateInstance(type.FullName);

            foreach (var d in dic)
            {
                md.GetType().GetProperty(d.Key).SetValue(md, d.Value);
            }
            return md;
        }
    }
}