using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Dabber;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Cobber
{
    #region // ADatabaseAccess
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
    #endregion
    #region // SqlAutoable
    /// <summary>
    /// 自动SQL模板
    /// </summary>
    public interface IAutoSqlModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 创建表SQL
        /// CREATE TABLE IF NOT EXISTS ...(?...)
        /// </summary>
        string Create { get; }
        /// <summary>
        /// 插入SQL
        /// INSERT INTO ...(?...) VALUES(@...)
        /// </summary>
        string Insert { get; }
        /// <summary>
        /// 无添有换
        /// REPLACE INTO ...(?...) VALUES(@...)
        /// </summary>
        string Replace { get; }
        /// <summary>
        /// 更新Sql
        /// UPDATE ... SET ?...=@... WHERE ?...=@...
        /// </summary>
        string UpdateID { get; }
        /// <summary>
        /// 更新Sql不包含Where
        /// UPDATE ... SET ?...=@... 
        /// </summary>
        string Update { get; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?...=@...
        /// </summary>
        string DeleteID { get; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?... IN @...
        /// </summary>
        string DeleteInID { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ...
        /// </summary>
        string Select { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?...=@...
        /// </summary>
        string SelectID { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?... IN @... 
        /// </summary>
        string SelectInID { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... LIMIT @Skip,@Take
        /// </summary>
        string SelectLimit { get; }
        /// <summary>
        /// 计数
        /// SELECT COUNT(*) FROM ...
        /// </summary>
        string SelectCount { get; }
        /// <summary>
        /// 获取列的集合
        /// </summary>
        string[] Cols { get; }
        /// <summary>
        /// 标记名称(不包括分隔符)
        /// </summary>
        String TagName { get; }
        /// <summary>
        /// 标记主键名称(不包括分隔符)
        /// </summary>
        String TagID { get; }
        /// <summary>
        /// 主键WHERE(不包括WHERE关键字)[?...=@...]
        /// </summary>
        string WhereID { get; }
        /// <summary>
        /// 表信息
        /// </summary>
        DbTableModel Table { get; }
    }
    /// <summary>
    /// 自动SQL类型
    /// 限制:
    /// 只支持单一主键,不支持复合主键
    /// 未完成索引添加
    /// </summary>
    public class AutoSqlModel : IAutoSqlModel
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        /// <param name="type"></param>
        public AutoSqlModel(Type type)
        {
            Type = type;
            Table = new DbTableModel(type);
        }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// 创建表SQL
        /// CREATE TABLE IF NOT EXISTS ...(?...)
        /// </summary>
        public string Create { get; set; }
        /// <summary>
        /// 插入SQL
        /// INSERT INTO ...(?...) VALUES(@...)
        /// </summary>
        public string Insert { get; set; }
        /// <summary>
        /// 无添有换
        /// REPLACE INTO ...(?...) VALUES(@...)
        /// </summary>
        public string Replace { get; set; }
        /// <summary>
        /// 更新Sql
        /// UPDATE ... SET ?...=@... WHERE ?...=@...
        /// </summary>
        public string UpdateID { get; set; }
        /// <summary>
        /// 更新Sql不包含Where
        /// UPDATE ... SET ?...=@... WHERE ?...=@...
        /// </summary>
        public string Update { get; set; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?...=@...
        /// </summary>
        public string DeleteID { get; set; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?... IN @...
        /// </summary>
        public string DeleteInID { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ...
        /// </summary>
        public string Select { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?... IN @... 
        /// </summary>
        public string SelectInID { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?...=@...
        /// </summary>
        public string SelectID { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... LIMIT @Skip,@Take
        /// </summary>
        public string SelectLimit { get; set; }
        /// <summary>
        /// 计数
        /// SELECT COUNT(*) FROM ...
        /// </summary>
        public string SelectCount { get; set; }
        /// <summary>
        /// 获取列的集合
        /// </summary>
        public string[] Cols { get => Table.ColumnNames; }
        /// <summary>
        /// 标记名称(不包括分隔符)
        /// </summary>
        public virtual String TagName { get => Table.TableName; }
        /// <summary>
        /// 标记主键名称(不包括分隔符)
        /// </summary>
        public virtual String TagID { get => Table.DefaultTableKey; }
        /// <summary>
        /// 主键WHERE(不包括WHERE关键字)[?...=@...]
        /// </summary>
        public virtual string WhereID { get; set; }
        /// <summary>
        /// 表信息模型
        /// </summary>
        public virtual DbTableModel Table { get; }

        #region // 参数模型
        /// <summary>
        /// 百分号
        /// </summary>
        public const string PERCENT = "%";
        /// <summary>
        /// 下划线
        /// </summary>
        public const string ULINE = "_";
        /// <summary>
        /// 下划线
        /// </summary>
        public const string UNDERLINE = "_";
        /// <summary>
        /// 百分号
        /// </summary>
        public const String PAH = "%";
        /// <summary>
        /// 字符串(SQL的LIKE)
        /// </summary>
        public class StringLike
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            public StringLike() : this(string.Empty, PAH, string.Empty) { }
            /// <summary>
            /// 初始值构造
            /// </summary>
            /// <param name="value"></param>
            public StringLike(string value) : this(PAH, value, PAH) { }
            /// <summary>
            /// 完整构造
            /// </summary>
            public StringLike(string left, string value, string right)
            {
                Value = value;
                Left = left;
                Right = right;
            }
            /// <summary>
            /// 值
            /// </summary>
            public String Value { get; set; }
            /// <summary>
            /// 左边
            /// </summary>
            public String Left { get; set; }
            /// <summary>
            /// 右边
            /// </summary>
            public String Right { get; set; }
            /// <summary>
            /// 转换成字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Left + Value + Right;
            }
            /// <summary>
            /// 隐式转换成StringLike
            /// </summary>
            public static implicit operator StringLike(string value) => new StringLike(value);
            /// <summary>
            /// 隐式转换成StringLike
            /// </summary>
            public static implicit operator StringLike(Tuple<string, string, string> item) => new StringLike(item.Item1, item.Item2, item.Item3);
            /// <summary>
            /// 隐式转换成字符串
            /// </summary>
            public static implicit operator string(StringLike val) => val.ToString();
        }
        /// <summary>
        /// 数组
        /// </summary>
        public class ArrayIn
        {
            /// <summary>
            /// 构造
            /// </summary>
            public ArrayIn() : this(new object[] { }) { }
            /// <summary>
            /// 初始值构造
            /// </summary>
            /// <param name="value"></param>
            public ArrayIn(Array value)
            {
                Value = value;
            }
            /// <summary>
            /// 值
            /// </summary>
            public Array Value { get; set; }
            /// <summary>
            /// 左间隔
            /// </summary>
            public string LSplit { get; set; } = "'";
            /// <summary>
            /// 中间隔
            /// </summary>
            public string MSplit { get; set; } = ",";
            /// <summary>
            /// 右间隔
            /// </summary>
            public string RSplit { get; set; } = "'";
            /// <summary>
            /// 转换成字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Value == null) { return null; }
                if (Value.Length == 0) { return string.Empty; }
                if (Value.Length == 1) { return $"({LSplit}{Value.GetValue(0)}{RSplit})"; }
                var joinString = LSplit + MSplit + RSplit;
                return $"({LSplit}{string.Join(joinString, Value)}{RSplit})";
            }
            /// <summary>
            /// 隐式转换成ArrayIn
            /// </summary>
            public static implicit operator ArrayIn(Array value) => new ArrayIn(value as Array);
            /// <summary>
            /// 隐式转换成字符串
            /// </summary>
            public static implicit operator string(ArrayIn val) => val.ToString();
        }
        /// <summary>
        /// 比较接口
        /// </summary>
        public interface ICompare
        {
            /// <summary>
            /// 标记(>-1:小于,-1:小于等于,0:相等,1大于等于,>1大于)
            /// </summary>
            int Sign { get; }
            /// <summary>
            /// 值
            /// </summary>
            object Value { get; }
            /// <summary>
            /// 获取符号
            /// </summary>
            /// <returns></returns>
            string GetSign();
        }
        /// <summary>
        /// 比较泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Compare<T> : ICompare
        {
            /// <summary>
            /// 构造
            /// </summary>
            protected Compare(T value, int sign)
            {
                Value = value;
                Sign = sign;
            }
            /// <summary>
            /// 值
            /// </summary>
            public virtual T Value { get; }
            /// <summary>
            /// 标记(>-1:小于,-1:小于等于,0:相等,1大于等于,>1大于)
            /// </summary>
            public virtual int Sign { get; }

            object ICompare.Value => this.Value;
            /// <summary>
            /// 获取比较符号
            /// </summary>
            /// <returns></returns>
            public string GetSign()
            {
                switch (Sign)
                {
                    case -2: return "<";
                    case -1: return "<=";
                    case 0: return "=";
                    case 1: return ">=";
                    case 2: return ">";
                    default: return Sign < 0 ? "<" : ">";
                }
            }
            /// <summary>
            /// 隐式转
            /// </summary>
            public static implicit operator Compare<T>(Tuple<T, int> item) => new Compare<T>(item.Item1, item.Item2);
        }
        /// <summary>
        /// 比较区域
        /// </summary>
        public interface ICompareRange
        {
            /// <summary>
            /// 或关系
            /// </summary>
            bool IsOr { get; }
            /// <summary>
            /// 为空
            /// </summary>
            bool IsEmpty { get; }
            /// <summary>
            /// 比较值列表
            /// </summary>
            List<ICompare> Compares { get; }
            /// <summary>
            /// 添加比较
            /// </summary>
            /// <param name="compare"></param>
            /// <returns></returns>
            List<ICompare> Add(ICompare compare);
        }
        /// <summary>
        /// 比较区域
        /// </summary>
        public class CompareRange : ICompareRange
        {
            /// <summary>
            /// 构造
            /// </summary>
            public CompareRange() : this(null) { }
            /// <summary>
            /// 构造
            /// </summary>
            public CompareRange(List<ICompare> compares) : this(compares, false) { }
            /// <summary>
            /// 构造
            /// </summary>
            public CompareRange(List<ICompare> compares, bool isOr)
            {
                Compares = compares ?? new List<ICompare>();
                IsOr = isOr;
            }
            /// <summary>
            /// 比较值列表
            /// </summary>
            public List<ICompare> Compares { get; }
            /// <summary>
            /// 为空
            /// </summary>
            public bool IsEmpty => Compares.Count() == 0;
            /// <summary>
            /// 或关系(默认False)
            /// </summary>
            public bool IsOr { get; set; }

            /// <summary>
            /// 添加到比较值列表
            /// </summary>
            /// <param name="compare"></param>
            /// <returns></returns>
            public List<ICompare> Add(ICompare compare)
            {
                Compares.Add(compare);
                return Compares;
            }
        }
        /// <summary>
        /// 在.. 和 .. 之间
        /// </summary>
        public interface ICompareBetween
        {
            /// <summary>
            /// 获取开始值
            /// </summary>
            /// <returns></returns>
            object Begin { get; }
            /// <summary>
            /// 获取结束值
            /// </summary>
            /// <returns></returns>
            object End { get; }
        }
        /// <summary>
        /// 在.. 和 .. 之间
        /// </summary>
        public class CompareBetween<T> : ICompareBetween
        {
            /// <summary>
            /// 开始值
            /// </summary>
            public T Begin { get; }
            /// <summary>
            /// 结束值
            /// </summary>
            public T End { get; }

            object ICompareBetween.Begin => this.Begin;

            object ICompareBetween.End => this.End;

            /// <summary>
            /// 初始值构造
            /// </summary>
            public CompareBetween(T begin, T end)
            {
                Begin = begin;
                End = end;
            }
            /// <summary>
            /// 隐式转
            /// </summary>
            public static implicit operator CompareBetween<T>(Tuple<T, T> item) => new CompareBetween<T>(item.Item1, item.Item2);
        }
        #endregion
    }
    /// <summary>
    /// 创建模型接口(绕过类型字典使用泛型)
    /// </summary>
    public interface ISqlBuilderModel
    {
        /// <summary>
        /// 获取SQL创建者
        /// </summary>
        /// <returns></returns>
        AutoSqlBuilder GetSqlModel();
    }
    /// <summary>
    /// 自动SQL创建类
    /// </summary>
    public class AutoSqlBuilder : AutoSqlModel, IAutoSqlModel
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public AutoSqlBuilder(Type type) : base(type)
        {

        }
    }

    /// <summary>
    /// 数据库表模型
    /// </summary>
    public class DbTableModel
    {
        /// <summary>
        /// 类型构造
        /// </summary>
        /// <param name="type"></param>
        public DbTableModel(Type type)
        {
            ClassName = type.Name;
            ClassType = type;
            DbCol = type.GetCustomAttribute<DbColAttribute>() ?? throw new NotSupportedException("当前类没有【DbColAttribute】标记");
            TableName = (DbCol.Name ??= type.Name);
            TableComment = Regex.Replace(DbCol.Display, "\\s+", " ");
            var cols = new List<DbColumnModel>();
            var props = new List<PropertyInfo>();
            var colNames = new List<string>();
            var propNames = new List<string>();
            var indexes = new List<DbIndexModel>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!DbColAttribute.TryGetAttribute(prop, out DbColAttribute colAttr)) { continue; }
                var colModel = new DbColumnModel(prop, colAttr);
                switch (colAttr.Key)
                {
                    case DbIxType.PK:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        DefaultTableKey = colModel.ColumnName;
                        DefaultPropertyKey = colModel.PropertyName;
                        break;
                    case DbIxType.APK: // 自增主键
                        DefaultTableKey = colModel.ColumnName;
                        DefaultPropertyKey = colModel.PropertyName;
                        break;
                    case DbIxType.PFK:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        DefaultTableKey = colModel.ColumnName;
                        DefaultPropertyKey = colModel.PropertyName;
                        break;
                    case DbIxType.FK:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        break;
                    case DbIxType.IX:
                    case DbIxType.UIX:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        if (string.IsNullOrWhiteSpace(colModel.Index))
                        {
                            colModel.Index = colModel.ColumnName;
                        }
                        indexes.Add(new DbIndexModel(colModel));
                        break;
                    default:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        break;
                }
                cols.Add(colModel);
                props.Add(prop);
            }
            Columns = cols.ToArray();
            Properties = props.ToArray();
            ColumnNames = colNames.ToArray();
            PropertyNames = propNames.ToArray();
            Indexes = indexes.ToArray();
        }
        /// <summary>
        /// 注解值
        /// </summary>
        public DbColAttribute DbCol { get; }
        /// <summary>
        /// 类名
        /// </summary>
        public virtual string ClassName { get; }
        /// <summary>
        /// 类类型
        /// </summary>
        public virtual Type ClassType { get; }
        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName { get; }
        /// <summary>
        /// 表注释
        /// </summary>
        public virtual String TableComment { get; }

        /// <summary>
        /// 表标识字段
        /// </summary>
        public virtual String[] TableID { get; }
        /// <summary>
        /// 默认数据库字段标识
        /// </summary>
        public virtual string DefaultTableKey { get; }
        /// <summary>
        /// 默认类属性标识
        /// </summary>
        public virtual string DefaultPropertyKey { get; }
        /// <summary>
        /// 列序列
        /// 1.使用时注意自增字段,DbColumnModel.IsAuto
        /// </summary>
        public virtual DbColumnModel[] Columns { get; }
        /// <summary>
        /// 属性列表
        /// </summary>
        public virtual PropertyInfo[] Properties { get; }
        /// <summary>
        /// 列名字段
        /// </summary>
        public virtual string[] ColumnNames { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public virtual string[] PropertyNames { get; }
        /// <summary>
        /// 索引序列
        /// </summary>
        public virtual DbIndexModel[] Indexes { get; }
    }
    /// <summary>
    /// 数据库列模型
    /// </summary>
    public class DbColumnModel : IDbColInfo
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="prop"></param>
        public DbColumnModel(PropertyInfo prop) : this(prop, DbColAttribute.GetAttribute(prop))
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="dbCol"></param>
        public DbColumnModel(PropertyInfo prop, DbColAttribute dbCol)
        {
            dbCol.Name = dbCol.Name ?? prop.Name;
            Property = prop;
            DbCol = dbCol;
            ColumnName = dbCol.Name = dbCol.Name ?? prop.Name;
            ColumnComment = dbCol.Display;
            PropertyName = prop.Name;
            IsPK = dbCol.Key == DbIxType.APK || dbCol.Key == DbIxType.FK || dbCol.Key == DbIxType.PFK;
            IsAuto = dbCol.Key == DbIxType.APK;

            #region // DbCol
            Display = dbCol.Display;
            Name = dbCol.Name ?? prop.Name;
            Type = dbCol.Type;
            Len = dbCol.Len;
            IsReq = dbCol.IsReq;
            Default = dbCol.Default;
            Key = dbCol.Key;
            Index = dbCol.Index;
            Ignore = dbCol.Ignore;
            Digit = dbCol.Digit;
            #endregion
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        public virtual PropertyInfo Property { get; }
        /// <summary>
        /// 列属性
        /// </summary>
        public virtual DbColAttribute DbCol { get; }
        /// <summary>
        /// 列名
        /// </summary>
        public virtual string ColumnName { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public virtual string PropertyName { get; }
        /// <summary>
        /// 列注释
        /// </summary>
        public virtual String ColumnComment { get; }
        /// <summary>
        /// 是主键
        /// </summary>
        public virtual bool IsPK { get; }
        /// <summary>
        /// 是自增
        /// </summary>
        public virtual bool IsAuto { get; }
        #region // DbCol
        /// <summary>
        /// 显示名默认类名
        /// </summary>
        public string Display { get; }
        /// <summary>
        /// 列名默认是属性名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 类型
        /// </summary>
        public DbColType Type { get; }
        /// <summary>
        /// 长度默认64位
        /// </summary>
        public long Len { get; }
        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsReq { get; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object Default { get; }
        /// <summary>
        /// 是否主键
        /// 0=>无
        /// 10=>主键
        /// 20=>主键+外键
        /// 30=>外键
        /// </summary>
        public DbIxType Key { get; }
        /// <summary>
        /// 索引名,使用|分割
        /// </summary>
        public string Index { get; internal set; }
        /// <summary>
        /// 忽略映射
        /// </summary>
        public bool Ignore { get; }
        /// <summary>
        /// 精度
        /// </summary>
        public int Digit { get; }
        #endregion
    }
    /// <summary>
    /// 数据库索引模型
    /// </summary>
    public class DbIndexModel
    {
        /// <summary>
        /// 标记列信息
        /// </summary>
        public DbColumnModel Column { get; }
        /// <summary>
        /// 使用列信息
        /// </summary>
        public DbColumnModel[] Columns { get; private set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string[] ColumnNames { get; }
        /// <summary>
        /// 索引名
        /// </summary>
        public String IndexName { get; }
        /// <summary>
        /// 是唯一
        /// </summary>
        public bool IsUnique { get; }
        /// <summary>
        /// 构造
        /// </summary>
        public DbIndexModel(DbColumnModel column)
        {
            Column = column;
            ColumnNames = column.Index.Split('|');
            IndexName = column.Index;
            IsUnique = column.Key == DbIxType.UIX;
        }
    }
    #endregion
    #region // RedisEnum
    /// <summary>
    /// Redis数据库序号功能枚举
    /// </summary>
    public enum RedisDbEnum : int
    {
        /// <summary>
        /// 全局
        /// </summary>
        Global = 0,
        /// <summary>
        /// 定义
        /// </summary>
        Definition = 1,
        /// <summary>
        /// 模型
        /// </summary>
        Model = 2,
        /// <summary>
        /// 实体
        /// </summary>
        Entity = 3,
        /// <summary>
        /// 服务
        /// </summary>
        Service = 4,
        /// <summary>
        /// 组件
        /// </summary>
        Component = 5,
        /// <summary>
        /// 区域
        /// </summary>
        Region = 6,
        /// <summary>
        /// 领域
        /// </summary>
        Domain = 7,
        /// <summary>
        /// 环境
        /// </summary>
        Environment = 8,
        /// <summary>
        /// 设置
        /// </summary>
        Setting = 9,
        /// <summary>
        /// 分享
        /// </summary>
        Share = 10,
        /// <summary>
        /// 订阅
        /// </summary>
        Subscribe = 11,
        /// <summary>
        /// 转换
        /// </summary>
        Transform = 12,
        /// <summary>
        /// 内存
        /// </summary>
        Memory = 13,
        /// <summary>
        /// 公共
        /// </summary>
        Common = 14,
        /// <summary>
        /// 最后
        /// </summary>
        Demon = 15,
    }
    #endregion
}
