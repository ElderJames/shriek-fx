using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace Shriek.EventStorage.LiteDB
{
    public static class EventStorageLiteDBExtensions
    {
        public static void UseLiteDbEventStorage<TLiteDatabase>(this ServiceCollection services, Action<LiteDBOptionsBuilder<TLiteDatabase>> optionAction) where TLiteDatabase : LiteDatabase
        {
            var options = new LiteDBOptionsBuilder<TLiteDatabase>();
            optionAction(options);
            services.AddScoped(x => options);
            services.AddScoped<TLiteDatabase>();
        }
    }
}