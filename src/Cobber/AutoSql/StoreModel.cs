using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Dabber;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Cobber
{
    /// <summary>
    /// 存储模型
    /// </summary>
    public class StoreModel : ICloneable
    {
        private int _timeout;
        private string _level;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public StoreType DbType { get; set; }
        /// <summary>
        /// 超时时间(分钟)
        /// </summary>
        public int Timeout { get { return _timeout <= 0 ? 20 : _timeout; } set { if (value > 0) { _timeout = value; } } }
        /// <summary>
        /// 日志级别
        /// </summary>
        public string Level { get { return _level ?? "Warning"; } set { if (!string.IsNullOrEmpty(value)) { _level = value; } } }
        /// <summary>
        /// 初始构造
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connString"></param>
        public StoreModel(StoreType dbType, string connString)
        {
            DbType = dbType;
            ConnString = connString;
            _timeout = 20;
            _level = "Warning";
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public StoreModel Clone()
        {
            return new StoreModel(DbType, ConnString)
            {
                Timeout = Timeout,
                Level = Level,
            };
        }
        /// <summary>
        /// 复制接口
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 数据服务类型
    /// </summary>
    [EDisplay("数据服务类型")]
    public enum StoreType : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = nameof(Unknown))]
        [EDisplay("未知")]
        Unknown = 0,
        /// <summary>
        /// SQLite
        /// </summary>
        [Display(Name = nameof(SQLite))]
        [EDisplay("SQLite")]
        SQLite = 1,
        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        [Display(Name = nameof(SqlServer))]
        [EDisplay("Microsoft SQL Server")]
        SqlServer = 2,
        /// <summary>
        /// MySQL
        /// </summary>
        [Display(Name = nameof(MySQL))]
        [EDisplay("MySQL")]
        MySQL = 3,
        /// <summary>
        /// Oracle
        /// </summary>
        [Display(Name = nameof(Oracle))]
        [EDisplay("Oracle")]
        Oracle = 4,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        [Display(Name = nameof(PostgreSQL))]
        [EDisplay("PostgreSQL")]
        PostgreSQL = 5,
        /// <summary>
        /// Redis
        /// </summary>
        [Display(Name = nameof(Redis))]
        [EDisplay("Redis")]
        Redis = 6,
        /// <summary>
        /// Access
        /// </summary>
        [Display(Name = nameof(Access))]
        [EDisplay("Access")]
        Access = 7,
        /// <summary>
        /// Excel
        /// </summary>
        [Display(Name = nameof(Excel))]
        [EDisplay("Excel")]
        Excel = 8,
        /// <summary>
        /// XML
        /// </summary>
        [Display(Name = nameof(Xml))]
        [EDisplay("Xml")]
        Xml = 9,
        /// <summary>
        /// 内存
        /// </summary>
        [Display(Name = nameof(Memory))]
        [EDisplay("Memory")]
        Memory = 20,
    }
    /// <summary>
    /// 连接创建者
    /// </summary>
    public class StoreBuilder
    {
        private string _server;
        /// <summary>
        /// 服务器
        /// </summary>
        public string Server
        {
            get => _server;
            set => _extra[nameof(Server)] = _server = value;
        }
        private string _dataSource;
        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource
        {
            get => _dataSource;
            set => _extra[nameof(DataSource)] = _dataSource = value;
        }
        private string _userID;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserID
        {
            get => _userID;
            set => _extra[nameof(UserID)] = _userID = value;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UID
        {
            get => _userID;
            set => _extra[nameof(UID)] = _userID = value;
        }
        private string _password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get => _password;
            set => _extra[nameof(Password)] = _password = value;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Pwd { get => Password; set => _extra[nameof(Pwd)] = _password = value; }
        private string _provider;
        /// <summary>
        /// 提供者
        /// </summary>
        public string Provider { get => _provider; set => _origin[nameof(Provider)] = _provider = value; }
        private string _version;
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get => _version; set => _origin[nameof(Version)] = _version = value; }
        private string _port;
        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get=> _port; set => _origin[nameof(Port)] = _port = value; }
        private string _database;
        /// <summary>
        /// 数据库
        /// </summary>
        public string Database { get => _database; set =>_origin[nameof(Database)] = _database = value; }
        private Dictionary<string, string> _extra = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 附加字典
        /// </summary>
        public ReadOnlyDictionary<string, string> Extra { get => new ReadOnlyDictionary<string, string>(_extra); }
        private Dictionary<string, string> _origin = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 全部字典
        /// </summary>
        public ReadOnlyDictionary<string, string> Origin { get => new ReadOnlyDictionary<string, string>(_origin); }
        /// <summary>
        /// 构造
        /// </summary>
        public StoreBuilder()
        {
        }
        /// <summary>
        /// 同连接字符串中提取
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static StoreBuilder FromConnString(String connString)
        {
            var builder = new StoreBuilder();
            if (string.IsNullOrEmpty(connString)) { return builder; }
            var array = new List<string>();
            var isBracket = false;
            StringBuilder section = new StringBuilder();
            foreach (var item in connString)
            {
                if (item.Equals('\''))
                {
                    isBracket = !isBracket;
                }
                if (isBracket || !item.Equals(';'))
                {
                    section.Append(item);
                }
                else
                {
                    var temp = section.ToString();
                    if (string.IsNullOrEmpty(temp)) { continue; }
                    array.Add(temp);
                    section.Clear();
                }
            }
            var extraProps = new List<string> { nameof(Origin), nameof(Extra) };
            var props = typeof(StoreBuilder).GetProperties()
                .Where(s => !extraProps.Contains(s.Name));
            var SetValue = PropertyAccess<StoreBuilder>.InternalSetValue;
            foreach (var temp in array)
            {
                var index = temp.IndexOf("=");
                if (index <= 0) { continue; }
                string key = temp.Substring(0, index).Trim();
                string value = string.Empty;
                if (index + 1 < temp.Length)
                {
                    value = temp.Substring(index + 1, temp.Length - index - 1).Trim();
                }
                var newKey = Regex.Replace(key, "\\s+", ""); // 替换掉所有空格
                var prop = props.FirstOrDefault(s => s.CanWrite && s.Name.Equals(newKey, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    try
                    {
                        SetValue(builder, prop.Name, value);
                        continue;
                    }
                    catch { }
                }
                builder.AddExtra(key, value);
            }
            return builder;
        }
        /// <summary>
        /// 添加附加值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoreBuilder AddExtra(string key, string value)
        {
            _origin[key] = _extra[key] = value;
            return this;
        }

        /// <summary>
        /// 从存储类模型中提取(使用其连接字符串)
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static StoreBuilder FromStore(StoreModel store) => FromConnString(store.ConnString);
    }
}
