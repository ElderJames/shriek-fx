using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System;

namespace Shriek.MongoDB.Serialization
{
    public class Int32IdGenerator : IntIdGeneratorBase
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
            return (Int32)id == 0;
        }
    }
}