using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Util
{
    /// <summary>
    /// 定义类型转换单元的接口
    /// </summary>
    public interface IConvert
    {
        /// <summary>
        /// 设置转换器
        /// </summary>
        Converter Converter { set; }

        /// <summary>
        /// 设置下一个转换单元
        /// </summary>
        IConvert NextConvert { set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        object Convert(object value, Type targetType);
    }
}
