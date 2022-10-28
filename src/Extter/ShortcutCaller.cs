// using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dibber;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Extter
{
    /// <summary>
    /// 快捷方式调用者
    /// 注:需要引入Com组件IWshRuntimeLibrary，Windows Script Host Object Model
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="directory">快捷方式所处的文件夹</param>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks></remarks>
        public static void CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            using var shellLink = CreateShortcut2(Path.Combine(directory, shortcutName), targetPath, description, iconLocation, args, null);
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称,不包括扩展名</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks>参数</remarks>
        public static void CreateShortcutDesktop(string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); //获取桌面文件夹路径
            using var shellLink = CreateShortcut2(Path.Combine(desktop, shortcutName), targetPath, description, iconLocation, args, null);
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称,不包括扩展名</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="icon">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks>参数</remarks>
        public static void CreateShortcutStartup(string shortcutName, string targetPath, string description = null, string icon = null, string args = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Startup); //获取启动文件夹路径
            using var shellLink = CreateShortcut2(Path.Combine(desktop, shortcutName), targetPath, description, icon, args, null);
        }
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="description"></param>
        /// <param name="icon"></param>
        /// <param name="workDir"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ShellLink CreateShortcut2(string shortcutPath, string targetPath, string description = null, string icon = null, string args = null, string workDir = null)
        {
            shortcutPath = ShellLink.GetFullPath(shortcutPath);
            targetPath = ShellLink.GetFullPath(targetPath);
            if (!System.IO.Path.HasExtension(shortcutPath)) { shortcutPath += ShellLink.Extension; }
            var link = new ShellLink(shortcutPath);
            link.Path = targetPath;
            link.WorkingDirectory = workDir ?? System.IO.Path.GetDirectoryName(targetPath);
            link.IconLocation = GetIconLocation(icon);
            if (description != null) { link.Description = description; }
            if (args != null) { link.Arguments = args; }
            link.Save(shortcutPath);
            return link;
            static Tuble2StringInt GetIconLocation(string iconLocation)
            {
                if (string.IsNullOrWhiteSpace(iconLocation)) { return null; }
                try
                {
                    if (File.Exists(Path.GetFullPath(iconLocation)))
                    {
                        return new Tuble2StringInt(iconLocation, 0);
                    }
                }
                catch { }
                var iconTag = iconLocation.LastIndexOf(',');
                int index = 0;
                var fileName = iconLocation;
                if (iconTag > 0)
                {
                    index = iconLocation.Substring(iconTag + 1).ToPInt32();
                    fileName = iconLocation.Substring(0, iconTag);
                }
                return new Tuble2StringInt(fileName, index);
            }
        }
        /// <summary>
        /// 是快捷方式路径
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        public static bool IsShortcutFile(string shortcutPath)
        {
            shortcutPath = ShellLink.GetFullPath(shortcutPath);
            if (File.Exists(shortcutPath))
            {
                try
                {
                    using var shellLink = new ShellLink(shortcutPath);
                    return true;
                }
                catch { }
            }
            return false;
        }
        /// <summary>
        /// 是快捷方式指向路径
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static bool IsShortcutPointTo(string shortcutPath, string targetPath)
        {
            try
            {
                using var shellLink = new ShellLink(shortcutPath);
                return shellLink.IsPointTo(targetPath);
            }
            catch { }
            return false;
        }
    }
}
