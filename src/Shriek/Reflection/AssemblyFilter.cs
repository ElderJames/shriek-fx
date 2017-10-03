using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shriek.Reflection
{
    public class AssemblyFilter
    {
        private readonly string _directoryName;
        private readonly string _mask;
        private Predicate<Assembly> _assemblyFilter;
        private Predicate<AssemblyName> _nameFilter;

        public AssemblyFilter(string directoryName, string mask = null)
        {
            if (directoryName == null)
                throw new ArgumentNullException(nameof(directoryName));

            _directoryName = GetFullPath(directoryName);
            _mask = mask;
        }

        /// <summary>
        ///     Gets the assemblies.
        /// </summary>
        /// <returns></returns>

        public IEnumerable<Assembly> GetAssemblies()
        {
            foreach (string file in GetFiles())
            {
                if (!ReflectionUtil.IsAssemblyFile(file))
                {
                    continue;
                }

                Assembly assembly = LoadAssemblyIgnoringErrors(file);
                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }

        /// <summary>
        ///     Filters the by assembly.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>

        public AssemblyFilter FilterByAssembly(Predicate<Assembly> filter)
        {
            _assemblyFilter += filter ?? throw new ArgumentNullException(nameof(filter));
            return this;
        }

        /// <summary>
        ///     Filters the name of the by.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>

        public AssemblyFilter FilterByName(Predicate<AssemblyName> filter)
        {
            _nameFilter += filter ?? throw new ArgumentNullException(nameof(filter));
            return this;
        }

        /// <summary>
        ///     Withes the key token.
        /// </summary>
        /// <param name="publicKeyToken">The public key token.</param>
        /// <returns></returns>

        public AssemblyFilter WithKeyToken(string publicKeyToken)
        {
            return WithKeyToken(ExtractKeyToken(publicKeyToken));
        }

        /// <summary>
        ///     Withes the key token.
        /// </summary>
        /// <param name="publicKeyToken">The public key token.</param>
        /// <returns></returns>

        public AssemblyFilter WithKeyToken(byte[] publicKeyToken)
        {
            if (publicKeyToken == null)
                throw new ArgumentNullException(nameof(publicKeyToken));

            return FilterByName(n => IsTokenEqual(n.GetPublicKeyToken(), publicKeyToken));
        }

        /// <summary>
        ///     Withes the key token.
        /// </summary>
        /// <param name="typeFromAssemblySignedWithKey">The type from assembly signed with key.</param>
        /// <returns></returns>

        public AssemblyFilter WithKeyToken(Type typeFromAssemblySignedWithKey)
        {
            return WithKeyToken(typeFromAssemblySignedWithKey.Assembly);
        }

        /// <summary>
        ///     Withes the key token.
        /// </summary>
        /// <typeparam name="TTypeFromAssemblySignedWithKey">The type of the type from assembly signed with key.</typeparam>
        /// <returns></returns>

        public AssemblyFilter WithKeyToken<TTypeFromAssemblySignedWithKey>()
        {
            return WithKeyToken(typeof(TTypeFromAssemblySignedWithKey).Assembly);
        }

        /// <summary>
        ///     Withes the key token.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>

        public AssemblyFilter WithKeyToken(Assembly assembly)
        {
            return WithKeyToken(assembly.GetName().GetPublicKeyToken());
        }

        private string GetFullPath(string path)
        {
            if (Path.IsPathRooted(path) == false)
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            return Path.GetFullPath(path);
        }

        private bool IsTokenEqual(byte[] actualToken, byte[] expectedToken)
        {
            if (actualToken?.Length != expectedToken.Length)
            {
                return false;
            }

            for (var i = 0; i < actualToken.Length; i++)
            {
                if (actualToken[i] != expectedToken[i])
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] ExtractKeyToken(string keyToken)
        {
            if (keyToken == null) throw new ArgumentNullException(nameof(keyToken));

            if (keyToken.Length != 16)
            {
                throw new ArgumentException(
                    $"The string '{keyToken}' does not appear to be a valid public key token. It should have 16 characters, has {keyToken.Length}.");
            }

            try
            {
                var tokenBytes = new byte[8];
                for (var i = 0; i < 8; i++)
                {
                    tokenBytes[i] = byte.Parse(keyToken.Substring(2 * i, 2), NumberStyles.HexNumber);
                }

                return tokenBytes;
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    $"The string '{keyToken}' does not appear to be a valid public key token. It could not be processed.",
                    e);
            }
        }

        private IEnumerable<string> GetFiles()
        {
            try
            {
                if (Directory.Exists(_directoryName) == false)
                {
                    return Enumerable.Empty<string>();
                }
                if (string.IsNullOrEmpty(_mask))
                {
                    return Directory.EnumerateFiles(_directoryName);
                }

                return Directory.EnumerateFiles(_directoryName, _mask);
            }
            catch (IOException e)
            {
                throw new ArgumentException("Could not resolve assemblies.", e);
            }
        }

        private Assembly LoadAssemblyIgnoringErrors(string file)
        {
            // based on MEF DirectoryCatalog
            try
            {
                return ReflectionUtil.GetAssemblyNamed(file, _nameFilter, _assemblyFilter);
            }
            catch (FileNotFoundException)
            {
            }
            catch (FileLoadException)
            {
                // File was found but could not be loaded
            }
            catch (BadImageFormatException)
            {
                // Dlls that contain native code or assemblies for wrong runtime (like .NET 4 asembly when we're in CLR2 process)
            }
            catch (ReflectionTypeLoadException)
            {
                // Dlls that have missing Managed dependencies are not loaded, but do not invalidate the Directory
            }

            return null;
        }
    }
}