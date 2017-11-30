using LiteDB;
using System;

namespace Shriek.EventStorage.LiteDB
{
    public class LiteDBOptions
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