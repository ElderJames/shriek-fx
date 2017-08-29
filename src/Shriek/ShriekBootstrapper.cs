using Microsoft.Extensions.DependencyInjection;
using Shriek.Messages;
using Shriek.Storage;
using Shriek.Utils;
using System;

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