using Shriek.Samples.WebApiProxy.Contracts;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class SampleTestService : Samples.Services.ITestService
    {
        private readonly ISimpleInterface socketService;

        public SampleTestService(ISimpleInterface socketService)
        {
            this.socketService = socketService;
        }

        public async Task<string> Test(string name)
        {
            return "http服务中通过socket请求另一个服务：" + await socketService.Test(name);
        }
    }
}