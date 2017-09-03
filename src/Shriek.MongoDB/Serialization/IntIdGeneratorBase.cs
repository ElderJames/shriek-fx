using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Shriek.MongoDB.Serialization
{
    public abstract class IntIdGeneratorBase<T> : IIdGenerator where T : class
    {
        private string idCollectionName;

        protected IntIdGeneratorBase(string idCollectionName)
        {
            this.idCollectionName = idCollectionName;
        }

        protected IntIdGeneratorBase() : this("IDs")
        {
        }

        protected abstract UpdateBuilder CreateUpdateBuilder();

        protected abstract object ConvertToInt(BsonValue value);

        public abstract bool IsEmpty(object id);

        public object GenerateId(object container, object document)
        {
            var idSequenceCollection = ((IMongoCollection<T>)container).Database
              .GetCollection<BsonDocument>(idCollectionName);

            var collectionName = document.GetType().Name;

            var filterQuery = Builders<BsonDocument>.Filter.Eq("_id", collectionName);
            var updates = Builders<BsonDocument>.Update.Inc("seq", 1);
            var updateOptions = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var doc = idSequenceCollection.FindOneAndUpdate(filterQuery, updates, updateOptions);

            return ConvertToInt(doc["seq"]);
        }
    }
}