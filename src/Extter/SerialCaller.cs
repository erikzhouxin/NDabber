using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
}
namespace System.Data.Extter
{
    /// <summary>
    /// 序列化调用
    /// </summary>
    public static class SerialCaller
    {
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <returns></returns>
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
        /// <returns></returns>
        public static byte[] GetBinBytes<T>(this T model)
        {
            using (var stream = new MemoryStream())
            {
                SerialByteBinder<T>.Formatter.Serialize(stream, model);
                return stream.ToArray();
            }
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
}
