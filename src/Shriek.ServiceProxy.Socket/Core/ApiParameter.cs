using System;
using System.Reflection;

namespace Shriek.ServiceProxy.Socket.Core
{
    /// <summary>
    /// 表示Api参数
    /// </summary>
    public sealed class ApiParameter
    {
        /// <summary>
        /// 获取参数信息
        /// </summary>
        public ParameterInfo Info { get; private set; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public Type Type
        {
            get
            {
                return this.Info.ParameterType;
            }
        }

        /// <summary>
        /// 获取参数名
        /// </summary>
        public string Name
        {
            get
            {
                return this.Info.Name;
            }
        }

        /// <summary>
        /// 获取参数的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Api参数
        /// </summary>
        /// <param name="info">参数信息</param>
        public ApiParameter(ParameterInfo info)
        {
            this.Info = info;
        }

        /// <summary>
        /// 是否定义指定类型的特性
        /// </summary>
        /// <typeparam name="T">特性的类型</typeparam>
        /// <returns></returns>
        public bool IsDefined<T>() where T : Attribute
        {
            return this.Info.IsDefined(typeof(T), false);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}