using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.EventSourcing.Sql.EFCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.EventStorage.EFCore
{
    public static class EventStorageEFCoreExtensions
    {
        public static void AddEFCoreEventStorage(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            services.AddDbContext<EventStorageSQLContext>(optionsAction);
            services.AddScoped<IEventStorageRepository, EventStorageSQLRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}