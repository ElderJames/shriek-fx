using Shriek.Samples.WebApiProxy.Contracts;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class TcpTestService : ITcpTestService
    {
        public async Task<string> Test(string sth)
        {
            return await Task.FromResult("tcp return " + sth);
        }
    }
}