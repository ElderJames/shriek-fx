using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.MongoDB
{
    public static class EventStorageMongoDBExtensions
    {
        public static void AddMongoDBEventStorage(this IServiceCollection services, Action<MongoDBOptions> optionsAction)
        {
            var options = new MongoDBOptions();
            optionsAction(options);

            services.AddScoped(x => new MongoDatabase(options));

            services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            services.AddScoped<IMementoRepository, EventStorageRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}