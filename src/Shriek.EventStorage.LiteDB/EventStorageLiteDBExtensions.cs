using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.LiteDB
{
    public static class EventStorageLiteDBExtensions
    {
        public static void UseLiteDbEventStorage(this ShriekOptionBuilder builder, Action<LiteDBOptions> optionAction)
        {
            var options = new LiteDBOptions();
            optionAction(options);
            builder.Services.AddScoped(x => options);
            builder.Services.AddScoped<EventStorageLiteDatabase>();
            builder.Services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            builder.Services.AddScoped<IMementoRepository, EventStorageRepository>();
            builder.Services.AddScoped<IEventStorage, DefalutEventStorage>();
        }
    }
}