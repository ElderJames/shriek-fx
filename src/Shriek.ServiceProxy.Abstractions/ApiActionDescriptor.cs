using Shriek.Utils;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Abstractions
{
    /// <summary>
    /// 表示请求Api描述
    /// </summary>
    public class ApiActionDescriptor : ICloneable
    {
        /// <summary>
        /// 获取Api名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public ApiActionAttribute[] Attributes { get; set; }

        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public ApiParameterDescriptor[] Parameters { get; set; }

        /// <summary>
        /// 获取Api返回的TaskOf(T)类型
        /// </summary>
        public Type ReturnTaskType { get; set; }

        /// <summary>
        /// 获取Api返回的TaskOf(T)的T类型
        /// </summary>
        public Type ReturnDataType { get; set; }

        /// <summary>
        /// 执行api
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public object Execute(ApiActionContext context)
        {
            if (ReturnTaskType.IsGenericType && ReturnTaskType.GetGenericTypeDefinition() == typeof(Task<>))
                return this.ExecuteAsync(context).CastResult(this.ReturnDataType);
            else
                return ((dynamic)this.ExecuteAsync(context).CastResult(this.ReturnDataType)).Result;
        }

        /// <summary>
        /// 异步执行api
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private async Task<object> ExecuteAsync(ApiActionContext context)
        {
            foreach (var methodAttribute in context.ApiActionDescriptor.Attributes)
            {
                await methodAttribute.BeforeRequestAsync(context);
            }

            foreach (var parameter in context.ApiActionDescriptor.Parameters)
            {
                foreach (var parameterAttribute in parameter.Attributes)
                {
                    await parameterAttribute.BeforeRequestAsync(context, parameter);
                }
            }

            foreach (var filter in context.ApiActionFilterAttributes)
            {
                await filter.OnBeginRequestAsync(context);
            }

            await context.HttpApiClient.SendAsync(context);

            foreach (var filter in context.ApiActionFilterAttributes)
            {
                await filter.OnEndRequestAsync(context);
            }

            if (context.ApiReturnAttribute != null)
                return await context.ApiReturnAttribute.GetTaskResult(context);

            foreach (var attr in Reflection.CurrentAssembiles.SelectMany(x => x.GetTypes())
                .Where(x => x.BaseType == typeof(ApiReturnAttribute)).Select(x => Activator.CreateInstance(x) as ApiReturnAttribute))
            {
                if (attr == null) continue;
                var result = await attr.GetTaskResult(context);
                if (result != null)
                    return result;
            }

            var message = string.Format("不支持的类型{0}的解析", context.ApiActionDescriptor.ReturnDataType);
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ApiActionDescriptor
            {
                Name = this.Name,
                Attributes = this.Attributes,
                ReturnDataType = this.ReturnDataType,
                ReturnTaskType = this.ReturnTaskType,
                Parameters = this.Parameters.Select(item => (ApiParameterDescriptor)item.Clone()).ToArray()
            };
        }
    }
}