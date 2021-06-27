using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Dabber.Cobber
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
    public enum StoreType : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = nameof(Unknown))]
        Unknown = 0,
        /// <summary>
        /// SQLite
        /// </summary>
        [Display(Name = nameof(SQLite))]
        SQLite = 1,
        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        [Display(Name = nameof(SqlServer))]
        SqlServer = 2,
        /// <summary>
        /// MySQL
        /// </summary>
        [Display(Name = nameof(MySQL))]
        MySQL = 3,
        /// <summary>
        /// Oracle
        /// </summary>
        [Display(Name = nameof(Oracle))]
        Oracle = 4,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        [Display(Name = nameof(PostgreSQL))]
        PostgreSQL = 5,
        /// <summary>
        /// Redis
        /// </summary>
        [Display(Name = nameof(Redis))]
        Redis = 6,
        /// <summary>
        /// Access
        /// </summary>
        [Display(Name = nameof(Access))]
        Access = 7,
        /// <summary>
        /// Excel
        /// </summary>
        [Display(Name = nameof(Excel))]
        Excel = 8,
        /// <summary>
        /// XML
        /// </summary>
        [Display(Name = nameof(Xml))]
        Xml = 9,
        /// <summary>
        /// 内存
        /// </summary>
        [Display(Name = nameof(Memory))]
        Memory = 20,
    }
    /// <summary>
    /// 连接创建者
    /// </summary>
    public class StoreBuilder
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UID { get => UserID; set => UserID = value; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Pwd { get => Password; set => Password = value; }
        /// <summary>
        /// 提供者
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 附加字典
        /// </summary>
        public Dictionary<string, string> Extra { get; }
        /// <summary>
        /// 全部字典
        /// </summary>
        public Dictionary<string, string> Origin { get; }
        /// <summary>
        /// 构造
        /// </summary>
        public StoreBuilder()
        {
            Extra = new Dictionary<string, string>();
            Origin = new Dictionary<string, string>();
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
            var array = connString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array)
            {
                var kv = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (kv == null || kv.Length == 0) { continue; }
                if (kv.Length == 1) { builder.Origin.Add(kv[0].Trim(), string.Empty); }
                if (kv.Length == 2) { builder.Origin.Add(kv[0].Trim(), kv[1].Trim()); }
            }
            var extraProps = new List<string> { nameof(Origin), nameof(Extra) };
            var props = typeof(StoreBuilder).GetProperties()
                .Where(s => !extraProps.Contains(s.Name));
            foreach (var item in builder.Origin)
            {
                var newKey = Regex.Replace(item.Key, "\\s+", "").ToLower();
                var prop = props.FirstOrDefault(s => s.CanWrite && s.Name.Equals(newKey, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    try
                    {
                        prop.SetValue(builder, item.Value, null);
                        continue;
                    }
                    catch { }
                }
                builder.Extra[item.Key] = item.Value;
            }
            return builder;
        }
        /// <summary>
        /// 从存储类模型中提取(使用其连接字符串)
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static StoreBuilder FromStore(StoreModel store) => FromConnString(store.ConnString);
    }
}
