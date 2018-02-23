using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.MongoDB
{
    public static class EventStorageMongoDBExtensions
    {
        public static void UseMongoDBEventStorage(this ShriekOptionBuilder builder, Action<MongoDBOptions> optionsAction)
        {
            var options = new MongoDBOptions();
            optionsAction(options);

            builder.Services.AddScoped(x => new MongoDatabase(options));
            builder.Services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            builder.Services.AddScoped<IMementoRepository, EventStorageRepository>();
            builder.Services.AddScoped<IEventStorage, DefalutEventStorage>();
        }
    }
}