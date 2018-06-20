using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Shriek.Reflection
{
    public static class New<T>
    {
        public static readonly Func<T> Instance = Creator();

        private static Func<T> Creator()
        {
            var t = typeof(T);
            if (t == typeof(string))
                return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

            if (t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null)
                return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

            return () => (T)FormatterServices.GetUninitializedObject(t);
        }
    }
}