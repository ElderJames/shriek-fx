using System;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示不作转换的转换单元
    /// </summary>
    public class NoConvert : IConvert
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
            result = value;
            if (targetType == typeof(object))
            {
                return true;
            }

            if (value != null && targetType == value.GetType())
            {
                return true;
            }

            result = null;
            return false;
        }
    }
}