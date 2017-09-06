using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.WebApi.Proxy
{
    //TODO:在uri模板实现控制器路由前缀
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// 获取路由
        /// </summary>
        public string Template { get; private set; }

        /// <summary>
        /// 设置路由
        /// </summary>
        /// <param name="template">路由</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public RouteAttribute(string template)
        {
            this.Template = template;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Template;
        }
    }
}