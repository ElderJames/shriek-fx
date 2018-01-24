using System;

namespace Shriek.ServiceProxy.Socket.Util.Converts
{
    /// <summary>
    /// 表示不作转换的转换单元
    /// </summary>
    public class NoConvert : IConvert
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
            if (targetType == typeof(object))
            {
                return value;
            }

            if (value != null && targetType == value.GetType())
            {
                return value;
            }

            return this.NextConvert.Convert(value, targetType);
        }
    }
}