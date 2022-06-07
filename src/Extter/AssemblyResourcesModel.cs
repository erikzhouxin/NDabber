using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace System.Data.Extter
{
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
}
