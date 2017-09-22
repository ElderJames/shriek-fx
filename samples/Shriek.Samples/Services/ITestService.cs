using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.Services
{
    [Route("test")]
    public interface ITestService
    {
        [HttpGet("{name}")]
        Task<string> Test(string name);
    }
}