using Shriek.Samples.WebApiProxy.Contracts;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class SampleTestService : Samples.Services.ITestService
    {
        private ITcpTestService testService;

        public SampleTestService(ITcpTestService testService)
        {
            this.testService = testService;
        }

        public async Task<string> Test(string name)
        {
            return await testService.Test(name);
        }
    }
}