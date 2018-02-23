using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.EFCore
{
    public static class EventStorageEFCoreExtensions
    {
        public static void UseEFCoreEventStorage(this ShriekOptionBuilder builder, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            builder.Services.AddDbContext<EventStorageSQLContext>(optionsAction);
            builder.Services.AddScoped<IEventStorageRepository, EventStorageSQLRepository>();
            builder.Services.AddScoped<IMementoRepository, EventStorageSQLRepository>();
            builder.Services.AddScoped<IEventStorage, DefalutEventStorage>();
        }
    }
}