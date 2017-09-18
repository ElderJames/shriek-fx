using System.Threading.Tasks;
using Shriek.ServiceProxy.Tcp.Attributes;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    [ServiceContract]
    public interface ITcpTestService
    {
        Task<string> Test(string sth);
    }
}