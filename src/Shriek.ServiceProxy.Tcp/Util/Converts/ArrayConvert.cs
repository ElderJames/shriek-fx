using System;
using System.Collections;

namespace Shriek.ServiceProxy.Tcp.Util.Converts
{
    /// <summary>
    /// 表示数组转换单元
    /// </summary>
    public class ArrayConvert : IConvert
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
            if (targetType.IsArray == false)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            var items = value as IEnumerable;
            var elementType = targetType.GetElementType();

            if (items == null)
            {
                return Array.CreateInstance(elementType, 0);
            }

            var length = 0;
            var list = items as IList;
            if (list != null)
            {
                length = list.Count;
            }
            else
            {
                var enumerator = items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    length = length + 1;
                }
            }

            var index = 0;
            var array = Array.CreateInstance(elementType, length);
            foreach (var item in items)
            {
                var itemCast = this.Converter.Convert(item, elementType);
                array.SetValue(itemCast, index);
                index = index + 1;
            }
            return array;
        }
    }
}