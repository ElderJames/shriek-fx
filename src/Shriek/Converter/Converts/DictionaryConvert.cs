using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示字典转换单元
    /// </summary>
    public class DictionaryConvert : IConvert
    {
        /// <summary>
        /// 将value转换为目标类型
        /// 并将转换所得的值放到result
        /// 如果不支持转换，则返回false
        /// </summary>
        /// <param name="converter">转换器实例</param>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <param name="result">转换结果</param>
        /// <returns>如果不支持转换，则返回false</returns>
        public virtual bool Convert(Converter converter, object value, Type targetType, out object result)
        {
            var dic = value as IDictionary<string, object>;
            if (dic == null)
            {
                result = null;
                return false;
            }

            var instance = Activator.CreateInstance(targetType);
            var setters = PropertySetter.GetPropertySetters(targetType);

            foreach (var set in setters)
            {
                var key = dic.Keys.FirstOrDefault(k => string.Equals(k, set.Name, StringComparison.OrdinalIgnoreCase));
                if (key != null)
                {
                    var targetValue = converter.Convert(dic[key], set.Type);
                    set.SetValue(instance, targetValue);
                }
            }

            result = instance;
            return true;
        }
    }
}