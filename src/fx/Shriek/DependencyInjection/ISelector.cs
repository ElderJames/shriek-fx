using Microsoft.Extensions.DependencyInjection;

namespace Shriek.DependencyInjection
{
    internal interface ISelector
    {
        void Populate(IServiceCollection services, RegistrationStrategy options);
    }
}