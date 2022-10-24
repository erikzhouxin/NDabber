using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Impeller
{
    /// <summary>
    /// Windws操作系统网络通讯相关模块
    /// </summary>
    public class MPR
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2")]
        private static extern uint WNetAddConnection2(NetResource lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        [DllImport("Mpr.dll", EntryPoint = "WNetCancelConnection2")]
        private static extern uint WNetCancelConnection2(string lpName, uint dwFlags, bool fForce);

        public enum ScopeType
        {
            ResourceConnected = 1,
            ResourceGlobalnet = 2,
            ResourceRemembered = 3,
            ResourceRecent = 4,
            ResourceContext = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ScopeType dwScope;

            public int dwType;

            public int dwDisplayType;

            public int dwUsage;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpLocalName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpRemoteName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpComment;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpProvider;
        }

        /// <summary>
        /// 为网络共享做本地映射
        /// </summary>
        /// <param name="username">访问用户名（windows系统需要加计算机名，如：comp-1\user-1）</param>
        /// <param name="password">访问用户密码</param>
        /// <param name="remoteName">网络共享路径（如：\\192.168.0.9\share）</param>
        /// <param name="localName">本地映射盘符</param>
        /// <returns></returns>
        [Obsolete]
        private static uint WNetAddConnection(string username, string password, string remoteName, string localName)
        {
            NetResource netResource = new NetResource();
            netResource.dwScope = ScopeType.ResourceGlobalnet;
            netResource.dwType = 1;
            netResource.dwDisplayType = 3;
            netResource.dwUsage = 1;
            netResource.lpLocalName = localName;
            netResource.lpRemoteName = remoteName.TrimEnd('\\');
            uint result = WNetAddConnection2(netResource, password, username, 0);

            return result;
        }

        /// <summary>
        /// 映射网络共享做为本地资源
        /// </summary>
        /// <param name="remoteName">网络共享路径（如：\\192.168.0.9\share）</param>
        /// <param name="userInfo">访问网络资源的域名、用户名、密码等</param>
        /// <param name="localName">本地映射盘符。(如：z：)</param>
        /// <returns></returns>
        public static uint WNetAddConnection(string remoteName, System.Net.NetworkCredential userInfo = null, string localName = null)
        {
            NetResource netResource = new NetResource();
            string username = "guest";
            string password = "";
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.UserName))
            {
                username = (string.IsNullOrEmpty(userInfo.Domain) ? "" : userInfo.Domain + "\\") + userInfo.UserName;
                password = userInfo.Password;
            }
            netResource.dwScope = ScopeType.ResourceGlobalnet;
            netResource.dwType = 1;
            netResource.dwDisplayType = 3;
            netResource.dwUsage = 1;
            netResource.lpLocalName = localName;
            netResource.lpRemoteName = remoteName.TrimEnd('\\');
            uint result = WNetAddConnection2(netResource, password, username, 0);

            return result;
        }


        public static uint WNetCancelConnection(string name, uint flags, bool force)
        {
            uint nret = WNetCancelConnection2(name, flags, force);
            return nret;
        }


        // SW_HIDE             0 //隐藏窗口，活动状态给令一个窗口
        // SW_SHOWNORMAL       1 //用原来的大小和位置显示一个窗口，同时令其进入活动状态
        // SW_NORMAL           1
        // SW_SHOWMINIMIZED    2
        // SW_SHOWMAXIMIZED    3
        // SW_MAXIMIZE         3
        // SW_SHOWNOACTIVATE   4 //用最近的大小和位置显示一个窗口，同时不改变活动窗口
        // SW_SHOW             5 //用当前的大小和位置显示一个窗口，同时令其进入活动状态
        // SW_MINIMIZE         6 //最小化窗口，活动状态给令一个窗口
        // SW_SHOWMINNOACTIVE  7 //最小化一个窗口，同时不改变活动窗口
        // SW_SHOWNA           8 //用当前的大小和位置显示一个窗口，不改变活动窗口
        // SW_RESTORE          9 //与 SW_SHOWNORMAL  1 相同
        // SW_SHOWDEFAULT      10
        // SW_FORCEMINIMIZE    11
        // SW_MAX              11
        [DllImport("kernel32.dll")]
        private static extern int WinExec(string exeName, int operType);

        /// <summary>
        /// 执行命令行代码的操作选项
        /// </summary>
        public enum ExecType
        {
            /// <summary>
            /// 隐藏窗口，活动状态给令一个窗口
            /// </summary>
            sw_hide = 0,

            /// <summary>
            /// 用原来的大小和位置显示一个窗口，同时令其进入活动状态
            /// </summary>
            sw_shownormal = 1,
            sw_normal = 1,
            sw_showminimized = 2,
            sw_showmaximized = 3,
            sw_maximize = 3,

            /// <summary>
            /// 用最近的大小和位置显示一个窗口，同时不改变活动窗口
            /// </summary>
            sw_shownoactivate = 4,

            /// <summary>
            /// 用当前的大小和位置显示一个窗口，同时令其进入活动状态
            /// </summary>
            sw_show = 5,

            /// <summary>
            /// 最小化窗口，活动状态给令一个窗口
            /// </summary>
            sw_minimize = 6,

            /// <summary>
            /// 最小化一个窗口，同时不改变活动窗口
            /// </summary>
            sw_showminnoactive = 7,

            /// <summary>
            /// 用当前的大小和位置显示一个窗口，不改变活动窗口
            /// </summary>
            sw_showna = 8,

            /// <summary>
            /// 与 sw_shownormal  1 相同
            /// </summary>
            sw_restore = 9,
            sw_showdefault = 10,
            sw_forceminimize = 11,
            sw_max = 11,
        }


        /// <summary>
        /// 执行命令行代码的
        /// </summary>
        /// <param name="commandCode">命令行字符串</param>
        /// <param name="operType">操作选项</param>
        /// <returns></returns>
        public static int ExecCommand(string commandCode, ExecType operType)
        {
            return WinExec(commandCode, (int)operType);
        }
    }
}
