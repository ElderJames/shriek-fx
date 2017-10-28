using Shriek.ServiceProxy.Tcp.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Util.Converts
{
    /// <summary>
    /// 表示动态类型转换单元
    /// </summary>
    public class DynamicObjectConvert : IConvert
    {
        /// <summary>
        /// 转换器实例
        /// </summary>
        public Converter Converter { get; set; }

        /// <summary>
        /// 下一个转换单元
        /// </summary>
        public IConvert NextConvert { get; set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType)
        {
            var dynamicObject = value as DynamicObject;
            if (dynamicObject == null)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            var instance = Activator.CreateInstance(targetType);
            var setters = Property.GetProperties(targetType);

            foreach (var setter in setters)
            {
                if (setter.Info.CanWrite == false)
                {
                    continue;
                }

                object targetValue;
                if (dynamicObject.TryGetMember(new MemberBinder(setter.Name, ignoreCase: true), out targetValue) == false)
                {
                    continue;
                }

                var valueCast = this.Converter.Convert(targetValue, setter.Info.PropertyType);
                setter.SetValue(instance, valueCast);
            }
            return instance;
        }

        /// <summary>
        /// 表示成员值的获取绑定
        /// </summary>
        private class MemberBinder : GetMemberBinder
        {
            /// <summary>
            /// 键的信息获取绑定
            /// </summary>
            /// <param name="key">键名</param>
            /// <param name="ignoreCase">是否忽略大小写</param>
            public MemberBinder(string key, bool ignoreCase)
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
