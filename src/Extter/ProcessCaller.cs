using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Data.Extter
{
    /// <summary>
    /// 进程切换
    /// </summary>
    public static class ProcessSwitcher
    {
        #region // 激活状态及导入
        /// <summary>
        /// 隐藏
        /// </summary>
        public const int SW_HIDE = 0;
        /// <summary>
        /// 正常
        /// </summary>
        public const int SW_NORMAL = 1;
        /// <summary>
        /// 最大化
        /// </summary>
        public const int SW_MAXIMIZE = 3;
        /// <summary>
        /// 显示并激活
        /// </summary>
        public const int SW_SHOWNOACTIVATE = 4;
        /// <summary>
        /// 显示
        /// </summary>
        public const int SW_SHOW = 5;
        /// <summary>
        /// 最小化
        /// </summary>
        public const int SW_MINIMIZE = 6;
        /// <summary>
        /// 还原
        /// </summary>
        public const int SW_RESTORE = 9;
        /// <summary>
        /// 显示默认
        /// </summary>
        public const int SW_SHOWDEFAULT = 10;
        /// <summary>
        /// 设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="cmdshow">指定窗口如何显示,查看允许值列表,请查阅showWindow函数的说明部分</param>
        /// <returns>如果函数原来可见,返回值为非零,如果函数原来被隐藏,返回值为零</returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hwnd, int cmdshow);
        /// <summary>
        /// 该函数将创建指定窗口线程设置到前台,并激活该窗口
        /// 键盘输入转向该窗口,并未用户改各种可视的记号
        /// 系统给创建前台窗口的线程分配的权限稍高于其他线程。   
        /// </summary>
        /// <param name="hwnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
        #endregion
        /// <summary>
        /// 打开名称
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <param name="status">状态</param>
        public static IntPtr ShowName(string name, int status = SW_SHOW)
        {
            Process tagProcess = GetOtherProcess(name);
            if (tagProcess != null)
            {
                var hwnd = tagProcess.MainWindowHandle;
                ShowWindowAsync(hwnd, status);
                SetForegroundWindow(hwnd);
                return hwnd;
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// 获取指定其他进程
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Process GetOtherProcess(string name)
        {
            var processes = Process.GetProcessesByName(name);
            var currentProcess = Process.GetCurrentProcess();
            Process tagProcess = null;
            foreach (var item in processes)
            {
                if (item.Id != currentProcess.Id)
                {
                    tagProcess = item;
                    break;
                }
            }
            return tagProcess;
        }
    }
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
