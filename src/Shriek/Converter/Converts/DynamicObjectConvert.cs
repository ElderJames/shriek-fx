using System;
using System.Dynamic;
using System.Linq;

namespace Shriek.Converter.Converts
{
    /// <summary>
    /// 表示动态类型转换单元
    /// </summary>
    public class DynamicObjectConvert : IConvert
    {
        /// <summary>
        /// 将value转换为目标类型
        /// 并将转换所得的值放到result
        /// 如果不支持转换，则返回false
        /// </summary>
        /// <param name="converter">转换器实例</param>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <param name="result">转换结果</param>
        /// <returns>如果不支持转换，则返回false</returns>
        public virtual bool Convert(Converter converter, object value, Type targetType, out object result)
        {
            if (!(value is DynamicObject dynamicObject))
            {
                result = null;
                return false;
            }

            var instance = Activator.CreateInstance(targetType);
            var setters = PropertySetter.GetPropertySetters(targetType);

            foreach (var set in setters)
            {
                if (!TryGetValue(dynamicObject, set.Name, out var targetValue))
                    continue;

                targetValue = converter.Convert(targetValue, set.Type);
                set.SetValue(instance, targetValue);
            }

            result = instance;
            return true;
        }

        /// <summary>
        /// 获取动态类型的值
        /// </summary>
        /// <param name="dynamicObject">实例</param>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static bool TryGetValue(DynamicObject dynamicObject, string key, out object value)
        {
            var keys = dynamicObject.GetDynamicMemberNames();
            key = keys.FirstOrDefault(item => string.Equals(item, key, StringComparison.OrdinalIgnoreCase));

            if (key != null)
            {
                return dynamicObject.TryGetMember(new KeyBinder(key, false), out value);
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 表示键的信息获取绑定
        /// </summary>
        private class KeyBinder : GetMemberBinder
        {
            /// <summary>
            /// 键的信息获取绑定
            /// </summary>
            /// <param name="key">键名</param>
            /// <param name="ignoreCase">是否忽略大小写</param>
            public KeyBinder(string key, bool ignoreCase)
                : base(key, ignoreCase)
            {
            }

            /// <summary>
            /// 在派生类中重写时，如果无法绑定目标动态对象，则执行动态获取成员操作的绑定
            /// </summary>
            /// <param name="target"></param>
            /// <param name="errorSuggestion"></param>
            /// <returns></returns>
            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }
    }
}