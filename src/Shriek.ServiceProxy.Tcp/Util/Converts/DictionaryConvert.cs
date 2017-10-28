using Shriek.ServiceProxy.Tcp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Util.Converts
{
    /// <summary>
    /// 表示字典转换单元
    /// </summary>
    public class DictionaryConvert : IConvert
    {
        /// <summary>
        /// 转换器实例
        /// </summary>
        public Converter Converter { get; set; }

        /// <summary>
        /// 下一个转换单元
        /// </summary>
        public IConvert NextConvert { get; set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType)
        {
            var dic = value as IDictionary<string, object>;
            if (dic == null)
            {
                return this.NextConvert.Convert(value, targetType);
            }
            else
            {
                dic = dic.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }

            var instance = Activator.CreateInstance(targetType);
            var setters = Property.GetProperties(targetType);

            foreach (var setter in setters)
            {
                if (setter.Info.CanWrite == false)
                {
                    continue;
                }

                object targetValue;
                if (dic.TryGetValue(setter.Name, out targetValue) == false)
                {
                    continue;
                }

                var valueCast = this.Converter.Convert(targetValue, setter.Info.PropertyType);
                setter.SetValue(instance, valueCast);
            }

            return instance;
        }
    }
}
