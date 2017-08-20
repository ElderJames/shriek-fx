using Shriek.Storage;
using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.DependencyInjection;
using Shriek.Utils;

namespace Shriek
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddShriek(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies())
            .AddClasses()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            // services.AddScoped<IEventStorage, InMemoryEventStorage>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}