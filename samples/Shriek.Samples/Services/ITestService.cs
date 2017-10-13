using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http;
using System.Threading.Tasks;

namespace Shriek.Samples.Services
{
    [Route("test")]
    public interface ITestService
    {
        [HttpGet("{name}")]
        Task<string> Test(string name);
    }
}