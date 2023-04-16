using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Impeller
{
    /// <summary>
    /// 一个高级API应用程序接口服务库的一部分，包含的函数与对象的安全性，注册表的操控以及事件日志有关
    /// </summary>
    public class ADVAPI32
    {
        /// <summary>
        /// kernel32.dll文件
        /// </summary>
        public const String DllFileName = "advapi32.dll";
        /// <summary>
        /// 允许账号使用空密码
        /// </summary>
        public const int ERROR_ACCOUNT_RESTRICTION = 1327;
        /// <summary>
        /// 登录失败,账户当前被禁用了
        /// </summary>
        public const int ERROR_ACCOUNT_DISABLED = 1331;
        /// <summary>
        /// 登录用户
        /// </summary>
        /// <param name="lpszUsername">用户名</param>
        /// <param name="lpszDomain">域</param>
        /// <param name="lpszPassword">密码</param>
        /// <param name="dwLogonType">登录类型</param>
        /// <param name="dwLogonProvider">登录提供</param>
        /// <param name="phToken">句柄</param>
        /// <returns></returns>
        [DllImport(DllFileName)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
    }
}
