using System;

namespace Shriek.Utils
{
    public static class Converter
    {
        public static Action<object> Convert<T>(Action<T> myActionT)
        {
            if (myActionT == null)
            {
                return null;
            }
            return o => myActionT((T)o);
        }

        public static dynamic ChangeTo(dynamic source, Type dest)
        {
            return System.Convert.ChangeType(source, dest);
        }

        /// <summary>
        /// 将当前对象转换成其他类型的对象
        /// </summary>
        /// <param name="originObject">源对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的对象</returns>
        public static object As(this object originObject, Type targetType)
        {
            return Shriek.Converter.Converter.Cast(originObject, targetType);
        }

        /// <summary>
        /// 将当前对象转换成其他类型的对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="originObject">源对象</param>
        /// <returns>转换后的对象</returns>
        public static object As<T>(this object originObject)
        {
            var targetType = typeof(T);
            return Shriek.Converter.Converter.Cast(originObject, targetType);
        }
    }
}