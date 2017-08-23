using System;
using Shriek.Messages;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Storage;
using Shriek.Utils;

namespace Shriek
{
    public static class ShriekBootstrapper
    {
        public static IServiceCollection AddShriek(this IServiceCollection services, Action<ShriekOption> optionAction = null)
        {
            services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies())
            .AddClasses()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            services.AddScoped<IEventStorage, InMemoryEventStorage>();
            services.AddTransient<IMessagePublisher, InProcessMessagePublisher>();

            if (optionAction != null)
            {
                var options = new ShriekOption();
                optionAction(options);
            }

            return services;
        }
    }
}