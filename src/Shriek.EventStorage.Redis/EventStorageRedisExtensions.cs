using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;
using System.Linq;

namespace Shriek.EventStorage.Redis
{
    public static class EventStorageRedisExtensions
    {
        public static void UseRedisEventStorage(this ShriekOptionBuilder builder, Action<RedisEventStorageOptions> optionAction)
        {
            var option = new RedisEventStorageOptions();

            optionAction(option);

            if (builder.Services.All(x => x.ServiceType != typeof(IDistributedCache)))
            {
                builder.Services.AddSingleton<ICacheService>(x => new RedisCacheService(new RedisCache(new RedisCacheOptions
                {
                    Configuration = option.Configuration,
                    InstanceName = option.InstanceName
                })));
            }
            else
            {
                builder.Services.AddSingleton<ICacheService, RedisCacheService>();
            }

            builder.Services.AddSingleton<IEventStorageRepository, EventStorageRepository>();
            builder.Services.AddSingleton<IMementoRepository, EventStorageRepository>();
            builder.Services.AddSingleton<IEventStorage, SqlEventStorage>();
        }
    }
}