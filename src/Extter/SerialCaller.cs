using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
}
