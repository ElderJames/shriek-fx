using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Shriek.ServiceProxy.Tcp.Exceptions;
using Shriek.ServiceProxy.Tcp.Util;

#if NETSTANDARD2_0

#else
using System.Web.Script.Serialization;
#endif

namespace Shriek.ServiceProxy.Tcp.Core.Internal
{
    /// <summary>
    /// 默认提供的动态Json序列化工具
    /// </summary>
    internal class DefaultDynamicJsonSerializer : IDynamicJsonSerializer
    {
        /// <summary>
        /// 序列化为Json
        /// </summary>
        /// <param name="model">实体</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public string Serialize(object model)
        {
            return this.Serialize(model, null);
        }

        /// <summary>
        /// 序列化为Json
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="datetimeFomat">时期时间格式</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public string Serialize(object model, string datetimeFomat)
        {
            try
            {
                return JSON.Parse(model, datetimeFomat);
            }
            catch (Exception ex)
            {
                throw new SerializerException(ex);
            }
        }

        /// <summary>
        /// 反序列化json为动态类型
        /// 异常时抛出SerializerException
        /// </summary>
        /// <param name="json">json数据</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public dynamic Deserialize(string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch (Exception ex)
            {
                throw new SerializerException(ex);
            }
        }

        /// <summary>
        /// 反序列化为实体
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="type">实体类型</param>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public object Deserialize(string json, Type type)
        {
            if (string.IsNullOrEmpty(json) || type == null)
            {
                return null;
            }

            try
            {
#if NETSTANDARD2_0

                return JsonConvert.DeserializeObject(json, type);
#else

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                return serializer.Deserialize(json, type);
#endif
            }
            catch (Exception ex)
            {
                throw new SerializerException(ex);
            }
        }

        /// <summary>
        /// 将值转换为目标类型
        /// 这些值有可能是反序列化得到的动态类型的值
        /// </summary>
        /// <param name="value">要转换的值，可能</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns>转换结果</returns>
        public object Convert(object value, Type targetType)
        {
            // JObject解析JSON得到动态类型是DynamicObject
            // 默认的Converter实例能转换
            // 如果要加入其它转换单元，请使用new Converter(params IConvert[] customConverts)
            return Util.Converter.Cast(value, targetType);
        }

        #region Json

        /// <summary>
        /// 提供Json序列化
        /// </summary>
        private static class JSON
        {
            /// <summary>
            /// 序列化得到Json
            /// </summary>
            /// <param name="model">模型</param>
            /// <param name="datetimeFomat">时期时间格式</param>
            /// <returns></returns>
            public static string Parse(object model, string datetimeFomat)
            {
                if (model == null)
                {
                    return null;
                }
#if NETSTANDARD2_0
                return JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DateFormatString = datetimeFomat
                });
#else
                var serializer = new JavaScriptSerializer();
                var dateTimeConverter = new DateTimeConverter(datetimeFomat);
                serializer.MaxJsonLength = int.MaxValue;
                serializer.RegisterConverters(new JavaScriptConverter[] { dateTimeConverter });

                var json = serializer.Serialize(model);
                return dateTimeConverter.Decode(json);
#endif
            }

