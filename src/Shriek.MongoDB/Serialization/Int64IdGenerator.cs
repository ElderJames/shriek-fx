using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;

namespace Shriek.MongoDB.Serialization
{
    public sealed class Int64IdGenerator<T> : IntIdGeneratorBase<T> where T : class
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
            if (value.BsonType == BsonType.Int32)
                return (Int64)value.AsInt32;

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