using Shriek.ServiceProxy.Tcp.Attributes;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    [ServiceContract]
    public interface ITcpTestService
    {
        Task<string> Test(string sth);
    }
}