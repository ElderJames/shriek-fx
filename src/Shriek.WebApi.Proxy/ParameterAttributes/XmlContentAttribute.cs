using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;

namespace Shriek.WebApi.Proxy
{
    /// <summary>
    /// 表示将参数体作为application/xml请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class XmlContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var xmlSerializer = new XmlSerializer(parameter.ParameterType);
            using (var stream = new MemoryStream())
            {
                xmlSerializer.Serialize(stream, parameter.Value);
                var xml = Encoding.UTF8.GetString(stream.ToArray());
                return new StringContent(xml, Encoding.UTF8, "application/xml");
            }
        }
    }
}