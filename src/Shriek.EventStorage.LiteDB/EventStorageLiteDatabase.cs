using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.EventStorage.LiteDB
{
    public class EventStorageLiteDatabase : LiteDatabase
    {
        public EventStorageLiteDatabase(LiteDBOptionsBuilder<EventStorageLiteDatabase> options) : base(options.ConnectionString, options.Mapper)
        {
        }
    }
}