using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Shriek.DependencyInjection;
using Shriek.Utils;

namespace Shriek
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddShriek(this IServiceCollection services)
        {
            return services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies()));
        }
    }
}