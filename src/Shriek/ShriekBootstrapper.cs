using Shriek.Messages;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Storage;
using Shriek.Utils;

namespace Shriek
{
    public static class ShriekBootstrapper
    {
        public static IServiceCollection AddShriek(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies())
            .AddClasses()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            services.AddScoped<IEventStorage, InMemoryEventStorage>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IMessageProcessor, InProcessMessageProcessor>();
            return services;
        }
    }
}