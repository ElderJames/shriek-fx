using AspectCore.DynamicProxy;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Abstractions.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示相关上下文
    /// </summary>
    public class AspectCoreContext
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
        private static readonly ConcurrentDictionary<MethodInfo, AspectCoreContext> cache;

        /// <summary>
        /// Castle相关上下文
        /// </summary>
        static AspectCoreContext()
        {
            cache = new ConcurrentDictionary<MethodInfo, AspectCoreContext>(new IInvocationComparer());
        }

        /// <summary>
        /// 从拦截内容获得
        /// 使用缓存
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        /// <returns></returns>
        public static AspectCoreContext From(AspectContext invocation)
        {
            return cache.GetOrAdd(invocation.ServiceMethod, GetContextNoCache(invocation));
        }

        /// <summary>
        /// 从拦截内容获得
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        /// <returns></returns>
        private static AspectCoreContext GetContextNoCache(AspectContext invocation)
        {
            var method = invocation.ServiceMethod;

            var routeAttributes = GetAttributesFromMethodAndInterface<RouteAttribute>(method, false) ??
                                 new RouteAttribute[] { };

            var hostAttribute = GetAttributeFromMethodOrInterface<HttpHostAttribute>(method, false) ?? new HttpHostAttribute("");

            var returnAttribute = GetAttributeFromMethodOrInterface<ApiReturnAttribute>(method, true);

            var methodFilters = method.GetCustomAttributes<ApiActionFilterAttribute>(true);
            var interfaceFilters = method.DeclaringType.GetCustomAttributes<ApiActionFilterAttribute>(true);
            var filterAttributes = methodFilters.Concat(interfaceFilters).Distinct(new ApiActionFilterAttributeComparer()).ToArray();

            return new AspectCoreContext
            {
                HostAttribute = hostAttribute,
                RouteAttributes = routeAttributes,
                ApiReturnAttribute = returnAttribute,
                ApiActionFilterAttributes = filterAttributes,
                ApiActionDescriptor = GetActionDescriptor(invocation)
            };
        }

        /// <summary>
        /// 生成ApiActionDescriptor
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        /// <returns></returns>
        private static ApiActionDescriptor GetActionDescriptor(AspectContext invocation)
        {
            var method = invocation.ServiceMethod;

            var descriptor = new ApiActionDescriptor
            {
                Name = $"{method.DeclaringType.Name}.{method.Name}",
                ReturnTaskType = method.ReturnType,
                ReturnDataType = method.ReturnType.IsGenericType ? method.ReturnType.GetGenericArguments().FirstOrDefault() : method.ReturnType,
                Attributes = method.GetCustomAttributes<ApiActionAttribute>(true).ToArray(),
                Parameters = method.GetParameters().Select(GetParameterDescriptor).ToArray()
            };

            return descriptor;
        }

        /// <summary>
        /// 生成ApiParameterDescriptor
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <param name="index">参数索引</param>
        /// <returns></returns>
        private static ApiParameterDescriptor GetParameterDescriptor(ParameterInfo parameter, int index)
        {
            var parameterDescriptor = new ApiParameterDescriptor
            {
                Name = parameter.Name,
                Index = index,
                ParameterType = parameter.ParameterType,
                IsUriParameterType = IsUriParameterType(parameter.ParameterType),
                Attributes = parameter.GetCustomAttributes<ApiParameterAttribute>(true).ToArray()
            };

            //if (typeof(HttpContent).IsAssignableFrom(parameter.ParameterType))
            //{
            //    parameterDescriptor.Attributes = new[] { new HttpContentAttribute() };
            //}
            //else if (!parameterDescriptor.Attributes.Any())
            //{
            //    parameterDescriptor.Attributes = new[] { new PathQueryAttribute() };
            //}
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

        private static bool IsUriParameterType(Type parameterType)
        {
            return parameterType == typeof(string) ||
                   parameterType == typeof(int) ||
                   parameterType == typeof(int?) ||
                   parameterType == typeof(byte) ||
                   parameterType == typeof(byte?) ||
                   parameterType == typeof(char) ||
                   parameterType == typeof(char?) ||
                   parameterType == typeof(short) ||
                   parameterType == typeof(short?) ||
                   parameterType == typeof(ushort) ||
                   parameterType == typeof(ushort?) ||
                   parameterType == typeof(uint) ||
                   parameterType == typeof(uint?) ||
                   parameterType == typeof(long) ||
                   parameterType == typeof(long?) ||
                   parameterType == typeof(ulong) ||
                   parameterType == typeof(ulong?) ||
                   parameterType == typeof(decimal) ||
                   parameterType == typeof(decimal?) ||
                   parameterType == typeof(float) ||
                   parameterType == typeof(float?) ||
                   parameterType == typeof(double) ||
                   parameterType == typeof(double?) ||
                   parameterType == typeof(DateTime) ||
                   parameterType == typeof(DateTime?) ||
                   parameterType == typeof(Guid) ||
                   parameterType == typeof(Guid?);
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