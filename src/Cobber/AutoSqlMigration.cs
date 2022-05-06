using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Dabber;
using System.Data.Extter;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 迁移类实现
    /// </summary>
    public interface IDbAMigration
    {
        /// <summary>
        /// 迁移内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        IAlertMsg Migration(DbConnection conn, DbTransaction trans);
    }
    /// <summary>
    /// 迁移类实现
    /// </summary>
    public interface IDbAutoMigration
    {
        /// <summary>
        /// 版本号
        /// </summary>
        long Version { get; }
        /// <summary>
        /// 迁移内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        IAlertMsg Migration(DbConnection conn, DbTransaction trans);
    }
    /// <summary>
    /// 自动迁移类
    /// </summary>
    public sealed class DbAutoMigration
    {
        internal static ConcurrentDictionary<string, DbAutoMigration> CtxInsDic = new(StringComparer.OrdinalIgnoreCase);
        internal static ConcurrentDictionary<string, IAlertMsg> CheckDic = new();
        /// <summary>
        /// 本地上下文[LocalContext]
        /// </summary>
        /// <see cref="Create(string)"/>
        /// <returns></returns>
        public static DbAutoMigration CreateLocal() => Create("LocalContext");
        /// <summary>
        /// 远程上下文[RemoteContext]
        /// </summary>
        /// <see cref="Create(string)"/>
        /// <returns></returns>
        public static DbAutoMigration CreateRemote() => Create("RemoteContext");
        /// <summary>
        /// 服务器上下文[ServerContext]
        /// </summary>
        /// <see cref="Create(string)"/>
        /// <returns></returns>
        public static DbAutoMigration CreateServer() => Create("ServerContext");
        /// <summary>
        /// 创建环境
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DbAutoMigration Create(string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                throw new ArgumentNullException("[Context]不能为空");
            }
            return CtxInsDic.GetOrAdd(context, (k) => new DbAutoMigration(k));
        }
        /// <summary>
        /// 环境
        /// </summary>
        public string Context { get; }
        internal Dictionary<long, InterDbMigrationModel> Migrations { get; }
        /// <summary>
        /// 迁移版本号
        /// </summary>
        public string Version { get; }
        private DbAutoMigration(string context)
        {
            Context = context;
            Migrations = new();
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        /// <summary>
        /// 注册实现类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbAutoMigration Regist<T>() where T : IDbAutoMigration, new() => Regist(new T());
        /// <summary>
        /// 注册实现类
        /// </summary>
        /// <returns></returns>
        public DbAutoMigration Regist<T>(T model) where T : IDbAutoMigration
        {
            if (model == null) { return this; }
            return Regist(model.Version, $"{model.GetType().FullName}.{nameof(IDbAutoMigration.Migration)}", model.Migration);
        }
        /// <summary>
        /// 注册实现类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbAutoMigration RegistAttribute<T>() where T : IDbAMigration, new() => RegistAttribute(new T());
        /// <summary>
        /// 注册实现类
        /// </summary>
        /// <returns></returns>
        public DbAutoMigration RegistAttribute<T>(T model) where T : IDbAMigration
        {
            if (model == null) { return this; }
            var type = model.GetType();
            var attr = type.GetCustomAttribute<DbMigrationAttribute>(false);
            if (attr == null)
            {
                throw new NotImplementedException($"类型【{type.Name}】未包含【{nameof(DbMigrationAttribute)}】定义");
            }
            return Regist(attr.Version, $"{model.GetType().FullName}.{nameof(IDbAutoMigration.Migration)}", model.Migration);
        }
        /// <summary>
        /// 注册版本方法
        /// 推荐使用(Version,FullName,Migration)全面的方法
        /// </summary>
        /// <param name="version">版本号,19490101000000-99991231235959</param>
        /// <param name="Migration">返回值为true时下次启动不进行,返回值为false时,下次还要执行</param>
        /// <returns></returns>
        public DbAutoMigration Regist(long version, Func<DbConnection, DbTransaction, IAlertMsg> Migration)
        {
            return Regist(version, Migration.GetMethodFullName(), Migration);
        }
        /// <summary>
        /// 注册版本方法
        /// 推荐使用(FullName,Migration)全面的方法
        /// </summary>
        /// <param name="Migration">返回值为true时下次启动不进行,返回值为false时,下次还要执行</param>
        /// <returns></returns>
        public DbAutoMigration RegistAttribute(Func<DbConnection, DbTransaction, IAlertMsg> Migration)
        {
            var member = Migration.Method;
            var attr = member.GetCustomAttribute<DbMigrationAttribute>(false);
            if (attr == null) { throw new NotImplementedException($"方法【{member.Name}】未包含【{nameof(DbMigrationAttribute)}】定义"); }
            return Regist(attr.Version, $"{member.DeclaringType?.FullName}.{member.Name}", Migration);
        }
        /// <summary>
        /// 注册版本方法
        /// </summary>
        /// <param name="version">版本号,19490101000000-99991231235959</param>
        /// <param name="fullName">全名称</param>
        /// <param name="Migration">返回值为true时下次启动不进行,返回值为false时,下次还要执行</param>
        /// <returns></returns>
        public DbAutoMigration Regist(long version, string fullName, Func<DbConnection, DbTransaction, IAlertMsg> Migration)
        {
            if (version < 19491001000000 || version > 99991231235959)
            {
                throw new ArgumentOutOfRangeException("迁移版本号【Version】必须在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间");
            }
            Migrations[version] = new InterDbMigrationModel()
            {
                Version = version,
                Name = fullName ?? Migration.GetMethodFullName(),
                Memo = Version,
                Migration = Migration,
            };
            return this;
        }
        /// <summary>
        /// 开始迁移(无缓存)
        /// </summary>
        /// <returns></returns>
        internal IAlertMsg Start(Func<DbConnection> GetConnection, AutoSqlModel sqlModel)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    DbTransaction trans = null;
                    try
                    {
                        trans = conn.BeginTransaction();
                        CheckDic.GetOrAdd(conn.ConnectionString, (k) => new AlertMsg(conn.Execute(sqlModel.Create) >= 0, "操作完成"));
                        var models = Migrations.Values.OrderBy(s => s.Version).ToList();
                        var migrations = conn.Query<TDbMigrations>(sqlModel.Select, null, trans);
                        Func<long, bool> HasVersion;
                        if (migrations == null || migrations.Count() == 0)
                        {
                            HasVersion = (ver) => false;
                        }
                        else
                        {
                            var verList = migrations.Select(s => s.ID).ToList();
                            HasVersion = verList.Contains;
                        }
                        foreach (var item in models)
                        {
                            if (HasVersion(item.Version)) { continue; }
                            try
                            {
                                var exeLine = item.Migration(conn, trans);
                                if (exeLine.IsSuccess)
                                {
                                    conn.Execute(sqlModel.Insert, new TDbMigrations { ID = item.Version, Name = item.Name, Time = DateTime.Now, Memo = item.Memo }, trans);
                                }
                                trans.Commit();
                                trans = conn.BeginTransaction();
                            }
                            catch (Exception ex)
                            {
                                trans?.Rollback();
                                return new AlertMsg(false, ex.Message) { Data = ex };
                            }
                        }
                        trans.Commit();
                        return AlertMsg.OperSuccess;
                    }
                    catch (Exception ex)
                    {
                        trans?.Rollback();
                        return new AlertMsg(false, ex.Message) { Data = ex };
                    }
                }
            }
            catch (Exception ex)
            {
                return new AlertMsg(false, ex.Message) { Data = ex };
            }
        }
        /// <summary>
        /// 开始迁移(无缓存)
        /// </summary>
        /// <returns></returns>
        public IAlertMsg Start(Func<DbConnection> GetConnection, Func<Type, AutoSqlModel> GetSqlModel) => Start(GetConnection, GetSqlModel(typeof(TDbMigrations)));
        /// <summary>
        /// 开始迁移,根据连接进行缓存检查
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="connString"></param>
        /// <param name="GetConnection"></param>
        /// <returns></returns>
        public IAlertMsg Start(StoreType storeType, string connString, Func<DbConnection> GetConnection)
        {
            string key;
            AutoSqlModel sqlModel;
            switch (storeType)
            {
                case StoreType.SQLite:
                    key = $"Migration:SQLite:{connString}";
                    sqlModel = AutoSQLiteBuilder.Builder<TDbMigrations>();
                    break;
                case StoreType.SqlServer:
                    key = $"Migration:SqlServer:{connString}";
                    sqlModel = AutoSqlServerBuilder.Builder<TDbMigrations>();
                    break;
                case StoreType.MySQL:
                    key = $"Migration:MySQL:{connString}";
                    sqlModel = AutoMySqlBuilder.Builder<TDbMigrations>();
                    break;
                case StoreType.Access:
                    key = $"Migration:Access:{connString}";
                    sqlModel = AutoAccessBuilder.Builder<TDbMigrations>();
                    break;
                case StoreType.Oracle:
                    return new AlertMsg(false, "当前不支持[Oracle]的数据库迁移");
                case StoreType.PostgreSQL:
                    return new AlertMsg(false, "当前不支持[PostgreSQL]的数据库迁移");
                case StoreType.Redis:
                    return new AlertMsg(false, "当前不支持[Redis]的数据库迁移");
                case StoreType.Excel:
                    return new AlertMsg(false, "当前不支持[Excel]的数据库迁移");
                case StoreType.Xml:
                    return new AlertMsg(false, "当前不支持[XML]的数据库迁移");
                case StoreType.Memory:
                    return new AlertMsg(false, "当前不支持[Memory]的数据库迁移");
                case StoreType.Unknown:
                default:
                    return new AlertMsg(false, "当前不支持[Unknown]的数据库迁移");
            }
            return CheckDic.GetOrAdd(key, (k) => storeType switch
            {
                StoreType.SQLite => Start(GetConnection, AutoSQLiteBuilder.Builder<TDbMigrations>()),
                StoreType.SqlServer => Start(GetConnection, AutoSqlServerBuilder.Builder<TDbMigrations>()),
                StoreType.MySQL => Start(GetConnection, AutoMySqlBuilder.Builder<TDbMigrations>()),
                StoreType.Access => Start(GetConnection, AutoAccessBuilder.Builder<TDbMigrations>()),
                StoreType.Oracle => new AlertMsg(false, "当前不支持[Oracle]的数据库迁移"),
                StoreType.PostgreSQL => new AlertMsg(false, "当前不支持[PostgreSQL]的数据库迁移"),
                StoreType.Redis => new AlertMsg(false, "当前不支持[Redis]的数据库迁移"),
                StoreType.Excel => new AlertMsg(false, "当前不支持[Excel]的数据库迁移"),
                StoreType.Xml => new AlertMsg(false, "当前不支持[XML]的数据库迁移"),
                StoreType.Memory => new AlertMsg(false, "当前不支持[Memory]的数据库迁移"),
                _ => new AlertMsg(false, "当前不支持[Unknown]的数据库迁移"),
            });
        }
    }
    /// <summary>
    /// 数据库迁移模型
    /// </summary>
    [DbCol("数据库迁移表", Name = "_TDbMigrations")]
    internal sealed class TDbMigrations
    {
        /// <summary>
        /// 标识
        /// </summary>
        [DbCol("标识", Type = DbColType.Int64, Key = DbIxType.PK)]
        public long ID { get; set; }
        /// <summary>
        /// 迁移名称
        /// </summary>
        [DbCol("迁移名称", Len = 255)]
        public string Name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DbCol("创建时间", Type = DbColType.DateTime)]
        public DateTime Time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("迁移名称", Len = 255)]
        public string Memo { get; set; }
    }
    internal class InterDbMigrationModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        public long Version { get; set; }
        /// <summary>
        /// 迁移名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 迁移
        /// </summary>
        public Func<DbConnection, DbTransaction, IAlertMsg> Migration { get; set; }
    }
    /// <summary>
    /// 数据库迁移属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DbMigrationAttribute : Attribute
    {
        /// <summary>
        /// 版本号,19490101000000-99991231235959
        /// </summary>
        /// <param name="version"></param>
        public DbMigrationAttribute(long version)
        {
            Version = version;
        }
        /// <summary>
        /// 版本号,19490101000000-99991231235959
        /// </summary>
        public long Version { get; }
    }
    /// <summary>
    /// SQLite迁移数据访问
    /// </summary>
    public abstract class SQLiteDbMigration
    {
        /// <summary>
        /// 获取自动SQL模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AutoSqlModel GetSqlModel<T>()
        {
            return AutoSQLiteBuilder.Builder<T>();
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateTable<T>(DbConnection conn, DbTransaction trans)
        {
            var sqlModel = GetSqlModel<T>();
            conn.Execute(sqlModel.Create, null, trans);
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IAlertMsg CheckTable<T>(DbConnection conn, DbTransaction trans)
        {
            var sqlModel = GetSqlModel<T>();
            conn.Execute(sqlModel.Create, null, trans);
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 检查列
        /// </summary>
        /// <typeparam name="TM">模型类</typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="trans">数据库事务</param>
        /// <param name="express">表达式</param>
        /// <returns></returns>
        public static IAlertMsg CheckColumn<TM>(DbConnection conn, DbTransaction trans, Expression<Func<TM, object>> express)
        {
            var attr = DbColAttribute.GetAttribute(express.GetPropertyInfo());
            if (attr.Ignore) { return AlertMsg.OperSuccess; }
            var sqlModel = GetSqlModel<TM>();
            var tableInfos = conn.Query<SqlitePragmaTableInfo>($"PRAGMA table_info([{sqlModel.TagName}])", null, trans);
            if (!tableInfos.Any() || tableInfos.Any(s => s.Name == attr.Name)) { return AlertMsg.OperSuccess; }
            conn.Execute($"ALTER TABLE [{sqlModel.TagName}] ADD COLUMN {GetColumnDefinition(attr)}", null, trans);
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 检查列
        /// </summary>
        /// <typeparam name="TM">模型类</typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="trans">数据库事务</param>
        /// <param name="expresses">表达式</param>
        /// <returns></returns>
        public static IAlertMsg CheckColumn<TM>(DbConnection conn, DbTransaction trans, params Expression<Func<TM, object>>[] expresses)
        {
            var sqlModel = GetSqlModel<TM>();
            var tableInfos = conn.Query<SqlitePragmaTableInfo>($"PRAGMA table_info([{sqlModel.TagName}])", null, trans);
            if (!tableInfos.Any()) { return AlertMsg.OperSuccess; }
            foreach (var express in expresses)
            {
                var attr = DbColAttribute.GetAttribute(express.GetPropertyInfo());
                if (attr.Ignore || tableInfos.Any(s => s.Name == attr.Name)) { continue; }
                conn.Execute($"ALTER TABLE [{sqlModel.TagName}] ADD COLUMN {GetColumnDefinition(attr)}", null, trans);
            }
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 检查索引
        /// </summary>
        /// <typeparam name="TM">模型类</typeparam>
        /// <param name="conn">连接类</param>
        /// <param name="trans">事务</param>
        /// <param name="express">表达式</param>
        /// <returns></returns>
        public static IAlertMsg CheckIndex<TM>(DbConnection conn, DbTransaction trans, Expression<Func<TM, object>> express) => CheckIndex(conn, trans, false, express);
        /// <summary>
        /// 检查索引
        /// </summary>
        /// <typeparam name="TM">模型类</typeparam>
        /// <param name="conn">连接类</param>
        /// <param name="trans">事务</param>
        /// <param name="isDrop">是否删除</param>
        /// <param name="express">表达式</param>
        /// <returns></returns>
        public static IAlertMsg CheckIndex<TM>(DbConnection conn, DbTransaction trans, bool isDrop, Expression<Func<TM, object>> express)
        {
            var attr = DbColAttribute.GetAttribute(express.GetPropertyInfo());
            if (attr.Ignore) { return AlertMsg.OperSuccess; }
            var sqlModel = GetSqlModel<TM>();
            //var indexInfos = conn.Query<SqlitePragmaMasterInfo>($"SELECT [type],[name],[tbl_name],[rootpage],[sql] FROM [sqlite_master] WHERE [type]='{SqlitePragmaMasterInfo.TypeIndex}' COLLATE NOCASE AND [tbl_name]='{sqlModel.TagName}' COLLATE NOCASE", null, trans);
            //if (!indexInfos.Any()) { return AlertMsg.OperSuccess; }
            // 表列标记上只支持一个索引
            string createSql = GetIndexCheckSql(attr, sqlModel, isDrop);
            if (createSql != null)
            {
                conn.Execute(createSql, null, trans);
            }
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 检查索引
        /// </summary>
        /// <typeparam name="TM">模型类</typeparam>
        /// <param name="conn">连接类</param>
        /// <param name="trans">事务</param>
        /// <param name="expresses">表达式</param>
        /// <returns></returns>
        public static IAlertMsg CheckIndex<TM>(DbConnection conn, DbTransaction trans, params Expression<Func<TM, object>>[] expresses) => CheckIndex(conn, trans, false, expresses);
        /// <summary>
        /// 检查索引
        /// </summary>
        /// <typeparam name="TM">模型类</typeparam>
        /// <param name="conn">连接类</param>
        /// <param name="trans">事务</param>
        /// <param name="isDrop">是否删除</param>
        /// <param name="expresses">表达式</param>
        /// <returns></returns>
        public static IAlertMsg CheckIndex<TM>(DbConnection conn, DbTransaction trans, bool isDrop, params Expression<Func<TM, object>>[] expresses)
        {
            if (expresses == null) { return AlertMsg.OperSuccess; }
            var sqlModel = GetSqlModel<TM>();
            //var indexInfos = conn.Query<SqlitePragmaMasterInfo>($"SELECT [type],[name],[tbl_name],[rootpage],[sql] FROM [sqlite_master] WHERE [type]='{SqlitePragmaMasterInfo.TypeIndex}' COLLATE NOCASE AND [tbl_name]='{sqlModel.TagName}' COLLATE NOCASE", null, trans);
            //if (!indexInfos.Any()) { return AlertMsg.OperSuccess; }
            // 表列标记上只支持一个索引
            foreach (var express in expresses)
            {
                var attr = DbColAttribute.GetAttribute(express.GetPropertyInfo());
                if (attr.Ignore) { continue; }
                string createSql = GetIndexCheckSql(attr, sqlModel, isDrop);
                if (createSql == null) { continue; }
                conn.Execute(createSql, null, trans);
            }
            return AlertMsg.OperSuccess;
        }

        private static string GetIndexCheckSql(DbColAttribute attr, AutoSqlModel sqlModel, bool isDrop)
        {
            if (attr.Key == DbIxType.UIX)
            {
                if (string.IsNullOrEmpty(attr.Index))
                {
                    return $"CREATE UNIQUE INDEX IF NOT EXISTS [IX_{sqlModel.TagName}_{attr.Name}] ON [{sqlModel.TagName}]([{attr.Name}])";
                }
                else
                {
                    return $"CREATE UNIQUE INDEX IF NOT EXISTS [IX_{sqlModel.TagName}_{attr.Index.Replace("|", "_")}] ON [{sqlModel.TagName}]([{attr.Index.Replace("|", "],[")}])";
                }
            }
            else if (attr.Key == DbIxType.IX)
            {
                if (string.IsNullOrEmpty(attr.Index))
                {
                    return $"CREATE INDEX IF NOT EXISTS [IX_{sqlModel.TagName}_{attr.Name}] ON [{sqlModel.TagName}]([{attr.Name}])";
                }
                else
                {
                    return $"CREATE INDEX IF NOT EXISTS [IX_{sqlModel.TagName}_{attr.Index.Replace("|", "_")}] ON [{sqlModel.TagName}]([{attr.Index.Replace("|", "],[")}])";
                }
            }
            else if (isDrop)
            {
                if (string.IsNullOrEmpty(attr.Index))
                {
                    return $"DROP INDEX IF EXISTS [IX_{sqlModel.TagName}_{attr.Name}]";
                }
                else
                {
                    return $"DROP INDEX IF EXISTS [IX_{sqlModel.TagName}_{attr.Index.Replace("|", "_")}]";
                }
            }
            return null;
        }

        private static String GetColumnDefinition(DbColAttribute dbCol)
        {
            String defType;
            var defVal = "";
            switch (dbCol.Type)
            {
                case DbColType.Guid:
                case DbColType.String:
                case DbColType.XmlString:
                case DbColType.StringMax:
                case DbColType.StringMedium:
                case DbColType.StringNormal: defType = string.Format("TEXT"); defVal = ""; break;
                case DbColType.Enum:
                case DbColType.Boolean:
                case DbColType.Byte:
                case DbColType.Char:
                case DbColType.Int16:
                case DbColType.Int32:
                case DbColType.Int64:
                case DbColType.UByte:
                case DbColType.UInt16:
                case DbColType.UInt32:
                case DbColType.UInt64: defType = string.Format("INTEGER"); defVal = "0"; break;
                case DbColType.Single:
                case DbColType.Double: defType = string.Format("REAL"); defVal = "0"; break;
                case DbColType.Decimal: defType = string.Format("DECIMAL({0},{1})", dbCol.Len, dbCol.Digit); defVal = "0"; break;
                case DbColType.DateTime: defType = string.Format("DATETIME"); defVal = "1970-1-1 00:00:00"; break;
                case DbColType.JsonString: defType = string.Format("TEXT"); defVal = ""; break;
                case DbColType.Set:
                case DbColType.Blob: defType = string.Format("BLOB"); defVal = ""; break;
                default: defType = string.Format("TEXT"); defVal = ""; break;
            }
            var nullable = dbCol.IsReq ? "NOT" : "";
            var keyable = dbCol.Key == DbIxType.PK ? " PRIMARY KEY" : (dbCol.Key == DbIxType.APK ? " PRIMARY KEY AUTOINCREMENT" : "");
            return $"[{dbCol.Name}] {defType} {nullable} NULL{keyable}{(dbCol.IsReq ? $" DEFAULT '{dbCol.Default ?? defVal}'" : "")}";
        }
    }
}
