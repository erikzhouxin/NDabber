using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Logger
{
    /// <summary>
    /// 文本文件日志写方法
    /// </summary>
    public class TextLogWriter : TextWriter
    {
        /// <summary>
        /// 老旧版本
        /// </summary>
        public TextWriter OldWriter { get; }
        /// <summary>
        /// 默认构造
        /// </summary>
        public TextLogWriter() : this(Console.Out) { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="oldWriter"></param>
        public TextLogWriter(TextWriter oldWriter)
        {
            OldWriter = oldWriter is TextLogWriter textLogWriter ? textLogWriter.OldWriter : oldWriter;
        }
        /// <summary>
        /// 编码
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object[]) 方法相同的语义将格式化字符串和新行写入文本字符串或流。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public override void Write(string format, params object[] arg) => TryWrite(() => string.Format(format, arg));
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object) 方法相同的语义将格式化字符串和新行写入文本字符串或流。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public override void Write(string format, object arg0, object arg1, object arg2) => TryWrite(() => string.Format(format, arg0, arg1, arg2));
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object) 方法相同的语义将格式化字符串和新行写入文本字符串或流。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public override void Write(string format, object arg0) => TryWrite(() => string.Format(format, arg0));
        /// <summary>
        /// 通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(object value) => TryWrite(() => JsonConvert.SerializeObject(value));
        /// <summary>
        /// 以异步形式将字符串写入到文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value) => TryWrite(() => value);
        /// <summary>
        /// 将十进制值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(decimal value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将 8 字节浮点值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(double value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object,System.Object) 方法相同的语义将格式化字符串和新行写入文本字符串或流。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public override void Write(string format, object arg0, object arg1) => TryWrite(() => string.Format(format, arg0, arg1));
        /// <summary>
        /// 将 8 字节无符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(ulong value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将 8 字节有符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(long value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将 4 字节无符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(uint value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将 4 字节有符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(int value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将字符写入该文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(char value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将 Boolean 值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(bool value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将字符的子数组写入文本字符串或流。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void Write(char[] buffer, int index, int count) => TryWrite(() => new String(buffer) + $"[{index}->{(index + count)}]");
        /// <summary>
        /// 将字符数组写入该文本字符串或流。
        /// </summary>
        /// <param name="buffer"></param>
        public override void Write(char[] buffer) => TryWrite(() => new String(buffer));
        /// <summary>
        /// 将 4 字节浮点值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void Write(float value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的字符串写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(string value) => TryWrite(() => value);
        /// <summary>
        /// 通过在对象上调用 ToString 方法将后跟行结束符的此对象的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(object value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object) 相同的语义写出格式化的字符串和一个新行。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public override void WriteLine(string format, params object[] arg) => TryWrite(() => string.Format(format, arg));
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object,System.Object) 方法相同的语义将格式化字符串和新行写入文本字符串或流。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public override void WriteLine(string format, object arg0, object arg1) => TryWrite(() => string.Format(format, arg0, arg1));
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object) 相同的语义写出格式化的字符串和一个新行。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public override void WriteLine(string format, object arg0, object arg1, object arg2) => TryWrite(() => string.Format(format, arg0, arg1, arg2));
        /// <summary>
        /// 将后面带有行结束符的十进制值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(decimal value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 使用与 System.String.Format(System.String,System.Object) 方法相同的语义将格式化字符串和新行写入文本字符串或流。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public override void WriteLine(string format, object arg0) => TryWrite(() => string.Format(format, arg0));
        /// <summary>
        /// 将后跟行结束符的 8 字节浮点值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(double value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的 4 字节无符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(uint value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的 8 字节无符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(ulong value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的 8 字节有符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(long value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的 4 字节有符号整数的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(int value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后面带有行结束符的 Boolean 值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(bool value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的字符子数组写入文本字符串或流。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteLine(char[] buffer, int index, int count) => TryWrite(() => new string(buffer) + $"[{index}->{index + count}]");
        /// <summary>
        /// 将后跟行结束符的字符数组写入文本字符串或流。
        /// </summary>
        /// <param name="buffer"></param>
        public override void WriteLine(char[] buffer) => TryWrite(() => new string(buffer));
        /// <summary>
        /// 将后跟行结束符的字符写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(char value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将后跟行结束符的 4 字节浮点值的文本表示形式写入文本字符串或流。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(float value) => TryWrite(() => value.ToString());
        /// <summary>
        /// 将行结束符的字符串写入文本字符串或流。
        /// </summary>
        public override void WriteLine() => TryWrite(() => "==================================");
        private static readonly object _logLocker = new object();
        /// <summary>
        /// 尝试写入
        /// </summary>
        /// <param name="GetContent"></param>
        /// <returns></returns>
        public virtual void TryWrite(Func<String> GetContent)
        {
            Task.Factory.StartNew(() =>
            {
                var content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffff}    {GetContent().Replace("\n", "\n                     ")}";
                OldWriter?.WriteLine(content);
                var path = GetLogPath();
                if(Monitor.TryEnter(_logLocker, TimeSpan.FromSeconds(10)))
                {
                    using (var file = new FileStream($"{path}{DateTime.Now:yyyy-MM}.log", FileMode.Append, FileAccess.Write))
                    {
                        var contByte = Encoding.UTF8.GetBytes(content + "\r\n");
                        file.Write(contByte, 0, contByte.Length);
                        file.Flush();
                    }
                    Monitor.Exit(_logLocker);
                }
            });
        }
        /// <summary>
        /// 获取日志目录
        /// </summary>
        /// <returns></returns>
        public static string GetLogPath()
        {
            var path = Path.GetFullPath("Logs/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
