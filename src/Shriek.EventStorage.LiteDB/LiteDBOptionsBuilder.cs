using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace Shriek.EventStorage.LiteDB
{
    public class LiteDBOptionsBuilder<TLiteDatabase> where TLiteDatabase : LiteDatabase
    {
        /// <summary>
        /// 数据库路径
        /// </summary>
        public string ConnectionString { get; set; }

        public IDiskService DiskService { get; set; }

        public BsonMapper Mapper { get; set; }

        public string Password { get; set; }
        public TimeSpan? Timeout { get; set; }

        public int CacheSize { get; set; } = 5000;

        public Logger Log { get; set; }
    }
}