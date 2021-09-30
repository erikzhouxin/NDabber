using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Data.Extter
{
    /// <summary>
    /// 进程通信
    /// </summary>
    public class ProcessCommunication
    {
        /// <summary>
        /// 导出SendMessage函数
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="IParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref SendStruct IParam);
        /// <summary>
        /// 导出FindWindow函数，用于找到目标窗口所在进程
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// 资源钩子
        /// </summary>
        /// <returns></returns>
        public static IntPtr ReceiveMessage<T>(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled, Action<T> AnylysisData)
        {
            if (msg == WM_COPYDATA)
            {
                T cds = (T)Marshal.PtrToStructure(lParam, typeof(T)); // 接收封装的消息
                AnylysisData(cds);
            }
            return hwnd;
        }
        /// <summary>
        /// 固定数值，不可更改
        /// </summary>
        private static int WM_COPYDATA { get; } = 0x004A;
        /// <summary>
        /// 传输数据
        /// </summary>
        public struct SendStruct
        {
            /// <summary>
            /// 任意值
            /// </summary>
            public IntPtr dwData;
            /// <summary>
            /// 指定lpData内存区域的字节数
            /// </summary>
            public int cbData;
            /// <summary>
            /// 发送给目标窗口所在进程的数据
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hnw"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        public static bool SendMessage<T>(IntPtr hnw, string cmd, T args)
        {
            return SendMessage (hnw, new SendModel<T>() { Cmd = cmd, Args = args });
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        public static bool SendMessage<T>(string name, string cmd, T args)
        {
            var process = ProcessSwitcher.GetOtherProcess(name);
            return SendMessage(process.MainWindowHandle, new SendModel<T>() { Cmd = cmd, Args = args });
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public static bool SendMessage<T>(IntPtr hnw, SendModel<T> model)
        {
            if (hnw == IntPtr.Zero) { return false; }
            SendStruct cds = CreateMessage(model?.GetJsonString() ?? string.Empty);
            SendMessage(hnw, WM_COPYDATA, IntPtr.Zero, ref cds);
            return true;
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        public static bool SendMessage<T>(IntPtr hnw, string cmd, T args, Func<String, SendStruct> Create)
        {
            return SendMessage(hnw, new SendModel<T>() { Cmd = cmd, Args = args }, Create);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public static bool SendMessage<T>(IntPtr hnw, SendModel<T> model, Func<String, SendStruct> Create)
        {
            if (hnw == IntPtr.Zero) { return false; }
            SendStruct cds = Create(model?.GetJsonString() ?? string.Empty);
            SendMessage(hnw, WM_COPYDATA, IntPtr.Zero, ref cds);
            return true;
        }

        private static SendStruct CreateMessage(string content)
        {
            byte[] sarr = System.Text.Encoding.UTF8.GetBytes(content);
            int len = sarr.Length;
            SendStruct cds;
            cds.dwData = IntPtr.Zero;//可以是任意值
            cds.cbData = len * 2 + 1;//指定lpData内存区域的字节数
            cds.lpData = content;//发送给目标窗口所在进程的数据
            return cds;
        }

        /// <summary>
        /// 发送模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class SendModel<T>
        {
            /// <summary>
            /// 命令行
            /// </summary>
            public string Cmd { get; set; }
            /// <summary>
            /// 参数
            /// </summary>
            public T Args { get; set; }
        }
    }
}
