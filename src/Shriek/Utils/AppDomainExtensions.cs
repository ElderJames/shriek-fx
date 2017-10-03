using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shriek.Reflection;

namespace Shriek
{
    public static class AppDomainExtensions
    {
        public static string GetActualDomainPath(this AppDomain @this)
        {
            return @this.RelativeSearchPath ?? @this.BaseDirectory;
        }

        private static IEnumerable<Assembly> _excutingAssembiles;

        public static IEnumerable<Assembly> GetExcutingAssembiles(this AppDomain @this)
        {
            if (_excutingAssembiles == null || !_excutingAssembiles.Any())
                _excutingAssembiles = ReflectionUtil.GetAssemblies(new AssemblyFilter(AppDomain.CurrentDomain.GetActualDomainPath()));

            return _excutingAssembiles;
        }
    }
}