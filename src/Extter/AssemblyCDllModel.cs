using System;
using System.Collections.Generic;
using System.Data.Impeller;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 动态链接库程序集接口模型
    /// </summary>
    public interface IAssemblyCDllModel : IDisposable
    {
        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Delegate GetMethod(string procName, Type type);
        /// <summary>
        /// 获取委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <returns></returns>
        T GetDelegate<T>(string procName) where T : Delegate;
    }
    /// <summary>
    /// 动态链接库程序集
    /// </summary>
    internal class AssemblyCDllModel : IAssemblyCDllModel
    {
        IntPtr hModule;
        /// <summary>
        /// dll 路径
        /// </summary>
        /// <param name="path"></param>
        public AssemblyCDllModel(string path)
        {
            hModule = KERNEL32.LoadLibrary(path, 0, (int)LoaderOptimization.MultiDomain);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            KERNEL32.FreeLibrary(hModule);
        }
        public Delegate GetMethod(string procName, Type type)
        {
            IntPtr func = KERNEL32.GetProcAddress(hModule, procName);
            return (Delegate)Marshal.GetDelegateForFunctionPointer(func, type);
        }
        public T GetDelegate<T>(string procName) where T : Delegate
        {
            IntPtr func = KERNEL32.GetProcAddress(hModule, procName);
            return (T)Marshal.GetDelegateForFunctionPointer(func, typeof(T));
        }
    }
}
