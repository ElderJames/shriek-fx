using Microsoft.Extensions.DependencyInjection;

namespace Shriek
{
    public class ShriekOptionBuilder
    {
        public ShriekOptionBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}