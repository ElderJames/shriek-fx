using System;

namespace Shriek.ServiceProxy.Socket.Util.Converts
{
    /// <summary>
    /// 表示null值转换单元
    /// </summary>
    public class NullConvert : IConvert
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
            if (value != null)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            if (targetType.IsValueType == true)
            {
                if (targetType.IsGenericType == false || targetType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    throw new NotSupportedException("不支持将null转换为" + targetType.Name);
                }
            }
            return null;
        }
    }
}