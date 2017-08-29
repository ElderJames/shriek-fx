using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.EFCore
{
    public static class EventStorageEFCoreExtensions
    {
        public static void AddEFCoreEventStorage(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            services.AddDbContext<EventStorageSQLContext>(optionsAction);
            services.AddScoped<IEventStorageRepository, EventStorageSQLRepository>();
            services.AddScoped<IMementoRepository, EventStorageSQLRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}