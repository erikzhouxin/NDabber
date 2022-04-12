﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Dabber;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Cobber
{
    /// <summary>
    /// 抽象
    /// </summary>
    public abstract class ADatabaseDataAccess
    {
        /// <summary>
        /// 检查字典
        /// </summary>
        protected ConcurrentDictionary<Type, bool> CheckedDic = new ConcurrentDictionary<Type, bool>();
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected virtual String ConnString { get; set; }
        /// <summary>
        /// 连接
        /// </summary>
        protected abstract DbConnection Connection { get; }
        /// <summary>
        /// 链接字符串构造
        /// </summary>
        /// <param name="connString"></param>
        protected ADatabaseDataAccess(String connString)
        {
            ConnString = connString;
        }
        /// <summary>
        /// 获取第一个或默认的泛型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T QueryFOD<T>(string sql, object args = null)
        {
            using (var conn = Connection)
            {
                return conn.QueryFirstOrDefault<T>(sql, args);
            }
        }
        /// <summary>
        /// 获取第一个或默认的泛型对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryQueryFOD(string sql, object args = null)
        {
            try
            {
                dynamic item;
                using (var conn = Connection)
                {
                    item = conn.QueryFirstOrDefault(sql, args);
                }
                return new AlertMsg(true, "")
                {
                    Data = item,
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 获取第一个或默认动态类型对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual dynamic QueryFOD(string sql, object args = null)
        {
            using (var conn = Connection)
            {
                return conn.QueryFirstOrDefault(sql, args);
            }
        }
        /// <summary>
        /// 获取动态类型对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg<T> TryQueryFOD<T>(string sql, object args = null)
        {
            try
            {
                T item;
                using (var conn = Connection)
                {
                    item = conn.QueryFirstOrDefault<T>(sql, args);
                }
                return new AlertMsg<T>(item.IsNotDefault(), "")
                {
                    Data = item,
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new AlertMsg<T>(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> Query<T>()
        {
            return Query<T>(GetSqlModel<T>().Select);
        }
        /// <summary>
        /// 获取泛型类型对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Query<T>(String sql, object args = null)
        {
            using (var conn = Connection)
            {
                return conn.Query<T>(sql, args);
            }
        }
        /// <summary>
        /// 获取泛型类型对象列表及计数
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<IEnumerable<T>, int> QueryAndCount<T>(String querySql, string countSql, object queryArgs = null, object countArgs = null)
        {
            using (var conn = Connection)
            {
                return new Tuple<IEnumerable<T>, int>(conn.Query<T>(querySql, queryArgs), conn.QueryFirstOrDefault<int>(countSql, countArgs));
            }
        }
        /// <summary>
        /// 获取动态类型对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsgs<T> TryQuery<T>(string sql, object args = null)
        {
            try
            {
                IEnumerable<T> items;
                using (var conn = Connection)
                {
                    items = conn.Query<T>(sql, args);
                }
                return new AlertMsgs<T>(true, "")
                {
                    Data = items,
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new AlertMsgs<T>(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 获取动态类型对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IEnumerable<dynamic> Query(string sql, object args = null)
        {
            using (var conn = Connection)
            {
                return conn.Query(sql, args);
            }
        }
        /// <summary>
        /// 获取动态类型对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryQuery(string sql, object args = null)
        {
            try
            {
                IEnumerable<dynamic> items;
                using (var conn = Connection)
                {
                    items = conn.Query(sql, args);
                }
                return new AlertMessage(true, "")
                {
                    Data = items,
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new AlertMessage(false, "查询错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 执行SQL语句(无事务)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual int Execute(string sql, object args = null)
        {
            using (var conn = Connection)
            {
                return conn.Execute(sql, args);
            }
        }
        /// <summary>
        /// 执行SQL语句(事务执行)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual int ExecuteTransaction(string sql, object args = null)
        {
            using (var conn = Connection)
            {
                DbTransaction trans = null;
                try
                {
                    trans = conn.BeginTransaction();
                    var effLine = conn.Execute(sql, args);
                    trans.Commit();
                    return effLine;
                }
                catch (Exception ex)
                {
                    trans?.Rollback();
                    Console.WriteLine(ex);
                    return -1;
                }
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 无影响行数判断
        /// 无Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryExecute(string sql, object args = null)
        {
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sql, args);
                }
                return new AlertMsg(true, "")
                {
                    Data = new
                    {
                        EffLine = effLine,
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 无影响行数判断
        /// 无Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryExecute(string sql, IEnumerable<KeyValuePair<string, object>> args)
        {
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sql, args);
                }
                return new AlertMsg(true, "")
                {
                    Data = new
                    {
                        EffLine = effLine,
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 无影响行数判断
        /// 无Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg Execute<T>(string sql, IEnumerable<T> args)
        {
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sql, args);
                }
                return new AlertMsg(true, "")
                {
                    Data = new
                    {
                        EffLine = effLine,
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 无影响行数判断
        /// 无Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg Execute<T>(string sql, object args)
        {
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sql, args);
                }
                return new AlertMsg(true, "")
                {
                    Data = new
                    {
                        EffLine = effLine,
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 影响行数判断
        /// 无Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryExecuteLine(string sql, object args = null)
        {
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sql, args);
                }
                return new AlertMsg(effLine > 0, $"执行成功，影响行数{effLine}")
                {
                    Data = new
                    {
                        EffLine = effLine,
                    }
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 带事务的影响行数判断批量执行
        /// 无Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryExecuteLine<T>(string sql, IEnumerable<T> args)
        {
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    IDbTransaction trans = null;
                    try
                    {
                        trans = conn.BeginTransaction();
                        effLine = conn.Execute(sql, args);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans?.Rollback();
                        Console.WriteLine(ex);
                    }
                }
                return new AlertMsg(effLine > 0, $"执行成功，影响行数{effLine}")
                {
                    Data = new
                    {
                        EffLine = effLine,
                    }
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
        /// 带事务的影响行数判断单条执行
        /// 有Connection异常
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAlertMsg TryExecute<T>(string sql, IEnumerable<T> args)
        {
            using (var conn = Connection)
            {
                IDbTransaction trans = null;
                try
                {
                    var effLine = 0;
                    trans = conn.BeginTransaction();
                    foreach (var item in args)
                    {
                        effLine += conn.Execute(sql, item, trans);
                    }
                    trans.Commit();
                    return new AlertMsg(effLine > 0, $"执行成功，影响行数{effLine}")
                    {
                        Data = new
                        {
                            EffLine = effLine,
                        }
                    };
                }
                catch (Exception ex)
                {
                    trans?.Rollback();
                    Console.Write(ex);
                    return new AlertMsg(false, "执行错误:{0}", ex.Message);
                }
            }
        }
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, IEnumerable<KeyValuePair<string, object>> args = null)
        {
            using (var conn = Connection)
            {
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        if (args != null)
                        {
                            foreach (var item in args)
                            {
                                var pam = cmd.CreateParameter();
                                pam.ParameterName = item.Key;
                                pam.Value = item.Value;
                                cmd.Parameters.Add(pam);
                            }
                        }
                        using (var sdr = cmd.ExecuteReader())
                        {
                            return sdr.GetDataTable();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    return new DataTable("Empty");
                }
            }
        }
        /// <summary>
        /// 获取SQL模型
        /// </summary>
        /// <typeparam name="T">带有DbCol标记的实体类</typeparam>
        /// <returns></returns>
        public virtual AutoSqlModel GetSqlModel<T>() => GetSqlModel(typeof(T));
        /// <summary>
        /// 获取SQL模型
        /// </summary>
        /// <returns></returns>
        public abstract AutoSqlModel GetSqlModel(Type type);
        /// <summary>
        /// 获取检查类型
        /// 不管成功与否都返回自动SQL模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual AutoSqlModel CheckTable<T>()
        {
            var type = typeof(T);
            var sqlModel = GetSqlModel<T>();
            if (CheckedDic.ContainsKey(type))
            {
                return sqlModel;
            }
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sqlModel.Create);
                }
                CheckedDic.TryAdd(type, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sqlModel;
        }
        /// <summary>
        /// 获取检查类型
        /// 不管成功与否都返回自动SQL模型
        /// </summary>
        /// <returns></returns>
        public virtual IAlertMsg<AutoSqlModel> CheckTable(Type type)
        {
            var sqlModel = GetSqlModel(type);
            if (CheckedDic.TryGetValue(type, out bool isChecked))
            {
                return new AlertMsg<AutoSqlModel>(isChecked, "") { Data = sqlModel };
            }
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sqlModel.Create);
                }
                CheckedDic.TryAdd(type, true);
                return new AlertMsg<AutoSqlModel>(true, "") { Data = sqlModel };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg<AutoSqlModel>(false, ex.Message) { Data = sqlModel };
            }
        }
        /// <summary>
        /// 获取检查类型
        /// 不管成功与否都输出自动SQL模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool CheckTable<T>(out AutoSqlModel sqlModel)
        {
            var type = typeof(T);
            sqlModel = GetSqlModel<T>();
            if (CheckedDic.TryGetValue(type, out bool isChecked))
            {
                return isChecked;
            }
            try
            {
                var effLine = 0;
                using (var conn = Connection)
                {
                    effLine = conn.Execute(sqlModel.Create);
                }
                CheckedDic.TryAdd(type, true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
