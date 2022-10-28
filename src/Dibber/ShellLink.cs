using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Extter;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace System.Data.Dibber
{
    /// <summary>
    /// 快捷方式(推荐使用ShellLink)
    /// </summary>
    public sealed class Shortcut : ShellLink, IShortcutCore
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Shortcut() : base() { }
        /// <summary>
        /// 从一个文件加载
        /// </summary>
        /// <param name = "linkFilePath">快捷方式文件</param>
        public Shortcut(string linkFilePath) : base(linkFilePath, ShellLink.STGM_FLAGS.STGM_READ) { }
        /// <summary>
        /// 从一个文件及加载方式
        /// </summary>
        /// <param name="linkFilePath"></param>
        /// <param name="flags"></param>
        public Shortcut(string linkFilePath, ShellLink.STGM_FLAGS flags) : base(linkFilePath, flags) { }

        #region // IPersistFile
        /// <summary>
        /// 获取类标记
        /// </summary>
        /// <param name="pClassID"></param>
        public void GetClassID(out Guid pClassID) => PersistFile.GetClassID(out pClassID);
        /// <summary>
        /// 是脏快捷方式
        /// </summary>
        /// <returns></returns>
        public int IsDirty() => PersistFile.IsDirty();
        /// <summary>
        /// 加载快捷方式
        /// </summary>
        /// <param name="pszFileName"></param>
        /// <param name="dwMode"></param>
        public void Load(string pszFileName, ShellLink.STGM_FLAGS dwMode) => PersistFile.Load(pszFileName, (int)dwMode);
        /// <summary>
        /// 保存快捷方式
        /// </summary>
        /// <param name="pszFileName"></param>
        /// <param name="fRemember"></param>
        public void Save(string pszFileName, bool fRemember) => PersistFile.Save(pszFileName, fRemember);
        /// <summary>
        /// 保存完整
        /// </summary>
        /// <param name="pszFileName"></param>
        public void SaveCompleted(string pszFileName) => PersistFile.SaveCompleted(pszFileName);
        /// <summary>
        /// 获取当前指向文件
        /// </summary>
        /// <param name="ppszFileName"></param>
        public void GetCurFile(out IntPtr ppszFileName) => PersistFile.GetCurFile(out ppszFileName);
        #endregion IPersistFile
        #region // IShellLink
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="pszFile"></param>
        /// <param name="cchMaxPath"></param>
        /// <param name="pfd"></param>
        /// <param name="fFlags"></param>
        public void GetPath(StringBuilder pszFile, int cchMaxPath, out ShellLink.WIN32_FIND_DATAW pfd, ShellLink.SLGP_FLAGS fFlags)
            => Shortcut.GetPath(pszFile, cchMaxPath, out pfd, fFlags);
        /// <summary>
        /// 获取标识列表
        /// </summary>
        /// <param name="ppidl"></param>
        public void GetIDList(out IntPtr ppidl) => Shortcut.GetIDList(out ppidl);
        /// <summary>
        /// 设置标识列表
        /// </summary>
        /// <param name="pidl"></param>
        public void SetIDList(IntPtr pidl) => Shortcut.SetIDList(pidl);
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="pszName"></param>
        /// <param name="cchMaxName"></param>
        public void GetDescription(StringBuilder pszName, int cchMaxName) => Shortcut.GetDescription(pszName, cchMaxName);
        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="pszName"></param>
        public void SetDescription(string pszName) => Shortcut.SetDescription(pszName);
        /// <summary>
        /// 获取工作目录
        /// </summary>
        /// <param name="pszDir"></param>
        /// <param name="cchMaxPath"></param>
        public void GetWorkingDirectory(StringBuilder pszDir, int cchMaxPath) => Shortcut.GetWorkingDirectory(pszDir, cchMaxPath);
        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="pszDir"></param>
        public void SetWorkingDirectory(string pszDir) => Shortcut.SetWorkingDirectory(pszDir);
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="pszArgs"></param>
        /// <param name="cchMaxPath"></param>
        public void GetArguments(StringBuilder pszArgs, int cchMaxPath) => Shortcut.GetArguments(pszArgs, cchMaxPath);
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pszArgs"></param>
        public void SetArguments(string pszArgs) => Shortcut.SetArguments(pszArgs);
        /// <summary>
        /// 获取热键
        /// </summary>
        /// <param name="pwHotkey"></param>
        public void GetHotkey(out short pwHotkey) => Shortcut.GetHotkey(out pwHotkey);
        /// <summary>
        /// 设置热键
        /// </summary>
        /// <param name="wHotkey"></param>
        public void SetHotkey(short wHotkey) => Shortcut.SetHotkey(wHotkey);
        /// <summary>
        /// 获取显示命令
        /// </summary>
        /// <param name="piShowCmd"></param>
        /// <returns></returns>
        public Int32 GetShowCmd(out ShellLink.ShowWindowCommand piShowCmd)
        {
            var res = Shortcut.GetShowCmd(out int cmd);
            piShowCmd = (ShellLink.ShowWindowCommand)cmd;
            return res;
        }
        /// <summary>
        /// 设置显示命令
        /// </summary>
        /// <param name="iShowCmd"></param>
        public void SetShowCmd(ShellLink.ShowWindowCommand iShowCmd) => Shortcut.SetShowCmd((int)iShowCmd);
        /// <summary>
        /// 获取图标位置
        /// </summary>
        /// <param name="pszIconPath"></param>
        /// <param name="cchIconPath"></param>
        /// <param name="piIcon"></param>
        /// <returns></returns>
        public Int32 GetIconLocation(StringBuilder pszIconPath, int cchIconPath, out int piIcon) => Shortcut.GetIconLocation(pszIconPath, cchIconPath, out piIcon);
        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="pszIconPath"></param>
        /// <param name="iIcon"></param>
        public void SetIconLocation(string pszIconPath, int iIcon) => Shortcut.SetIconLocation(pszIconPath, iIcon);
        /// <summary>
        /// 设置相对路径
        /// </summary>
        /// <param name="pszPathRel"></param>
        /// <param name="dwReserved"></param>
        public void SetRelativePath(string pszPathRel, int dwReserved) => Shortcut.SetRelativePath(pszPathRel, dwReserved);
        /// <summary>
        /// 尝试找到快捷方式的目标，即使它已经被移动或重命名
        /// </summary>
        /// <param name = "hwnd">窗口的句柄，使用它作为对话框的父级。如果需要在解析快捷方式时提示用户提供更多信息，则会显示对话框。</param>
        /// <param name = "fFlags">控制解析过程的标志</param>
        public override void Resolve(IntPtr hwnd, ShellLink.SLR_FLAGS fFlags) => base.Resolve(hwnd, fFlags);
        /// <summary>
        /// 设置目标路径
        /// </summary>
        /// <param name="pszFile"></param>
        public void SetPath(string pszFile) => Shortcut.SetPath(pszFile);
        #endregion IShellLinkf
        #region // IShellLinkDataList
        /// <summary>
        /// 添加数据块
        /// </summary>
        /// <param name="pDataBlock"></param>
        /// <returns></returns>
        public Int32 AddDataBlock(IntPtr pDataBlock) => DataList.AddDataBlock(pDataBlock);
        /// <summary>
        /// 复制数据块
        /// </summary>
        /// <param name="dwSig"></param>
        /// <param name="ppDataBlock"></param>
        /// <returns></returns>
        public Int32 CopyDataBlock(UInt32 dwSig, out IntPtr ppDataBlock) => DataList.CopyDataBlock(dwSig, out ppDataBlock);
        /// <summary>
        /// 移除数据块
        /// </summary>
        /// <param name="dwSig"></param>
        /// <returns></returns>
        public Int32 RemoveDataBlock(UInt32 dwSig) => DataList.RemoveDataBlock(dwSig);
        /// <summary>
        /// 获取数据标记
        /// </summary>
        /// <param name="pdwFlags"></param>
        public void GetFlags(out ShellLink.DATA_FLAGS pdwFlags)
        {
            DataList.GetFlags(out uint flags);
            pdwFlags = (ShellLink.DATA_FLAGS)flags;
        }
        /// <summary>
        /// 设置数据标记
        /// </summary>
        /// <param name="dwFlags"></param>
        public void SetFlags(ShellLink.DATA_FLAGS dwFlags) => DataList.SetFlags((uint)dwFlags);
        #endregion IShellLinkDataList
    }
    /// <summary>
    /// 快捷方式
    /// </summary>
    public class ShellLink : IDisposable
    {
        /// <summary>
        /// 最大路径长度
        /// </summary>
        public const int MAX_PATH = 260;
        /// <summary>
        /// 扩展名
        /// </summary>
        public const string Extension = ".LNK";
        internal ShellLinkCore shellLinkObject;
        internal IShellLink Shortcut => (IShellLink)shellLinkObject;
        internal IShellLinkDataList DataList => (IShellLinkDataList)shellLinkObject;
        internal IPersistFile PersistFile => (IPersistFile)shellLinkObject;
        #region // 构造和释放
        /// <summary>
        /// 构造
        /// </summary>
        public ShellLink()
        {
            shellLinkObject = new ShellLinkCore();
        }
        /// <summary>
        /// 从一个文件加载
        /// </summary>
        /// <param name = "linkFilePath">快捷方式文件</param>
        public ShellLink(string linkFilePath) : this(linkFilePath, STGM_FLAGS.STGM_READ) { }
        /// <summary>
        /// 从一个文件及加载方式
        /// </summary>
        /// <param name="linkFilePath"></param>
        /// <param name="flags"></param>
        public ShellLink(string linkFilePath, STGM_FLAGS flags) : this()
        {
            var linkPath = GetFullPath(linkFilePath);
            if (File.Exists(linkPath))
            {
                PersistFile.Load(linkPath, (int)flags);
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ShellLink()
        {
            Dispose();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (shellLinkObject != null)
            {
#pragma warning disable CA1416 // 验证平台兼容性
                Marshal.ReleaseComObject(shellLinkObject);
#pragma warning restore CA1416 // 验证平台兼容性
                shellLinkObject = null;
            }
        }
        #endregion
        /// <summary>
        /// 保存文件的快捷方式。shell需要一个` .lnk `文件扩展名。
        /// </summary>
        /// <remarks>如果文件存在，它会静默地被覆盖。</remarks>
        public virtual void Save()
        {
            PersistFile.Save(ShortPath, true);
        }
        /// <summary>
        /// 保存文件的快捷方式。shell需要一个` .lnk `文件扩展名。
        /// </summary>
        /// <remarks>如果文件存在，它会静默地被覆盖。</remarks>
        /// <param name = "lnkPath">The path to the saved file. </param>
        public virtual void Save(string lnkPath)
        {
            PersistFile.Save(lnkPath, true);
        }
        /// <summary>
        /// 保存文件的快捷方式。shell需要一个` .lnk `文件扩展名。
        /// </summary>
        /// <remarks>如果文件存在，它会静默地被覆盖。</remarks>
        /// <param name = "lnkPath">The path to the saved file. </param>
        public virtual void SaveAs(string lnkPath)
        {
            PersistFile.Save(lnkPath, true);
        }
        /// <summary>
        /// 获取或设置快捷方式的目标。
        /// 这个属性的读取方法使用SLGP_RAWPATH标志。
        /// </summary>
        public virtual String Path
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                Shortcut.GetPath(sb, sb.Capacity, out _, SLGP_FLAGS.SLGP_RAWPATH);
                return sb.ToString();
            }
            set { Shortcut.SetPath(value); }
        }
        /// <summary>
        /// 使用SLGP_SHORTPATH标志获取快捷方式(.lnk)文件的路径
        /// </summary>
        public virtual string ShortPath
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                Shortcut.GetPath(sb, sb.Capacity, out _, SLGP_FLAGS.SLGP_SHORTPATH);
                return sb.ToString();
            }
        }
        /// <summary>
        /// 使用SLGP_UNCPRIORITY标志获取快捷方式(.lnk)文件的路径
        /// </summary>
        public virtual string UncPriorityPath
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                Shortcut.GetPath(sb, sb.Capacity, out _, SLGP_FLAGS.SLGP_UNCPRIORITY);
                return sb.ToString();
            }
        }
        /// <summary>
        /// 将命令行参数添加到快捷方式
        /// </summary>
        public virtual String Arguments
        {
            get
            {
                var sb = new StringBuilder(260);
                Shortcut.GetArguments(sb, sb.Capacity);
                return sb.ToString();
            }
            set { Shortcut.SetArguments(value); }
        }
        /// <summary>
        /// 尝试找到快捷方式的目标，即使它已经被移动或重命名
        /// </summary>
        /// <param name = "flags">控制解析过程的标志</param>
        public virtual void Resolve(SLR_FLAGS flags)
        {
            Shortcut.Resolve(IntPtr.Zero, (SLR_FLAGS)flags);
        }
        /// <summary>
        /// 尝试找到快捷方式的目标，即使它已经被移动或重命名
        /// </summary>
        /// <param name = "hwnd">窗口的句柄，使用它作为对话框的父级。如果需要在解析快捷方式时提示用户提供更多信息，则会显示对话框。</param>
        /// <param name = "flags">控制解析过程的标志</param>
        public virtual void Resolve(IntPtr hwnd, SLR_FLAGS flags) => Shortcut.Resolve(hwnd, flags);
        /// <summary>
        /// 尝试找到快捷方式的目标，即使它已经被移动或重命名
        /// </summary>
        /// <param name = "flags">控制解析过程的标志</param>
        /// <param name = "noUxTimeoutMs">当没有用户体验时等待解析的超时时间，单位为ms</param>
        public virtual void Resolve(SLR_FLAGS flags, int noUxTimeoutMs)
        {
            if ((flags & SLR_FLAGS.SLR_NO_UI) == 0)
            {
                throw new ArgumentException("这个方法需要ResolveFlags。在flags参数中设置了NoUi标志位。");
            }
            if (noUxTimeoutMs > short.MaxValue)
            {
                throw new ArgumentException(string.Format("nouxTimeoutMs值必须 <= {0}", short.MaxValue));
            }
            unchecked
            {
                flags = flags & (SLR_FLAGS)0x0000FFFF;
                flags |= (SLR_FLAGS)(noUxTimeoutMs << 16);
            }
            Shortcut.Resolve(IntPtr.Zero, (SLR_FLAGS)flags);
        }
        /// <summary>
        /// 工作目录
        /// </summary>
        public virtual String WorkingDirectory
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                Shortcut.GetWorkingDirectory(sb, sb.Capacity);
                return sb.ToString();
            }
            set { Shortcut.SetWorkingDirectory(value); }
        }
        /// <summary>
        /// 快捷方式描述
        /// </summary>
        public virtual String Description
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                Shortcut.GetDescription(sb, sb.Capacity);
                return sb.ToString();
            }
            set { Shortcut.SetDescription(value); }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public virtual Tuble2StringInt IconLocation
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                if (Shortcut.GetIconLocation(sb, sb.Capacity, out int iIcon) < 0)
                {
                    return new Tuble2StringInt();
                }
                return new Tuble2StringInt(sb.ToString(), iIcon);
            }
            set
            {
                if (value == null) { return; }
                Shortcut.SetIconLocation(value.Key, value.Value);
            }
        }
        /// <summary>
        /// 快捷方式对象的显示命令。
        /// </summary>
        public virtual ShowWindowCommand ShowCommand
        {
            get => Shortcut.GetShowCmd(out int showCmd) < 0 ? ShowWindowCommand.Hide : (ShowWindowCommand)showCmd;
            set => Shortcut.SetShowCmd((int)value);
        }
        /// <summary>
        /// 快捷方式的链接数据标志
        /// </summary>
        public virtual DATA_FLAGS Flags
        {
            get
            {
                DataList.GetFlags(out UInt32 flags);
                return (DATA_FLAGS)flags;
            }
            set { DataList.SetFlags((UInt32)value); }
        }
        /// <summary>
        /// 文件信息
        /// </summary>
        public virtual WIN32_FIND_DATAW FileInfo
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                Shortcut.GetPath(sb, sb.Capacity, out WIN32_FIND_DATAW info, SLGP_FLAGS.SLGP_RAWPATH);
                return info;
            }
        }
        /// <summary>
        /// 是指向路径
        /// </summary>
        /// <param name="tagPath"></param>
        /// <returns></returns>
        public bool IsPointTo(string tagPath)
        {
            if (tagPath == null) { return false; }
            var refPath = Path;
            if (string.IsNullOrWhiteSpace(refPath)) { return false; }
            return GetFullPath(refPath).EqualIgnoreCase2(GetFullPath(tagPath));
        }
        internal static string GetFullPath(string path)
        {
            try
            {
                return System.IO.Path.GetFullPath(path.Trim('"'));
            }
            catch { }
            return path;
        }
        #region // 内部类
        /// <summary>
        /// 这些值直接映射到IShellLink::Resolve()方法的原生SLR_*值
        /// </summary>
        [Flags]
        public enum SLR_FLAGS
        {
            /// <summary>
            /// 无
            /// </summary>
            None = 0,
            /// <summary>
            /// 如果快捷方式无效，则不显示对话框。
            /// </summary>
            SLR_NO_UI = 0x1,
            /// <summary>
            /// 未使用-忽略
            /// </summary>
            SLR_ANY_MATCH = 0x2,
            /// <summary>
            /// 如果链接对象已更改，则更新其路径和标识符列表。
            /// 如果设置了SLR_UPDATE，则不需要调用IPersistFile::IsDirty来确定链接对象是否发生了变化。
            /// </summary>
            SLR_UPDATE = 0x4,
            /// <summary>
            /// 不要更新链接信息
            /// </summary>
            SLR_NOUPDATE = 0x8,
            /// <summary>
            /// 不要执行搜索启发式。
            /// </summary>
            SLR_NOSEARCH = 0x10,
            /// <summary>
            /// 不要使用分布式链接跟踪。
            /// </summary>
            SLR_NOTRACK = 0x20,
            /// <summary>
            /// 关闭分布式链路跟踪功能。
            /// 默认情况下，分布式链接跟踪根据卷名跨多个设备跟踪可移动媒体。
            /// 它还使用UNC路径跟踪驱动器号已更改的远程文件系统。
            /// 设置NoLinkInfo可以禁用这两种跟踪
            /// </summary>
            SLR_NOLINKINFO = 0x40,
            /// <summary>
            /// 调用Windows安装程序
            /// </summary>
            SLR_INVOKE_MSI = 0x80,
            /// <summary>
            /// Windows XP及以上版本。
            /// </summary>
            SLR_NO_UI_WITH_MSG_PUMP = 0x101,
            /// <summary>
            /// Windows 7及更高版本。
            /// 当此方法无法解决时，提供删除快捷方式的选项，即使该快捷方式不是文件的快捷方式。
            /// </summary>
            SLR_OFFER_DELETE_WITHOUT_FILE = 0x201,
            /// <summary>
            /// Windows 7及更高版本。
            /// 如果目标是一个已知文件夹并且已知文件夹被重定向，则报告为脏。
            /// 仅当原始目标路径是文件系统路径或ID列表，而不是别名已知的文件夹ID列表时，才有效。
            /// </summary>
            SLR_KNOWNFOLDER = 0x400,
            /// <summary>
            /// Windows 7及更高版本。
            /// 解析UNC目标中指向本地计算机的计算机名。
            /// 该值与SLDF_KEEP_LOCAL_IDLIST_FOR_UNC_TARGET一起使用。
            /// </summary>
            SLR_MACHINE_IN_LOCAL_TARGET = 0x800,
            /// <summary>
            /// Windows 7及更高版本。
            /// 如有必要，更新计算机GUID和用户SID。
            /// </summary>
            SLR_UPDATE_MACHINE_AND_SID = 0x1000
        }
        /// <summary>
        /// 这些标志值映射到原生的SHELL_LINK_DATA_FLAGS枚举。看到MSDN
        /// </summary>
        [Flags]
        public enum DATA_FLAGS
        {
            /// <summary>
            /// 在没有显式设置其他标志时使用的默认值。
            /// </summary>
            None = 0x00000000,
            /// <summary>
            /// 在没有显式设置其他标志时使用的默认值。
            /// </summary>
            Default = 0x00000000,
            /// <summary>
            /// 快捷方式保存为一个ID列表
            /// </summary>
            HasIdList = 0x00000001,
            /// <summary>
            /// 快捷方式与链接信息一起保存，以支持分布式跟踪。
            /// 如果目标的路径发生了变化，.lnk文件会使用这些信息来定位目标。
            /// 它包括诸如卷标签和序列号之类的信息，尽管特定的存储信息可能在不同版本之间发生变化。
            /// </summary>
            HasLinkInfo = 0x00000002,
            /// <summary>
            /// 链接具有名称
            /// </summary>
            HasName = 0x00000004,
            /// <summary>
            /// 链接具有相对路径。
            /// </summary>
            HasRelativePath = 0x00000008,
            /// <summary>
            /// 这个链接有一个工作目录。
            /// </summary>
            HasWorkingDirectory = 0x00000010,
            /// <summary>
            /// 这个链接有参数。
            /// </summary>
            HasArguments = 0x00000020,
            /// <summary>
            /// 链接有一个图标位置。
            /// </summary>
            HasIconLocation = 0x00000040,
            /// <summary>
            /// 存储的字符串是Unicode。
            /// </summary>
            Unicode = 0x00000080,
            /// <summary>
            /// 防止链路跟踪信息的存储。
            /// 如果设置了该标志，那么在目标移动的情况下，链接找到目标的可能性较小，但也不是不可能。
            /// </summary>
            ForceNoLinkInfo = 0x00000100,
            /// <summary>
            /// 该链接包含可扩展的环境字符串，如%windir%。
            /// </summary>
            HasExpSz = 0x00000200,
            /// <summary>
            /// 导致16位目标应用程序运行在单独的虚拟DOS机器(VDM)/Windows上的Windows (WOW)。
            /// </summary>
            RunInSeparate = 0x00000400,
            /// <summary>
            /// 不受支持的。注意，从Windows Vista开始，这个值不再定义。
            /// </summary>
            HasLogo3Id = 0x00000800,
            /// <summary>
            /// 这个链接是一个特殊的Windows安装程序链接。
            /// </summary>
            HasDarwinId = 0x00001000,
            /// <summary>
            /// 使目标应用程序以不同的用户身份运行。
            /// </summary>
            RunAsUser = 0x00002000,
            /// <summary>
            /// 链接中的图标路径包含一个可扩展的环境字符串，例如%windir%。
            /// </summary>
            HasExpIconSz = 0x00004000,
            /// <summary>
            /// 当从路径解析ID列表时，防止使用ID列表别名映射。
            /// </summary>
            NoPidlAlias = 0x00008000,
            /// <summary>
            /// 强制使用UNC名称(完整的网络资源名称)，而不是本地名称
            /// </summary>
            ForceUncName = 0x00010000,
            /// <summary>
            /// 使此链路的目标启动与垫片层活动。
            /// shim是一个中间的DLL，它促进了不兼容的软件服务之间的兼容性。
            /// 垫片通常用于提供版本兼容性。
            /// </summary>
            RunWithShimLayer = 0x00020000,
            /// <summary>
            /// Windows Vista及更高版本。
            /// 禁用对象ID分布式跟踪信息。
            /// </summary>
            ForceNoLinkTrack = 0x00040000,
            /// <summary>
            /// Windows Vista及更高版本。
            /// 将目标元数据缓存到链接文件中。
            /// </summary>
            EnableTargetMetadata = 0x000800000,
            /// <summary>
            /// Windows 7及更高版本。
            /// 禁用快捷方式跟踪。
            /// </summary>
            DisableLinkpathTracking = 0x00100000,
            /// <summary>
            /// Windows Vista及更高版本。
            /// 禁用已知的文件夹跟踪信息。
            /// </summary>
            DisableKnownFolderRelativeTracking = 0x00200000,
            /// <summary>
            /// Windows 7及更高版本。
            /// 在反序列化过程中加载IDList时禁用已知文件夹别名映射。
            /// </summary>
            NoKfAlias = 0x00400000,
            /// <summary>
            /// Windows 7及更高版本。
            /// 允许link指向另一个快捷方式，只要不创建周期。
            /// </summary>
            AllowLinkToLInk = 0x00800000,
            /// <summary>
            /// Windows 7及更高版本。
            /// 在保存IDList时移除别名
            /// </summary>
            UnaliasOnSave = 0x01000000,
            /// <summary>
            /// Windows 7及更高版本。
            /// 在加载时使用环境变量重新计算路径中的IDList，而不是持久化IDList。
            /// </summary>
            PreferEnvironmentPath = 0x02000000,
            /// <summary>
            /// Windows 7及更高版本。
            /// 如果目标是本地机器上的UNC位置，除了保持远程目标外，还要保持本地IDList目标。
            /// </summary>
            KeepLocalIDListForUncTarget = 0x04000000,
            /// <summary>
            /// W7的有效值
            /// </summary>
            W7Valid = 0x07FFF7FF,
            /// <summary>
            /// W8的有效值
            /// </summary>
            VistaValid = 0x003FF7FF,
            /// <summary>
            /// 保留，请勿使用
            /// </summary>
            Reserved = -2147483648
        }
        /// <summary>
        /// 显示窗口命令-查看Win32 ShowWindow() API了解更多信息
        /// </summary>
        public enum ShowWindowCommand
        {
            /// <summary>
            /// SW_FORCEMINIMIZE
            /// 最小化窗口，即使拥有窗口的线程没有响应。
            /// 这个标志只应该在最小化来自不同线程的窗口时使用。
            /// </summary>
            ForceMinimize = 11,
            /// <summary>
            /// SW_HIDE
            /// 隐藏窗口并激活另一个窗口。
            /// </summary>
            Hide = 0,
            /// <summary>
            /// SW_MAXIMIZE
            /// 最大化指定的窗口。
            /// </summary>
            Maximize = 3,
            /// <summary>
            /// SW_MINIMIZE
            /// 最小化指定的窗口，并按Z顺序激活下一个顶级窗口。
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// SW_RESTORE
            /// 激活并显示窗口。
            /// 如果窗口被最小化或最大化，系统会将其恢复到原始大小和位置。
            /// 应用程序在恢复最小化窗口时应该指定此标志。
            /// </summary>
            Restore = 9,
            /// <summary>
            /// SW_SHOW
            /// 激活窗口并以当前大小和位置显示它。
            /// </summary>
            Show = 5,
            /// <summary>
            /// SW_SHOWDEFAULT
            /// 根据启动应用程序的程序传递给CreateProcess函数的STARTUPINFO结构中指定的SW_值设置显示状态。
            /// </summary>
            ShowDeafult = 10,
            /// <summary>
            /// SW_SHOWMAXIMIZED
            /// 激活窗口并显示为最大化窗口。
            /// </summary>
            ShowMaximized = 3,
            /// <summary>
            /// SW_SHOWMINIMIZED
            /// 激活窗口并显示为最小化窗口。
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// SW_SHOWMINNOACTIVE
            /// 将窗口显示为最小化窗口。
            /// 这个值类似于sw_show，只是窗口没有激活。
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// SW_SHOWNORMAL
            /// 激活并显示一个窗口。
            /// 如果窗口被最小化或最大化，系统会将其恢复到原始大小和位置。
            /// 应用程序应该在第一次显示窗口时指定此标志。
            /// </summary>
            ShowNormal = 1
        }
        /// <summary>
        /// STGM 常量表示在创建或删除对象以及对象访问模式的条件
        /// STGM 常量用于 IStorage, IStream, and IPropertySetStorage 接口
        /// 和 StgCreateDocfile, StgCreateStorageEx, StgCreateDocfileOnILockBytes, StgOpenStorage 
        /// 以及 StgOpenStorageEx 方法
        /// Group                           Flag Value
        /// Access                          STGM_READ 0x00000000L
        ///                                 STGM_WRITE 0x00000001L
        ///                                 STGM_READWRITE 0x00000002L
        /// Sharing                         STGM_SHARE_DENY_NONE 0x00000040L
        ///                                 STGM_SHARE_DENY_READ 0x00000030L
        ///                                 STGM_SHARE_DENY_WRITE 0x00000020L
        ///                                 STGM_SHARE_EXCLUSIVE 0x00000010L
        ///                                 STGM_PRIORITY 0x00040000L
        /// Creation                        STGM_CREATE 0x00001000L
        ///                                 STGM_CONVERT 0x00020000L
        ///                                 STGM_FAILIFTHERE 0x00000000L
        /// Transactioning                  STGM_DIRECT 0x00000000L
        ///                                 STGM_TRANSACTED 0x00010000L
        /// Transactioning Performance      STGM_NOSCRATCH 0x00100000L
        ///                                 STGM_NOSNAPSHOT 0x00200000L
        /// Direct SWMR and Simple          STGM_SIMPLE 0x08000000L
        ///                                 STGM_DIRECT_SWMR 0x00400000L
        /// Delete On Release               STGM_DELETEONRELEASE 0x04000000L
        /// 说明：
        /// 你可以组合这些标志，但只能从每一个组里面选择一个标志。
        /// 延时模式 (Transacted Mode)
        /// 当 STGM_DIRECT 被指定的时候，只有下面访问和共享组中的一种组合可以使用。
        /// STGM_READ | STGM_SHARE_DENY_WRITE
        /// STGM_READWRITE | STGM_SHARE_EXCLUSIVE
        /// STGM_READ | STGM_PRIORITY
        /// 注意：在没有 STGM_TRANSACTED 的时候就是默认使用直接模式。
        /// 在 STGM_TRANSACTED 使用时，对象的修改直到进行提交操作前是不固定的。比如：延时模式的 storage 对象在 IStroage::Commit 调用之前不是不变的。对这样的 storage 作的修改在提交或者撤销方法之前调用 release 就丢失了。
        /// 当一个对象在这个模式创建或打开，对象的实现要记录原始数据和更新过的数据，让在必要时可以撤销修改。这就是为什么要把修改写入到 scratch 区域，或者创建副本，调用快照。
        /// 当用延时模式打开根 storage 对象， scratch 数据和快照的位置和行为可以通过设置 STGM_NOSCRATCH 和 STGM_NOSNAPSHOT 来控制优化性能。（如：可以通过 StgOpenStorageEx 来获得一个根 storage 对象， IStorage::OpenStorage 方法获得子 storage 对象）其实 scratch 数据和快照是存贮在临时文件，与 storage 分开。
        /// 这些标志的效果依赖于对访问根 storage 读者和 / 或写者的数量。
        /// 在单写者的情况下，一个延时模式的 storage 对象用写权限打开，没有其他人可以访问这个文件。亦即文件用 STGM_TRANSACTED ，权限组的 STGM_WRITE 或 STGM_READWRITE ，和共享组的 STGM_SHARE_EXCLUSIVE 等标志打开。对 storage 的修改被写入 scratch 区域。当修改被提交的时候，修改的部分被复制到原始 storage 。然而，如果没有修改，就没有必要做数据的传递。
        /// 在多写者情况下，一个延时模式下用于写操作的 storage ，和其他的写者一起工作。亦即使用 STGM_TRANSACTED, STGM_WRITE 或者 STGM_READWRITE, 和 STGM_SHARE_DENY_READ 标志的组合打开 storage 对象。如果使用 STGM_SHARE_DENY_NONE ，那么就成了“多写者，多读者”的情况。这时，在文件打开时会对原始数据进行快照。即使没有对 storage 修改或没有被其他写者同时打开，在打开过程中数据传递仍然有必要。使用 STGM_SHARE_DENY_WRITE 或 STGM_SHARE_EXCLUSIVE 可以提高打开文件的速度。关于在多写者时修改的提交更多信息请参考 IStorage::Commit 。
        /// 在“单写者，多读者”情况下，延时写的 storage 对象被多个读者共享。写者用 STGM_TRANSACTED, STGM_READWRITE 或者 STGM_WRITE, 和 STGM_SHARE_DENY_WRITE 标志打开。读者用 STGM_TRANSACTED, STGM_READ, 和 STGM_SHARE_DENY_NONE. 标志打开。写者用 scratch 区域存贮没有提交的修改。在上面的情况中，读者受到创建快照的影响打开速度增加。
        /// 一般， scratch 区域是一个临时文件，与原始数据分割开。当没有提交的修改提交回原始数据是，数据从临时文件转出。为了避免数据传递，可以使用 STGM_NOSCRATCH 标志。当这个标志被指定以后， storage 对象文件的部分区域用于 scratch 区域，不是用临时文件。因为只需要少量的数据传递所以提交修改的速度大幅度提高。这个做的缺点是存贮文件的大小会比其他方式要大的多。因为要足够大才能存下原始数据和 scratch 区域。为了混合数据和移除不必要的书，用延时模式重新打开根 storage ，不设置 STGM_NOSCRATCH 标志，接着调用带 STGC_CONSOLIDATE 参数的 IStorage::Commit 方法。
        /// 快照区域和 scratch 区域一样一般是临时文件，也会受 STMG 标志的影响。设置 STGM_NOSNAPSHOT ，独立的快照文件不会被创建，即使每个对象有一个写者或多个写者都不会修改原始数据。当提交修改的时候，他们被追加到文件中，但是原始数据保持不变。这种模式可以提高效率，因为通过放弃在打开时创建快照的要求而减少了运行时间。然而这个模式也许会导致非常大的 storage 文件，因为文件中的数据重来没有被覆盖过。在没有快照模式对文件的大小是没有限制的。
        /// 直接的单写者、多读者模式（ Direct Single-Writer, Multiple-Reader Mode ）
        /// 如前面描述，一个 storage 对象有一个单独的写者、多个读者是可能的，如果那个对象是工作在延时模式。也可以通过设置 STGM_DIRECT_SWMR 标志来得到单写者、多读者模式。
        /// 设置 STGM_DIRECT_SWMR 之后，调用者可以在其他调用者以只读的权限打开文件的同时，用读 / 写权限打开同一个文件。用本标志和 STGM_TRANSACTED 标志是无效的。在本模式下，写者用下面的组合打开对象：
        /// STGM_DIRECT_SWMR | STGM_READWRITE | STGM_SHARE_DENYWRITE
        /// 每个读者用下面的标志打开对象：
        /// STGM_DIRECT_SWMR | STGM_READ | STGM_SHARE_DENY_NONE
        /// 在本模式下，修改 storage 对象，写者必须获得对象独占的权限。只有当所有读者关闭文件之后才有可能。读者通过 IDirectWriterLock 接口去获得独占权限。
        /// 简单模式（ Simple Mode ）
        /// 简单模式在完成保存操作时非常有用，虽然可以获得效率但也有下面的限制：
        /// 不支持子 storage
        /// 从该模式下的对象获得的 storage 对象和 stream 对象不能被封装
        /// 每个 stream 都有一个最小大小。如果在 stream 释放的时候写入比最小值还少的字节数，这个 stream 被扩展到最小值。例如：对某一个 IStream 实现来说最小值是 4KB ，一个 stream 只有 1KB ，在 stream 释放的时候，就被自动扩展到 4KB 。接下来打开或者调用 IStream::Stat 都会显示 4KB 的大小。
        /// 不是 IStorage 和 IStream 的所有方法都被实现。获得更多，请看 IStorage – 复合文件的实现， IStream- 复合文件的实现。
        /// 封送处理（ Marshaling ） 是打包，解包，使用 RPC 穿透线程或者进程边界进行接口方法参数传递的过程。获得更多信息，请看封送详解和接口封送。
        /// 当一个 storage 对象是通过简单模式创建：
        /// stream 元素可以被创建，但不能被打开。
        /// 当一个 stream 元素通过 IStorage::CreateStream 创建，只有当这个 stream 对象被释放之后才能创建另一个对象。
        /// 当所有的 stream 被写入之后，调用 IStorage::Commit 完成修改。
        /// 如果一个 storage 对象是用简单模式打开而获得：
        /// 一次只能打开一个 stream.
        /// 不能调用 IStream::SetSize 或者在 stream 结束之外搜索和写入来设置 stream 的大小。然而，如果所有的 stream 都小于大小的最小值，可以用 stream 去扩展到最小值，即使没有数据。设置 stream 的大小使用 IStream::Stat 方法。
        /// 注意：如果一个 storage 元素被一个不是简单模式的 storage 对象修改，那么无法再以简单模式打开这个 storage 元素。
        /// </summary>
        [Flags]
        public enum STGM_FLAGS
        {
            /// <summary>
            /// 表示对象只能读不能修改
            /// 例如：如果一个 stream 用 STGM_READ 打开， ISequentialStream::Read 方法可以调用，
            /// 但是 ISequentialStream::Write 方法不可以，
            /// 同样，如果一个 stream 用 STGM_READ 打开， IStorage::OpenStream 、 IStorage::OpenStrorage 方法可以调用，
            /// 但是 IStorage::CreateStream 、 IStorage::CreateStorage 方法不可以用
            /// </summary>
            STGM_READ = 0x00000000,
            /// <summary>
            /// 允许你保存对对象的修改，但是不允许访问它的数据。
            /// 建议在实现 IPropertyStorage 和 IPropertySetStorage 接口的时候不要支持只写模式。
            /// </summary>
            STGM_WRITE = 0x00000001,
            /// <summary>
            /// 允许对对象数据的访问和修改。
            /// 例如：如果一个 stream 是以这种模式打开，就可以同时使用 IStream::Read 和 IStream::Write 方法。
            /// 要注意这个常量不是 STGM_WRITE 和 STGM_READ 常量的简单和运算。
            /// </summary>
            STGM_READWRITE = 0x00000002,
            /// <summary>
            /// 表示接下来打开的对象不拒绝读写方法。
            /// 如果这个组中没有其他的常量被使用时，这是默认的设置。
            /// </summary>
            STGM_SHARE_DENY_NONE = 0x00000040,
            /// <summary>
            /// 阻止其他方法用 STGM_READ 模式打开对象。
            /// 它典型的使用是在根 storage 对象。
            /// </summary>
            STGM_SHARE_DENY_READ = 0x00000030,
            /// <summary>
            /// 阻止其他方法用 STGM_WRITE 模式和 STGM_READWRITE 打开对象。
            /// 在延时模式， STGM_SHARE_DENY_WRITE 或则 STGM_SHARE_EXCLUSIVE 能够提升效率，因为他们不要求快照。
            /// 获得更多延时（ transactioning ）的信息，请参考提示部分。
            /// </summary>
            STGM_SHARE_DENY_WRITE = 0x00000020,
            /// <summary>
            /// 阻止其他方法用任何模式打开对象。
            /// 注意这个值不是 STGM_SHARE_DENY_READ 和 STGM_SHARE_DENY_WRITE 简单的位与运算。
            /// 在延时模式， STGM_SHARE_DENY_WRITE 或则 STGM_SHARE_EXCLUSIVE 能够提升效率，因为他们不要求快照。
            /// 获得更多延时（ transactioning ）的信息，请参考提示部分。
            /// </summary>
            STGM_SHARE_EXCLUSIVE = 0x00000010,
            /// <summary>
            /// 使用排斥模式去打开 storage 对象，获取最近提交的版本。
            /// 因此，当用特权模式（ priority mode ）其他用户不能提交对象的任何修改。
            /// 在复制操作的时候可以获得较好的效率，但是你阻止了其他用户修改对象。
            /// 要限制使用特权模式，必须把 STGM_DIRECT 、 STGM_READ 和 STGM_PRIORITY 一起使用，
            /// 并且不能使用 STGM_DELETEONRELEASE ，因为它只在创建根对象时使用，如 StgCreateStorageEx 使用。
            /// 当打开一个已经存在的根对象时无效，如 StgOpenStorageEx 使用。
            /// 当创建一个子元素的时候也是无效的，如 IStorage::OpenStorage 。
            /// </summary>
            STGM_PRIORITY = 0x00040000,
            /// <summary>
            /// 表示在新的对象替换已经存在的对象之前，之前的对象应该被移除。
            /// 仅在一个已经存在的对象已经成功删除时才能用这个标志创建新对象。
            /// 使用情况如下 :
            /// 创建一个磁盘上的对象，但已有同名的文件。
            /// 创建一个 storage 中的对象，但已有同名的子对象存在。
            /// 创建一个字节数组对象，但以后同名存在。
            /// </summary>
            STGM_CREATE = 0x00001000,
            /// <summary>
            /// 为了保护在名字为 ”Contents” 的 stream 中已有的数据而创建一个新的对象。
            /// 有的时候，一个 storage 或者字节数组中的久数据被格式化到一个 stream ，
            /// 不管已经存在的文件或者字节数组当前是否包含一个 storage 对象。
            /// 这个标志只能在创建一个根 storage 对象时使用。
            /// 它不能使用在 storage 对象，如 IStorage::CreateStream 。
            /// 在同时使用 STGM_DELETEONRELEASE 和本标志时，本标志变无效。
            /// </summary>
            STGM_CONVERT = 0x00020000,
            /// <summary>
            /// 如果已经有同名对象存在时创建操作失败。
            /// 此时 STG_E_FILEALREADYEXISTS 会被返回。
            /// 这个是创建模式（ creation mode ）的默认模式。
            /// </summary>
            STGM_FAILIFTHERE = 0x00000000,
            /// <summary>
            /// 表示对 storage 和 stream 元素的每一次修改都被立即写入。
            /// 这是 Transactioning 组的默认设置。
            /// </summary>
            STGM_DIRECT = 0x00000000,
            /// <summary>
            /// 在延时模式（ transacted mode ）下，修改被推迟，仅当显示的执行提交操作。
            /// 为了忽略作的修改，可以调用 IStream, IStorage, 和 IPropertyStorage 接口的 Revert 方法。 
            /// IStorage 的 COM 复合文件的实现不支持延时式的 stream ，亦即 stream 只能用直接模式打开，并不能撤销修改，但是延时模式的 storage 是支持的。 
            /// IPropertySetStorage 接口在复合文件、普通文件亦即 NTFS 文件系统的实现中同样不支持延时处理，简单属性设置是因为这些属性保存在 stream 对象中。
            /// 非简单属性设置的处理，能通过设置 IPropertySetStorage::Create 方法的 grfFlags 参数为 PROPSETFLAG_NONSIMPLE 常量来创建。
            /// </summary>
            STGM_TRANSACTED = 0x00010000,
            /// <summary>
            /// 在本模式下，通常使用一个临时 scratch 文件来保存修改，直到提交方法被调用。
            /// 使用本标志，可以允许使用原始文件不经常使用的部分作为工作区来代替创建新的文件。
            /// 这样不会影响原始文件的数据，有时候还可以提升效率。
            /// 本标志必须和 STGM_TRANSACTED 一起使用，并只能用于根 storage 。更多信息请参考说明部分。
            /// </summary>
            STGM_NOSCRATCH = 0x00100000,
            /// <summary>
            /// 在打开一个用标志 STGM_TRANSACTED 而没有 STGM_SHARE_EXCLUSIVE 和 STGM_SHARE_DENY_WRITE 的时候使用本标志。
            /// 这时，指定本标志阻止系统提供的实现创建文件的快照。反而，对文件的修改被写入文件的尾部。
            /// 除非在提交时执行合并操作，否则未使用的空间不会被收回，只有一个对文件的写者。
            /// 当文件在本模式打开，另一个打开操作在没有被指定 STGM_NOSNAPSHOT 标志时无法完成。
            /// 这个标志可能只用于根 storage 。更多信息请参考说明部分。
            /// </summary>
            STGM_NOSNAPSHOT = 0x00200000,
            /// <summary>
            /// 提供一种有限制的复合文件的快速实现，但经常使用。更多信息请参考说明部分。
            /// </summary>
            STGM_SIMPLE = 0x08000000,
            /// <summary>
            /// 支持单写者多读者的直接模式文件操作。更多信息请参考说明部分。
            /// </summary>
            STGM_DIRECT_SWMR = 0x00400000,
            /// <summary>
            /// 表示底层文件在根 storage 被释放的时候被自动删除。
            /// 这个方法对于创建临时文件非常有用。这个文件只用于创建根对象，如 StgCreateStorageEx 。
            /// 当打开一个根对象或者创建和打开一个子元素的时候，这个标志无效，如 StgOpenStorageEx ，当然同时使用 STGM_CONVERT 的时候也是无效的。
            /// </summary>
            STGM_DELETEONRELEASE = 0x04000000
        }
        /// <summary>
        /// 指示IShellLinkW::GetPath将返回一个路径字符串
        /// </summary>
        [Flags]
        public enum SLGP_FLAGS
        {
            /// <summary>
            /// 快捷方式路径
            /// </summary>
            SLGP_SHORTPATH = 0x1,
            /// <summary>
            /// 通用命名快捷方式名称
            /// </summary>
            SLGP_UNCPRIORITY = 0x2,
            /// <summary>
            /// 快捷方式指向路径名称
            /// </summary>
            SLGP_RAWPATH = 0x4
        }
        /// <summary>
        /// 在用findfirst()和findnext()函数去查找磁盘文件时经常使用的一个数据结构WIN32_FIND_DATA的成员变量里包含了以上所有的文件属性，因此可以通过这个结构作为获取和更改文件属性的手段
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct WIN32_FIND_DATAW
        {
            /// <summary>
            /// 属性
            /// </summary>
            public int DwFileAttributes;
            /// <summary>
            /// 创建时间
            /// </summary>
            public FILETIME FtCreationTime;
            /// <summary>
            /// 最后访问时间
            /// </summary>
            public FILETIME FtLastAccessTime;
            /// <summary>
            /// 最后写入时间
            /// </summary>
            public FILETIME FtLastWriteTime;
            /// <summary>
            /// 占用空间
            /// </summary>
            public int NFileSizeHigh;
            /// <summary>
            /// 文件大小
            /// </summary>
            public int NFileSizeLow;
            /// <summary>
            /// 保留0
            /// </summary>
            public int DwReserved0;
            /// <summary>
            /// 保留1
            /// </summary>
            public int DwReserved1;
            /// <summary>
            /// 文件名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string CFileName;
            /// <summary>
            /// 扩展文件名
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string CAlternateFileName;
        }
        /// <summary>
        /// 快捷方式文件
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        public interface IPersistFile
        {
            /// <summary>
            /// 文件标记
            /// </summary>
            /// <param name="pClassID"></param>
            void GetClassID(out Guid pClassID);
            /// <summary>
            /// 是脏
            /// </summary>
            /// <returns></returns>
            [PreserveSig]
            int IsDirty();
            /// <summary>
            /// 加载
            /// </summary>
            /// <param name="pszFileName"></param>
            /// <param name="dwMode"></param>
            void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);
            /// <summary>
            /// 保存
            /// </summary>
            /// <param name="pszFileName"></param>
            /// <param name="fRemember"></param>
            void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
            /// <summary>
            /// 保存完整
            /// </summary>
            /// <param name="pszFileName"></param>
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            /// <summary>
            /// 获取当前快捷方式文件
            /// </summary>
            /// <param name="ppszFileName"></param>
            void GetCurFile(out IntPtr ppszFileName);
        }
        /// <summary>
        /// 快捷方式接口定义
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214EE-0000-0000-C000-000000000046")]
        public interface IShellLink
        {
            /// <summary>
            /// 获取路径
            /// </summary>
            /// <param name="pszFile"></param>
            /// <param name="cchMaxPath"></param>
            /// <param name="pfd"></param>
            /// <param name="fFlags"></param>
            void GetPath([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
            /// <summary>
            /// 获取标识列表
            /// </summary>
            /// <param name="ppidl"></param>
            void GetIDList(out IntPtr ppidl);
            /// <summary>
            /// 设置标记列表
            /// </summary>
            /// <param name="pidl"></param>
            void SetIDList(IntPtr pidl);
            /// <summary>
            /// 获取描述
            /// </summary>
            /// <param name="pszName"></param>
            /// <param name="cchMaxName"></param>
            void GetDescription([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName, int cchMaxName);
            /// <summary>
            /// 设置描述
            /// </summary>
            /// <param name="pszName"></param>
            void SetDescription([MarshalAs(UnmanagedType.LPStr)] string pszName);
            /// <summary>
            /// 获取工作目录
            /// </summary>
            /// <param name="pszDir"></param>
            /// <param name="cchMaxPath"></param>
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir, int cchMaxPath);
            /// <summary>
            /// 设置工作目录
            /// </summary>
            /// <param name="pszDir"></param>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string pszDir);
            /// <summary>
            /// 获取参数
            /// </summary>
            /// <param name="pszArgs"></param>
            /// <param name="cchMaxPath"></param>
            void GetArguments([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs, int cchMaxPath);
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="pszArgs"></param>
            void SetArguments([MarshalAs(UnmanagedType.LPStr)] string pszArgs);
            /// <summary>
            /// 获取热键
            /// </summary>
            /// <param name="pwHotkey"></param>
            void GetHotkey(out short pwHotkey);
            /// <summary>
            /// 设置热键
            /// </summary>
            /// <param name="wHotkey"></param>
            void SetHotkey(short wHotkey);
            /// <summary>
            /// 或显示命令
            /// </summary>
            /// <param name="piShowCmd"></param>
            /// <returns></returns>
            [PreserveSig]
            Int32 GetShowCmd(out int piShowCmd);
            /// <summary>
            /// 设置显示命令
            /// </summary>
            /// <param name="iShowCmd"></param>
            void SetShowCmd(int iShowCmd);
            /// <summary>
            /// 获取图标位置
            /// </summary>
            /// <param name="pszIconPath"></param>
            /// <param name="cchIconPath"></param>
            /// <param name="piIcon"></param>
            /// <returns></returns>
            [PreserveSig]
            Int32 GetIconLocation([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            /// <summary>
            /// 设置图标位置
            /// </summary>
            /// <param name="pszIconPath"></param>
            /// <param name="iIcon"></param>
            void SetIconLocation([MarshalAs(UnmanagedType.LPStr)] string pszIconPath, int iIcon);
            /// <summary>
            /// 设置相对路径
            /// </summary>
            /// <param name="pszPathRel"></param>
            /// <param name="dwReserved"></param>
            void SetRelativePath([MarshalAs(UnmanagedType.LPStr)] string pszPathRel, int dwReserved);
            /// <summary>
            /// 尝试找到快捷方式的目标，即使它已经被移动或重命名
            /// </summary>
            /// <param name = "hwnd">窗口的句柄，使用它作为对话框的父级。如果需要在解析快捷方式时提示用户提供更多信息，则会显示对话框。</param>
            /// <param name = "fFlags">控制解析过程的标志</param>
            void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
            /// <summary>
            /// 设置目标路径
            /// </summary>
            /// <param name="pszFile"></param>
            void SetPath([MarshalAs(UnmanagedType.LPStr)] string pszFile);
        }
        /// <summary>
        /// 快捷方式数据列表
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("45e2b4ae-b1c3-11d0-b92f-00a0c90312e1")]
        public interface IShellLinkDataList
        {
            /// <summary>
            /// 添加数据库
            /// </summary>
            /// <param name="pDataBlock"></param>
            /// <returns></returns>
            [PreserveSig]
            Int32 AddDataBlock(IntPtr pDataBlock);
            /// <summary>
            /// 复制数据库
            /// </summary>
            /// <param name="dwSig"></param>
            /// <param name="ppDataBlock"></param>
            /// <returns></returns>
            [PreserveSig]
            Int32 CopyDataBlock(UInt32 dwSig, out IntPtr ppDataBlock);
            /// <summary>
            /// 移除数据块
            /// </summary>
            /// <param name="dwSig"></param>
            /// <returns></returns>
            [PreserveSig]
            Int32 RemoveDataBlock(UInt32 dwSig);
            /// <summary>
            /// 获取标记
            /// <see cref="DATA_FLAGS"/>
            /// </summary>
            /// <param name="pdwFlags"></param>
            void GetFlags(out UInt32 pdwFlags);
            /// <summary>
            /// 设置标记
            /// <see cref="DATA_FLAGS"/>
            /// </summary>
            /// <param name="dwFlags"></param>
            void SetFlags(UInt32 dwFlags);
        }
        /// <summary>
        /// 实现了快捷方式接口
        /// </summary>
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        public class ShellLinkCore { }
        #endregion
    }
    /// <summary>
    /// 快捷方式完整接口
    /// </summary>
    public interface IShortcutCore
    {
        #region // IPersistFile
        /// <summary>
        /// 获取类标记
        /// </summary>
        /// <param name="pClassID"></param>
        void GetClassID(out Guid pClassID);
        /// <summary>
        /// 是脏快捷方式
        /// </summary>
        /// <returns></returns>
        int IsDirty();
        /// <summary>
        /// 加载快捷方式
        /// </summary>
        /// <param name="pszFileName"></param>
        /// <param name="dwMode"></param>
        void Load(string pszFileName, ShellLink.STGM_FLAGS dwMode);
        /// <summary>
        /// 保存快捷方式
        /// </summary>
        /// <param name="pszFileName"></param>
        /// <param name="fRemember"></param>
        void Save(string pszFileName, bool fRemember);
        /// <summary>
        /// 保存完整
        /// </summary>
        /// <param name="pszFileName"></param>
        void SaveCompleted(string pszFileName);
        /// <summary>
        /// 获取当前指向文件
        /// </summary>
        /// <param name="ppszFileName"></param>
        void GetCurFile(out IntPtr ppszFileName);
        #endregion IPersistFile
        #region // IShellLink
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="pszFile"></param>
        /// <param name="cchMaxPath"></param>
        /// <param name="pfd"></param>
        /// <param name="fFlags"></param>
        void GetPath(StringBuilder pszFile, int cchMaxPath, out ShellLink.WIN32_FIND_DATAW pfd, ShellLink.SLGP_FLAGS fFlags);
        /// <summary>
        /// 获取标识列表
        /// </summary>
        /// <param name="ppidl"></param>
        void GetIDList(out IntPtr ppidl);
        /// <summary>
        /// 设置标识列表
        /// </summary>
        /// <param name="pidl"></param>
        void SetIDList(IntPtr pidl);
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="pszName"></param>
        /// <param name="cchMaxName"></param>
        void GetDescription(StringBuilder pszName, int cchMaxName);
        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="pszName"></param>
        void SetDescription(string pszName);
        /// <summary>
        /// 获取工作目录
        /// </summary>
        /// <param name="pszDir"></param>
        /// <param name="cchMaxPath"></param>
        void GetWorkingDirectory(StringBuilder pszDir, int cchMaxPath);
        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="pszDir"></param>
        void SetWorkingDirectory(string pszDir);
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="pszArgs"></param>
        /// <param name="cchMaxPath"></param>
        void GetArguments(StringBuilder pszArgs, int cchMaxPath);
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pszArgs"></param>
        void SetArguments(string pszArgs);
        /// <summary>
        /// 获取热键
        /// </summary>
        /// <param name="pwHotkey"></param>
        void GetHotkey(out short pwHotkey);
        /// <summary>
        /// 设置热键
        /// </summary>
        /// <param name="wHotkey"></param>
        void SetHotkey(short wHotkey);
        /// <summary>
        /// 获取显示命令
        /// </summary>
        /// <param name="piShowCmd"></param>
        /// <returns></returns>
        Int32 GetShowCmd(out ShellLink.ShowWindowCommand piShowCmd);
        /// <summary>
        /// 设置显示命令
        /// </summary>
        /// <param name="iShowCmd"></param>
        void SetShowCmd(ShellLink.ShowWindowCommand iShowCmd);
        /// <summary>
        /// 获取图标位置
        /// </summary>
        /// <param name="pszIconPath"></param>
        /// <param name="cchIconPath"></param>
        /// <param name="piIcon"></param>
        /// <returns></returns>
        Int32 GetIconLocation(StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="pszIconPath"></param>
        /// <param name="iIcon"></param>
        void SetIconLocation(string pszIconPath, int iIcon);
        /// <summary>
        /// 设置相对路径
        /// </summary>
        /// <param name="pszPathRel"></param>
        /// <param name="dwReserved"></param>
        void SetRelativePath(string pszPathRel, int dwReserved);
        /// <summary>
        /// 尝试找到快捷方式的目标，即使它已经被移动或重命名
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="fFlags"></param>
        void Resolve(IntPtr hwnd, ShellLink.SLR_FLAGS fFlags);
        /// <summary>
        /// 设置目标路径
        /// </summary>
        /// <param name="pszFile"></param>
        void SetPath(string pszFile);
        #endregion IShellLinkf
        #region // IShellLinkDataList
        /// <summary>
        /// 添加数据块
        /// </summary>
        /// <param name="pDataBlock"></param>
        /// <returns></returns>
        Int32 AddDataBlock(IntPtr pDataBlock);
        /// <summary>
        /// 复制数据块
        /// </summary>
        /// <param name="dwSig"></param>
        /// <param name="ppDataBlock"></param>
        /// <returns></returns>
        Int32 CopyDataBlock(UInt32 dwSig, out IntPtr ppDataBlock);
        /// <summary>
        /// 移除数据块
        /// </summary>
        /// <param name="dwSig"></param>
        /// <returns></returns>
        Int32 RemoveDataBlock(UInt32 dwSig);
        /// <summary>
        /// 获取数据标记
        /// </summary>
        /// <param name="pdwFlags"></param>
        void GetFlags(out ShellLink.DATA_FLAGS pdwFlags);
        /// <summary>
        /// 设置数据标记
        /// </summary>
        /// <param name="dwFlags"></param>
        void SetFlags(ShellLink.DATA_FLAGS dwFlags);
        #endregion IShellLinkDataList
    }
}
