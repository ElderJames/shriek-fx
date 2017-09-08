using Microsoft.Extensions.DependencyInjection;

namespace Shriek
{
    public interface IShriekBuilder
    {
        IServiceCollection Services { get; }
    }
}