using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
