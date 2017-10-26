using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using Shriek.ServiceProxy.Abstractions.Context;

namespace Shriek.ServiceProxy.Http.ParameterAttributes
{
    /// <summary>
    /// 表示将参数体作为application/xml请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class XmlContentAttribute : HttpContentAttribute
    {
        public override string MediaType => "application/xml";

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