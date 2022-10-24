using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Impeller
{
    /// <summary>
    /// 内核级文件
    /// kernel32.dll是Windows中非常重要的32位动态链接库文件，属于内核级文件。
    /// 它控制着系统的内存管理、数据的输入输出操作和中断处理，
    /// 当Windows启动时，kernel32.dll就驻留在内存中特定的写保护区域，使别的程序无法占用这个内存区域。
    /// </summary>
    public class KERNEL32
    {
        #region // 文件信息
        /// <summary>
        /// 用于获取盘信息的api
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <param name="sectorsPerCluster"></param>
        /// <param name="bytesPerSector"></param>
        /// <param name="numberOfFreeClusters"></param>
        /// <param name="totalNumbeOfClusters"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName, ref uint sectorsPerCluster, ref uint bytesPerSector, ref uint numberOfFreeClusters, ref uint totalNumbeOfClusters);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFile(string name);
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="lpExistingFileName"></param>
        /// <param name="lpNewFileName"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileEx")]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        #endregion
        #region // 错误信息
        /// <summary>
        /// 获取最后的错误
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        #endregion
        #region // 动态加载
        /// <summary>
        /// 加载库
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="h"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadLibrary(string lpFileName, int h, int flags);
        /// <summary>
        /// 加载库
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="hReservedNull"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryEx", SetLastError = true)]
        public static extern int LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);
        /// <summary>
        /// 获取处理地址
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="funcname"></param>
        /// <returns></returns>
        [DllImport("Kernel32", EntryPoint = "GetProcAddress", SetLastError = true)]
        public static extern int GetProcAddress(int handle, string funcname);
        /// <summary>
        /// 获取处理地址
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="lProcName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lProcName);
        /// <summary>
        ///  释放库
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("Kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        public static extern int FreeLibrary(int handle);
        /// <summary>
        /// 释放库
        /// </summary>
        /// <param name="hModule"></param>
        /// <returns></returns>

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool FreeLibrary(IntPtr hModule);
        #endregion
        #region // 公开类
        /// <summary>
        /// LoadLibraryFlags
        /// </summary>
        public enum LoadLibraryFlags : uint
        {
            /// <summary>
            /// DONT_RESOLVE_DLL_REFERENCES
            /// </summary>
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,

            /// <summary>
            /// LOAD_IGNORE_CODE_AUTHZ_LEVEL
            /// </summary>
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,

            /// <summary>
            /// LOAD_LIBRARY_AS_DATAFILE
            /// </summary>
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,

            /// <summary>
            /// LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE
            /// </summary>
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,

            /// <summary>
            /// LOAD_LIBRARY_AS_IMAGE_RESOURCE
            /// </summary>
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,

            /// <summary>
            /// LOAD_LIBRARY_SEARCH_APPLICATION_DIR
            /// </summary>
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,

            /// <summary>
            /// LOAD_LIBRARY_SEARCH_DEFAULT_DIRS
            /// </summary>
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,

            /// <summary>
            /// LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR
            /// </summary>
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,

            /// <summary>
            /// LOAD_LIBRARY_SEARCH_SYSTEM32
            /// </summary>
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,

            /// <summary>
            /// LOAD_LIBRARY_SEARCH_USER_DIRS
            /// </summary>
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,

            /// <summary>
            /// LOAD_WITH_ALTERED_SEARCH_PATH
            /// </summary>
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }
        /// <summary>
        /// 移动文件标识
        /// </summary>
        public enum MoveFileFlags
        {
            /// <summary>
            /// 替换
            /// </summary>
            MOVEFILE_REPLACE_EXISTING = 1,
            /// <summary>
            /// 复制
            /// </summary>
            MOVEFILE_COPY_ALLOWED = 2,
            /// <summary>
            /// 等到重启
            /// </summary>
            MOVEFILE_DELAY_UNTIL_REBOOT = 4,
            /// <summary>
            /// 通过写入
            /// </summary>
            MOVEFILE_WRITE_THROUGH = 8
        }
        #endregion
    }
}
