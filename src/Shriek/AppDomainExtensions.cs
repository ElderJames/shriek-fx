using Shriek.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shriek
{
    public static class AppDomainExtensions
    {
        private static string GetActualDomainPath(this AppDomain @this)
        {
            return @this.RelativeSearchPath ?? @this.BaseDirectory;
        }

        private static IEnumerable<Assembly> _excutingAssembiles;

        /// <summary>
        /// 获取引用了Shriek的程序集
        /// </summary>
        /// <param name="this"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetExcutingAssembiles(this AppDomain @this, Type type = null)
        {
            type = type ?? typeof(AppDomainExtensions);

            if (_excutingAssembiles == null || !_excutingAssembiles.Any())
                _excutingAssembiles = ReflectionUtil.GetAssemblies(new AssemblyFilter(@this.GetActualDomainPath())).Where(assembly =>
                   !assembly.IsDynamic && assembly.FullName == type.AssemblyQualifiedName.Replace(type.FullName + ", ", "")
                   || assembly.GetReferencedAssemblies().Any(ass => ass.FullName == type.AssemblyQualifiedName.Replace(type.FullName + ", ", ""))
                );

            return _excutingAssembiles;
        }
    }
}