using Shriek.Samples.WebApiProxy.Contracts;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class SampleTestService : Samples.Services.ITestService
    {
        public async Task<string> Test(string name)
        {
            return await Task.FromResult(name);
        }
    }
}