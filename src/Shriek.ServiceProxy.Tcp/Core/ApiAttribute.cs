using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 表示Api标记特性
    /// 提供给远程端来调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiAttribute : Attribute
    {
        /// <summary>
        /// Api名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Api标记特性
        /// 提供给远程端来调用
        /// </summary> 
        public ApiAttribute()
        {
        }
        /// <summary>
        /// Api标记特性
        /// 提供给远程端来调用
        /// </summary>       
        /// <param name="name">Api名称</param>       
        public ApiAttribute(string name)
        {
            this.Name = name;
        }
    }
}
