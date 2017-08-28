using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.EventStorage.LiteDB
{
    public class EventStorageLiteDatabase : LiteDatabase
    {
        public EventStorageLiteDatabase(LiteDBOptions options) : base(options.ConnectionString, options.Mapper)
        {
        }
    }
}