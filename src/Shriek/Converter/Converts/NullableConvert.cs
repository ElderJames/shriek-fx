using System;
using System.Linq;
using System.Reflection;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示可空类型转换单元
    /// </summary>
    public class NullableConvert : IConvert
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
            if (targetType.GetTypeInfo().IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericArgument = targetType.GetGenericArguments().First();
                result = converter.Convert(value, genericArgument);
                return true;
            }

            result = null;
            return false;
        }
    }
}