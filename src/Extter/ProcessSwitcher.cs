using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
        public static void ShowName(string name, int status = SW_SHOW)
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
            if (tagProcess != null)
            {
                var hwnd = tagProcess.MainWindowHandle;
                ShowWindowAsync(hwnd, status);
                SetForegroundWindow(hwnd);
            }
        }

    }
}
