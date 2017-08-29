using System;
using System.Collections;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示数组转换单元
    /// </summary>
    public class ArrayConvert : IConvert
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
            if (targetType.IsArray == false)
            {
                result = null;
                return false;
            }

            var items = value as IEnumerable;
            var elementType = targetType.GetElementType();

            if (items == null)
            {
                result = Array.CreateInstance(elementType, 0);
                return true;
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
                var itemCast = converter.Convert(item, elementType);
                array.SetValue(itemCast, index);
                index = index + 1;
            }

            result = array;
            return true;
        }
    }
}