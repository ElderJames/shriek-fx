using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Redis;

namespace Shriek.EventStorage.Redis
{
    public class RedisEventStorageOptions
    {
        public string Configuration { get; set; }
        public string InstanceName { get; set; }
    }
}