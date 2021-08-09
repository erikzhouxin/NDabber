using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Extter
{
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
            var jsonArray = new JArray();
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
        /// <typeparam name="T"></typeparam>
        public class JSpanString : JSpan<char>
        {
            /// <summary>
            /// 字符串
            /// </summary>
            /// <param name="array"></param>
            public JSpanString(string json) : this(json.ToArray())
            {
            }
            /// <summary>
            /// 数组
            /// </summary>
            /// <param name="array"></param>
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
            public JNumber ReadNumber()
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
}
