using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.ObjectPool;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.Abstractions.Internal;
using Shriek.ServiceProxy.Http.Contexts;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Shriek.ServiceProxy.Http.ParameterAttributes
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
            var pathQuery = GetPathQuery(uri, parameter);
            httpContext.RequestMessage.RequestUri = new Uri(uri, pathQuery);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 绑定路由参数到模版
        /// </summary>
        /// <param name="template">路由模版</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetPathQuery(Uri uri, ApiParameterDescriptor parameter)
        {
            var _params = new RouteValueDictionary();

            var template = uri.LocalPath.Trim('/');
            var queryString = HttpUtility.UrlDecode(uri.Query).TrimStart('?');

            if (!string.IsNullOrEmpty(queryString))
            {
                var keyValues = queryString.Split('&').Select(group =>
                {
                    var keyvalue = group.Split('=');
                    return new { key = keyvalue[0], value = keyvalue[1] };
                })
                .GroupBy(x => x.key).ToDictionary(x => x.Key, x => x.Count() > 1 ? (object)x.Select(o => o.value) : x.FirstOrDefault()?.value);

                foreach (var kv in keyValues)
                {
                    _params.Add(kv.Key, kv.Value);
                }
            }
            if (parameter.ParameterType.IsArray && parameter.Value is Array array)
            {
                _params.Add(parameter.Name, array);
            }
            else if (parameter.ParameterType.IsEnum)
            {
                _params.Add(parameter.Name, parameter.Value);
            }
            else if (parameter.IsUriParameterType)
            {
                _params.Add(parameter.Name, string.Format(CultureInfo.InvariantCulture, "{0}", parameter.Value));
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
                new DefaultObjectPoolProvider().Create(new UriBuilderContextPooledObjectPolicy()),
                TemplateParser.Parse(template),
                new RouteValueDictionary()
                );

            // Act & Assert
            var result = binder.GetValues(new RouteValueDictionary(), values);

            return binder.BindValues(result.AcceptedValues);
        }
    }
}