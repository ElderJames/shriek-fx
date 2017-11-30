using System;

namespace Shriek.ServiceProxy.Tcp.Util.Converts
{
    /// <summary>
    /// 表示简单类型转换单元
    /// 支持基元类型、guid和枚举相互转换
    /// </summary>
    public class SimpleContert : IConvert
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
            var valueString = value.ToString();
            if (targetType.IsEnum == true)
            {
                return Enum.Parse(targetType, valueString, true);
            }

            if (typeof(string) == targetType)
            {
                return valueString;
            }

            if (typeof(Guid) == targetType)
            {
                return Guid.Parse(valueString);
            }

            var convertible = value as IConvertible;
            if (convertible != null && typeof(IConvertible).IsAssignableFrom(targetType) == true)
            {
                return convertible.ToType(targetType, null);
            }

            return this.NextConvert.Convert(value, targetType);
        }
    }
}