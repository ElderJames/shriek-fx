using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shriek.Reflection;

namespace Shriek.Utils
{
    /// <summary>
    /// 反射工具
    /// </summary>
    public static class Reflection
    {
        private static IEnumerable<Assembly> _excutingAssembiles;

        public static IEnumerable<Assembly> ExcutingAssembiles
        {
            get
            {
                if (_excutingAssembiles == null || !_excutingAssembiles.Any())
                    _excutingAssembiles = ReflectionUtil.GetAssemblies(new AssemblyFilter(AppDomain.CurrentDomain.GetActualDomainPath()));

                return _excutingAssembiles;
            }
        }

        public static IEnumerable<Assembly> GetAssemblies(string filter = null, Type type = null)
        {
            List<Assembly> assemblies = new List<Assembly>();

            type = type ?? typeof(Reflection);

            var libs = DependencyContext.Default.CompileLibraries;
            foreach (var lib in libs.Where(lib => !lib.Serviceable && lib.Type != "package" && (string.IsNullOrEmpty(filter) || lib.Name.ToLower().Contains(filter.ToLower()))))
            {
                var assembly = Assembly.Load(new AssemblyName(lib.Name));

                if (assembly.IsDynamic) continue;

                //匹配tyoe所在程序集
                if (assembly.FullName == type.AssemblyQualifiedName.Replace(type.FullName + ", ", ""))
                {
                    assemblies.Add(assembly);
                    continue;
                }

                //匹配引用了type的程序集
                if (assembly.GetReferencedAssemblies().Any(ass => ass.FullName == type.AssemblyQualifiedName.Replace(type.FullName + ", ", "")))
                {
                    assemblies.Add(assembly);
                }
            }

            return assemblies;
        }

        #region 类型搜索

        /// <summary>
        /// 获取子类型
        /// </summary>
        /// <param name="type">父类型</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypes(Type type)
        {
            var assemblies = GetAssemblies().Where(a =>
            {
                Assembly assembly = type.GetTypeInfo().Assembly;
                //基类所在程序集或依赖于基类的其他程序集
                return a.FullName == assembly.FullName || a.GetReferencedAssemblies().Any(ra => ra.FullName == assembly.FullName);
            });

            TypeInfo typeInfo = type.GetTypeInfo();

            return assemblies.SelectMany(a =>
            {
                return a.GetTypes().Where(t =>
                {
                    if (type == t)
                    {
                        return false;
                    }

                    TypeInfo tInfo = t.GetTypeInfo();

                    if (tInfo.IsAbstract || !tInfo.IsClass || !tInfo.IsPublic)
                    {
                        return false;
                    }

                    //if (typeInfo.IsGenericType )
                    //{
                    //    return type == t.GetGenericTypeDefinition();
                    //}

                    return type.IsAssignableFrom(t);
                });
            });
        }

        /// <summary>
        /// 获取子类型
        /// </summary>
        /// <typeparam name="T">父类型</typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypes<T>()
        {
            return GetSubTypes(typeof(T));
        }

        /// <summary>
        /// 获取程序集里以该类型为类型参数的所有的泛型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetGenericTypes(Type type)
        {
            var assemblies = GetAssemblies().Where(a =>
            {
                Assembly assembly = type.GetTypeInfo().Assembly;
                //基类所在程序集或依赖于基类的其他程序集
                return a.FullName == assembly.FullName || a.GetReferencedAssemblies().Any(ra => ra.FullName == assembly.FullName);
            });

            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => !t.GetTypeInfo().IsGenericType && t.GetInterfaces().Any(aa => aa.GetTypeInfo().IsGenericType && aa.GetGenericTypeDefinition() == type))
                //.Where(t => t.GetInterfaces().Any(aa => aa.GetGenericArguments().Any(ii => ii == type)))
                .ToList();

            return types;
        }

        #endregion 类型搜索
    }
}