using Shriek.Samples.WebApiProxy.Contracts;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Services
{
    /// <summary>
    /// 样例测试
    /// </summary>
    public class SampleTestService : Samples.Services.ITestService
    {
        private readonly ISimpleInterface socketService;

        public SampleTestService(ISimpleInterface socketService)
        {
            this.socketService = socketService;
        }

        /// <summary>
        /// 传入字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<string> Test(string name)
        {
            return "http服务中通过socket请求另一个服务：" + await socketService.Test(name);
        }
    }
}