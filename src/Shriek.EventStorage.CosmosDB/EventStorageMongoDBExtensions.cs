using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;

namespace Shriek.EventStorage.MongoDB
{
    public static class EventStorageMongoDBExtensions
    {
        public static void AddMongoDBEventStorage(this IServiceCollection services, Action<MongoDBOptions> optionsAction)
        {
            var options = new MongoDBOptions();
            optionsAction(options);

            BsonClassMap.RegisterClassMap<StoredEvent>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
            });

            BsonClassMap.RegisterClassMap<Memento>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
            });

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(options.ConnectionString));
            if (options.IsSSL)
            {
                settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            }
            var mongoClient = new MongoClient(settings);
            var database = mongoClient.GetDatabase(options.DatabaseName);

            services.AddScoped(x => database);

            services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            services.AddScoped<IMementoRepository, EventStorageRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}