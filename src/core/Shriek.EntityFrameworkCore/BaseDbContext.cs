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
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition().FullName == typeof(DbSet<>).FullName).Select(x => x.GetGenericArguments()[0]).ToArray();
        }

        /// <summary>
        /// 获取DbContext中所有作为DbSet<>的类型参数的实体类型对应的实体配置
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetEntityBuilderMaps()
        {
            var entities = GetEntityTypes();
            if (!entities.Any())
                return new Type[] { };

            return Reflection.GetAssemblies(type: entities.FirstOrDefault()).SelectMany(ass => ass.GetTypes())
                 .Where(x =>
                 {
                     //获取接口是IEntityTypeConfiguration<>以及接口的类型参数被包含在entities里
                     var intf = x.GetInterface(typeof(IEntityTypeConfiguration<>).Name);
                     return intf != null && intf.GetGenericArguments().Intersect(entities).Count() > 0;
                 }
               );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var map in GetEntityBuilderMaps())
            {
                modelBuilder.ApplyConfiguration((dynamic)Assembly.GetAssembly(map).CreateInstance(map.FullName));
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}