using Shriek.ServiceProxy.Tcp.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义对象的json序列化与json动态反序列化的接口    
    /// </summary>
    public interface IDynamicJsonSerializer
    {
        /// <summary>
        /// 序列化为Json
        /// 异常时抛出SerializerException
        /// </summary>
        /// <param name="model">实体</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        string Serialize(object model);

        /// <summary>
        /// 反序列化json为动态类型
        /// 异常时抛出SerializerException
        /// </summary>
        /// <param name="json">json数据</param>      
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        dynamic Deserialize(string json);

        /// <summary>
        /// 将值转换为目标类型
        /// 这些值有可能是反序列化得到的动态类型的值
        /// </summary>       
        /// <param name="value">要转换的值，可能</param>
        /// <param name="targetType">转换的目标类型</param>   
        /// <returns>转换结果</returns>
        object Convert(object value, Type targetType);
    }
}
