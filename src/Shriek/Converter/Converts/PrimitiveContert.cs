using System;
using System.Reflection;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示简单类型转换单元
    /// 支持基元类型、guid和枚举相互转换
    /// </summary>
    public class PrimitiveContert : IConvert
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
            var valueString = value.ToString();
            if (targetType.GetTypeInfo().IsEnum)
            {
                result = Enum.Parse(targetType, valueString, true);
                return true;
            }

            if (value is IConvertible convertible && typeof(IConvertible).GetTypeInfo().IsAssignableFrom(targetType))
            {
                result = convertible.ToType(targetType, null);
                return true;
            }

            if (typeof(Guid) == targetType)
            {
                result = Guid.Parse(valueString);
                return true;
            }
            if (typeof(string) == targetType)
            {
                result = valueString;
                return true;
            }

            result = null;
            return false;
        }
    }
}