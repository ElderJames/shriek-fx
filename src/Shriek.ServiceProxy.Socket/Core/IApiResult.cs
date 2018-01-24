using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Socket.Core
{
    /// <summary>
    /// 定义IApiResult接口
    /// </summary>
    public interface IApiResult
    {
        /// <summary>
        /// 返回Task对象
        /// </summary>
        /// <returns></returns>
        Task GetTask();
    }

    /// <summary>
    /// 定义IApiResult接口
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    public interface IApiResult<TResult>
    {
        /// <summary>
        /// 返回Task对象
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetTask();
    }
}