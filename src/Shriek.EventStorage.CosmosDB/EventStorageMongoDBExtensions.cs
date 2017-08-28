using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
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
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Memento>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(options.ConnectionString));
            if (options.IsSSL)
            {
                settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            }
            var mongoClient = new MongoClient(settings);
            services.AddScoped(typeof(IEventStorageMongoDatabase), x => mongoClient.GetDatabase(options.DatabaseName));

            services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            services.AddScoped<IMementoRepository, EventStorageRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}