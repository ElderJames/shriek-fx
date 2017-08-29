using LiteDB;

namespace Shriek.EventStorage.LiteDB
{
    public class EventStorageLiteDatabase : LiteDatabase
    {
        public EventStorageLiteDatabase(LiteDBOptions options) : base(options.ConnectionString, options.Mapper)
        {
        }
    }
}