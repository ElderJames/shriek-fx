using Shriek.ServiceProxy.Tcp.Reflection;
using Shriek.ServiceProxy.Tcp.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 表示Api行为
    /// </summary>
    [DebuggerDisplay("ApiName = {ApiName}")]
    public class ApiAction : ICloneable<ApiAction>
    {
        /// <summary>
        /// 获取方法成员信息
        /// </summary>
        public Method Method { get; protected set; }

        /// <summary>
        /// 获取Api行为的Api名称
        /// </summary>
        public string ApiName { get; protected set; }

        /// <summary>
        /// 获取Api行为的方法成员返回类型是否为void
        /// </summary>
        public bool IsVoidReturn { get; protected set; }

        /// <summary>
        /// 获取Api行为的方法成员返回类型Task或TaskOf(T)
        /// </summary>
        public bool IsTaskReturn { get; protected set; }

        /// <summary>
        /// 获取声明该成员的服务类型
        /// </summary>
        public Type DeclaringService { get; protected set; }

        /// <summary>
        /// 获取Api参数
        /// </summary>
        public ApiParameter[] Parameters { get; protected set; }

        /// <summary>
        /// Api行为
        /// </summary>
        protected ApiAction()
        {
        }

        /// <summary>
        /// Api行为
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <exception cref="ArgumentException"></exception>
        public ApiAction(MethodInfo method)
        {
            this.Method = new Method(method);
            this.ApiName = this.GetApiName(method);
            this.DeclaringService = method.DeclaringType;
            this.IsTaskReturn = typeof(Task).IsAssignableFrom(method.ReturnType);
            this.IsVoidReturn = method.ReturnType.Equals(typeof(void)) || method.ReturnType.Equals(typeof(Task));
            this.Parameters = method.GetParameters().Select(p => new ApiParameter(p)).ToArray();
        }

        /// <summary>
        /// 获取ApiName
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        private string GetApiName(MethodInfo method)
        {
            var api = Attribute.GetCustomAttribute(method, typeof(ApiAttribute)) as ApiAttribute;
            if (api != null && string.IsNullOrWhiteSpace(api.Name) == false)
            {
                return api.Name;
            }
            else
            {
                return Regex.Replace(method.Name, @"Async$", string.Empty, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// 获取类级过滤器特性
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<FilterAttribute> GetClassFilterAttributes()
        {
            return this.DeclaringService.GetCustomAttributes<FilterAttribute>(inherit: true);
        }

        /// <summary>
        /// 获取方法级过滤器特性
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<FilterAttribute> GetMethodFilterAttributes()
        {
            return this.Method.Info.GetCustomAttributes<FilterAttribute>(inherit: true);
        }

        /// <summary>
        /// 获取所有参数的参数过滤器
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ParameterFilterAttribute> GetParametersFilterAttributes()
        {
            return this.Parameters.SelectMany((p, i) => this.GetParameterFilterAttributes(p, i));
        }

        /// <summary>
        /// 获取参数的参数过滤器
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        private IEnumerable<ParameterFilterAttribute> GetParameterFilterAttributes(ApiParameter parameter, int index)
        {
            return parameter.Info
                .GetCustomAttributes<ParameterFilterAttribute>(inherit: true)
                .Select(filter => filter.InitWith(index));
        }

        /// <summary>
        /// 获取Api行为或Api行为的声明类型是否声明了特性
        /// </summary>
        /// <param name="type">特性类型</param>
        /// <param name="inherit">是否继承</param>
        /// <returns></returns>
        public bool IsDefined(Type type, bool inherit)
        {
            return this.Method.Info.IsDefined(type, inherit) || this.DeclaringService.IsDefined(type, inherit);
        }

        /// <summary>
        /// 执行Api行为
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="parameters">参数实例</param>
        /// <returns></returns>
        public object Execute(object service, params object[] parameters)
        {
            return this.Method.Invoke(service, parameters);
        }

        /// <summary>
        /// 异步执行Api行为
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="parameters">参数实例</param>
        /// <returns></returns>
        public Task<object> ExecuteAsync(object service, params object[] parameters)
        {
            if (this.IsTaskReturn == true)
            {
                var task = this.Execute(service, parameters) as Task;
                if (task == null)
                {
                    return Task.FromResult<object>(null);
                }
                return task.Cast<object>(this.Method.Info.ReturnType);
            }
            else
            {
                var result = this.Execute(service, parameters);
                return Task.FromResult(result);
            }
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ApiName;
        }

        /// <summary>
        /// 克隆构造器
        /// </summary>
        /// <returns></returns>
        ApiAction ICloneable<ApiAction>.CloneConstructor()
        {
            return new ApiAction
            {
                ApiName = this.ApiName,
                Method = this.Method,
                IsTaskReturn = this.IsTaskReturn,
                IsVoidReturn = this.IsVoidReturn,
                DeclaringService = this.DeclaringService,
                Parameters = this.Parameters.Select(p => new ApiParameter(p.Info)).ToArray()
            };
        }
    }
}