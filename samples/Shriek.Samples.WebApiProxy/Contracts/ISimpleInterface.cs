using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    public interface ISimpleInterface
    {
        Task<string> Test(string sth);
    }
}