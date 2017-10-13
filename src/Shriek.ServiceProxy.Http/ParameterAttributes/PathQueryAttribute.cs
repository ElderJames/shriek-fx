using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.ObjectPool;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http.Contexts;
using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http
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
            if (!(context is HttpApiActionContext httpContext)) return;

            var uri = httpContext.RequestMessage.RequestUri;
            var baseUrl = uri.Scheme + "://" + uri.Host + (uri.IsDefaultPort ? "" : ":" + uri.Port);
            var pathQuery = GetPathQuery(uri.LocalPath.Trim('/'), parameter);

            httpContext.RequestMessage.RequestUri = new Uri(new Uri(baseUrl), pathQuery);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 绑定路由参数到模版
        /// </summary>
        /// <param name="template">路由模版</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetPathQuery(string template, ApiParameterDescriptor parameter)
        {
            var _params = new RouteValueDictionary();
            if (parameter.IsUriParameterType)
            {
                _params.Add(parameter.Name, string.Format(CultureInfo.InvariantCulture, "{0}", parameter.Value));
            }
            else if (parameter.ParameterType.IsArray && parameter.Value is Array array)
            {
                foreach (var item in array)
                {
                    _params.Add(parameter.Name, string.Format(CultureInfo.InvariantCulture, "{0}", item));
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
                    _params.Add(p.Name, string.Format(CultureInfo.InvariantCulture, "{0}", value));
                }
            }

            return BoundTemplate(template, _params);
        }

        /// <summary>
        /// 使用mvc路由模版绑定参数
        /// </summary>
        /// <param name="template">路由模版</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        private string BoundTemplate(string template, RouteValueDictionary values)
        {
            var binder = new TemplateBinder(
                UrlEncoder.Default,
                new DefaultObjectPoolProvider().Create(new UriBuilderContextPooledObjectPolicy(UrlEncoder.Default)),
                TemplateParser.Parse(template),
                null
                );

            // Act & Assert
            var result = binder.GetValues(new RouteValueDictionary(), values);

            return binder.BindValues(result.AcceptedValues);
        }
    }
}