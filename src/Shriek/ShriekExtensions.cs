using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.Events;
using Shriek.Messages;
using Shriek.Notifications;
using Shriek.Storage;
using Shriek.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace Shriek
{
    public static class ShriekExtensions
    {
        public static IShriekBuilder AddShriek(this IServiceCollection services, Action<ShriekOptionBuilder> optionAction = null)
        {
            var builder = new ShriekBuilder(services);

            builder.Services.Scan(scan => scan.FromAssemblies(Utils.Reflection.ExcutingAssembiles)
            .AddClasses()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            builder.Services.AddScoped<IEventStorage, InMemoryEventStorage>();
            builder.Services.AddScoped<IMessagePublisher, InProcessMessagePublisher>();

            builder.Services.AddSingleton(typeof(IMessageSubscriber<DomainNotification>), typeof(EventMessageSubscriber<DomainNotification>));

            var messages = Utils.Reflection.ExcutingAssembiles.SelectMany(x => x.GetTypes()).Where(x => x.Assembly != Assembly.GetExecutingAssembly() && typeof(Message).IsAssignableFrom(x));

            foreach (var msg in messages)
            {
                var type = typeof(IMessageSubscriber<>).MakeGenericType(msg);
                if (typeof(Command).IsAssignableFrom(msg))
                {
                    var impl = typeof(CommandMessageSubscriber<>).MakeGenericType(msg);
                    builder.Services.AddSingleton(type, impl);
                }
                if (typeof(Event).IsAssignableFrom(msg))
                {
                    var impl = typeof(EventMessageSubscriber<>).MakeGenericType(msg);
                    builder.Services.AddSingleton(type, impl);
                }
            }

            if (optionAction != null)
            {
                var options = new ShriekOptionBuilder(services);
                optionAction(options);
            }

            return builder;
        }
    }
}