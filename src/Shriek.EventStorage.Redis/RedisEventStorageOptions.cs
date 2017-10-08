using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Redis;

namespace Shriek.EventStorage.Redis
{
    public class RedisEventStorageOptions
    {
        public RedisCacheOptions RedisCacheOptions { get; set; }
    }
}