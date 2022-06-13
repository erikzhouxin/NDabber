using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 序列化调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 获取格式化的Json代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetJsonFormatString<T>(this T value)
        {
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            //格式化json字符串
            new JsonSerializer().Serialize(jsonWriter, value);
            return textWriter.ToString();
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic GetJsonObject(this string json)
        {
            return JsonConvert.DeserializeObject(json, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetJsonObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static object GetJsonObject(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 默认设置
        /// </summary>
        public static JsonSerializerSettings DefaultNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        /// <summary>
        /// 默认设置
        /// </summary>
        public static JsonSerializerSettings CurrentNewtonsoftSetting { get; set; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 帕斯卡(大驼峰)命名
        /// </summary>
        public static JsonSerializerSettings PascalCaseNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
        };
        /// <summary>
        /// 小驼峰命名
        /// </summary>
        public static JsonSerializerSettings CamelCaseNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
        };
        /// <summary>
        /// 小写属性命名
        /// </summary>
        public static JsonSerializerSettings LowerNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new LowercaseContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
        };
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetWebJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, CurrentNewtonsoftSetting);
        }
        /// <summary>
        /// 获取对象的Json字符串(小写属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLowerJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, LowerNewtonsoftSetting);
        }
    }
    /// <summary>
    /// 小写转换
    /// </summary>
    public class LowercaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 转换属性名
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
    /// <summary>
    /// 大写转换
    /// </summary>
    public class UppercaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 转换属性名
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToUpper();
        }
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 序列化调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <see cref="BinaryFormatter.Deserialize(Stream)"/>
        /// <see cref="GetBytesModel"/>
        /// <returns></returns>
        [Obsolete("替代方案:GetBytesModel,原因:高版本.NET已弃用,请使用另外的方法")]
        public static T GetBinModel<T>(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return (T)SerialByteBinder<T>.Formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <see cref="BinaryFormatter.Serialize(Stream, object)"/>
        /// <see cref="GetModelBytes"/>
        /// <returns></returns>
        [Obsolete("替代方案:GetModelBytes,原因:高版本.NET已弃用,请使用另外的方法")]
        public static byte[] GetBinBytes<T>(this T model)
        {
            using (var stream = new MemoryStream())
            {
                SerialByteBinder<T>.Formatter.Serialize(stream, model);
                return stream.ToArray();
            }
        }
        /// <summary>
        /// 将对象转换为byte数组
        /// </summary>
        /// <param name="model">被转换对象</param>
        /// <returns>转换后byte数组</returns>
        public static byte[] GetModelBytes<T>(this T model)
        {
            byte[] buff = new byte[Marshal.SizeOf(model)];
            Marshal.StructureToPtr(model, Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0), true);
            return buff;
        }

        /// <summary>
        /// 将byte数组转换成对象
        /// </summary>
        /// <param name="bytes">被转换byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static T GetBytesModel<T>(this byte[] bytes)
        {
            return (T)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), typeof(T));
        }
        /// <summary>
        /// 将byte数组转换成对象
        /// </summary>
        /// <param name="bytes">被转换byte数组</param>
        /// <param name="type">转换成的类名</param>
        /// <returns>转换完成后的对象</returns>
        public static object GetBytesModel(this byte[] bytes, Type type)
        {
            return Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), type);
        }
    }
    /// <summary>
    /// 序列化绑定
    /// </summary>
    internal class SerialByteBinder<T> : SerializationBinder
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static SerializationBinder Instance { get; }
        /// <summary>
        /// 格式化
        /// </summary>
        public static BinaryFormatter Formatter { get; }
        /// <summary>
        /// 静态构造
        /// </summary>
        static SerialByteBinder()
        {
            Formatter = new BinaryFormatter();
            Formatter.Binder = Instance = new SerialByteBinder<T>();
        }
        /// <summary>
        /// 绑定到类型
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            return typeof(T);
        }
    }

    /// <summary>
    /// Json分析
    /// </summary>
    public class JsonAnalysis
    {
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic ConvertObject(string json)
        {
            var span = new JSpanString(json);
            span.ReadSkipBlank();
            return span.Read() == '[' ? ConvertArray(span) : ConvertObject(span);
        }
        /// <summary>
        /// 获取Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string Json<T>(T model)
        {
            if (model == null) { return null; }
            return string.Empty;
        }

        private static dynamic ConvertObject(JSpanString span)
        {
            IDictionary<string, object> expObj = new ExpandoObject();
            dynamic jsonArray = expObj;
            do
            {
                span.ReadSkipBlank();
                var splitChar = span.Read();
                if (splitChar == '}') { break; }
                if (splitChar != '"')
                {
                    throw new JException($"不能识别的字符“{splitChar}”！应为“\"”", span.RIndex - 1);
                }
                //读取字符串
                var name = span.ReadProperty();
                span.ReadSkipBlank();
                splitChar = span.Read();
                if (splitChar != ':')
                {
                    throw new JException($"不能识别的字符“{splitChar}”！", span.RIndex - 1);
                }
                span.ReadSkipBlank();
                expObj[name] = ReadElement(span);
                //读取到非空白字符
                span.ReadSkipBlank();
                splitChar = span.Read();
                if (splitChar == '}') { break; }
                if (splitChar != ',')
                {
                    throw new JException($"不能识别的字符“{splitChar}”！", span.RIndex - 1);
                }
            } while (true);
            return jsonArray;
        }

        private static dynamic ReadElement(JSpanString span)
        {
            char splitChar = span.Read();
            switch (splitChar)
            {
                case '"':
                    return span.ReadContent();
                case '{':
                    return ConvertObject(span);
                case '[':
                    return ConvertArray(span);
                case 't':
                    return span.ReadTrue();
                case 'f':
                    return span.ReadFalse();
                case 'n':
                    return span.ReadNull();
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return span.ReadNumber();
                default:
                    throw new JException($"未知Element“{splitChar}”应该为【[、{{、\"、true、false、null】", span.RIndex - 1);
            }
        }

        private static dynamic ConvertArray(JSpanString span)
        {
            var jsonArray = new List<dynamic>();
            do
            {
                span.ReadSkipBlank();
                if (span.Array[span.RIndex] == ']') { break; }
                //读取下一个Element
                jsonArray.Add(ReadElement(span));
                //读取到非空白字符
                span.ReadSkipBlank();
                char splitChar = span.Read();
                if (splitChar == ']') { break; }
                if (splitChar != ',') throw new JException($"不能识别的字符“{splitChar}”！", span.RIndex - 1);
            } while (true);

            return jsonArray;
        }
        #region // 内部类
        /// <summary>
        /// 元组
        /// </summary>
        public class JSpanString : JSpan<char>
        {
            /// <summary>
            /// 字符串
            /// </summary>
            /// <param name="json"></param>
            public JSpanString(string json) : this(json.ToArray())
            {
            }
            /// <summary>
            /// 数组
            /// </summary>
            /// <param name="json"></param>
            public JSpanString(char[] json) : base(json)
            {
            }
            /// <summary>
            /// 读取到非空字符
            /// </summary>
            public void ReadSkipBlank()
            {
                while (RIndex < Array.Length && char.IsWhiteSpace(Array[RIndex]))
                {
                    RIndex++;
                }
            }
            /// <summary>
            /// 读取内容
            /// </summary>
            /// <returns></returns>
            public char Read()
            {
                if (RIndex >= Array.Length)
                {
                    return char.MaxValue;
                }
                return Array[RIndex++];
            }
            /// <summary>
            /// 读取属性
            /// </summary>
            /// <returns></returns>
            public string ReadProperty()
            {
                var value = new StringBuilder();
                while (RIndex < Array.Length)
                {
                    var c = Array[RIndex++];
                    //判断是否是转义字符
                    if (c == '\\')
                    {
                        value.Append('\\');
                        if (RIndex >= Array.Length) { throw new JException("未知的结尾！", RIndex - 1); }
                        c = Array[RIndex++];
                        value.Append(c);
                        if (c == 'u')
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                c = Array[RIndex++];
                                if (IsHex(c))
                                {
                                    value.Append(c);
                                }
                                else
                                {
                                    throw new JException("不是有效的Unicode字符！", RIndex - 1);
                                }
                            }
                        }
                    }
                    else if (c == '"')
                    {
                        break;
                    }
                    else if (c == '\r' || c == '\n')
                    {
                        throw new JException("传入的JSON属性中不允许有换行！", RIndex - 1);
                    }
                    else
                    {
                        value.Append(c);
                    }
                }
                return value.ToString();
            }
            /// <summary>
            /// 判断是否为16进制字符
            /// </summary>
            private static bool IsHex(char c)
            {
                return c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';
            }
            /// <summary>
            /// 读取内容字符
            /// </summary>
            /// <returns></returns>
            public string ReadContent()
            {
                var value = new StringBuilder();
                while (RIndex < Array.Length)
                {
                    var c = Array[RIndex++];
                    //判断是否是转义字符
                    if (c == '\\')
                    {
                        value.Append('\\');
                        if (RIndex >= Array.Length) { throw new JException("未知的结尾！", RIndex - 1); }
                        c = Array[RIndex++];
                        value.Append(c);
                        if (c == 'u')
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                c = Array[RIndex++];
                                if (IsHex(c))
                                {
                                    value.Append(c);
                                }
                                else
                                {
                                    throw new JException("不是有效的Unicode字符！", RIndex - 1);
                                }
                            }
                        }
                    }
                    else if (c == '"')
                    {
                        break;
                    }
                    else
                    {
                        value.Append(c);
                    }
                }
                return value.ToString();
            }
            /// <summary>
            /// 读取bool的True值
            /// </summary>
            /// <returns></returns>
            public bool ReadTrue()
            {
                var r = Array[RIndex++];
                var u = Array[RIndex++];
                var e = Array[RIndex++];
                if (r == 'r' && u == 'u' && e == 'e')
                {
                    return true;
                }
                throw new JException("读取布尔值[True]出错！", RIndex - 4);
            }
            /// <summary>
            /// 读取bool的False值
            /// </summary>
            /// <returns></returns>
            public bool ReadFalse()
            {
                var a = Array[RIndex++];
                var l = Array[RIndex++];
                var s = Array[RIndex++];
                var e = Array[RIndex++];
                if (a == 'a' && l == 'l' && s == 's' && e == 'e')
                {
                    return false;
                }
                throw new JException("读取布尔值[False]出错！", RIndex - 5);
            }
            /// <summary>
            /// 读取空值
            /// </summary>
            public object ReadNull()
            {
                var u = Array[RIndex++];
                var l1 = Array[RIndex++];
                var l2 = Array[RIndex++];
                if (u == 'u' && l1 == 'l' && l2 == 'l')
                {
                    return null;
                }
                throw new JException("读取NULL值出错！", RIndex - 4);
            }
            /// <summary>
            /// 读取数字类型
            /// </summary>
            /// <returns></returns>
            public dynamic ReadNumber()
            {
                var i = RIndex;
                while (i < Array.Length && (char.IsNumber(Array[i]) || Array[i] == '.'))
                {
                    i++;
                }
                var number = string.Empty;
                for (int j = RIndex - 1; j < i; j++)
                {
                    number += Array[j];
                }
                if (double.TryParse(number, out var value))
                {
                    RIndex = i;
                    return new JNumber(value);
                }
                throw new JException("不能识别的数字类型！", i);
            }
        }
        /// <summary>
        /// 元组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class JSpan<T>
        {
            /// <summary>
            /// 内容数组
            /// </summary>
            public T[] Array { get; private set; }
            /// <summary>
            /// 读取位置
            /// </summary>
            public int RIndex { get; set; }
            /// <summary>
            /// 可序列化
            /// </summary>
            /// <param name="list"></param>
            public JSpan(IEnumerable<T> list) : this(list.ToArray()) { }
            /// <summary>
            /// 数组
            /// </summary>
            /// <param name="array"></param>
            public JSpan(T[] array)
            {
                Array = array;
            }
        }
        /// <summary>
        /// JSON解析异常
        /// </summary>
        public class JException : Exception
        {
            /// <summary>
            /// 实例化JSON解析异常
            /// </summary>
            /// <param name="index"></param>
            /// <param name="message"></param>
            public JException(string message, int index) : base(message)
            {
                Index = index;
            }
            /// <summary>
            /// 解析异常位置
            /// </summary>
            public int Index { get; }
        }
        /// <summary>
        /// Json元素
        /// </summary>
        public abstract class JElement
        {

        }
        /// <summary>
        /// Json集合
        /// </summary>
        public class JArray : JElement, IList<JElement>, IEquatable<JArray>
        {
            /// <summary>
            /// 
            /// </summary>
            public JArray()
            {
                _Elements = new List<JElement>();
            }

            private readonly List<JElement> _Elements;
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerator<JElement> GetEnumerator()
            {
                return _Elements.GetEnumerator();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            public void Add(JElement item) => _Elements.Add(item);
            /// <summary>
            /// 
            /// </summary>
            public void Clear() => _Elements.Clear();
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(JElement item) => _Elements.Contains(item);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(JElement[] array, int arrayIndex) => _Elements.CopyTo(array, arrayIndex);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(JElement item) => _Elements.Remove(item);
            /// <summary>
            /// 
            /// </summary>
            public int Count => _Elements.Count;
            /// <summary>
            /// 
            /// </summary>
            public bool IsReadOnly => false;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public int IndexOf(JElement item) => _Elements.IndexOf(item);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public void Insert(int index, JElement item) => _Elements.Insert(index, item);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            public void RemoveAt(int index) => _Elements.RemoveAt(index);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public JElement this[int index]
            {
                get => _Elements[index];
                set => _Elements[index] = value;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"[{string.Join(",", this)}]";
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(JArray other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return !other._Elements.Where((t, i) => !t.Equals(this._Elements[i])).Any();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((JArray)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return _Elements.Aggregate(0, (current, jsonElement) => current ^ jsonElement.GetHashCode());
            }
        }
        /// <summary>
        /// JSON布尔值
        /// </summary>
        public class JBoolean : JElement, IEquatable<JBoolean>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public JBoolean(bool value)
            {
                Value = value;
            }
            /// <summary>
            /// 
            /// </summary>
            public bool Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Value ? "true" : "false";
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(JBoolean other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Value == other.Value;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((JBoolean)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }
        /// <summary>
        /// JSON NULL 类型
        /// </summary>
        public class JNull : JElement, IEquatable<JNull>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "null";
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(JNull other)
            {
                return other != null;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((JNull)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return "null".GetHashCode();
            }
        }
        /// <summary>
        /// JSON数值类型
        /// </summary>
        public class JNumber : JElement, IEquatable<JNumber>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public JNumber(double value)
            {
                Value = value;
            }
            /// <summary>
            /// 
            /// </summary>
            public double Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Value.ToString();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(JNumber other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Value.Equals(other.Value);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((JNumber)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }
        /// <summary>
        /// JSON的对象
        /// </summary>
        public class JObject : JElement, IDictionary<string, JElement>, IEquatable<JObject>
        {
            /// <summary>
            /// 
            /// </summary>
            public JObject()
            {
                _propertyMap = new Dictionary<string, JElement>();
            }

            /// <summary>
            /// 属性字典
            /// </summary>
            private readonly Dictionary<string, JElement> _propertyMap;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool ContainsKey(string key) => _propertyMap.ContainsKey(key);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void Add(string key, JElement value) => _propertyMap.Add(key, value);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool Remove(string key) => _propertyMap.Remove(key);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool TryGetValue(string key, out JElement value) => _propertyMap.TryGetValue(key, out value);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public JElement this[string name]
            {
                get => _propertyMap[name];
                set => _propertyMap[name] = value;
            }
            /// <summary>
            /// 
            /// </summary>
            public ICollection<string> Keys => _propertyMap.Keys;
            /// <summary>
            /// 
            /// </summary>
            public ICollection<JElement> Values => _propertyMap.Values;
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerator<KeyValuePair<string, JElement>> GetEnumerator() => _propertyMap.GetEnumerator();
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            public void Add(KeyValuePair<string, JElement> item) => _propertyMap.Add(item.Key, item.Value);
            /// <summary>
            /// 
            /// </summary>
            public void Clear() => _propertyMap.Clear();
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(KeyValuePair<string, JElement> item) => _propertyMap.Contains(item);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            void ICollection<KeyValuePair<string, JElement>>.CopyTo(KeyValuePair<string, JElement>[] array,
                int arrayIndex)
            {
                throw new Exception();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(KeyValuePair<string, JElement> item)
            {
                return _propertyMap.Remove(item.Key);
            }
            /// <summary>
            /// 
            /// </summary>
            public int Count => _propertyMap.Count;
            /// <summary>
            /// 
            /// </summary>
            public bool IsReadOnly => false;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(JObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (other.Count != this.Count) return false;
                foreach (var property in this._propertyMap)
                {
                    if (!other.TryGetValue(property.Key, out var value) || !value.Equals(property.Value))
                        return false;
                }

                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((JObject)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return _propertyMap.Aggregate(0,
                    (current, jsonElement) => current ^ jsonElement.Key.GetHashCode() ^ jsonElement.Value.GetHashCode());
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{{{string.Join(",", _propertyMap.Select(x => $"\"{x.Key}\":{x.Value}"))}}}";
            }
        }
        /// <summary>
        /// 数组
        /// </summary>
        public class JString : JElement
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public JString(string value)
            {
                Value = value;
            }
            /// <summary>
            /// 
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"\"{Value}\"";
            }
        }
        #endregion
    }
    /// <summary>
    /// 序列化
    /// </summary>
    public static class JsonSerials
    {
        /// <summary>
        /// Serialize object to json string.
        /// </summary>
        /// <param name="obj">Instance of the type T.</param>
        /// <returns>json string.</returns>
        public static string Serialize(object obj)
        {
            if (obj == null)
            {
                return "{}";
            }

            // Get the type of obj.
            Type t = obj.GetType();
            PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            StringBuilder json = new StringBuilder("{");
            if (pis != null && pis.Length > 0)
            {
                int i = 0;
                int lastIndex = pis.Length - 1;
                foreach (PropertyInfo p in pis)
                {
                    var propObj = p.GetValue(obj, null);

                    switch (propObj)
                    {
                        case string propString:
                            json.AppendFormat("\"{0}\":\"{1}\"", p.Name, propString.Replace("\"", "\\\""));
                            break;
                        case null:
                            json.AppendFormat("\"{0}\":null", p.Name);
                            break;
                        default:
                            break;
                    }
                    ++i;
                }
            }
            json.Append("}");
            return json.ToString();
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public static String Serialize<T>(this T model)
        {
            if (model == null) { return "{}"; };
            var pis = Jsonm<T>.Properties;
            StringBuilder json = new StringBuilder("{");
            if (pis.Length > 0)
            {
                int i = 0;
                int lastIndex = pis.Length - 1;
                foreach (PropertyInfo p in pis)
                {
                    var propObj = p.GetValue(model, null);
                    switch (propObj)
                    {
                        case string stringVal:
                            json.AppendFormat("\"{0}\":\"{1}\"", p.Name, stringVal.Replace("\"", "\\\""));
                            continue;
                        case long:
                        case ulong:
                        case int:
                        case uint:
                        case bool:
                        case byte:
                        case sbyte:
                        case short:
                        case ushort:
                        case double:
                        case float:
                        case decimal:
                            json.AppendFormat("\"{0}\":{1}", p.Name, propObj);
                            continue;
                        case DateTime:
                            json.AppendFormat("\"{0}\":{1:yyyy-MM-dd HH:mm:ss}", p.Name, propObj);
                            continue;
                        case TimeSpan:
                            json.AppendFormat("\"{0}\":{1}", p.Name, propObj);
                            continue;
                        case null:
                            json.AppendFormat("\"{0}\":null", p.Name);
                            continue;
                        default:
                            break;
                    }
                    // Array.
                    if (IsArrayType(p.PropertyType))
                    {
                        // Array case.
                        object o = p.GetValue(model, null);

                        if (o == null)
                        {
                            json.AppendFormat("\"{0}\":{1}", p.Name, "null");
                        }
                        else
                        {
                            json.AppendFormat("\"{0}\":{1}", p.Name, GetArrayValue((Array)p.GetValue(model, null)));
                        }
                    }
                    // Class type. custom class, list collections and so forth.
                    else if (IsCustomClassType(p.PropertyType))
                    {
                        object v = p.GetValue(model, null);
                        if (v is IList)
                        {
                            IList il = v as IList;
                            string subJsString = GetIListValue(il);

                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                        else
                        {
                            // Normal class type.
                            string subJsString = Serialize(p.GetValue(model, null));

                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                    }
                    // Datetime
                    else if (p.PropertyType.Equals(typeof(DateTime)))
                    {
                        DateTime dt = (DateTime)p.GetValue(model, null);

                        if (dt == default(DateTime))
                        {
                            json.AppendFormat("\"{0}\":\"\"", p.Name);
                        }
                        else
                        {
                            json.AppendFormat("\"{0}\":\"{1}\"", p.Name, ((DateTime)p.GetValue(model, null)).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    else
                    {
                        // TODO: extend.
                    }

                    if (i >= 0 && i != lastIndex)
                    {
                        json.Append(",");
                    }
                    ++i;
                }
            }
            json.Append("}");
            return json.ToString();
        }

        /// <summary>
        /// Deserialize json string to object.
        /// </summary>
        /// <typeparam name="T">The type to be deserialized.</typeparam>
        /// <param name="jsonString">json string.</param>
        /// <returns>instance of type T.</returns>
        public static T Deserialize<T>(string jsonString)
        {
            throw new NotImplementedException("Not implemented :(");
        }

        /// <summary>
        /// Get array json format string value.
        /// </summary>
        /// <param name="obj">array object</param>
        /// <returns>js format array string.</returns>
        static string GetArrayValue(Array obj)
        {
            if (obj != null)
            {
                if (obj.Length == 0)
                {
                    return "[]";
                }

                object firstElement = obj.GetValue(0);
                Type et = firstElement.GetType();
                bool quotable = et == typeof(string);

                StringBuilder sb = new StringBuilder("[");
                int index = 0;
                int lastIndex = obj.Length - 1;

                if (quotable)
                {
                    foreach (var item in obj)
                    {
                        sb.AppendFormat("\"{0}\"", item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }
                else
                {
                    foreach (var item in obj)
                    {
                        sb.Append(item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }

                sb.Append("]");

                return sb.ToString();
            }

            return "null";
        }

        /// <summary>
        /// Get Ilist json format string value.
        /// </summary>
        /// <param name="obj">IList object</param>
        /// <returns>js format IList string.</returns>
        static string GetIListValue(IList obj)
        {
            if (obj != null)
            {
                if (obj.Count == 0)
                {
                    return "[]";
                }

                object firstElement = obj[0];
                Type et = firstElement.GetType();
                bool quotable = et == typeof(string);

                StringBuilder sb = new StringBuilder("[");
                int index = 0;
                int lastIndex = obj.Count - 1;

                if (quotable)
                {
                    foreach (var item in obj)
                    {
                        sb.AppendFormat("\"{0}\"", item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }
                else
                {
                    foreach (var item in obj)
                    {
                        sb.Append(item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }

                sb.Append("]");

                return sb.ToString();
            }

            return "null";
        }

        /// <summary>
        /// Check whether t is array type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static bool IsArrayType(Type t)
        {
            if (t != null)
            {
                return t.IsArray;
            }

            return false;
        }

        /// <summary>
        /// Check whether t is custom class type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static bool IsCustomClassType(Type t)
        {
            if (t != null)
            {
                return t.IsClass && t != typeof(string);
            }

            return false;
        }
    }
    /// <summary>
    /// 序列化的Json泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Jsonm<T>
    {
        /// <summary>
        /// 属性内容
        /// </summary>
        public static PropertyInfo[] Properties { get; }
        /// <summary>
        /// 类型
        /// </summary>
        public static Type Type { get; } = typeof(T);
        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        public static Func<T, string, object> GetValue;
        /// <summary>
        /// 设置值(instance,memberName,newValue)
        /// </summary>
        public static Action<T, string, object> SetValue;
        static Jsonm()
        {
            GetValue = GenerateGetValue();
            SetValue = GenerateSetValue();
            Properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
        #region // 辅助方法
        private static Func<T, string, object> GenerateGetValue()
        {
            var type = typeof(T);
            var instance = Expression.Parameter(type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var property = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                var property = Expression.Property(null, type, propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));
                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Func<T, string, object>>(methodBody, instance, memberName).Compile();
        }
        private static Action<T, string, object> GenerateSetValue()
        {
            var type = typeof(T);
            var instance = Expression.Parameter(type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(null, propertyInfo);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Action<T, string, object>>(methodBody, instance, memberName, newValue).Compile();
        }
        #endregion
    }
}
