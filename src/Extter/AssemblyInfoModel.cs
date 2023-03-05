using System;
using System.Collections.Generic;
using System.Data.Impeller;
using System.Linq;
using System.Reflection;
using System.Resources;
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
    /// <summary>
    /// 程序集资源模型
    /// </summary>
    public class AssemblyResourcesModel
    {
        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assembly { get; set; }
        /// <summary>
        /// 资源管理器
        /// </summary>
        public ResourceManager Manager { get; set; }
        /// <summary>
        /// 程序集及资源管理构造
        /// </summary>
        public AssemblyResourcesModel(Assembly assembly, ResourceManager manager)
        {
            Assembly = assembly;
            Manager = manager;
        }
        /// <summary>
        /// 程序集路径以及Resources命名空间
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resNamespace"></param>
        public AssemblyResourcesModel(string path, string resNamespace)
        {
            try
            {
                Assembly = Assembly.LoadFile(path);
                Manager = new ResourceManager(resNamespace, Assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// 程序集路径以及Resources命名空间
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resType"></param>
        public AssemblyResourcesModel(string path, Type resType)
        {
            try
            {
                Assembly = Assembly.LoadFile(path);
                Manager = new ResourceManager(resType.FullName, Assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetObject(string key)
        {
            return Manager?.GetObject(key);
        }
        /// <summary>
        /// 获取值内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            return (T)(Manager?.GetObject(key) ?? default(T));
        }
        /// <summary>
        /// 获取值内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defVal)
        {
            return (T)(Manager?.GetObject(key) ?? defVal);
        }
    }

    ///// <summary>
    ///// 程序集加载模型接口
    ///// </summary>
    //public interface IAssemblyLoadModel : IDisposable
    //{
    //    /// <summary>
    //    /// 重加载
    //    /// </summary>
    //    /// <returns></returns>
    //    Assembly Reload();
    //    /// <summary>
    //    /// 卸载
    //    /// </summary>
    //    /// <returns></returns>
    //    bool Unload();
    //}
    ///// <summary>
    ///// 程序集加载模型
    ///// </summary>
    //internal class AssemblyLoadProxy : IAssemblyLoadModel
    //{
    //    static object _locker = new object();
    //    static Dictionary<string, IAssemblyLoadModel> _loaders = new Dictionary<string, IAssemblyLoadModel>();
    //    IAssemblyLoadModel _proxy;
    //    /// <summary>
    //    /// 构造
    //    /// </summary>
    //    public AssemblyLoadProxy(string path)
    //    {
    //        if (!_loaders.TryGetValue(path, out _proxy))
    //        {
    //            lock (_locker)
    //            {
    //                if (!_loaders.TryGetValue(path, out _proxy))
    //                {
    //                    AssemblyName aName;
    //                    try
    //                    {
    //                        var fullPath = Path.GetFullPath(path);
    //                        if (!File.Exists(fullPath))
    //                        {
    //                            throw new FileNotFoundException(fullPath);
    //                        }
    //                        aName = AssemblyName.GetAssemblyName(fullPath);
    //                    }
    //                    catch(FileNotFoundException fileEx)
    //                    {
    //                        throw fileEx;
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        try
    //                        {
    //                            aName = new AssemblyName(path);
    //                        }
    //                        catch (Exception ex1)
    //                        {
    //                            throw new DllNotFoundException(ex.Message, ex1);
    //                        }
    //                    }
    //                    _loaders[aName.FullName] = new AssemblyLoadModel(aName);
    //                }
    //            }
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        _proxy?.Dispose();
    //    }

    //    public Assembly Reload()
    //    {
    //        return _proxy?.Reload();
    //    }

    //    public bool Unload()
    //    {
    //        return _proxy?.Unload() ?? false;
    //    }
    //}
    //internal class AssemblyLoadModel : IAssemblyLoadModel
    //{
    //    private Assembly _assembly;
    //    private AppDomain _domain;
    //    public AssemblyLoadModel(AssemblyName aName)
    //    {
    //        AppDomainSetup setup = new AppDomainSetup();
    //        setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
    //        setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
    //        setup.ShadowCopyFiles = "true";
    //        var dir = Path.GetDirectoryName(aName.CodeBase);
    //        setup.ShadowCopyDirectories = dir;// 需要动态加载的Assembly所在文件夹,影像副本目录
    //        setup.ApplicationName = "Dynamic";
    //        // Set up the Evidence
    //        Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
    //        Evidence evidence = new Evidence(baseEvidence);


    //        // Create the AppDomain      
    //        _domain = AppDomain.CreateDomain("newDomain", evidence, setup);
    //    }
    //    public void Dispose()
    //    {
    //        Unload();
    //    }

    //    public Assembly Reload()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Unload()
    //    {
    //        AppDomain.Unload(_domain);
    //        _domain = null;
    //        return true;
    //    }
    //}
}
