using Shriek.Samples.WebApiProxy.Contracts;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Socket.Core;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class SimpleInterface : ISimpleInterface
    {
        public async Task<string> Test(string sth)
        {
            return await Task.FromResult("tcp return " + sth);
        }
    }
}