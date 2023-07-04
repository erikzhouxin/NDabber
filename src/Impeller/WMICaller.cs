using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Impeller
{
    /// <summary>
    /// WMI调用者
    /// </summary>
    public partial class WMICaller
    {
        /// <summary>
        /// Win32类型
        /// </summary>
        public enum Win32Type
        {
            /// <summary>
            /// Win32_BIOS WMI 类表示计算机系统的基本输入/输出服务的属性， (计算机上安装的 BIOS) 。
            /// https://learn.microsoft.com/zh-cn/windows/win32/cimwin32prov/win32-bios
            /// </summary>
            Win32_BIOS,
            /// <summary>
            /// Win32_ComputerSystemProduct WMI 类表示产品。 这包括在此计算机系统上使用的软件和硬件。
            /// https://learn.microsoft.com/zh-cn/windows/win32/cimwin32prov/win32-computersystemproduct
            /// </summary>
            Win32_ComputerSystemProduct,
            /// <summary>
            /// Win32_ComputerSystemProcessor关联 WMI 类将计算机系统和在该系统上运行的处理器相关联。
            /// https://learn.microsoft.com/zh-cn/windows/win32/cimwin32prov/win32-computersystemprocessor
            /// </summary>
            Win32_ComputerSystemProcessor,
            /// <summary>
            /// Win32_NetworkAdapterConfiguration WMI 类表示网络适配器的属性和行为。 
            /// 此类包括支持管理独立于网络适配器的 TCP/IP 协议的额外属性和方法。
            /// https://learn.microsoft.com/zh-cn/windows/win32/cimwin32prov/win32-networkadapterconfiguration
            /// </summary>
            Win32_NetworkAdapterConfiguration,
            /// <summary>
            /// Win32_DiskDrive WMI 类表示运行Windows操作系统的计算机看到的物理磁盘驱动器。
            /// https://learn.microsoft.com/zh-cn/windows/win32/cimwin32prov/win32-diskdrive
            /// </summary>
            Win32_DiskDrive,
            /// <summary>
            /// Win32_Processor WMI 类表示可以解释Windows操作系统上运行的计算机上的指令序列的设备。
            /// https://learn.microsoft.com/zh-cn/windows/win32/cimwin32prov/win32-processor
            /// </summary>
            Win32_Processor,
        }
        #region // 基本输入输出 Win32_BIOS
        #endregion 基本输入输出 Win32_BIOS
        #region // 网络适配器配置 Win32_NetworkAdapterConfiguration
        /// <summary>
        /// 查询网络配置项
        /// </summary>
        public static String Search_Win32_NetworkAdapterConfiguration => $"SELECT * FROM {nameof(Win32Type.Win32_NetworkAdapterConfiguration)}";
        /// <summary>
        /// 列名 IPEnabled
        /// </summary>
        public static String Column_IPEnabled => "IPEnabled";
        /// <summary>
        /// 列名 MacAddress
        /// </summary>
        public static String Column_MacAddress => "MacAddress";
        #endregion 网络适配器配置 Win32_NetworkAdapterConfiguration
        #region // 硬盘驱动器 Win32_DiskDrive
        /// <summary>
        /// 查询硬盘驱动器
        /// </summary>
        public static String Search_Win32_DiskDrive => $"SELECT * FROM {nameof(Win32Type.Win32_DiskDrive)}";
        /// <summary>
        /// 列名 Model
        /// </summary>
        public static String Column_Model => "Model";
        /// <summary>
        /// 列名 SerialNumber
        /// </summary>
        public static String Column_SerialNumber => "SerialNumber";
        #endregion 硬盘驱动器 Win32_DiskDrive
        #region // 处理器指令 Win32_Processor
        /// <summary>
        /// 查询处理器指令
        /// </summary>
        public static String Search_Win32_Processor => $"SELECT * FROM {nameof(Win32Type.Win32_Processor)}";
        /// <summary>
        /// 列名 ProcessorId
        /// </summary>
        public static String Column_ProcessorId => "ProcessorId";
        #endregion 处理器指令 Win32_Processor
        #region // 操作系统处理器 Win32_ComputerSystemProcessor
        /// <summary>
        /// 查询操作系统处理器
        /// </summary>
        public static String Search_Win32_ComputerSystemProcessor => $"SELECT * FROM {nameof(Win32Type.Win32_ComputerSystemProcessor)}";
        #endregion 操作系统处理器 Win32_ComputerSystemProcessor
        #region // 操作系统处理器 Win32_ComputerSystemProduct
        /// <summary>
        /// 查询操作系统处理器
        /// </summary>
        public static String Search_Win32_ComputerSystemProduct => $"SELECT * FROM {nameof(Win32Type.Win32_ComputerSystemProduct)}";
        /// <summary>
        /// 查询操作系统处理器
        /// </summary>
        public static String Column_UUID => "UUID";
        #endregion 操作系统处理器 Win32_ComputerSystemProduct
    }
}
