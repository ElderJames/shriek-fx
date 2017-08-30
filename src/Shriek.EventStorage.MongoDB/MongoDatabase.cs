using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using Shriek.MongoDB.Serialization;

namespace Shriek.EventStorage.MongoDB
{
    public class MongoDatabase
    {
        public IMongoDatabase Database { get; }

        public MongoDatabase(MongoDBOptions options)
        {
            BsonClassMap.RegisterClassMap<StoredEvent>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id).SetIdGenerator(new Int32IdGenerator<StoredEvent>());
            });

            BsonClassMap.RegisterClassMap<Memento>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id).SetIdGenerator(new Int32IdGenerator<Memento>());
            });

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(options.ConnectionString));
            if (options.IsSSL)
            {
                settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            }
            var mongoClient = new MongoClient(settings);
            Database = mongoClient.GetDatabase(options.DatabaseName);
        }
    }
}