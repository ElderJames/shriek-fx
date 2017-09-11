using System;

namespace Shriek.WebApi.Proxy
{
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