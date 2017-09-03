using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;

namespace Shriek.MongoDB.Serialization
{
    public sealed class Int32IdGenerator<T> : IntIdGeneratorBase<T> where T : class
    {
        public Int32IdGenerator(string idCollectionName) : base(idCollectionName)
        {
        }

        public Int32IdGenerator() : base("IdInt32")
        {
        }

        protected override UpdateBuilder CreateUpdateBuilder()
        {
            return Update.Inc("seq", 1);
        }

        protected override object ConvertToInt(BsonValue value)
        {
            return value.AsInt32;
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