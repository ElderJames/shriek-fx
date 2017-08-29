using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.LiteDB
{
    public static class EventStorageLiteDBExtensions
    {
        public static void AddLiteDbEventStorage(this ServiceCollection services, Action<LiteDBOptions> optionAction)
        {
            var options = new LiteDBOptions();
            optionAction(options);
            services.AddScoped(x => options);
            services.AddScoped<EventStorageLiteDatabase>();
            services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            services.AddScoped<IMementoRepository, EventStorageRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}