using Shriek.Samples.WebApiProxy.Contacts;
using Shriek.Samples.WebApiProxy.Models;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class TestService : ITestService
    {
        public Todo Test(int id)
        {
            return new Todo() { Name = id.ToString() };
        }
    }
}