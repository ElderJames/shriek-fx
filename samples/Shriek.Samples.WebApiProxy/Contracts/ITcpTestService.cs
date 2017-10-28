using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    public interface ITcpTestService
    {
        Task<string> Test(string sth);
    }
}