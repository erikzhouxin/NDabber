using System;
using System.Collections.Generic;
using System.Data.Dabber;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// Window操作系统调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// C:\Windows
        /// </summary>
        public static String WindowDir => Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
        /// <summary>
        /// 获取特殊目录全路径
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        public static String GetFullPath(this Environment.SpecialFolder special)
        {
            return Path.GetFullPath(Environment.GetFolderPath(special));
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="exeFile"></param>
        /// <param name="startDir"></param>
        /// <param name="args"></param>
        public static IAlertMsg ExecHidden(string exeFile, string startDir, string args)
        {
            var result = new AlertMsg(true, "");
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    FileName = exeFile,
                    Arguments = args,
                    WorkingDirectory = Path.GetFullPath(startDir),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                }
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null) { return; }
                result.AddMsg(e.Data);
            };
            p.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null) { return; }
                result.AddMsg(e.Data);
            };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();

            return result;
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        public static IAlertMsg ExecHidden(string command) => ExecHidden("cmd.exe", WindowDir, command);
        /// <summary>
        /// 执行命令行
        /// </summary>
        public static IAlertMsg ExecHidden(string exeFile, string command) => ExecHidden(exeFile, WindowDir, command);
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IAlertMsg NetStart(string serviceName) => ExecHidden("net", WindowDir, $" start {serviceName}");
        /// <summary>
        /// 关闭服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IAlertMsg NetStop(string serviceName) => ExecHidden("net", WindowDir, $" stop {serviceName}");
        /// <summary>
        /// 资源管理器打开目录(不重复打开)
        /// </summary>
        /// <param name="dir"></param>
        public static void StartExplorer(this DirectoryInfo dir) => ExecHidden("cmd", dir.FullName, $" /c start \"\" \"{dir.FullName}\"");
        /// <summary>
        /// 启动链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IAlertMsg StartUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { return AlertMsg.OperSuccess; }
            url = url.Trim().Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 启动链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IAlertMsg TryStartUrl(string url)
        {
            try { return StartUrl(url); }
            catch { return AlertMsg.OperError; }
        }
    }
}
