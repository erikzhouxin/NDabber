using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 应用文件配置
    /// </summary>
    public abstract class AppConfigJsonFile
    {
        /// <summary>
        /// 资源字典
        /// </summary>
        public ConcurrentDictionary<string, string> ResDic { get; } = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 文件监听
        /// </summary>
        protected FileSystemWatcher FileWatcher { get; }
        /// <summary>
        /// 文件路径
        /// </summary>
        protected string FilePath { get; }
        /// <summary>
        /// 安全码
        /// </summary>
        protected string SecurityKey { get; }
        /// <summary>
        /// 不加密构造
        /// </summary>
        /// <param name="filePath"></param>
        protected AppConfigJsonFile(string filePath) : this(filePath, string.Empty) { }
        /// <summary>
        /// 构造
        /// </summary>
        protected AppConfigJsonFile(string filePath,string securityKey)
        {
            FilePath = filePath;
            SecurityKey = securityKey;
            var fileWatcher = new FileSystemWatcher();
            try
            {
                //初始化监听
                fileWatcher.BeginInit();
                //设置监听的路径
                fileWatcher.Path = System.IO.Path.GetDirectoryName(filePath);
                //设置监听文件类型
                fileWatcher.Filter = System.IO.Path.GetFileName(filePath);
                //设置是否监听子目录
                fileWatcher.IncludeSubdirectories = false;
                //设置是否启用监听?
                fileWatcher.EnableRaisingEvents = true;
                //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
                fileWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                //注册创建文件或目录时的监听事件
                //watcher.Created += new FileSystemEventHandler(watch_created);
                //注册当指定目录的文件或者目录发生改变的时候的监听事件
                fileWatcher.Changed += new FileSystemEventHandler((sender, e) => LoadConfigFile());
                //注册当删除目录的文件或者目录的时候的监听事件
                fileWatcher.Deleted += new FileSystemEventHandler((sender, e) => TrySave());
                //当指定目录的文件或者目录发生重命名的时候的监听事件
                //fileWatcher.Renamed += new RenamedEventHandler((sender, e) =>
                //{
                //    if (!File.Exists(FilePath)) { Save(); }
                //});
                //结束初始化
                fileWatcher.EndInit();
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            FileWatcher = fileWatcher;
            LoadingDefault();
            LoadConfigFile();
        }
        /// <summary>
        /// 加载默认
        /// </summary>
        protected virtual void LoadingDefault()
        {
            var properties  = GetType().GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in properties)
            {
                try
                {
                    ResDic[prop.Name] = prop.GetValue(null, null).ToString();
                }
                catch { }
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        protected virtual void LoadConfigFile()
        {
            if (!System.IO.File.Exists(FilePath))
            {
                TrySave();
                return;
            }
            try
            {
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(FilePath));
                var isEncrypt = !string.IsNullOrEmpty(SecurityKey);
                var type = GetType();
                foreach (var item in dic)
                {
                    try
                    {
                        var prop = type.GetProperty(item.Key);
                        if (prop != null && prop.CanWrite)
                        {
                            var value = item.Value;
                            if (isEncrypt)
                            {
                                value = UserCrypto.GetAesDecrypt(item.Value, SecurityKey);
                            }
                            prop.SetValue(null, value, null);
                            ResDic[item.Key] = value;
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        public virtual void TrySave()
        {
            try
            {
                FileWatcher.EnableRaisingEvents = false;
                File.WriteAllText(FilePath, ResDic.GetJsonFormatString());
                FileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual string Get(string property)
        {
            if (ResDic.TryGetValue(property, out string value))
            {
                return value;
            }
            return property ?? string.Empty;
        }
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="content"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual String Get(string content, params string[] args)
        {
            return Get(string.Format(content, args));
        }
    }
}
