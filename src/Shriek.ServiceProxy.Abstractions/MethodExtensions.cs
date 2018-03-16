using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Shriek.ServiceProxy.Abstractions
{
    public static class MethodExtensions
    {
        public static string GetPath(this MethodInfo method, Type declaringType = null)
        {
            return Regex.Replace($"method/{(declaringType ?? method.DeclaringType).FullName}/{method.Name}/{string.Join("-", method.GetParameters().Select(x => x.ParameterType.FullName))}".ToLower(), "[^a-z|0-9]", "-");
        }
    }
}