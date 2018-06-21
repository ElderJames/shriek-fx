using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.Events;
using Shriek.Messages;
using Shriek.Notifications;
using Shriek.Storage;
using System;
using System.Linq;
using System.Reflection;
using Shriek.EventSourcing;

namespace Shriek
{
    public static class ShriekExtensions
    {
        public static IShriekBuilder AddShriek(this IServiceCollection services, Action<ShriekOptionBuilder> optionAction = null)
        {
            var builder = new ShriekBuilder(services);

            var allTyeps = AppDomain.CurrentDomain.GetAllTypes(false);
            var interfaces = allTyeps.Where(x => x.IsInterface);
            foreach (var itfc in interfaces.Where(x => !x.IsGenericTypeDefinition))
            {
                var impls = allTyeps.Where(x => x.IsClass && !x.IsGenericType && !x.IsAbstract && itfc.IsAssignableFrom(x));
                foreach (var impl in impls)
                {
                    builder.Services.Add(new ServiceDescriptor(itfc, impl, ServiceLifetime.Scoped));
                }
            }

            var handlers = allTyeps.Where(x => x.IsClass && x.GetInterfaces().Any(o => o.IsGenericType && (o.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || o.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
            foreach (var hdl in handlers)
            {
                foreach (var itf in hdl.GetInterfaces())
                {
                    builder.Services.Add(new ServiceDescriptor(itf, hdl, ServiceLifetime.Transient));
                }
            }

            builder.Services.AddScoped<IMementoRepository, InMemoryEventStorageRepository>();
            builder.Services.AddScoped<IEventStorageRepository, InMemoryEventStorageRepository>();
            builder.Services.AddScoped<IEventStorage, DefalutEventStorage>();
            builder.Services.AddScoped<IMessagePublisher, InProcessMessagePublisher>();

            builder.Services.AddSingleton(typeof(IMessageSubscriber<DomainNotification>), typeof(EventMessageSubscriber<DomainNotification>));

            var messages = allTyeps.Where(x => x.Assembly != Assembly.GetExecutingAssembly() && typeof(Message).IsAssignableFrom(x));

            foreach (var msg in messages)
            {
                var type = typeof(IMessageSubscriber<>).MakeGenericType(msg);
                if (typeof(ICommand).IsAssignableFrom(msg))
                {
                    var impl = typeof(CommandMessageSubscriber<>).MakeGenericType(msg);
                    builder.Services.AddSingleton(type, impl);
                }
                else if (typeof(IEvent).IsAssignableFrom(msg))
                {
                    var impl = typeof(EventMessageSubscriber<>).MakeGenericType(msg);
                    builder.Services.AddSingleton(type, impl);
                }
                else
                {
                    allTyeps.Where(x => type.IsAssignableFrom(x)).ForEach(x => builder.Services.AddSingleton(type, x));
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