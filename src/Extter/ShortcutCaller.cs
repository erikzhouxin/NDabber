// using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 快捷方式调用者
    /// 注:需要引入Com组件IWshRuntimeLibrary，Windows Script Host Object Model
    /// </summary>
    /*
		<COMReference Include="IWshRuntimeLibrary">
			<WrapperTool>tlbimp</WrapperTool>
			<VersionMinor>0</VersionMinor>
			<VersionMajor>1</VersionMajor>
			<Guid>f935dc20-1cf0-11d0-adb9-00c04fd58a0b</Guid>
			<Lcid>0</Lcid>
			<Isolated>false</Isolated>
			<EmbedInteropTypes>true</EmbedInteropTypes>
		</COMReference>
    */
    public static partial class ExtterCaller
    {
        internal static readonly Guid CLSID_WshShell = new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8");
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="directory">快捷方式所处的文件夹</param>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，
        /// 例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks></remarks>
        public static void CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }
            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            if (System.IO.File.Exists(shortcutPath)) { IO.File.Delete(shortcutPath); }
            dynamic wshShell = null, shortcut = null;
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                wshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WshShell)); // WshShell shell = new WshShell();
#pragma warning restore CA1416 // 验证平台兼容性
                shortcut = wshShell.CreateShortcut(shortcutPath); // shortcut = (IWshShortcut)wshShell.CreateShortcut(shortcutPath); //创建快捷方式对象
                shortcut.TargetPath = targetPath; //指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath); //设置起始位置
                shortcut.WindowStyle = 1; //设置运行方式，默认为常规窗口
                shortcut.Description = description; //设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation; //设置图标路径
                shortcut.Arguments = args;
                shortcut.Save(); //保存快捷方式
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Marshal.ReleaseComObject(shortcut);
                Marshal.ReleaseComObject(wshShell);
            }

        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"</param>
        /// <param name="args"></param>
        /// <remarks>参数</remarks>
        public static void CreateShortcutDesktop(string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); //获取桌面文件夹路径
            CreateShortcut(desktop, shortcutName, targetPath, description, iconLocation, args);
        }
    }
}