#if NETSTANDARD2_0
#else
            /// <summary>
            /// 时间转换器
            /// </summary>
            private class DateTimeConverter : JavaScriptConverter
            {
                /// <summary>
                /// 获取转换器是否已使用过
                /// </summary>
                private bool isUsed = false;

                /// <summary>
                /// 时期时间格式化
                /// </summary>
                private readonly string datetimeFomat;

                /// <summary>
                /// 时间转换
                /// </summary>
                /// <param name="datetimeFomat">时期时间格式</param>
                public DateTimeConverter(string datetimeFomat)
                {
                    this.datetimeFomat = datetimeFomat;
                }

                /// <summary>
                /// 时间解码
                /// </summary>
                /// <param name="json">json内容</param>
                /// <returns></returns>
                public string Decode(string json)
                {
                    return this.isUsed ? UriEscapeValue.Decode(json) : json;
                }

                /// <summary>
                /// 反序化
                /// </summary>
                /// <param name="dictionary"></param>
                /// <param name="type"></param>
                /// <param name="serializer"></param>
                /// <returns></returns>
                public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 序列化对象
                /// </summary>
                /// <param name="obj">对象</param>
                /// <param name="serializer">序列化实例</param>
                /// <returns></returns>
                public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
                {
                    var dateTime = (DateTime?)obj;
                    if (dateTime.HasValue == false)
                    {
                        return null;
                    }

                    this.isUsed = true;
                    var dateTimeString = dateTime.Value.ToString(this.datetimeFomat, DateTimeFormatInfo.InvariantInfo);
                    return new UriEscapeValue(dateTimeString);
                }

                /// <summary>
                /// 支持的类型
                /// </summary>
                public override IEnumerable<Type> SupportedTypes
                {
                    get
                    {
                        yield return typeof(DateTime);
                        yield return typeof(Nullable<DateTime>);
                    }
                }

                /// <summary>
                /// 表示将值进行Uri转义
                /// </summary>
                private class UriEscapeValue : Uri, IDictionary<string, object>
                {
                    /// <summary>
                    /// 标记
                    /// </summary>
                    private static readonly string Mask = "UriEscaped_";

                    /// <summary>
                    /// 表达式
                    /// </summary>
                    private static readonly string Pattern = Mask + ".+?(?=\")";

                    /// <summary>
                    /// 将值进行Uri转义
                    /// </summary>
                    /// <param name="value">值</param>
                    public UriEscapeValue(string value)
                        : base(UriEscapeValue.Mask + value, UriKind.Relative)
                    {
                    }

                    /// <summary>
                    /// URI解码
                    /// </summary>
                    /// <param name="content">内容</param>
                    /// <returns></returns>
                    public static string Decode(string content)
                    {
                        return Regex.Replace(content, Pattern, m =>
                        {
                            var vlaue = m.Value.Substring(UriEscapeValue.Mask.Length);
                            return HttpUtility.UrlDecode(vlaue, Encoding.UTF8);
                        });
                    }

            #region IDictionary<string, object>

                    void IDictionary<string, object>.Add(string key, object value)
                    {
                        throw new NotImplementedException();
                    }

                    bool IDictionary<string, object>.ContainsKey(string key)
                    {
                        throw new NotImplementedException();
                    }

                    ICollection<string> IDictionary<string, object>.Keys
                    {
                        get
                        {
                            throw new NotImplementedException();
                        }
                    }

                    bool IDictionary<string, object>.Remove(string key)
                    {
                        throw new NotImplementedException();
                    }

                    bool IDictionary<string, object>.TryGetValue(string key, out object value)
                    {
                        throw new NotImplementedException();
                    }

                    ICollection<object> IDictionary<string, object>.Values
                    {
                        get
                        {
                            throw new NotImplementedException();
                        }
                    }

                    object IDictionary<string, object>.this[string key]
                    {
                        get
                        {
                            throw new NotImplementedException();
                        }
                        set
                        {
                            throw new NotImplementedException();
                        }
                    }

                    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
                    {
                        throw new NotImplementedException();
                    }

                    void ICollection<KeyValuePair<string, object>>.Clear()
                    {
                        throw new NotImplementedException();
                    }

                    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
                    {
                        throw new NotImplementedException();
                    }

                    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
                    {
                        throw new NotImplementedException();
                    }

                    int ICollection<KeyValuePair<string, object>>.Count
                    {
                        get
                        {
                            throw new NotImplementedException();
                        }
                    }

                    bool ICollection<KeyValuePair<string, object>>.IsReadOnly
                    {
                        get
                        {
                            throw new NotImplementedException();
                        }
                    }

                    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
                    {
                        throw new NotImplementedException();
                    }

                    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
                    {
                        throw new NotImplementedException();
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        throw new NotImplementedException();
                    }

            #endregion IDictionary<string, object>
                }
            }

