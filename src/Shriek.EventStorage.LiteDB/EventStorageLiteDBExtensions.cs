using Shriek.EventSourcing;
using Shriek.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace Shriek.EventStorage.LiteDB
{
    public static class EventStorageLiteDBExtensions
    {
        public static void AddLiteDbEventStorage<TLiteDatabase>(this ServiceCollection services, Action<LiteDBOptions<TLiteDatabase>> optionAction) where TLiteDatabase : LiteDatabase
        {
            var options = new LiteDBOptions<TLiteDatabase>();
            optionAction(options);
            services.AddScoped(x => options);
            services.AddScoped<TLiteDatabase>();
            services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            services.AddScoped<IMementoRepository, EventStorageRepository>();
            services.AddScoped<IEventStorage, SqlEventStorage>();
        }
    }
}