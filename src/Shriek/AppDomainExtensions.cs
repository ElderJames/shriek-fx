using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shriek
{
    public static class AppDomainExtensions
    {
        private static readonly object Locker = new object();

        private static IEnumerable<Assembly> excutingAssembiles;

        private static Type[] typeCache;

        private static string GetActualDomainPath(this AppDomain @this)
        {
            return @this.RelativeSearchPath ?? @this.BaseDirectory;
        }

        /// <summary>
        /// 获取引用了Shriek的程序集
        /// </summary>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetExcutingAssemblies(this AppDomain @this, Func<Assembly, bool> predicate)
        {
            if (excutingAssembiles == null || !excutingAssembiles.Any())
                lock (Locker)
                {
                    if (excutingAssembiles == null || !excutingAssembiles.Any())
                        excutingAssembiles = DependencyContext.Default.RuntimeLibraries
                            .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                            .Select(Assembly.Load)
                            .Where(predicate)
                            .ToArray();
                }

            return excutingAssembiles;
        }

        public static IEnumerable<Assembly> GetExcutingAssemblies(this AppDomain @this) => @this.GetExcutingAssemblies(_ => true);

        public static void UpdateExcutingAssemblies(this AppDomain @this, Func<Assembly, bool> predicate)
        {
            var assemblies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .Where(predicate)
                .ToArray();

            excutingAssembiles = @this.GetExcutingAssemblies().Union(assemblies).Union(new[] { Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() }).Distinct();
        }

        public static void UpdateExcutingAssemblies(this AppDomain @this) => @this.UpdateExcutingAssemblies(_ => true);

        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <param name="this">程序域</param>
        /// <param name="fromCache">从缓存获取</param>
        /// <returns></returns>
        public static Type[] GetAllTypes(this AppDomain @this, bool fromCache = true)
        {
            if (fromCache && (typeCache == null || !typeCache.Any()) || !fromCache)
            {
                typeCache = @this.GetExcutingAssemblies()
                    .SelectMany(x =>
                    {
                        try
                        {
                            return x.DefinedTypes.Select(t => t.AsType());
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            return ex.Types.Where(t => t != null);
                        }
                    }).ToArray();
            }

            return typeCache;
        }
    }
}