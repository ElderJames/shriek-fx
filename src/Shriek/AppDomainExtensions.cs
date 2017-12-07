using Shriek.Reflection;
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

        private static string GetActualDomainPath(this AppDomain @this)
        {
            return @this.RelativeSearchPath ?? @this.BaseDirectory;
        }

        /// <summary>
        /// 获取引用了Shriek的程序集
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetExcutingAssemblies(this AppDomain @this)
        {
            if (excutingAssembiles == null || !excutingAssembiles.Any())
                lock (Locker)
                {
                    if (excutingAssembiles == null || !excutingAssembiles.Any())
                        excutingAssembiles = ReflectionUtil.GetAssemblies(new AssemblyFilter(@this.GetActualDomainPath()));
                }

            return excutingAssembiles;
        }

        public static void UpdateExcutingAssemblies(this AppDomain @this)
        {
            try
            {
                var assemblies = ReflectionUtil.GetAssemblies(new AssemblyFilter(@this.GetActualDomainPath()));

                excutingAssembiles = @this.GetExcutingAssemblies().Union(assemblies)
                    .Union(new[] { Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() }).Distinct();
            }
            catch
            {
            }
        }
    }
}