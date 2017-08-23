using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Utils;

namespace Shriek.EntityFrameworkCore
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// 获取DbContext中所有作为DbSet<>的类型参数的实体类型
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetEntityTypes()
        {
            return this.GetType().GetProperties((BindingFlags.Public | BindingFlags.Instance)).Select(x => x.PropertyType)
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(x => x.GetGenericArguments()[0]).ToArray();
        }

        /// <summary>
        /// 获取DbContext中所有作为DbSet<>的类型参数的实体类型对应的实体配置
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetEntityTypeConfigurations()
        {
            //var entities = GetEntityTypes();
            //if (!entities.Any())
            //    return new Type[] { };

            var types = Reflection.GetAssemblies().SelectMany(ass => ass.GetTypes())
                 .Where(x =>
                 {
                     //获取IEntityTypeConfiguration<>的实现类
                     var type = x.GetInterface(typeof(IEntityTypeConfiguration<,>).Name);
                     return type != null && type.GetGenericArguments().Contains(GetType());
                 });
            return types;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var map in GetEntityTypeConfigurations())
            {
                modelBuilder.ApplyConfiguration((dynamic)Activator.CreateInstance(map));
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}