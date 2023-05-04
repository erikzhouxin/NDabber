using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Impeller
{
    /// <summary>
    /// 重要的Windows NT内核级文件。描述了windows本地NTAPI的接口。
    /// 当Windows启动时，ntdll.dll就驻留在内存中特定的写保护区域，使别的程序无法占用这个内存区域。
    /// </summary>
    public class NTDLL
    {
        /// <summary>
        /// ntdll.dll文件
        /// </summary>
        public const String DllFileName = "ntdll.dll";
        /// <summary>
        /// 内存复制
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport(DllFileName, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Memcpy(IntPtr dest, IntPtr source, int length);
    }
}
