using Newtonsoft.Json;
using System;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    internal class DefaultJsonFormatter : IJsonFormatter
    {
        /// <summary>
        /// 序列化为json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string json, Type objType)
        {
            return JsonConvert.DeserializeObject(json, objType);
        }
    }
}