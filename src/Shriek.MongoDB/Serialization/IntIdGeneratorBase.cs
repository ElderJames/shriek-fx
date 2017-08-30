using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;

namespace Shriek.MongoDB.Serialization
{
    public abstract class IntIdGeneratorBase : IIdGenerator
    {
        private string m_idCollectionName;

        protected IntIdGeneratorBase(string idCollectionName)
        {
            m_idCollectionName = idCollectionName;
        }

        protected IntIdGeneratorBase() : this("IDs")
        {
        }

        protected abstract UpdateBuilder CreateUpdateBuilder();

        protected abstract object ConvertToInt(BsonValue value);

        public abstract bool IsEmpty(object id);

        public object GenerateId(object container, object document)
        {
            var idSequenceCollection = ((MongoCollection)container).Database
                .GetCollection(m_idCollectionName);

            var query = Query.EQ("_id", ((MongoCollection)container).Name);

            return ConvertToInt(idSequenceCollection.FindAndModify(new FindAndModifyArgs()
            {
                Query = query,
                Update = CreateUpdateBuilder(),
                VersionReturned = FindAndModifyDocumentVersion.Modified,
                Upsert = true
            }).ModifiedDocument["seq"]);
        }
    }
}