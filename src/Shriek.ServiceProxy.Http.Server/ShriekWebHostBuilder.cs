using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.ServiceProxy.Http.Server
{
    public class ShriekWebHostBuilder : WebHostBuilder, IShriekBuilder
    {
        public IServiceCollection Services { get; }
    }
}