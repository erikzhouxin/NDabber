using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Cobber;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 管道发送
    /// </summary>
    [Obsolete("管道连接已经移植到[NSystem.Data.Piper]项目中")]
    public class PipeCommunication
    {
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static void SendNamed<T>(string pipeName, Model<T> model) => SendClientNamed(".", pipeName, model);
        /// <summary>
        /// 命名管道指定机子
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <param name="model"></param>
        public static void SendClientNamed<T>(string serverName, string pipeName, Model<T> model)
        {
            try
            {
                using (var pipe = new NamedPipeClientStream(serverName, pipeName, PipeDirection.Out, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
                {
                    pipe.Connect();
                    byte[] data = Encoding.UTF8.GetBytes(model?.GetJsonString() ?? string.Empty);
                    pipe.Write(data, 0, data.Length);
                }
            }
            catch { }
        }
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static void SendNamed<T>(string pipeName, string cmd, T model = default) => SendNamed(pipeName, new Model<T>() { C = cmd, M = model });
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static void SendNamed(string pipeName, string cmd, string content = "") => SendNamed(pipeName, new ModelString() { C = cmd, M = content });
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static NamedPipeServerStream ReceiveNamed<T>(string pipeName, Action<Model<T>> Callback, int size = 65535)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
#pragma warning restore CA1416 // 验证平台兼容性
            serverPipe.BeginWaitForConnection((ar) =>
            {
                var ps = (NamedPipeServerStream)ar.AsyncState;
                ps.EndWaitForConnection(ar);
                var data = new byte[size];
                var count = ps.Read(data, 0, size);
                if (count > 0)
                {
                    Callback(Encoding.UTF8.GetString(data, 0, count).GetJsonObject<Model<T>>());
                }
                ps.Dispose();
                ReceiveNamed<T>(pipeName, Callback, size);
            }, serverPipe);
            return serverPipe;
        }
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static NamedPipeServerStream ReceiveNamed(string pipeName, Action<ModelString> Callback, int size = 65535)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
#pragma warning restore CA1416 // 验证平台兼容性
            serverPipe.BeginWaitForConnection((ar) =>
            {
                var ps = (NamedPipeServerStream)ar.AsyncState;
                ps.EndWaitForConnection(ar);
                var data = new byte[size];
                var count = ps.Read(data, 0, size);
                if (count > 0)
                {
                    Callback(Encoding.UTF8.GetString(data, 0, count).GetJsonObject<ModelString>());
                }
                ps.Dispose();
                ReceiveNamed(pipeName, Callback, size);
            }, serverPipe);
            return serverPipe;
        }
        /// <summary>
        /// 字符串数据
        /// </summary>
        public class ModelString : Model<String> { }
        /// <summary>
        /// 泛型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Model<T>
        {
            /// <summary>
            /// 命令
            /// </summary>
            public string C { get; set; }
            /// <summary>
            /// 模型内容
            /// </summary>
            public T M { get; set; }
        }
    }
}
