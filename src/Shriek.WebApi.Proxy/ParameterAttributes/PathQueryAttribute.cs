using Shriek.WebApi.Proxy.UriTemplates;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Shriek.WebApi.Proxy
{
    /// <summary>
    /// 表示Url路径参数或query参数的特性
    /// 此特性不需要显示声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class PathQueryAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 获取或设置当值为null时忽略此参数
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        public PathQueryAttribute()
        {
        }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// <param name="ignoreWhenNull">当值为null时忽略此参数</param>
        /// </summary>
        public PathQueryAttribute(bool ignoreWhenNull)
        {
            this.IgnoreWhenNull = ignoreWhenNull;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public override async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.IgnoreWhenNull && parameter.Value == null)
            {
                return;
            }
            var uri = context.RequestMessage.RequestUri;
            var template = new UriTemplate(uri.ToString(), false, true);
            this.GetPathQuery(template, parameter);
            context.RequestMessage.RequestUri = new Uri(template.Resolve());
            await TaskExtensions.CompletedTask;
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="uriTemplate"></param>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        private void GetPathQuery(UriTemplate uriTemplate, ApiParameterDescriptor parameter)
        {
            if (parameter.IsUriParameterType)
            {
                uriTemplate.SetParameter(parameter.Name, string.Format(CultureInfo.InvariantCulture, "{0}", parameter.Value));
            }
            else if (parameter.ParameterType.IsArray && parameter.Value is Array array)
            {
                foreach (var item in array)
                {
                    uriTemplate.SetParameter(parameter.Name, string.Format(CultureInfo.InvariantCulture, "{0}", item));
                }
            }
            else
            {
                var instance = parameter.Value;
                var instanceType = parameter.ParameterType;

                var properties = Property.GetProperties(instanceType);
                foreach (var p in properties)
                {
                    var value = instance == null ? null : p.GetValue(instance);
                    uriTemplate.SetParameter(p.Name, string.Format(CultureInfo.InvariantCulture, "{0}", value));
                }
            }
        }
    }
}