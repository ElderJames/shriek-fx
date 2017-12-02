using Shriek.ServiceProxy.Abstractions.Attributes;
using System;

namespace Shriek.ServiceProxy.Abstractions
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    public class ApiParameterDescriptor : ICloneable
    {
        /// <summary>
        /// 获取参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取参数索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取关联的参数特性
        /// </summary>
        public ApiParameterAttribute[] Attributes { get; set; }

        /// <summary>
        /// 获取参数类型是否为Uri参数类型
        /// </summary>
        public bool IsUriParameterType { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{ParameterType.FullName}]:{this.Name} = {this.Value}";
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ApiParameterDescriptor
            {
                Attributes = this.Attributes,
                Index = this.Index,
                IsUriParameterType = this.IsUriParameterType,
                Name = this.Name,
                ParameterType = this.ParameterType,
                Value = this.Value
            };
        }
    }
}