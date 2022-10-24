using System;
using System.Collections.Generic;
using System.Data.Impeller;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// DLLWrapper
    /// </summary>
    public class DllWrapper
    {
        ///<summary>
        /// 通过非托管函数名转换为对应的委托 , by jingzhongrong
        ///</summary>
        ///<param name="dllModule"> 通过 LoadLibrary 获得的 DLL 句柄 </param>
        ///<param name="functionName"> 非托管函数名 </param>
        ///<param name="t"> 对应的委托类型 </param>
        ///<returns> 委托实例，可强制转换为适当的委托类型 </returns>
        public static Delegate GetFunctionAddress(int dllModule, string functionName, Type t)
        {
            int address = KERNEL32.GetProcAddress(dllModule, functionName);
            if (address == 0)
                return null;
            else
                return Marshal.GetDelegateForFunctionPointer(new IntPtr(address), t);
        }

        ///<summary>
        /// 将表示函数地址的 intPtr 实例转换成对应的委托
        ///</summary>
        public static Delegate GetDelegateFromIntPtr(IntPtr address, Type t)
        {
            if (address == IntPtr.Zero)
                return null;
            else
                return Marshal.GetDelegateForFunctionPointer(address, t);
        }

        ///<summary>
        /// 将表示函数地址的 int  转换成对应的委托
        ///</summary>
        public static Delegate GetDelegateFromIntPtr(int address, Type t)
        {
            if (address == 0)
                return null;
            else
                return Marshal.GetDelegateForFunctionPointer(new IntPtr(address), t);
        }

        /// <summary>
        /// 加载sdk
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        public static int LoadSDK(string lpFileName)
        {
            if (File.Exists(lpFileName))
            {
                var hReservedNull = IntPtr.Zero;
                var dwFlags = KERNEL32.LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH;

                var result = KERNEL32.LoadLibraryEx(lpFileName, hReservedNull, dwFlags);

                var errCode = KERNEL32.GetLastError();
                Console.WriteLine($"LoadSDK Result:{result}, ErrorCode: {errCode}");

                return result;
            }
            return 0;
        }

        /// <summary>
        /// 释放sdk
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static int ReleaseSDK(int handle)
        {
            try
            {
                if (handle > 0)
                {
                    Console.WriteLine($"FreeLibrary handle：{handle}");
                    var result = KERNEL32.FreeLibrary(handle);
                    var errCode = KERNEL32.GetLastError();
                    Console.WriteLine($"FreeLibrary Result:{result}, ErrorCode: {errCode}");
                    return 0;
                }
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
