using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Cobber;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
/*************************************************************************************
 * 使用其他程序集相同类:
 * 首部引用别名 右击引用项目的属性中别名填写
 * extern alias tester;
 * 默认本程序集为global
 * 此例中此类
 * 本部使用 => using AppConfigJsonFile = global::System.Data.Cobber.AppConfigJsonFile;
 * 测试使用 => using AppConfigJsonFile = tester::System.Data.Cobber.AppConfigJsonFile;
 ************************************************************************************/
namespace System.Data.Extter
{
    /// <summary>
    /// 设置Json文件
    /// </summary>
    public class SettingJsonFile : SettingJsonFile<object>
    {

    }
    /// <summary>
    /// 设置Json文件
    /// </summary>
    public class SettingJsonFile<T>
    {
        // 资源字典
        readonly ConcurrentDictionary<string, string> _resDic = new(StringComparer.OrdinalIgnoreCase);
        // 文件监听
        readonly FileSystemWatcher _fileWatcher;
        readonly string _filePath;
        readonly string _fileName;
        readonly string _fileKey;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get => Path.Combine(_filePath, _fileName); }
        /// <summary>
        /// 加密密码
        /// </summary>
        public string FileKey { get => _fileKey; }
        /// <summary>
        /// 构造
        /// </summary>
        public SettingJsonFile() : this($"{typeof(T).Name}.json") { }
        /// <summary>
        /// 不加密构造
        /// </summary>
        /// <param name="filePath"></param>
        public SettingJsonFile(string filePath) : this(filePath, string.Empty) { }
        /// <summary>
        /// 构造
        /// </summary>
        public SettingJsonFile(string filePath, string securityKey)
        {
            _fileWatcher = new FileSystemWatcher();
            _fileKey = securityKey ?? String.Empty;
            try
            {
                _filePath = Path.GetFullPath(filePath);
                //初始化监听
                _fileWatcher.BeginInit();
                //设置监听的路径
                _fileWatcher.Path = System.IO.Path.GetDirectoryName(filePath);
                //设置监听文件类型
                _fileWatcher.Filter = System.IO.Path.GetFileName(filePath);
                //设置是否监听子目录
                _fileWatcher.IncludeSubdirectories = false;
                //设置是否启用监听?
                _fileWatcher.EnableRaisingEvents = true;
                //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
                _fileWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                //注册创建文件或目录时的监听事件
                //watcher.Created += new FileSystemEventHandler(watch_created);
                //注册当指定目录的文件或者目录发生改变的时候的监听事件
                //fileWatcher.Changed += new FileSystemEventHandler((sender, e) => LoadConfigFile());
                //注册当删除目录的文件或者目录的时候的监听事件
                //fileWatcher.Deleted += new FileSystemEventHandler((sender, e) => TrySave());
                //当指定目录的文件或者目录发生重命名的时候的监听事件
                //fileWatcher.Renamed += new RenamedEventHandler((sender, e) =>
                //{
                //    if (!File.Exists(FilePath)) { Save(); }
                //});
                //结束初始化
                _fileWatcher.EndInit();
            }
            catch (Exception ex) { Console.WriteLine(ex); throw ex; }
        }
    }
    /// <summary>
    /// 设置Json文件多个对象
    /// </summary>
    public class SettingJsonFiles<T>
    {

    }
    /// <summary>
    /// 设置类型
    /// </summary>
    [Flags]
    [EDisplay("设置类型")]
    public enum SettingType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EDisplay("未知")]
        Unknown = 0,
        /// <summary>
        /// 通用设置
        /// </summary>
        [EDisplay("应用内容")]
        App = 1,
        /// <summary>
        /// 应用设置
        /// </summary>
        [EDisplay("设置内容")]
        Settings = 2,
        /// <summary>
        /// 资源设置
        /// </summary>
        [EDisplay("资源设置")]
        Resources = 4,
        /// <summary>
        /// 内部设置
        /// </summary>
        [EDisplay("内部设置")]
        Inner = 8,
        /// <summary>
        /// 外部设置
        /// </summary>
        [EDisplay("外部设置")]
        Outer = 16,
        /// <summary>
        /// 硬件设置
        /// </summary>
        [EDisplay("硬件设置")]
        Hardware = 32,
        /// <summary>
        /// 数据库设置
        /// </summary>
        [EDisplay("数据库设置")]
        Database = 64,
        /// <summary>
        /// 消息设置
        /// </summary>
        [EDisplay("消息设置")]
        Message = 128,
        /// <summary>
        /// 跟踪设置
        /// </summary>
        [EDisplay("跟踪设置")]
        Trace = 256,
        /// <summary>
        /// 信息设置
        /// </summary>
        [EDisplay("信息设置")]
        Info = 512,
        /// <summary>
        /// 调试设置
        /// </summary>
        [EDisplay("调试设置")]
        Debug = 1024,
        /// <summary>
        /// 警告设置
        /// </summary>
        [EDisplay("警告设置")]
        Warn = 2048,
        /// <summary>
        /// 错误设置
        /// </summary>
        [EDisplay("错误设置")]
        Error = 4096,
        /// <summary>
        /// 崩溃设置
        /// </summary>
        [EDisplay("崩溃设置")]
        Crash = 8192,
        /// <summary>
        /// 发布设置
        /// </summary>
        [EDisplay("发布设置")]
        Publish = 16384,
        /// <summary>
        /// 发行设置
        /// </summary>
        [EDisplay("发行设置")]
        Release = 16384,
        /// <summary>
        /// 隐藏设置
        /// </summary>
        [EDisplay("隐藏设置")]
        Hide = 32768,
        /// <summary>
        /// 优先设置
        /// </summary>
        [EDisplay("优先设置")]
        Advance = 65536,
        /// <summary>
        /// 开发设置
        /// </summary>
        [EDisplay("开发设置")]
        Develop = 131072,
        /// <summary>
        /// 测试设置
        /// </summary>
        [EDisplay("测试设置")]
        Test = 262144,
        /// <summary>
        /// 用户设置
        /// </summary>
        [EDisplay("用户设置")]
        User = 524288,
        /// <summary>
        /// 自动设置
        /// </summary>
        [EDisplay("自动设置")]
        Auto = 1048576,
        /// <summary>
        /// 临时设置
        /// </summary>
        [EDisplay("临时设置")]
        Temp = 2097152,
        /// <summary>
        /// 设计设置
        /// </summary>
        [EDisplay("设计设置")]
        Design = 4194304,
        /// <summary>
        /// 模板设置
        /// </summary>
        [EDisplay("模板设置")]
        Template = 8388608,
        /// <summary>
        /// 代码设置
        /// </summary>
        [EDisplay("代码设置")]
        Coder = 16777216,
        /// <summary>
        /// 服务设置
        /// </summary>
        [EDisplay("服务设置")]
        Service = 33554432,
        /// <summary>
        /// 代理设置
        /// </summary>
        [EDisplay("代理设置")]
        Proxy = 67108864,
        /// <summary>
        /// 访问设置
        /// </summary>
        [EDisplay("访问设置")]
        Access = 134217728,
        /// <summary>
        /// 请求设置
        /// </summary>
        [EDisplay("请求设置")]
        Request = 268435456,
        /// <summary>
        /// 网络设置
        /// </summary>
        [EDisplay("网络设置")]
        Web = 268435456,
        /// <summary>
        /// 环境设置
        /// </summary>
        [EDisplay("环境设置")]
        Context = 536870912,
        /// <summary>
        /// 附加设置
        /// </summary>
        [EDisplay("附加设置")]
        Extra = 1073741824,

        #region // 组合值内容
        /// <summary>
        /// 应用设置
        /// </summary>
        [EDisplay("应用设置")]
        AppSettings = App | Settings,
        /// <summary>
        /// 应用资源
        /// </summary>
        [EDisplay("应用资源")]
        AppResources = App | Resources,
        /// <summary>
        /// 应用缓存类
        /// </summary>
        [EDisplay("应用缓存类")]
        AppCacheClass = App | Settings | Resources | Context | Proxy | Outer | User,
        #endregion
    }
    /// <summary>
    /// 启动调试设置属性
    /// 建议文件名称(StartSettingV{Version}.start)优先级如下:
    /// 1.AppEnvironment中的Documents
    /// 2.ADataSettings中的Resources
    /// 3.Properties.Resources中的键名(去除扩展名后)
    /// </summary>
    public class StartSettingAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="version">版本号</param>
        public StartSettingAttribute(long version)
        {
            Version = version.GetVersionRange();
        }
        /// <summary>
        /// 版本号
        /// </summary>
        public long Version { get; }
    }
    /// <summary>
    /// 启动设置内容
    /// </summary>
    public abstract class AStartSettings
    {
        /// <summary>
        /// 开始执行
        /// </summary>
        public abstract IAlertMsg Start(FileInfo file, string secret);
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public abstract string GetSecret();
        /// <summary>
        /// 获取修正列表
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Action> GetFixes()
        {
            var startDic = new Dictionary<long, Action>();
            var type = GetType();
            foreach (var item in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (item.GetParameters().Length > 0) { continue; }
                if (item.ReturnType != typeof(void)) { continue; }
                foreach (var attr in item.GetCustomAttributes(false))
                {
                    if (attr is StartSettingAttribute attrObj)
                    {
                        var model = this;
                        startDic[attrObj.Version] = () => item.Invoke(model, null);
                    }
                }
            }
            return startDic.OrderBy(s => s.Key).Select(s => s.Value);
        }
        /// <summary>
        /// 启动内容修正
        /// </summary>
        /// <returns></returns>
        public virtual IAlertMsg StartFixing()
        {
            foreach (var starter in GetFixes())
            {
                starter.Invoke();
            }
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 启动参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg StartArgs(string args)
        {
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 启动信息
        /// </summary>
        /// <param name="startInfo"></param>
        /// <returns></returns>
        public virtual IAlertMsg StartInfo(ProcessStartInfoModel startInfo)
        {
            return AlertMsg.OperSuccess;
        }
    }
    /// <summary>
    /// 启动设置
    /// </summary>
    public abstract class AStartSettings<T> : AStartSettings
        where T : AStartSettings<T>
    {
        /// <summary>
        /// 启动资源文件
        /// </summary>
        public const String StartSettingResources = "startsetting.jres";
        /// <summary>
        /// 启动设置目录
        /// </summary>
        public const String StartSettingFolder = "Resources";
        /// <summary>
        /// 启动屏幕
        /// </summary>
        public static string SplashScreen { get; set; }
        /// <summary>
        /// 默认域名称
        /// </summary>
        public static String DefaultDomainName { get; set; }
        /// <summary>
        /// 默认启动域
        /// </summary>
        public static Int64 DefaultStartDomain { get; set; }
        /// <summary>
        /// 是默认加密
        /// </summary>
        public static bool IsDefaultEncrypt { get; set; }
        static AStartSettings()
        {
            IsDefaultEncrypt = !TestTry.IsDebugMode;
            try
            {
                var filePath = Path.GetFullPath(Path.Combine(StartSettingFolder, StartSettingResources));
                String content = null;
                if (File.Exists(filePath)) { content = File.ReadAllText(filePath); }
                else
                {
                    var assembly = typeof(T).Assembly;
                    var nameFixed = $"{StartSettingFolder}.{StartSettingResources}";
                    var contentKey = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(nameFixed, StringComparison.OrdinalIgnoreCase));
                    if (contentKey == null) { return; }
                    using (var resStream = new StreamReader(assembly.GetManifestResourceStream(contentKey)))
                    {
                        content = resStream.ReadToEnd();
                    }
                }
                if (!content.StartsWith("{"))
                {
                    content = UserCrypto.GetAesDecrypt(content, UserPassword.DefaultPasswordB);
                }
                var dic = content.GetJsonObject<Dictionary<string, object>>();
                IPropertyAccess access = PropertyAccess<T>.Proxy;
                foreach (var item in dic)
                {
                    try
                    {
                        access.FuncSetValue(null, item.Key, item.Value);
                    }
                    catch { }
                }
            }
            catch { }
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IAlertMsg SetData(T data);
        private string _secret;
        /// <summary>
        /// 获取密钥内容
        /// </summary>
        /// <returns></returns>
        public sealed override string GetSecret()
        {
            return _secret;
        }
        /// <summary>
        /// 开始进行,启动成功一次即删除
        /// </summary>
        public sealed override IAlertMsg Start(FileInfo file, string secret)
        {
            if (file == null || !file.Exists) { return AlertMsg.NotFound; }
            var text = File.ReadAllText(file.FullName, Encoding.UTF8);
            if (text.Length <= 32) { return AlertMsg.NotFound; }
            if (IsDefaultEncrypt && text[0] == '{') { return AlertMsg.NotSupported; }
            _secret = secret + UserPassword.DefaultPasswordB;
            string content = text[0] == '{' ? text : UserCrypto.GetAesDecrypt(text, _secret);
            var model = content.GetJsonObject<T>();
            var res = SetData(model);
            if (res.IsSuccess) { file.Delete(); }
            return res;
        }
    }
}
