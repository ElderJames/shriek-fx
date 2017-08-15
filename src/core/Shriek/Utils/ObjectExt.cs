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
        public static Dictionary<string, string> ToMap(this object o)
        {
            var map = new Dictionary<string, string>();
            foreach (var p in o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.CanRead)
                {
                    var value = p.GetValue(o);
                    if (value != null)
                    {
                        map.Add(p.Name, value.ToString());
                    }
                }
            }
            return map;
        }
    }
}