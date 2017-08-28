using MongoDB.Driver;
using System;
using MongoDB.Bson;
using System.Threading;
using System.Threading.Tasks;

namespace Shriek.EventStorage.MongoDB
{
    public interface IEventStorageMongoDatabase : IMongoDatabase
    {
    }
}