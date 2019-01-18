using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Shriek.Reflection
{
    internal static class ReflectionUtil
    {
        /// <summary>
        /// The factories
        /// </summary>
        private static readonly ConcurrentCache<ConstructorInfo, Func<object[], object>> factories = new ConcurrentCache<ConstructorInfo, Func<object[], object>>();

        /// <summary>
        /// The open generic array interfaces
        /// </summary>
        public static readonly Type[] OpenGenericArrayInterfaces = typeof(object[]).GetInterfaces()
                                                                                   .Where(i => i.IsGenericType)
                                                                                   .Select(i => i.GetGenericTypeDefinition())
                                                                                   .ToArray();

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <param name="subtypeofTBase">The subtypeof t base.</param>
        /// <param name="ctorArgs">The ctor arguments.</param>
        /// <returns></returns>
        public static TBase CreateInstance<TBase>(this Type subtypeofTBase, params object[] ctorArgs)
        {
            EnsureIsAssignable<TBase>(subtypeofTBase);

            return Instantiate<TBase>(subtypeofTBase, ctorArgs ?? new object[0]);
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="rootAssembly">The root assembly.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static IEnumerable<Assembly> GetApplicationAssemblies(Assembly rootAssembly)
        {
            int index = rootAssembly.FullName.IndexOfAny(new[] { '.', ',' });
            if (index < 0)
            {
                throw new ArgumentException(
                    $"Could not determine application name for assembly \"{rootAssembly.FullName}\". Please use a different method for obtaining assemblies.");
            }

            string applicationName = rootAssembly.FullName.Substring(0, index);
            var assemblies = new HashSet<Assembly>();
            AddApplicationAssemblies(rootAssembly, assemblies, applicationName);
            return assemblies;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The assembly filter.</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies(AssemblyFilter assemblyFilter)
        {
            return assemblyFilter.GetAssemblies();
        }

        /// <summary>
        /// Gets the assemblies contains.
        /// </summary>
        /// <param name="assemblyPrefix">The assembly prefix.</param>
        /// <param name="assemblyFilter">The assembly filter.</param>
        /// <returns></returns>

        public static IEnumerable<Assembly> GetAssembliesContains(string assemblyPrefix, AssemblyFilter assemblyFilter)
        {
            return assemblyFilter.GetAssemblies().Where(assembly => assembly.FullName.Contains(assemblyPrefix));
        }

        /// <summary>
        /// Gets the assembly named.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Assembly GetAssemblyNamed(string assemblyName)
        {
            try
            {
                Assembly assembly;
                if (IsAssemblyFile(assemblyName))
                {
                    if (Path.GetDirectoryName(assemblyName) == AppDomain.CurrentDomain.BaseDirectory)
                    {
                        assembly = Assembly.LoadFrom(Path.GetFileNameWithoutExtension(assemblyName));
                    }
                    else
                    {
                        assembly = Assembly.LoadFile(assemblyName);
                    }
                }
                else
                {
                    assembly = Assembly.LoadFrom(assemblyName);
                }
                return assembly;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (FileLoadException)
            {
                throw;
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    var exFileNotFound = exSub as FileNotFoundException;
                    if (!string.IsNullOrEmpty(exFileNotFound?.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }
                    sb.AppendLine();
                }
                var errorMessage = sb.ToString();
                throw new Exception(errorMessage, ex);
            }
            catch (BadImageFormatException)
            {
                throw;
            }
            catch (Exception e)
            {
                // in theory there should be no other exception kind
                throw new Exception($"Could not load assembly {assemblyName}", e);
            }
        }

        /// <summary>
        /// Gets the assembly named.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="nameFilter">The name filter.</param>
        /// <param name="assemblyFilter">The assembly filter.</param>
        /// <returns></returns>
        public static Assembly GetAssemblyNamed(string filePath, Predicate<AssemblyName> nameFilter, Predicate<Assembly> assemblyFilter)
        {
            AssemblyName assemblyName = GetAssemblyName(filePath);
            if (nameFilter != null)
            {
                foreach (Delegate @delegate in nameFilter.GetInvocationList())
                {
                    var predicate = (Predicate<AssemblyName>)@delegate;
                    if (predicate(assemblyName) == false)
                    {
                        return null;
                    }
                }
            }

            Assembly assembly = LoadAssembly(assemblyName);
            if (assemblyFilter != null)
            {
                foreach (Delegate @delegate in assemblyFilter.GetInvocationList())
                {
                    var predicate = (Predicate<Assembly>)@delegate;
                    if (predicate(assembly) == false)
                    {
                        return null;
                    }
                }
            }

            return assembly;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>

        public static TAttribute[] GetAttributes<TAttribute>(this MemberInfo item) where TAttribute : Attribute
        {
            return (TAttribute[])Attribute.GetCustomAttributes(item, typeof(TAttribute), true);
        }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="includeNonExported">if set to <c>true</c> [include non exported].</param>
        /// <returns></returns>

        public static Type[] GetAvailableTypes(this Assembly assembly, bool includeNonExported = false)
        {
            try
            {
                if (includeNonExported)
                {
                    return assembly.GetTypes();
                }

                return assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.FindAll(t => t != null);

                // NOTE: perhaps we should not ignore the exceptions here.
            }
        }

        /// <summary>
        /// Gets the available types ordered.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="includeNonExported">if set to <c>true</c> [include non exported].</param>
        /// <returns></returns>

        public static Type[] GetAvailableTypesOrdered(this Assembly assembly, bool includeNonExported = false)
        {
            return assembly.GetAvailableTypes(includeNonExported).OrderBy(t => t.FullName).ToArray();
        }

        /// <summary>
        /// If the extended type is a Foo[] or IEnumerable{Foo} which is assignable from Foo[] this
        /// method will return typeof(Foo) otherwise <c>null</c>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>

        public static Type GetCompatibleArrayItemType(this Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (type.IsGenericType == false || type.IsGenericTypeDefinition)
            {
                return null;
            }

            Type openGeneric = type.GetGenericTypeDefinition();
            if (OpenGenericArrayInterfaces.Contains(openGeneric))
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns></returns>

        public static Assembly[] GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Determines whether [has default value].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [has default value] [the specified item]; otherwise, <c>false</c>.</returns>
        public static bool HasDefaultValue(this ParameterInfo item)
        {
            return (item.Attributes & ParameterAttributes.HasDefault) != 0;
        }

        /// <summary>
        /// Instantiates the specified ctor arguments.
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        /// <param name="ctorArgs">The ctor arguments.</param>
        /// <returns></returns>
        public static object Instantiate(this ConstructorInfo ctor, object[] ctorArgs)
        {
            Func<object[], object> factory;

            factory = factories.GetOrAdd(ctor, BuildFactory);

            return factory.Invoke(ctorArgs);
        }

        /// <summary>
        /// Determines whether [is].
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool Is<TType>(this Type type)
        {
            return typeof(TType).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether [is assembly file] [the specified file path].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// <c>true</c> if [is assembly file] [the specified file path]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">filePath</exception>
        public static bool IsAssemblyFile(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            string extension;
            try
            {
                extension = Path.GetExtension(filePath);
            }
            catch (ArgumentException)
            {
                // path contains invalid characters...
                return false;
            }

            return IsDll(extension) || IsExe(extension);
        }

        /// <summary>
        /// Adds the application assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="applicationName">Name of the application.</param>
        private static void AddApplicationAssemblies(Assembly assembly, HashSet<Assembly> assemblies, string applicationName)
        {
            if (assemblies.Add(assembly) == false)
            {
                return;
            }

            foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
            {
                if (IsApplicationAssembly(applicationName, referencedAssembly.FullName))
                {
                    AddApplicationAssemblies(LoadAssembly(referencedAssembly), assemblies, applicationName);
                }
            }
        }

        /// <summary>
        /// Builds the factory.
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        /// <returns></returns>
        private static Func<object[], object> BuildFactory(ConstructorInfo ctor)
        {
            ParameterInfo[] parameterInfos = ctor.GetParameters();
            var parameterExpressions = new Expression[parameterInfos.Length];
            ParameterExpression argument = Expression.Parameter(typeof(object[]), "parameters");
            for (var i = 0; i < parameterExpressions.Length; i++)
            {
                parameterExpressions[i] = Expression.Convert(
                    Expression.ArrayIndex(argument, Expression.Constant(i, typeof(int))),
                    parameterInfos[i].ParameterType.IsByRef ? parameterInfos[i].ParameterType.GetElementType() : parameterInfos[i].ParameterType);
            }

            return Expression.Lambda<Func<object[], object>>(
                Expression.New(ctor, parameterExpressions),
                argument).Compile();
        }

        /// <summary>
        /// Ensures the is assignable.
        /// </summary>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <param name="subtypeofTBase">The subtypeof t base.</param>
        /// <exception cref="System.InvalidCastException"></exception>
        private static void EnsureIsAssignable<TBase>(Type subtypeofTBase)
        {
            if (subtypeofTBase.Is<TBase>())
            {
                return;
            }

            string message = typeof(TBase).IsInterface
                ? $"Type {subtypeofTBase.FullName} does not implement the interface {typeof(TBase).FullName}."
                : $"Type {subtypeofTBase.FullName} does not inherit from {typeof(TBase).FullName}.";

            throw new InvalidCastException(message);
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static AssemblyName GetAssemblyName(string filePath)
        {
            AssemblyName assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(filePath);
            }
            catch (ArgumentException)
            {
                assemblyName = new AssemblyName { CodeBase = filePath };
            }
            return assemblyName;
        }

        /// <summary>
        /// Instantiates the specified subtypeof t base.
        /// </summary>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <param name="subtypeofTBase">The subtypeof t base.</param>
        /// <param name="ctorArgs">The ctor arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Exception"></exception>
        private static TBase Instantiate<TBase>(Type subtypeofTBase, object[] ctorArgs)
        {
            ctorArgs = ctorArgs ?? new object[0];
            Type[] types = ctorArgs.ConvertAll(a => a == null ? typeof(object) : a.GetType());
            ConstructorInfo constructor = subtypeofTBase.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, types, null);
            if (constructor != null)
            {
                return (TBase)Instantiate(constructor, ctorArgs);
            }

            try
            {
                return (TBase)Activator.CreateInstance(subtypeofTBase, ctorArgs);
            }
            catch (MissingMethodException ex)
            {
                string message;
                if (ctorArgs.Length == 0)
                {
                    message = $"Type {subtypeofTBase.FullName} does not have a public default constructor and could not be instantiated.";
                }
                else
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine(
                        $"Type {subtypeofTBase.FullName} does not have a public constructor matching arguments of the following types:");
                    foreach (Type type in ctorArgs.Select(o => o.GetType()))
                    {
                        messageBuilder.AppendLine(type.FullName);
                    }

                    message = messageBuilder.ToString();
                }

                throw new ArgumentException(message, ex);
            }
            catch (Exception ex)
            {
                string message = $"Could not instantiate {subtypeofTBase.FullName}.";
                throw new Exception(message, ex);
            }
        }

        /// <summary>
        /// Determines whether [is application assembly] [the specified application name].
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// <c>true</c> if [is application assembly] [the specified application name]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsApplicationAssembly(string applicationName, string assemblyName)
        {
            return assemblyName.StartsWith(applicationName);
        }

        /// <summary>
        /// Determines whether the specified extension is DLL.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns><c>true</c> if the specified extension is DLL; otherwise, <c>false</c>.</returns>
        private static bool IsDll(string extension)
        {
            return ".dll".Equals(extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified extension is executable.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns><c>true</c> if the specified extension is executable; otherwise, <c>false</c>.</returns>
        private static bool IsExe(string extension)
        {
            return ".exe".Equals(extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        private static Assembly LoadAssembly(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    var exFileNotFound = exSub as FileNotFoundException;
                    if (!string.IsNullOrEmpty(exFileNotFound?.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }
                    sb.AppendLine();
                }
                var errorMessage = sb.ToString();
                throw new Exception(errorMessage, ex);
            }
        }
    }
}