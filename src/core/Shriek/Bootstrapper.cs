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
            CommandBus.ContainerAccessor = () => services.BuildServiceProvider();
            EventBus.ContainerAccessor = () => services.BuildServiceProvider();
            return services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies()).AddClasses().AsImplementedInterfaces().AsSelf().WithScopedLifetime());
        }
    }
}