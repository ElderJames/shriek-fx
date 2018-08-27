using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.ActionAttributes;
using Shriek.ServiceProxy.Http.ParameterAttributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http.Contexts
{
    /// <summary>
    /// 表示Aop相关上下文
    /// </summary>
    internal class AspectContext
    {
        /// <summary>
        /// 获取HttpHostAttribute
        /// </summary>
        public HttpHostAttribute HostAttribute { get; private set; }

        /// <summary>
        /// 中间路由模版
        /// </summary>
        public RouteAttribute[] RouteAttributes { get; internal set; }

        /// <summary>
        /// 获取ApiReturnAttribute
        /// </summary>
        public ApiReturnAttribute ApiReturnAttribute { get; private set; }

        /// <summary>
        /// 获取ApiActionFilterAttribute
        /// </summary>
        public ApiActionFilterAttribute[] ApiActionFilterAttributes { get; set; }

        /// <summary>
        /// 获取ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; private set; }

        /// <summary>
        /// 缓存字典
        /// </summary>
        private static readonly ConcurrentCache<MethodInfo, AspectContext> cache;

        /// <summary>
        /// Castle相关上下文
        /// </summary>
        static AspectContext()
        {
            cache = new ConcurrentCache<MethodInfo, AspectContext>(new IInvocationComparer());
        }

        /// <summary>
        /// 从拦截内容获得 使用缓存
        /// </summary>
        /// <param name="method">拦截方法</param>
        /// <returns></returns>
        public static AspectContext From(MethodInfo method)
        {
            return cache.GetOrAdd(method, GetContextNoCache(method));
        }

        /// <summary>
        /// 从拦截内容获得
        /// </summary>
        /// <param name="method">拦截方法</param>
        /// <returns></returns>
        private static AspectContext GetContextNoCache(MethodInfo method)
        {
            var routeAttributes = GetAttributesFromMethodAndInterface<RouteAttribute>(method, false) ?? Array.Empty<RouteAttribute>();

            var hostAttribute = GetAttributeFromMethodOrInterface<HttpHostAttribute>(method, false) ?? new HttpHostAttribute("");

            var returnAttribute = GetAttributeFromMethodOrInterface<ApiReturnAttribute>(method, true);

            var methodFilters = method.GetCustomAttributes<ApiActionFilterAttribute>(true);

            var interfaceFilters = method.DeclaringType.GetCustomAttributes<ApiActionFilterAttribute>(true);
            var filterAttributes = methodFilters.Concat(interfaceFilters).Distinct(new ApiActionFilterAttributeComparer()).ToArray();

            return new AspectContext
            {
                HostAttribute = hostAttribute,
                RouteAttributes = routeAttributes,
                ApiReturnAttribute = returnAttribute,
                ApiActionFilterAttributes = filterAttributes,
                ApiActionDescriptor = GetActionDescriptor(method)
            };
        }

        /// <summary>
        /// 生成ApiActionDescriptor
        /// </summary>
        /// <param name="method">拦截方法</param>
        /// <returns></returns>
        private static ApiActionDescriptor GetActionDescriptor(MethodInfo method)
        {
            var descriptor = new ApiActionDescriptor
            {
                Name = method.Name,
                ReturnTaskType = method.ReturnType,
                ReturnDataType = method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)) ? method.ReturnType.GetGenericArguments().FirstOrDefault() : method.ReturnType,
                Attributes = method.GetCustomAttributes<ApiActionAttribute>(true).ToArray(),
                Parameters = method.GetParameters().Select((param, index) => GetParameterDescriptor(param, index, method)).ToArray()
            };

            if (!descriptor.Attributes.Any(x => x is HttpMethodAttribute))
                descriptor.Attributes = descriptor.Attributes.Concat(new[] { new HttpPostAttribute(method.GetPath()) }).ToArray();

            if (descriptor.Parameters.Count(x => x.Attributes.Any(o => o.GetType() != typeof(PathQueryAttribute))) > 1)
                throw new NotSupportedException("不支持多个非值类型作为参数，请使用实体封装。");

            return descriptor;
        }

        /// <summary>
        /// 生成ApiParameterDescriptor
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <param name="index">参数索引</param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static ApiParameterDescriptor GetParameterDescriptor(ParameterInfo parameter, int index, MethodInfo method)
        {
            var parameterDescriptor = new ApiParameterDescriptor
            {
                Name = parameter.Name,
                Index = index,
                ParameterType = parameter.ParameterType,
                IsUriParameterType = parameter.ParameterType.IsUriParameterType(),
                Attributes = parameter.GetCustomAttributes<ApiParameterAttribute>(true).ToArray()
            };

            var methodAttrs = method.GetCustomAttributes<ApiActionAttribute>(true);

            if (methodAttrs.Any(x => x is HttpGetAttribute) && !parameterDescriptor.IsUriParameterType && !parameterDescriptor.ParameterType.IsUriParameterTypeArray())
                throw new Exception($"Get请求方法不支持非Uri支持类型的参数[{parameter.ParameterType}]:{parameter.Name}，请在操作方法标记HttpPostAttribute方法特性。");

            if (!parameterDescriptor.Attributes.Any())
            {
                if (methodAttrs.Any(x => x is HttpGetAttribute) || parameterDescriptor.IsUriParameterType || parameterDescriptor.ParameterType.IsUriParameterTypeArray())
                    parameterDescriptor.Attributes = new[] { new PathQueryAttribute() };
                else
                    parameterDescriptor.Attributes = new[] { new JsonContentAttribute() };
            }

            return parameterDescriptor;
        }

        /// <summary>
        /// 从方法或接口获取特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="method">方法</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        private static TAttribute GetAttributeFromMethodOrInterface<TAttribute>(MethodInfo method, bool inherit) where TAttribute : Attribute
        {
            var attribute = method.GetCustomAttribute<TAttribute>(inherit);
            if (attribute == null)
            {
                attribute = method.DeclaringType.GetCustomAttribute<TAttribute>(inherit);
            }
            return attribute;
        }

        private static TAttribute[] GetAttributesFromMethodAndInterface<TAttribute>(MethodInfo method, bool inherit) where TAttribute : Attribute
        {
            IEnumerable<TAttribute> attributes = new TAttribute[] { };

            //接口优先
            var attribute = method.DeclaringType.GetCustomAttribute<TAttribute>(inherit);
            if (attribute != null) attributes = attributes.Concat(new[] { attribute });

            //第二是方法
            attribute = method.GetCustomAttribute<TAttribute>(inherit);
            if (attribute != null) attributes = attributes.Concat(new[] { attribute });

            return attributes.ToArray();
        }

        /// <summary>
        /// ApiActionFilterAttribute比较器
        /// </summary>
        private class ApiActionFilterAttributeComparer : IEqualityComparer<ApiActionFilterAttribute>
        {
            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(ApiActionFilterAttribute x, ApiActionFilterAttribute y)
            {
                return true;
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(ApiActionFilterAttribute obj)
            {
                return obj.TypeId.GetHashCode();
            }
        }

        /// <summary>
        /// IInvocation对象的比较器
        /// </summary>
        private class IInvocationComparer : IEqualityComparer<MethodInfo>
        {
            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(MethodInfo x, MethodInfo y)
            {
                return x.GetHashCode() == y.GetHashCode();
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(MethodInfo obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}