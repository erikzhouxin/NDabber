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
        /// <summary>
        /// WNetGetConnection函数可用于获取有关网络资源的信息，例如打印机。
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="remoteName"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);
        /// <summary>
        /// 创建同一个网络资源的连接
        /// </summary>
        /// <param name="lpNetResource"></param>
        /// <param name="lpPassword"></param>
        /// <param name="lpUsername"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2")]
        private static extern uint WNetAddConnection2(NetResource lpNetResource, string lpPassword, string lpUsername, uint dwFlags);
        /// <summary>
        /// 取消与网络资源的连接
        /// </summary>
        /// <param name="lpName"></param>
        /// <param name="dwFlags"></param>
        /// <param name="fForce"></param>
        /// <returns></returns>
        [DllImport("Mpr.dll", EntryPoint = "WNetCancelConnection2")]
        private static extern uint WNetCancelConnection2(string lpName, uint dwFlags, bool fForce);
        /// <summary>
        /// 范围类型
        /// </summary>
        public enum ScopeType
        {
            /// <summary>
            /// 资源已连接
            /// </summary>
            ResourceConnected = 1,
            /// <summary>
            /// 全局网络资源
            /// </summary>
            ResourceGlobalnet = 2,
            /// <summary>
            /// 资源已缓存
            /// </summary>
            ResourceRemembered = 3,
            /// <summary>
            /// 最近资源
            /// </summary>
            ResourceRecent = 4,
            /// <summary>
            /// 环境资源
            /// </summary>
            ResourceContext = 5
        }
        /// <summary>
        /// 网络资源结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NetResource
        {
            /// <summary>
            /// 资源类型
            /// </summary>
            public ScopeType DwScope;
            /// <summary>
            /// 资源类型
            /// </summary>
            public int DwType;
            /// <summary>
            /// 资源显示类型
            /// </summary>
            public int DwDisplayType;
            /// <summary>
            /// 资源使用
            /// </summary>
            public int DwUsage;
            /// <summary>
            /// 本地名称
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string LpLocalName;
            /// <summary>
            /// 远程名称
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string LpRemoteName;
            /// <summary>
            /// 注释
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string LpComment;
            /// <summary>
            /// 提供
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string LpProvider;
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
            netResource.DwScope = ScopeType.ResourceGlobalnet;
            netResource.DwType = 1;
            netResource.DwDisplayType = 3;
            netResource.DwUsage = 1;
            netResource.LpLocalName = localName;
            netResource.LpRemoteName = remoteName.TrimEnd('\\');
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
            netResource.DwScope = ScopeType.ResourceGlobalnet;
            netResource.DwType = 1;
            netResource.DwDisplayType = 3;
            netResource.DwUsage = 1;
            netResource.LpLocalName = localName;
            netResource.LpRemoteName = remoteName.TrimEnd('\\');
            uint result = WNetAddConnection2(netResource, password, username, 0);

            return result;
        }
        /// <summary>
        /// 取消连接
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <param name="force"></param>
        /// <returns></returns>
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
            /// <summary>
            /// 通用
            /// </summary>
            sw_normal = 1,
            /// <summary>
            /// 最小化
            /// </summary>
            sw_showminimized = 2,
            /// <summary>
            /// 最大化
            /// </summary>
            sw_showmaximized = 3,
            /// <summary>
            /// 最大化
            /// </summary>
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
            /// <summary>
            /// 默认
            /// </summary>
            sw_showdefault = 10,
            /// <summary>
            /// 强制最小化
            /// </summary>
            sw_forceminimize = 11,
            /// <summary>
            /// 最大化
            /// </summary>
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
