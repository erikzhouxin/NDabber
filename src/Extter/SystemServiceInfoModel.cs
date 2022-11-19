using System;
using System.Collections.Generic;
using System.Data.Dabber;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace System.Data.Extter
{
    /// <summary>
    /// 系统服务信息模型
    /// </summary>
    public class SystemServiceInfoModel
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public virtual String Name { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public virtual String Description { get; set; }
        /// <summary>
        /// 指定一个友好名称，用于标识用户界面程序中的服务。 
        /// 例如，一个特定服务的子项名称为 wuauserv，其显示名称更自动更新。
        /// </summary>
        public virtual String DisplayName { get; set; }
        /// <summary>
        /// 启动类型
        /// </summary>
        public virtual StartType Start { get; set; }
        /// <summary>
        /// 错误类型
        /// </summary>
        public virtual ErrorType Error { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public virtual String ServerName { get; set; }
        /// <summary>
        /// 指定密码。 如果使用 LocalSystem 帐户外的帐户，则这是必需的。
        /// </summary>
        public virtual String Password { get; set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public virtual ServerType Type { get; set; }
        /// <summary>
        /// 指定此服务是该组的成员的名称。 组列表存储在注册表中的 HKLM\System\CurrentControlSet\Control\ServiceGroupOrder 子项中。 
        /// 默认值为 null。
        /// </summary>
        public virtual String Group { get; set; }
        /// <summary>
        /// 指定服务二进制文件的路径。 binpath=没有默认值，必须提供此字符串。
        /// </summary>
        public virtual String BinPath { get; set; }
        /// <summary>
        /// 指定必须在此服务之前启动的服务或组的名称。 名称由 / (的正斜杠) 。
        /// </summary>
        public virtual String Depend { get; set; }
        /// <summary>
        /// 指定服务将在其中运行的帐户的名称，或指定要Windows驱动程序的驱动程序对象的名称。 
        /// 默认设置为 LocalSystem。
        /// </summary>
        public virtual String Obj { get; set; }
        /// <summary>
        /// 动态程序启动参数
        /// </summary>
        public virtual String Args { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public virtual String Source { get; set; }
        /// <summary>
        /// 创建服务
        /// </summary>
        /// <returns></returns>
        public virtual IAlertMsg Create()
        {
            var cmd = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ServerName)) { cmd.Append($" {ServerName}"); }
            cmd.Append($" create \"{Name}\"")
               .Append($" binpath= \"{BinPath}{Args}\"")
               .Append($" displayname= \"{Name}\"")
               .Append($" start= {GetStart(Start)}")
               .Append($" error= {GetError(Error)}");
            GetType(cmd, Type);
            if (!string.IsNullOrWhiteSpace(Obj)) { cmd.Append($" obj= \"{Obj}\""); }
            if (!string.IsNullOrWhiteSpace(Password)) { cmd.Append($" password= \"{Password}\""); }
            return ExtterCaller.ExecHidden("SC", Path.GetDirectoryName(Path.GetFullPath(Source)), cmd.ToString());
        }
        private static StringBuilder GetType(StringBuilder sb, ServerType type)
        {
            switch (type)
            {
                case ServerType.Own:
                    sb.Append(" type= own");
                    break;
                case ServerType.Share:
                    sb.Append(" type= share");
                    break;
                case ServerType.Kernel:
                    sb.Append(" type= kernal");
                    break;
                case ServerType.FileSys:
                    sb.Append(" type= filesys");
                    break;
                case ServerType.Rec:
                    sb.Append(" type= rec");
                    break;
                case ServerType.Interact:
                    sb.Append(" type= share type= interact");
                    break;
                case ServerType.InteractOwn:
                    sb.Append(" type= own type= interact");
                    break;
                case ServerType.Unknown:
                default:
                    break;
            }
            return sb;

        }
        private static String GetError(ErrorType error)
        {
            switch (error)
            {
                case ErrorType.Severe: return "severe";
                case ErrorType.Critical: return "critical";
                case ErrorType.Ignore: return "ignore";
                case ErrorType.Unknown:
                case ErrorType.Normal:
                default: return "normal";
            }
        }

        private static String GetStart(StartType start)
        {
            switch (start)
            {
                case StartType.Boot: return "boot";
                case StartType.System: return "system";
                case StartType.Auto: return "auto";
                case StartType.Disabled: return "disabled";
                case StartType.Delayed: return "delayed-auto";
                case StartType.Demand:
                case StartType.Unknown:
                default: return "demand";
            }
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <returns></returns>
        public virtual IAlertMsg Delete()
        {
            StopService();
            return ExtterCaller.ExecHidden("SC", Path.GetDirectoryName(Path.GetFullPath(Source)), $" delete {Name}");
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public virtual IAlertMsg StartService()
        {
            return ExtterCaller.NetStart(Name);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public virtual IAlertMsg StopService()
        {
            return ExtterCaller.NetStop(Name);
        }
        /// <summary>
        /// 安装服务
        /// </summary>
        public virtual void InstallService(Action<IAlertMsg> showMsg, Func<SystemServiceInfoModel, IAlertMsg> edit)
        {
            showMsg?.Invoke(Create());
            if (edit != null)
            { showMsg?.Invoke(edit?.Invoke(this)); }
            showMsg?.Invoke(StartService());
        }
        /// <summary>
        /// 卸载服务
        /// </summary>
        public virtual void UninstallService(Action<IAlertMsg> showMsg, Func<SystemServiceInfoModel, IAlertMsg> edit)
        {
            showMsg?.Invoke(StopService());
            Thread.Sleep(1000); // 等一秒看看情况
            if (edit != null)
            { showMsg?.Invoke(edit.Invoke(this)); }
            showMsg?.Invoke(Delete());
        }
        /// <summary>
        /// 错误类型
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// normal - 指定记录错误并显示消息框，通知用户服务无法启动。 启动将继续。 这是默认设置。
            /// </summary>
            Normal = 1,
            /// <summary>
            /// 严重 - 指定在可能的情况下， (记录) 。 计算机尝试使用上次已知的良好配置重启。 这可能会导致计算机能够重启，但服务仍可能无法运行。
            /// </summary>
            Severe = 2,
            /// <summary>
            /// critical - 指定在可能的情况下， (记录) 。 计算机尝试使用上次已知的良好配置重启。 如果上次已知的良好配置失败，启动也会失败，启动进程将停止并出现"停止"错误。
            /// </summary>
            Critical = 3,
            /// <summary>
            /// ignore - 指定记录错误并继续启动。 除了在事件日志中记录错误外，不会向用户发送任何通知。
            /// </summary>
            Ignore = 4,
        }
        /// <summary>
        /// 启动类型
        /// </summary>
        public enum StartType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// boot - 指定启动加载程序加载的设备驱动程序。
            /// </summary>
            Boot,
            /// <summary>
            /// system - 指定在内核初始化期间启动的设备驱动程序。
            /// </summary>
            System,
            /// <summary>
            /// auto - 指定每次重新启动计算机时自动启动的服务，即使没有用户登录到计算机，该服务也运行。
            /// </summary>
            Auto,
            /// <summary>
            /// demand - 指定必须手动启动的服务。 如果未指定 start= ，则这是默认值。
            /// </summary>
            Demand,
            /// <summary>
            /// disabled - 指定无法启动的服务。 若要启动已禁用的服务，将启动类型更改为其他值。
            /// </summary>
            Disabled,
            /// <summary>
            /// delayed-auto - 指定在其他自动服务启动后的一小段时间自动启动的服务。
            /// </summary>
            Delayed
        }
        /// <summary>
        /// 服务类型
        /// </summary>
        public enum ServerType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// own - 指定在其自己的进程中运行的服务。 它不会与其他服务共享可执行文件。 这是默认值。
            /// </summary>
            Own,
            /// <summary>
            /// share - 指定作为共享进程运行的服务。 它与其他服务共享可执行文件。
            /// </summary>
            Share,
            /// <summary>
            /// kernel - 指定驱动程序。
            /// </summary>
            Kernel,
            /// <summary>
            /// filesys - 指定文件系统驱动程序。
            /// </summary>
            FileSys,
            /// <summary>
            /// rec - 指定一个文件系统识别的驱动程序，用于标识计算机上使用的文件系统。
            /// </summary>
            Rec,
            /// <summary>
            /// interact - 指定可以与桌面交互的服务，从用户处接收输入。 交互式服务必须在 LocalSystem 帐户下运行。 
            /// 此类型必须与 type= own 或 type= shared (一起使用，例如 ，type= interacttype= own) 。 使用 type= 自行 交互将生成错误。
            /// </summary>
            Interact,
            /// <summary>
            /// localsystem+允许与桌面进行交互
            /// </summary>
            InteractOwn,
        }
    }
}