#endif
        }

        #endregion Json

        #region JObject

        /// <summary>
        /// 表示动态Json对象
        /// </summary>
        [DebuggerTypeProxy(typeof(DebugView))]
        private class JObject : DynamicObject
        {
            /// <summary>
            /// 解析Json
            /// </summary>
            /// <param name="json">json</param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="ArgumentException"></exception>
            /// <exception cref="InvalidOperationException"></exception>
            /// <returns></returns>
            public static dynamic Parse(string json)
            {
#if NETSTANDARD2_0
                return JsonConvert.DeserializeObject(json);
#else

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                serializer.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
                return serializer.Deserialize(json, typeof(object));
#endif
            }

            /// <summary>
            /// 数据字典
            /// </summary>
            private readonly IDictionary<string, object> data;

            /// <summary>
            /// 表示动态Json对象
            /// </summary>
            /// <param name="data">内容字典</param>
            private JObject(IDictionary<string, object> data)
            {
                this.data = data.ToDictionary(k => k.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 获取成员名称
            /// </summary>
            /// <returns></returns>
            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return this.data.Keys;
            }

            /// <summary>
            /// 转换为目标类型
            /// </summary>
            /// <param name="binder"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                try
                {
                    result = Util.Converter.Cast(this, binder.Type);
                    return true;
                }
                catch (Exception)
                {
                    result = null;
                    return false;
                }
            }

            /// <summary>
            /// 获取成员的值
            /// </summary>
            /// <param name="binder"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (this.data.TryGetValue(binder.Name, out result))
                {
                    result = this.CastToJObject(result);
                }
                return true;
            }

            /// <summary>
            /// 转换结果为JObject结构或JArray结构
            /// </summary>
            /// <param name="result"></param>
            /// <returns></returns>
            private object CastToJObject(object result)
            {
                if (result == null)
                {
                    return null;
                }

                var dicResult = result as IDictionary<string, object>;
                if (dicResult != null)
                {
                    return new JObject(dicResult);
                }

                var listResult = result as IList;
                if (listResult != null)
                {
                    for (var i = 0; i < listResult.Count; i++)
                    {
                        var castValue = this.CastToJObject(listResult[i]);
                        listResult[i] = castValue;
                    }
                }
                return result;
            }

#if NETSTANDARD2_0
#else

            #region DynamicJsonConverter

            /// <summary>
            /// Json转换器
            /// </summary>
            private class DynamicJsonConverter : JavaScriptConverter
            {
                /// <summary>
                /// 获取支持的类型
                /// </summary>
                public override IEnumerable<Type> SupportedTypes
                {
                    get
                    {
                        yield return typeof(object);
                    }
                }

                /// <summary>
                /// 不作序列化
                /// </summary>
                /// <param name="obj"></param>
                /// <param name="serializer"></param>
                /// <returns></returns>
                public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="dictionary"></param>
                /// <param name="type"></param>
                /// <param name="serializer"></param>
                /// <returns></returns>
                public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
                {
                    return new JObject(dictionary);
                }
            }

            #endregion DynamicJsonConverter

#endif

            #region DebugView

            /// <summary>
            /// 调试视图
            /// </summary>
            private class DebugView
            {
                /// <summary>
                /// 查看的对象
                /// </summary>
                private JObject view;

                /// <summary>
                /// 调试视图
                /// </summary>
                /// <param name="view">查看的对象</param>
                public DebugView(JObject view)
                {
                    this.view = view;
                }

                /// <summary>
                /// 查看的内容
                /// </summary>
                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public KeyValuePair<string, object>[] Values
                {
                    get
                    {
                        return view.data.ToArray();
                    }
                }
            }

            #endregion DebugView
        }

        #endregion JObject
    }
}