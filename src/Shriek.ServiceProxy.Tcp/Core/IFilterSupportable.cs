namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义支持过滤器功能的接口
    /// </summary>
    public interface IFilterSupportable
    {
        /// <summary>
        /// 获取全局过滤器管理者
        /// </summary>
        IGlobalFilters GlobalFilters { get; }

        /// <summary>
        /// 获取或设置Api行为特性过滤器提供者
        /// </summary>
        IFilterAttributeProvider FilterAttributeProvider { get; set; }
    }
}