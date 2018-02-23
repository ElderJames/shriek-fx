using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.Dapper
{
    public static class EventStorageDapperExtensions
    {
        public static void UseDapperEventStorage(this ShriekOptionBuilder builder, Action<DapperOptions> optionsAction)
        {
            var options = new DapperOptions();
            optionsAction?.Invoke(options);

            builder.Services.AddScoped(x => options);
            builder.Services.AddScoped<IEventStorageRepository, EventRepository>();
            builder.Services.AddScoped<IMementoRepository, MementoRepository>();
            builder.Services.AddScoped<IEventStorage, DefalutEventStorage>();
        }
    }
}