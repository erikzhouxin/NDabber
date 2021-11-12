﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Dabber;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
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
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class DbMigrationAttribute : Attribute
    {

    }
}