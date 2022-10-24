using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Impeller
{
    /// <summary>
    /// 用户界面相关应用程序接口
    /// Windows用户界面相关应用程序接口，
    /// 用于包括Windows处理，基本用户界面等特性，
    /// 如创建窗口和发送消息。
    /// </summary>
    public static class USER32
    {
        /// <summary>
        /// 设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="cmdshow">指定窗口如何显示,查看允许值列表,请查阅showWindow函数的说明部分</param>
        /// <returns>如果函数原来可见,返回值为非零,如果函数原来被隐藏,返回值为零</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hwnd, int cmdshow);
        /// <summary>
        /// 该函数将创建指定窗口线程设置到前台,并激活该窗口
        /// 键盘输入转向该窗口,并未用户改各种可视的记号
        /// 系统给创建前台窗口的线程分配的权限稍高于其他线程。   
        /// </summary>
        /// <param name="hwnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);
        /// <summary>
        /// 导出SendMessage函数
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="IParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref SendStruct IParam);
        /// <summary>
        /// 导出FindWindow函数，用于找到目标窗口所在进程
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        #region // 输入输出
        /// <summary>
        /// 获取最后输入信息
        /// </summary>
        /// <param name="plii"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref PLASTINPUTINFO plii);
        #endregion
        #region // 公开类
        /// <summary>
        /// 最后一次输入信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PLASTINPUTINFO
        {
            /// <summary>
            /// 大小
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int CbSize;
            /// <summary>
            /// 时间
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint DwTime;
        }
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
        #endregion
    }
}
