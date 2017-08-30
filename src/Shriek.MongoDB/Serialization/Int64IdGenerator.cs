using System;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Shriek.MongoDB.Serialization
{
    public sealed class Int64IdGenerator : IntIdGeneratorBase
    {
        public Int64IdGenerator(string idCollectionName) : base(idCollectionName)
        {
        }

        public Int64IdGenerator() : base("IdInt64")
        {
        }

        protected override UpdateBuilder CreateUpdateBuilder()
        {
            return Update.Inc("seq", 1L);
        }

        protected override object ConvertToInt(BsonValue value)
        {
            return value.AsInt64;
        }

        public override bool IsEmpty(object id)
        {
            if (id is Int64)
                return (Int64)id == 0;
            else
                return (Int32)id == 0;
        }
    }
}