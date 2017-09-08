using Microsoft.Extensions.DependencyInjection;
using Shriek.Messages;
using Shriek.Storage;
using Shriek.Utils;
using System;

namespace Shriek
{
    public static class ShriekExtensions
    {
        public static IShriekBuilder AddShriek(this IServiceCollection services, Action<ShriekOption> optionAction = null)
        {
            var builder = new ShriekBuilder(services);

            builder.Services.Scan(scan => scan.FromAssemblies(Reflection.GetAssemblies())
            .AddClasses()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            builder.Services.AddScoped<IEventStorage, InMemoryEventStorage>();
            builder.Services.AddTransient<IMessagePublisher, InProcessMessagePublisher>();

            if (optionAction != null)
            {
                var options = new ShriekOption();
                optionAction(options);
            }

            return builder;
        }
    }
}