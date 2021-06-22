using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Dabber
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
                return new AlertMsg<T>(true, "")
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
        /// 执行SQL语句
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
        /// 安全执行SQL语句
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
                Console.Write(ex);
                return new AlertMsg(false, "执行错误:{0}", ex.Message);
            }
        }
        /// <summary>
        /// 安全执行SQL语句
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
                    if (trans != null) { trans.Rollback(); }
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
        public virtual DataTable GetDataTable(string sql, Dictionary<string, object> args = null)
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
    }
}
