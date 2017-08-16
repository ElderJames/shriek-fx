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
            services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies()).AddClasses().AsImplementedInterfaces().AsMatchingInterface().AsSelf().WithScopedLifetime());

            //services.Scan(scan => scan.FromAssemblyOf<IEventStorage>().AddClasses(x => x.AssignableTo<IEventStorage>()).UsingRegistrationStrategy(RegistrationStrategy.Replace()).AsImplementedInterfaces().WithSingletonLifetime());
            //services.AddScoped<ICommandBus, CommandBus>();
            //services.AddScoped<IEventBus, EventBus>();
            services.AddSingleton<IEventStorage, InMemoryEventStorage>();
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

            CommandBus.ContainerAccessor = () => services.BuildServiceProvider();
            EventBus.ContainerAccessor = () => services.BuildServiceProvider();

            return services;
        }
    }
}