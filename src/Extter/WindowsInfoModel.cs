using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
using System.Data.Impeller;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    ///<summary>
    /// 结构。硬盘信息
    /// </summary>
    public struct DiskSizeInfo
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public string RootPathName;
        /// <summary>
        /// 每簇的扇区数
        /// </summary>
        public uint SectorsPerCluster;
        /// <summary>
        /// 每扇区字节
        /// </summary>
        public uint BytesPerSector;
        /// <summary>
        /// 可用簇
        /// </summary>
        public uint NumberOfFreeClusters;
        /// <summary>
        /// 总簇数
        /// </summary>
        public uint TotalNumberOfClusters;
        /// <summary>
        /// 簇字节数=每簇扇区数*每扇区字节数
        /// </summary>
        public long ClusterSize { get => BytesPerSector * SectorsPerCluster; }
    }
    /// <summary>
    /// 文件夹大小信息
    /// </summary>
    public class FolderSizeInfo
    {
        /// <summary>
        /// 文件计数
        /// </summary>
        public int FileCount { get; set; }
        /// <summary>
        /// 文件夹计数
        /// </summary>
        public int FolderCount { get; set; }
        /// <summary>
        /// 文件长度
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件夹长度
        /// </summary>
        public long FolderSize { get; set; }
    }
    /// <summary>
    /// Window快捷方式启动模型
    /// </summary>
    public class WindowShortcutInfoModel
    {
        /// <summary>
        /// 保存路径(必填)
        /// </summary>
        public virtual String SavePath { get; set; }
        /// <summary>
        /// 快捷方式名称(必填),不包括扩展名
        /// </summary>
        public virtual String SaveName { get; set; }
        /// <summary>
        /// 目标全路径(必填)
        /// </summary>
        public virtual String SourceName { get; set; }
        /// <summary>
        /// 快捷方式描述(选填)
        /// </summary>
        public virtual String Description { get; set; }
        /// <summary>
        /// 图标路径(选填)，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"
        /// </summary>
        public virtual String Icon { get; set; }
        /// <summary>
        /// 参数(选填)
        /// </summary>
        public virtual String Args { get; set; }
        /// <summary>
        /// 启动目录(选填)
        /// </summary>
        public virtual String WorkDir { get; set; }
    }
    /// <summary>
    /// 指定用于将目录路径检索到系统专用目录的枚举常数
    /// 文件夹。
    /// </summary>
    public enum WindowSpecialFolder
    {
        /// <summary>
        /// 逻辑桌面而不是物理文件系统位置。
        /// </summary>
        Desktop = 0,
        /// <summary>
        /// 包含用户程序组的目录。
        /// </summary>
        Programs = 2,
        /// <summary>
        /// “我的文件”文件夹。这个成员等价于System.Environment.SpecialFolder.Personal。
        /// </summary>
        MyDocuments = 5,
        /// <summary>
        /// 作为文档的公共存储库的目录。这个成员等价于System.Environment.SpecialFolder.MyDocuments。
        /// </summary>
        Personal = 5,
        /// <summary>
        /// 作为用户喜欢的项目的公共存储库的目录。
        /// </summary>
        Favorites = 6,
        /// <summary>
        /// 对应于用户启动程序组的目录。该系统
        /// 当用户登录或启动Windows时，启动这些程序。
        /// </summary>
        Startup = 7,
        /// <summary>
        /// 包含用户最近使用的文档的目录。
        /// </summary>
        Recent = 8,
        /// <summary>
        /// 包含“发送到”菜单项的目录。
        /// </summary>
        SendTo = 9,
        /// <summary>
        /// 包含开始菜单项的目录。
        /// </summary>
        StartMenu = 11,
        /// <summary>
        /// My Music文件夹。
        /// </summary>
        MyMusic = 13,
        /// <summary>
        /// 文件系统目录，用作存储所属视频的存储库
        /// 对用户来说。在。net框架4中添加。
        /// </summary>
        MyVideos = 14,
        /// <summary>
        /// 用于在桌面上物理存储文件对象的目录。不要混淆
        /// 这个目录包含desktop文件夹本身，这是一个虚拟文件夹。
        /// </summary>
        DesktopDirectory = 16,
        /// <summary>
        /// 我的电脑文件夹。传递到环境时。GetFolderPath方法,
        /// 枚举成员MyComputer总是返回空字符串("")，因为
        /// 没有为“我的计算机”文件夹定义路径。
        /// </summary>
        MyComputer = 17,
        /// <summary>
        /// 中可能存在的链接对象的文件系统目录我的网络放置虚拟文件夹。在。net框架4中添加。
        /// </summary>
        NetworkShortcuts = 19,
        /// <summary>
        /// 包含字体的虚拟文件夹。在。net框架4中添加。
        /// </summary>
        Fonts = 20,
        /// <summary>
        /// 作为文档模板的公共存储库的目录。
        /// </summary>
        Templates = 21,
        /// <summary>
        /// 包含出现的程序和文件夹的文件系统目录
        /// 在所有用户的开始菜单上。此特殊文件夹仅对Windows有效
        /// NT系统。在。net框架4中添加。
        /// </summary>
        CommonStartMenu = 22,
        /// <summary>
        /// 用于跨应用程序共享的组件的文件夹。这个特殊的文件夹
        /// 仅对Windows NT、Windows 2000和Windows XP系统有效。添加到
        /// . net框架4。
        /// </summary>
        CommonPrograms = 23,
        /// <summary>
        /// 包含出现在启动中的程序的文件系统目录
        /// 所有用户的文件夹。此特殊文件夹仅对Windows NT系统有效。
        /// 在。net框架4中添加。
        /// </summary>
        CommonStartup = 24,
        /// <summary>
        /// 中包含文件和文件夹的文件系统目录
        /// 所有用户的桌面。此特殊文件夹仅对Windows NT系统有效。
        /// 在。net框架4中添加。
        /// </summary>
        CommonDesktopDirectory = 25,
        /// <summary>
        /// 作为特定于应用程序的数据的公共存储库的目录
        /// 对于当前漫游用户。漫游用户可以在多台计算机上工作
        /// 在网络上。漫游用户的配置文件保存在网络上的服务器上
        /// 在用户登录时加载到系统上。
        /// </summary>
        ApplicationData = 26,
        /// <summary>
        /// 文件系统目录，包含可以存在于
        /// 打印机虚拟文件夹。在。net框架4中添加。
        /// </summary>
        PrinterShortcuts = 27,
        /// <summary>
        /// 作为特定于应用程序的数据的公共存储库的目录
        /// 由当前非漫游用户使用。
        /// </summary>
        LocalApplicationData = 28,
        /// <summary>
        /// 作为临时Internet文件的公共存储库的目录。
        /// </summary>
        InternetCache = 32,
        /// <summary>
        /// 用作Internet cookie公共存储库的目录。
        /// </summary>
        Cookies = 33,
        /// <summary>
        /// 作为因特网历史记录项的公共存储库的目录。
        /// </summary>
        History = 34,
        /// <summary>
        /// 作为特定于应用程序的数据的公共存储库的目录
        /// 所有用户都使用。
        /// </summary>
        CommonApplicationData = 35,
        /// <summary>
        /// Windows目录或SYSROOT。这对应于%windir%或%SYSTEMROOT%
        /// 环境变量。在。net框架4中添加。
        /// </summary>
        Windows = 36,
        /// <summary>
        /// 系统目录。
        /// </summary>
        System = 37,
        /// <summary>
        /// 程序文件目录。在非x86系统上，传入system . environment . specialfolder . programfiles
        /// 切换到System.Environment.GetFolderPath(System.Environment.SpecialFolder)方法
        /// 返回非x86程序的路径。获取x86程序文件目录
        /// 在非x86系统上，使用system . environment . specialfolder . programfilesx86
        /// 成员。
        /// </summary>
        ProgramFiles = 38,
        /// <summary>
        /// My Pictures文件夹。
        /// </summary>
        MyPictures = 39,
        /// <summary>
        /// 用户的个人资料文件夹。应用程序不应该在
        /// 这个水平;他们应该把数据放在System.Environment.SpecialFolder.ApplicationData所指的位置下。
        /// 在。net框架4中添加。
        /// </summary>
        UserProfile = 40,
        /// <summary>
        /// Windows系统文件夹。在。net框架4中添加。
        /// </summary>
        SystemX86 = 41,
        /// <summary>
        /// x86程序文件文件夹。在。net框架4中添加。
        /// </summary>
        ProgramFilesX86 = 42,
        /// <summary>
        /// 用于跨应用程序共享的组件的目录。为了得到
        /// x86通用程序文件目录在非x86系统上，使用System.Environment.SpecialFolder.Programfilesx86
        /// 成员。
        /// </summary>
        CommonProgramFiles = 43,
        /// <summary>
        /// 程序文件文件夹。在.Net Framework 4中添加。
        /// </summary>
        CommonProgramFilesX86 = 44,
        /// <summary>
        /// 包含所有模板的文件系统目录
        /// 用户。此特殊文件夹仅对Windows NT系统有效。在.Net Framework 4中添加。
        /// </summary>
        CommonTemplates = 45,
        /// <summary>
        /// 文件系统目录，包含所有用户共用的文档。
        /// 此特殊文件夹对Windows NT系统、Windows 95和Windows有效
        /// 98系统安装了Shfolder.dll。在.Net Framework 4中添加。
        /// </summary>
        CommonDocuments = 46,
        /// <summary>
        /// 包含所有用户的管理工具的文件系统目录
        /// 电脑。在.Net Framework 4中添加。
        /// </summary>
        CommonAdminTools = 47,
        /// <summary>
        /// 文件系统目录，用于存储个人的管理工具
        /// 用户。微软管理控制台(MMC)将自定义控制台保存到
        /// 这个目录，它将与用户一起漫游。在.Net Framework 4中添加。
        /// </summary>
        AdminTools = 48,
        /// <summary>
        /// 文件系统目录，通常用作音乐文件的存储库
        /// 对所有用户。在.Net Framework 4中添加。
        /// </summary>
        CommonMusic = 53,
        /// <summary>
        /// 文件系统目录，作为常见图像文件的存储库
        /// 对所有用户。在.Net Framework 4中添加。
        /// </summary>
        CommonPictures = 54,
        /// <summary>
        /// 文件系统目录，用于存储常见的视频文件
        /// 对所有用户。在.Net Framework 4中添加。
        /// </summary>
        CommonVideos = 55,
        /// <summary>
        /// 包含资源数据的文件系统目录。在.Net Framework 4中添加
        /// </summary>
        Resources = 56,
        /// <summary>
        /// 包含本地化资源数据的文件系统目录。在.Net Framework 4中添加
        /// </summary>
        LocalizedResources = 57,
        /// <summary>
        /// 这个值在Windows Vista中被识别是为了向后兼容，但是
        /// 特殊文件夹本身不再使用。在.Net Framework 4中添加。
        /// </summary>
        CommonOemLinks = 58,
        /// <summary>
        /// 文件系统目录，作为等待被保存的文件的暂存区
        /// 在.Net Framework 4中添加。
        /// </summary>
        CDBurning = 59
    }
}
