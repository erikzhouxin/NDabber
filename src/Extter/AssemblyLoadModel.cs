using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 程序集加载模型接口
    /// </summary>
    public interface IAssemblyLoadModel : IDisposable
    {
        /// <summary>
        /// 重加载
        /// </summary>
        /// <returns></returns>
        Assembly Reload();
        /// <summary>
        /// 卸载
        /// </summary>
        /// <returns></returns>
        bool Unload();
    }
    /// <summary>
    /// 程序集加载模型
    /// </summary>
    internal class AssemblyLoadProxy : IAssemblyLoadModel
    {
        static object _locker = new object();
        static Dictionary<string, IAssemblyLoadModel> _loaders = new Dictionary<string, IAssemblyLoadModel>();
        IAssemblyLoadModel _proxy;
        /// <summary>
        /// 构造
        /// </summary>
        public AssemblyLoadProxy(string path)
        {
            if (!_loaders.TryGetValue(path, out _proxy))
            {
                lock (_locker)
                {
                    if (!_loaders.TryGetValue(path, out _proxy))
                    {
                        AssemblyName aName;
                        try
                        {
                            var fullPath = Path.GetFullPath(path);
                            if (!File.Exists(fullPath))
                            {
                                throw new FileNotFoundException(fullPath);
                            }
                            aName = AssemblyName.GetAssemblyName(fullPath);
                        }
                        catch(FileNotFoundException fileEx)
                        {
                            throw fileEx;
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                aName = new AssemblyName(path);
                            }
                            catch (Exception ex1)
                            {
                                throw new DllNotFoundException(ex.Message, ex1);
                            }
                        }
                        _loaders[aName.FullName] = new AssemblyLoadModel(aName);
                    }
                }
            }
        }

        public void Dispose()
        {
            _proxy?.Dispose();
        }

        public Assembly Reload()
        {
            return _proxy?.Reload();
        }

        public bool Unload()
        {
            return _proxy?.Unload() ?? false;
        }
    }
    internal class AssemblyLoadModel : IAssemblyLoadModel
    {
        private Assembly _assembly;
        private AppDomain _domain;
        public AssemblyLoadModel(AssemblyName aName)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            setup.ShadowCopyFiles = "true";
            var dir = Path.GetDirectoryName(aName.CodeBase);
            setup.ShadowCopyDirectories = dir;// 需要动态加载的Assembly所在文件夹,影像副本目录
            setup.ApplicationName = "Dynamic";
            // Set up the Evidence
            Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
            Evidence evidence = new Evidence(baseEvidence);


            // Create the AppDomain      
            _domain = AppDomain.CreateDomain("newDomain", evidence, setup);
        }
        public void Dispose()
        {
            Unload();
        }

        public Assembly Reload()
        {
            throw new NotImplementedException();
        }

        public bool Unload()
        {
            AppDomain.Unload(_domain);
            _domain = null;
            return true;
        }
    }
}
