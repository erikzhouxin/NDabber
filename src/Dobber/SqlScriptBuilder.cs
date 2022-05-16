using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.Sqller
{
    /// <summary>
    /// Sql脚本创建者
    /// </summary>
    public sealed class SqlScriptBuilder
    {
        /// <summary>
        /// 创建自身对象
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public SqlScriptBuilder Create(StoreType storeType)
        {
            return new SqlScriptBuilder(storeType);
        }
        /// <summary>
        /// 创建自身对象
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public SqlScriptBuilder GetInstance(StoreType storeType)
        {
            return new SqlScriptBuilder(storeType);
        }
        /// <summary>
        /// 创建插入
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptInsertFromBuilder CreateInsert(StoreType storeType)
        {
            return SqlScriptInsertBuilder.Create(storeType);
        }
        /// <summary>
        /// 创建简单查询
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptFromBuilder CreateSimpleSelect(StoreType storeType)
        {
            return SqlScriptSimpleSelectBuilder.Create(storeType);
        }
        /// <summary>
        /// 创建更新语句
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptUpdateFromBuilder CreateUpdate(StoreType storeType)
        {
            return SqlScriptUpdateBuilder.Create(storeType);
        }
        #region // 脚本创建者
        /// <summary>
        /// 存储类型
        /// </summary>
        public StoreType StoreType { get; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storeType"></param>
        private SqlScriptBuilder(StoreType storeType)
        {
            StoreType = storeType;
        }
        /// <summary>
        /// 更新脚本
        /// </summary>
        public ISqlScriptUpdateFromBuilder Updater => GetUpdater();
        /// <summary>
        /// 更新脚本
        /// </summary>
        /// <returns></returns>
        public ISqlScriptUpdateFromBuilder GetUpdater()
        {
            return SqlScriptUpdateBuilder.Create(StoreType);
        }
        /// <summary>
        /// 插入脚本
        /// </summary>
        public ISqlScriptInsertFromBuilder Inserter => GetInserter();
        /// <summary>
        /// 插入脚本
        /// </summary>
        /// <returns></returns>
        public ISqlScriptInsertFromBuilder GetInserter()
        {
            return SqlScriptInsertBuilder.Create(StoreType);
        }
        /// <summary>
        /// 删除脚本
        /// </summary>
        public ISqlScriptDeleteFromBuilder Deleter => GetDeleter();
        /// <summary>
        /// 删除脚本
        /// </summary>
        /// <returns></returns>
        public ISqlScriptDeleteFromBuilder GetDeleter()
        {
            return SqlScriptDeleteBuilder.Create(StoreType);
        }
        #endregion
    }
    /// <summary>
    /// SQL脚本标记模型
    /// </summary>
    public class SqlScriptTagModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 是类型
        /// </summary>
        public bool IsType { get; set; }
        /// <summary>
        /// SQL模型
        /// </summary>
        public AutoSqlModel SqlModel { get; set; }
        /// <summary>
        /// 加括号表名
        /// 没括号的使用[SqlModel.TagName]
        /// </summary>
        public String Table { get; set; }
        /// <summary>
        /// 加括号表序别名
        /// </summary>
        public String TAlias { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public int TCount { get; set; }
    }
    /// <summary>
    /// SQL脚本绑定创建者
    /// </summary>
    internal abstract class ASqlScriptBindBuilder : ISqlScriptParameters
    {
        protected StringBuilder _columnsClause;
        protected StringBuilder _fromClause;
        protected StringBuilder _whereClause;
        protected StringBuilder _setClause;
        protected StringBuilder _groupClause;
        protected StringBuilder _havingClause;
        protected StringBuilder _orderClause;
        protected int? _skip;
        protected int? _take;
        /// <summary>
        /// 获取SQL模型
        /// </summary>
        public virtual Func<Type, AutoSqlBuilder> GetSqlModel { get; protected set; }
        /// <summary>
        /// 获取括号类型
        /// </summary>
        public virtual Func<string, string> GetQuot { get; protected set; }
        /// <summary>
        /// 左括号
        /// </summary>
        public virtual String QuotStart => "(";
        /// <summary>
        /// 右括号
        /// </summary>
        public virtual String QuotEnd => ")";
        /// <summary>
        /// 存储类型
        /// </summary>
        public virtual StoreType StoreType { get; protected set; }
        /// <summary>
        /// 当前
        /// </summary>
        public virtual SqlScriptTagModel CurrentTag { get; protected set; }
        /// <summary>
        /// 参数列表
        /// </summary>

        public virtual Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        public virtual List<SqlScriptTagModel> Tags { get; } = new List<SqlScriptTagModel>();
        /// <summary>
        /// SQL脚本
        /// </summary>
        public virtual String SqlScript => ToString();
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storeType"></param>
        protected ASqlScriptBindBuilder(StoreType storeType)
        {
            SetStoreType(storeType);
        }
        /// <summary>
        /// 设置存储类型
        /// </summary>
        /// <param name="storeType"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected virtual void SetStoreType(StoreType storeType)
        {
            GetSqlModel = storeType switch
            {
                StoreType.SQLite => AutoSQLiteBuilder.Builder,
                StoreType.SqlServer => AutoSqlServerBuilder.Builder,
                StoreType.MySQL => AutoMySqlBuilder.Builder,
                StoreType.Access => AutoAccessBuilder.Builder,
                _ => throw new NotSupportedException(),
            };
            GetQuot = storeType switch
            {
                StoreType.SQLite => AutoSQLiteBuilder.GetQuot,
                StoreType.SqlServer => AutoSqlServerBuilder.GetQuot,
                StoreType.MySQL => AutoMySqlBuilder.GetQuot,
                StoreType.Access => AutoAccessBuilder.GetQuot,
                _ => throw new NotSupportedException(),
            };
            StoreType = storeType;
        }
        /// <summary>
        /// 添加From子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        protected virtual void AddFromClause(string clause)
        {
            if (_fromClause == null) { _fromClause = new StringBuilder(clause); }
            else { _fromClause.Append($", { clause}"); }
        }
        /// <summary>
        /// 添加From子句
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        protected virtual void AddFromJoinOnClause(string joinType, string clause)
        {
            _fromClause.Append($" {joinType} {clause}");
        }
        /// <summary>
        /// 添加Select子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        protected virtual void AddSelectClause(string clause)
        {
            if (_columnsClause == null) { _columnsClause = new StringBuilder(clause); }
            else { _columnsClause.Append($", {clause}"); }
        }
        /// <summary>
        /// 添加Where子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        protected virtual void AddWhereClause(string clause)
        {
            if (_whereClause == null) { _whereClause = new StringBuilder(clause); }
            else { _whereClause.Append($" AND {clause}"); }
        }
        /// <summary>
        /// 添加Set子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        protected virtual void AddSetClause(string clause)
        {
            if (_setClause == null) { _setClause = new StringBuilder(clause); }
            else { _setClause.Append($", {clause}"); }
        }
        /// <summary>
        /// 添加Group子句
        /// </summary>
        /// <param name="clause"></param>
        protected virtual void AddGroupClause(string clause)
        {
            if (_groupClause == null) { _groupClause = new StringBuilder(clause); }
            else { _groupClause.Append($", {clause}"); }
        }
        /// <summary>
        /// 添加having子句
        /// </summary>
        /// <param name="clause"></param>
        protected virtual void AddHavingClause(string clause)
        {
            if (_havingClause == null) { _havingClause = new StringBuilder(clause); }
            else { _havingClause.Append($", {clause}"); }
        }
        /// <summary>
        /// 添加order子句
        /// </summary>
        /// <param name="clause"></param>
        protected virtual void AddOrderClause(string clause)
        {
            if (_orderClause == null) { _orderClause = new StringBuilder(clause); }
            else { _orderClause.Append($", {clause}"); }
        }
        /// <summary>
        /// 添加Where参数
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual void AddParameter(string pName, object value)
        {
            Parameters[pName] = value;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual SqlScriptTagModel AddTable(Type type)
        {
            if (type == null)
            {
                throw new NotSupportedException("添加表类型不能为空");
            }
            var sqlModel = GetSqlModel(type);
            Tags.Add(CurrentTag = new SqlScriptTagModel
            {
                Type = type,
                IsType = type != null,
                SqlModel = sqlModel,
                TCount = Tags.Count,
                TAlias = GetQuot($"t{Tags.Count}"),
                Table = GetQuot(sqlModel.TagName),
            });
            return CurrentTag;
        }
        /// <summary>
        /// 获取懒加载内容
        /// </summary>
        /// <returns></returns>
        public Tuble2SqlArgs GetSqlWithArgs(object args)
        {
            SetParameter(args);
            return new Tuble2SqlArgs(ToString(), Parameters);
        }
        /// <summary>
        /// 获取懒加载内容
        /// </summary>
        /// <returns></returns>
        public Tuble2SqlArgs GetSqlAndArgs(IDictionary<string, object> args)
        {
            if (args != null)
            {
                foreach (var arg in args)
                {
                    Parameters[arg.Key] = arg.Value;
                }
            }
            return new Tuble2SqlArgs(ToString(), Parameters);
        }
        /// <summary>
        /// 获取懒加载内容
        /// </summary>
        /// <returns></returns>
        public Tuble2SqlArgs GetSqlAndArgs(params KeyValuePair<string, object>[] args)
        {
            if (args != null)
            {
                foreach (var arg in args)
                {
                    Parameters[arg.Key] = arg.Value;
                }
            }
            return new Tuble2SqlArgs(ToString(), Parameters);
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="args"></param>
        public ISqlScriptParameters SetParameter<T>(T args) where T : class
        {
            if (args == null) { return this; }
            if (args is IEnumerable<KeyValuePair<string, object>> list)
            {
                foreach (KeyValuePair<string, object> item in list)
                {
                    Parameters[item.Key] = item.Value;
                }
                return this;
            }
            var access = PropertyAccess.Get(args.GetType());
            foreach (var item in Parameters)
            {
                access.FuncGetValue(args, item.Key);
            }
            return this;
        }
        public virtual string GetLimit()
        {
            if (!_take.HasValue)
            {
                return string.Empty;
            }
            switch (StoreType)
            {
                case StoreType.SQLite:
                case StoreType.MySQL:
                    return _skip.HasValue ? $" LIMIT {_skip.Value},{_take.Value}" : $" LIMIT {_take.Value}";
                case StoreType.SqlServer:
                    break;
                case StoreType.Oracle:
                    break;
                case StoreType.PostgreSQL:
                    break;
                case StoreType.Redis:
                    break;
                case StoreType.Access:
                    break;
                case StoreType.Excel:
                    break;
                case StoreType.Xml:
                    break;
                case StoreType.Memory:
                    break;
                default:
                    break;
            }
            return String.Empty;
        }
    }

    #region // SqlScriptDeleteBuilder
    /// <summary>
    /// 删除脚本From部分
    /// </summary>
    public interface ISqlScriptDeleteFromBuilder
    {
        /// <summary>
        /// 使用逻辑删除
        /// </summary>
        /// <returns></returns>
        ISqlScriptDeleteLogicalFromBuilder UseLogical<T>();
        /// <summary>
        /// 使用逻辑删除
        /// </summary>
        /// <returns></returns>
        ISqlScriptDeleteLogicalFromBuilder UseLogical(Type type);
        /// <summary>
        /// 删除条件
        /// </summary>
        ISqlScriptDeleteWhereBuilder Where<T>();
    }
    /// <summary>
    /// 逻辑删除逻辑
    /// </summary>
    public interface ISqlScriptDeleteLogicalFromBuilder
    {
        /// <summary>
        /// 删除条件
        /// </summary>
        ISqlScriptDeleteWhereBuilder Where();
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptDeleteWhereBuilder SetAParam(string name, object value = null);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptDeleteWhereBuilder SetBParam(string cName, string pName, object value = null);
    }
    /// <summary>
    /// 删除脚本Where部分
    /// </summary>
    public interface ISqlScriptDeleteWhereBuilder : ISqlScriptParameters
    {

    }
    /// <summary>
    /// SQL脚本删除创建
    /// </summary>
    internal class SqlScriptDeleteBuilder : ASqlScriptBindBuilder, ISqlScriptDeleteFromBuilder, ISqlScriptDeleteLogicalFromBuilder, ISqlScriptDeleteWhereBuilder
    {
        #region // 静态定义
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptDeleteFromBuilder Create(StoreType storeType)
        {
            return new SqlScriptDeleteBuilder(storeType);
        }
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        private SqlScriptDeleteBuilder(StoreType storeType) : base(storeType) { }
        /// <summary>
        /// 获取SQL脚本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_isLogical)
            {
                return new StringBuilder("UPDATE ")
                    .Append(CurrentTag.TAlias)
                    .ToString();
            }
            return new StringBuilder("DELETE FROM ")
                .Append(CurrentTag.TAlias)
                .Append(_whereClause == null ? "" : $" WHERE {_whereClause}")
                .ToString();
        }

        /// <summary>
        /// 删除条件
        /// </summary>
        public ISqlScriptDeleteWhereBuilder Where => this;

        #region // 逻辑删除部分
        private bool _isLogical;
        /// <summary>
        /// 使用逻辑删除
        /// </summary>
        /// <returns></returns>
        ISqlScriptDeleteLogicalFromBuilder ISqlScriptDeleteFromBuilder.UseLogical<T>()
        {
            _isLogical = true;
            AddTable(typeof(T));
            return this;
        }
        /// <summary>
        /// 使用逻辑删除
        /// </summary>
        /// <returns></returns>
        ISqlScriptDeleteLogicalFromBuilder ISqlScriptDeleteFromBuilder.UseLogical(Type type)
        {
            _isLogical = true;
            AddTable(type);
            return this;
        }

        ISqlScriptDeleteWhereBuilder ISqlScriptDeleteLogicalFromBuilder.SetAParam(string name, object value)
        {
            throw new NotImplementedException();
        }

        ISqlScriptDeleteWhereBuilder ISqlScriptDeleteLogicalFromBuilder.SetBParam(string cName, string pName, object value)
        {
            throw new NotImplementedException();
        }

        ISqlScriptDeleteWhereBuilder ISqlScriptDeleteLogicalFromBuilder.Where()
        {
            return this;
        }

        ISqlScriptDeleteWhereBuilder ISqlScriptDeleteFromBuilder.Where<T>()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #endregion // SqlScriptDeleteBuilder
    #region // SqlScriptInsertBuilder
    /// <summary>
    /// 插入From表创建
    /// </summary>
    public interface ISqlScriptInsertFromBuilder
    {
        /// <summary>
        /// 指定表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptParameters From<T>();
        /// <summary>
        /// 指定类名
        /// </summary>
        /// <returns></returns>
        ISqlScriptParameters From(Type type);
        /// <summary>
        /// 指定表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptInsertWhereBuilder UseSelect<T>();
        /// <summary>
        /// 指定类名
        /// </summary>
        /// <returns></returns>
        ISqlScriptInsertWhereBuilder UseSelect(Type type);
    }
    /// <summary>
    /// 插入的Where部分
    /// </summary>
    public interface ISqlScriptInsertWhereBuilder : ISqlScriptParameters
    {
        /// <summary>
        /// 添加查询结果
        /// </summary>
        /// <param name="selectResult"></param>
        /// <returns></returns>
        ISqlScriptParameters Select(ISqlScriptParameters selectResult);
    }
    internal class SqlScriptInsertBuilder : ASqlScriptBindBuilder, ISqlScriptInsertFromBuilder, ISqlScriptInsertWhereBuilder
    {
        #region // 静态定义
        public static ISqlScriptInsertFromBuilder Create(StoreType storeType)
        {
            return new SqlScriptInsertBuilder(storeType);
        }
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storeType"></param>
        private SqlScriptInsertBuilder(StoreType storeType) : base(storeType) { }
        #region // From部分
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptParameters ISqlScriptInsertFromBuilder.From<T>() => From(typeof(T));
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual ISqlScriptParameters From(Type type)
        {
            AddTable(type);
            return this;
        }
        #endregion // From部分
        /// <summary>
        /// 获取SQL脚本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_isSelect)
            {
                return new StringBuilder("INSERT INTO ").Append(CurrentTag.Table)
                    .Append(QuotStart).Append(CurrentTag.SqlModel.Table.Columns.Where(s => !s.IsAuto).Select(s => GetQuot(s.ColumnName)).JoinString(",")).Append(QuotEnd)
                    .Append(_selectScript)
                    .ToString();
            }
            return CurrentTag.SqlModel.Insert;
        }
        #region // Select部分
        private bool _isSelect;
        private string _selectScript;
        ISqlScriptInsertWhereBuilder ISqlScriptInsertFromBuilder.UseSelect<T>()
        {
            AddTable(typeof(T));
            _isSelect = true;
            return this;
        }

        ISqlScriptInsertWhereBuilder ISqlScriptInsertFromBuilder.UseSelect(Type type)
        {
            AddTable(type);
            _isSelect = true;
            return this;
        }

        ISqlScriptParameters ISqlScriptInsertWhereBuilder.Select(ISqlScriptParameters selectResult)
        {
            _selectScript = selectResult.SqlScript;
            foreach (var item in selectResult.Parameters)
            {
                Parameters[item.Key] = item.Value;
            }
            return this;
        }
        #endregion
    }
    #endregion // SqlScriptInsertBuilder
    #region // SqlScriptUpdateBuilder
    /// <summary>
    /// SQL脚本更新From部分
    /// </summary>
    public interface ISqlScriptUpdateFromBuilder
    {
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder From<T>();
        /// <summary>
        /// 添加自动类
        /// </summary>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder From(Type type);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder FromWhere(Type type);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder FromWhere<T>();
    }
    /// <summary>
    /// SQL脚本更新Set部分
    /// </summary>
    public interface ISqlScriptUpdateSetBuilder
    {
        /// <summary>
        /// 设置子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetClause(string clause);
        /// <summary>
        /// 添加字段名等于属性名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetAParam(string name, object value = null);
        /// <summary>
        /// 添加字段名等于属性名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetAParam<TM>(Expression<Func<TM, object>> prop, object value = null);
        /// <summary>
        /// 添加字段名等于属性名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetBParam<TM>(Expression<Func<TM, object>> prop, string pName, object value = null);
        /// <summary>
        /// 添加字段名及属性名分开
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetBParam(string cName, string pName, object value = null);
        /// <summary>
        /// 添加字段常量赋值
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="value"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetConstantParam(string cName, object value, bool isQuot = false);
        /// <summary>
        /// 添加字段常量赋值
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetConstantParam<TM>(Expression<Func<TM, object>> prop, object value, bool isQuot = false);
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder Column<T>(Expression<Func<T>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder Column<TM>(Expression<Func<TM, object>> prop, string pName = null);
        /// <summary>
        /// 获取Where创建者
        /// </summary>
        ISqlScriptUpdateWhereBuilder Where();
    }
    /// <summary>
    /// SQL脚本更新Where部分
    /// </summary>
    public interface ISqlScriptUpdateWhereBuilder : ISqlScriptParameters
    {
        /// <summary>
        /// AND相等参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndEqualAParam(string name, object value = null);
        /// <summary>
        /// AND相等参数
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndEqualAParam<TM>(Expression<Func<TM, object>> prop, object value = null);
        /// <summary>
        /// AND相等参数
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndEqualBParam<TM>(Expression<Func<TM, object>> prop, string pName, object value = null);
        /// <summary>
        /// AND相等参数
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndEqualBParam(string cName, string pName, object value = null);
        /// <summary>
        /// AND相等常量参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="hasQuot"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndEqualConstantParam(string name, object value, bool hasQuot = false);
        /// <summary>
        /// AND相等常量参数
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="hasQuot"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndEqualConstantParam<TM>(Expression<Func<TM, object>> prop, object value, bool hasQuot = false);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndInAParam(string name, IEnumerable value = null);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndInAParam<TM>(Expression<Func<TM, object>> prop, IEnumerable value = null);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndInBParam(string cName, string pName, IEnumerable value = null);
        /// <summary>
        /// AND为空
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndIsNull(string name);
        /// <summary>
        /// AND为空
        /// </summary>
        /// <typeparam name="TM"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndIsNull<TM>(Expression<Func<TM, object>> prop);
        /// <summary>
        /// AND不为空
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndIsNotNull(string name);
        /// <summary>
        /// AND不为空
        /// </summary>
        /// <typeparam name="TM"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndIsNotNull<TM>(Expression<Func<TM, object>> prop);
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndColumn<T>(Expression<Func<T>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndColumn<TM, TP>(Expression<Func<TM, TP>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder AndColumn<TM>(Expression<Func<TM, object>> prop, string pName = null);
    }
    /// <summary>
    /// SQL脚本更新创建者
    /// </summary>
    internal class SqlScriptUpdateBuilder : ASqlScriptBindBuilder, ISqlScriptUpdateFromBuilder, ISqlScriptUpdateSetBuilder, ISqlScriptUpdateWhereBuilder
    {
        #region // 静态定义
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptUpdateFromBuilder Create(StoreType storeType)
        {
            return new SqlScriptUpdateBuilder(storeType);
        }
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storeType"></param>
        private SqlScriptUpdateBuilder(StoreType storeType) : base(storeType)
        {
        }
        /// <summary>
        /// 获取SQL脚本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new StringBuilder("UPDATE ")
                .Append(CurrentTag.Table).Append(" SET ")
                .Append(_setClause)
                .Append(_whereClause == null ? "" : $" WHERE {_whereClause}")
                .ToString();
        }
        #region // From部分
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder ISqlScriptUpdateFromBuilder.From<T>() => From(typeof(T));
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual ISqlScriptUpdateSetBuilder From(Type type)
        {
            AddTable(type);
            return this;
        }
        /// <summary>
        /// 设置修改表并添加修改所有字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual ISqlScriptUpdateWhereBuilder FromWhere(Type type)
        {
            AddTable(type);
            foreach (var item in CurrentTag.SqlModel.Table.Columns)
            {
                AddSetClause($"{GetQuot(item.ColumnName)}=@{GetQuot(item.PropertyName)}");
                Parameters.Add(item.PropertyName, DefaultValue.Get(item.Property.PropertyType));
            }
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateFromBuilder.FromWhere<T>() => FromWhere(typeof(T));
        #endregion // From部分
        #region // Set部分
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateSetBuilder.Where() => this;

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetClause(string clause)
        {
            AddSetClause(clause);
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetAParam(string name, object value)
        {
            AddSetClause($"{GetQuot(name)}=@{name}");
            Parameters[name] = value;
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetAParam<TM>(Expression<Func<TM, object>> prop, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)}=@{keyName.Name}");
                Parameters[keyName.Name] = value;
            }
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetBParam(string cName, string pName, object value)
        {
            AddSetClause($"{GetQuot(cName)}=@{pName}");
            Parameters[pName] = value;
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetBParam<TM>(Expression<Func<TM, object>> prop, string pName, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
                Parameters[pName ?? keyName.Name] = value;
            }
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.Column<T>(Expression<Func<T>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
            }
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
            }
            return this;
        }

        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.Column<TM>(Expression<Func<TM, object>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
            }
            return this;
        }
        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetConstantParam(string cName, object value, bool isQuot)
        {
            AddSetClause($"{GetQuot(cName)}={(isQuot ? $"'{value}'" : value)}");
            return this;
        }
        ISqlScriptUpdateSetBuilder ISqlScriptUpdateSetBuilder.SetConstantParam<TM>(Expression<Func<TM, object>> prop, object value, bool isQuot)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)}={(isQuot ? $"'{value}'" : value)}");
            }
            return this;
        }
        #endregion // 设置部分
        #region // Where部分
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndEqualAParam(string name, object value)
        {
            AddWhereClause($"{GetQuot(name)}=@{name}");
            Parameters[name] = value;
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndEqualAParam<TM>(Expression<Func<TM, object>> prop, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)}=@{keyName.Name}");
                Parameters[keyName.Name] = value;
            }
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndEqualBParam(string cName, string pName, object value)
        {
            AddWhereClause($"{GetQuot(cName)}=@{pName}");
            Parameters[pName] = value;
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndEqualBParam<TM>(Expression<Func<TM, object>> prop, string pName, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)}=@{pName}");
                Parameters[pName] = value;
            }
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndEqualConstantParam(string name, object value, bool isQuot)
        {
            AddWhereClause($"{GetQuot(name)}={(isQuot ? $"'{value}'" : value)}");
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndEqualConstantParam<TM>(Expression<Func<TM, object>> prop, object value, bool isQuot)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)}={(isQuot ? $"'{value}'" : value)}");
            }
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndIsNull(string name)
        {
            AddWhereClause($"{GetQuot(name)} IS NULL");
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndIsNull<TM>(Expression<Func<TM, object>> prop)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)} IS NULL");
            }
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndIsNotNull(string name)
        {
            AddWhereClause($"{GetQuot(name)} IS NOT NULL");
            return this;
        }

        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndIsNotNull<TM>(Expression<Func<TM, object>> prop)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSetClause($"{GetQuot(keyName.Key)} IS NOT NULL");
            }
            return this;
        }
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndInAParam(string name, IEnumerable value)
        {
            AddWhereClause($"{GetQuot(name)} IN @{name}");
            Parameters[name] = value;
            return this;
        }
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndInAParam<TM>(Expression<Func<TM, object>> prop, IEnumerable value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)} IN @{keyName.Name}");
                Parameters[keyName.Name] = value;
            }
            return this;
        }
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndInBParam(string cName, string pName, IEnumerable value)
        {
            AddWhereClause($"{GetQuot(cName)} IN @{pName}");
            Parameters[pName] = value;
            return this;
        }
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndColumn<T>(Expression<Func<T>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
            }
            return this;
        }
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndColumn<TM, TP>(Expression<Func<TM, TP>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
            }
            return this;
        }
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptUpdateWhereBuilder ISqlScriptUpdateWhereBuilder.AndColumn<TM>(Expression<Func<TM, object>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{GetQuot(keyName.Key)}=@{pName ?? keyName.Name}");
            }
            return this;
        }
        #endregion Where部分
    }
    #endregion // SqlScriptUpdateBuilder
    #region // SqlScriptSimpleSelectBuilder
    /// <summary>
    /// SQL的参数字典
    /// </summary>
    public interface ISqlScriptParameters
    {
        /// <summary>
        /// 参数列表
        /// </summary>
        Dictionary<string, object> Parameters { get; }
        /// <summary>
        /// sql脚本
        /// </summary>
        string SqlScript { get; }
        /// <summary>
        /// 获取SQL及模型参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Tuble2SqlArgs GetSqlWithArgs(object model);
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        ISqlScriptParameters SetParameter<T>(T model) where T : class;
    }
    /// <summary>
    /// 查询创建
    /// </summary>
    public interface ISqlScriptSelectBuilder
    {
        /// <summary>
        /// 添加子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder AddClause(string clause);
        /// <summary>
        /// 添加名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Add(string name);
        /// <summary>
        /// 添加字段及属性名
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Add(string cName, string pName);
        /// <summary>
        /// 添加别名及字段及属性名
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Add(string tag, string cName, string pName);
        /// <summary>
        /// 添加一个类的所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptSelectBuilder Add<T>();
        /// <summary>
        /// 获取Where创建者
        /// </summary>
        ISqlScriptWhereBuilder Where();
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Column<T>(Expression<Func<T>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Column<TM>(Expression<Func<TM, object>> prop, string pName = null);
    }
    /// <summary>
    /// From创建者
    /// </summary>
    public interface ISqlScriptFromBuilder
    {
        /// <summary>
        /// 添加自动类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptFromBuilder From<T>();
        /// <summary>
        /// 添加自动类
        /// </summary>
        /// <returns></returns>
        ISqlScriptFromBuilder From(Type type);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder FromWhere(Type type);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <returns></returns>
        ISqlScriptWhereBuilder FromWhere<T>();
        /// <summary>
        /// 获取一个查询
        /// </summary>
        /// <returns></returns>
        ISqlScriptSelectBuilder Select();
    }
    /// <summary>
    /// Where创建者
    /// </summary>
    public interface ISqlScriptWhereBuilder : ISqlScriptParameters
    {
        /// <summary>
        /// 添加子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AddClause(string clause);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualAParam(string cName, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualAParam<TM>(Expression<Func<TM, object>> prop, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualBParam(string cName, string pName, object value = null);
        /// <summary>
        /// 添加where常量条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="value"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualConstant(string cName, object value, bool isQuot = false);
        /// <summary>
        /// 添加where常量条件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualConstant<TM>(Expression<Func<TM, object>> prop, object value, bool isQuot = false);
        /// <summary>
        /// 添加where常量条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndBetweenEqualConstant(string cName, object val1, object val2, bool isQuot = false);
        /// <summary>
        /// 添加where常量条件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndBetweenEqualConstant<TM>(Expression<Func<TM, object>> prop, object val1, object val2, bool isQuot = false);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndLikeAParam(string name, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndLikeAParam<TM>(Expression<Func<TM, object>> prop, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndLikelBParam(string cName, string pName, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndSymbolAParam(string name, string symbol, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndSymbolAParam<TM>(Expression<Func<TM, object>> prop, string symbol, object value = null);
        /// <summary>
        /// 添加Where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="symbol"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndSymbolBParam(string cName, string symbol, string pName, object value = null);
        /// <summary>
        /// 添加Where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndSymbolConstant(string cName, string symbol, object value, bool isQuot = false);
        /// <summary>
        /// 添加Where条件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndSymbolConstant<TM>(Expression<Func<TM, object>> prop, string symbol, object value, bool isQuot = false);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndInAParam(string name, IEnumerable value = null);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndInAParam<TM>(Expression<Func<TM, object>> prop, IEnumerable value = null);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndInBParam(string cName, string pName, IEnumerable value = null);
        /// <summary>
        /// AND在列表参数
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndInBParam<TM>(Expression<Func<TM, object>> prop, string pName, IEnumerable value = null);
        /// <summary>
        /// 内关联表
        /// </summary>
        ISqlScriptSelectBuilder InnerJoin<T>(string joinOn);
        /// <summary>
        /// 内关联表
        /// </summary>
        ISqlScriptSelectBuilder InnerJoin(Type type, string joinOn);
        /// <summary>
        /// 左关联表
        /// </summary>
        ISqlScriptSelectBuilder LeftJoin<T>(string joinOn);
        /// <summary>
        /// 左关联表
        /// </summary>
        ISqlScriptSelectBuilder LeftJoin(Type type, string joinOn);
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="cName"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder GroupBy(string cName);
        /// <summary>
        /// 分组
        /// </summary>
        /// <typeparam name="TM"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder GroupBy<TM>(Expression<Func<TM, object>> prop);
        /// <summary>
        /// 分组Having
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder Having(string clause);
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder OrderBy(string cName, bool isDesc = false);
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TM"></typeparam>
        /// <param name="prop"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder OrderBy<TM>(Expression<Func<TM, object>> prop, bool isDesc);
        /// <summary>
        /// 限制条数(目前只支持sqlite|mysql)
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        ISqlScriptParameters Limit(int take);
        /// <summary>
        /// 限制条数(目前只支持sqlite|mysql)
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        ISqlScriptParameters Limit(int skip, int take);
    }
    /// <summary>
    /// SQL脚本创建者
    /// </summary>
    internal class SqlScriptSimpleSelectBuilder : ASqlScriptBindBuilder, ISqlScriptSelectBuilder, ISqlScriptFromBuilder, ISqlScriptWhereBuilder
    {
        #region // 静态定义
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptFromBuilder Create(StoreType storeType)
        {
            return new SqlScriptSimpleSelectBuilder(storeType);
        }
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storeType"></param>
        private SqlScriptSimpleSelectBuilder(StoreType storeType) : base(storeType)
        {
        }
        /// <summary>
        /// 获取查询内容语句
        /// </summary>
        public ISqlScriptSelectBuilder Select() => this;
        /// <summary>
        /// 获取Where语句
        /// </summary>
        public ISqlScriptWhereBuilder Where() => this;
        /// <summary>
        /// 获取SQL脚本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new StringBuilder("SELECT ")
                .Append(_columnsClause)
                .Append(" FROM ")
                .Append(_fromClause)
                .Append(_whereClause == null ? String.Empty : $" WHERE {_whereClause}")
                .Append(_groupClause == null ? String.Empty : $" GROUP BY {_groupClause}")
                .Append(_havingClause == null ? String.Empty : $" HAVING {_havingClause}")
                .Append(_orderClause == null ? String.Empty : $" ORDER BY {_orderClause}")
                .Append(GetLimit())
                .ToString();
        }
        #region // FROM
        /// <summary>
        /// 添加类
        /// </summary>
        /// <returns></returns>
        ISqlScriptFromBuilder ISqlScriptFromBuilder.From(Type type)
        {
            return AddFromTable(type);
        }
        /// <summary>
        /// 添加类及标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptFromBuilder ISqlScriptFromBuilder.From<T>()
        {
            return AddFromTable(typeof(T));
        }
        /// <summary>
        /// 添加类
        /// </summary>
        /// <returns></returns>
        ISqlScriptWhereBuilder ISqlScriptFromBuilder.FromWhere(Type type)
        {
            return AddFromSelect(type);
        }
        /// <summary>
        /// 添加类及标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptWhereBuilder ISqlScriptFromBuilder.FromWhere<T>()
        {
            return AddFromSelect(typeof(T));
        }
        public ISqlScriptWhereBuilder AddFromSelect(Type type)
        {
            AddTable(type);
            var sqlModel = GetSqlModel(CurrentTag.Type);
            foreach (var item in sqlModel.Table.Columns)
            {
                AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(item.ColumnName)} AS {GetQuot(item.PropertyName)}");
            }
            AddFromClause($"{GetQuot(sqlModel.TagName)} AS {CurrentTag.TAlias}");
            return this;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <returns></returns>
        public ISqlScriptFromBuilder AddFromTable(Type type)
        {
            AddTable(type);
            var sqlModel = GetSqlModel(CurrentTag.Type);
            AddFromClause($"{GetQuot(sqlModel.TagName)} AS {CurrentTag.TAlias}");
            return this;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <param name="tableAs"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public ISqlScriptFromBuilder AddInnerJoinTable(string tableAs, string clause)
        {
            _fromClause.Append($" INNER JOIN {tableAs} ON {clause}");
            return this;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <param name="tableAs"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public ISqlScriptFromBuilder AddLeftJoinTable(string tableAs, string clause)
        {
            _fromClause.Append($" LEFT JOIN {tableAs} ON {clause}");
            return this;
        }
        #endregion // FROM
        #region // SELECT
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.AddClause(string clause)
        {
            AddSelectClause(clause);
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add(string name)
        {
            AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(name)}");
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add<T>()
        {
            var sqlModel = GetSqlModel(typeof(T));
            foreach (var item in sqlModel.Table.Columns)
            {
                AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(item.ColumnName)} AS {GetQuot(item.PropertyName)}");
            }
            return this;
        }

        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add(string cName, string pName)
        {
            AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(cName)} AS {GetQuot(pName)}");
            return this;
        }

        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add(string tag, string cName, string pName)
        {
            AddSelectClause($"{tag ?? CurrentTag.TAlias}.{GetQuot(cName)} AS {GetQuot(pName)}");
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Column<T>(Expression<Func<T>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Column<TM>(Expression<Func<TM, object>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        #endregion // SELECT
        #region // WHERE
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AddClause(string clause)
        {
            AddWhereClause(clause);
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualAParam(string cName, object value)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(cName)}=@{cName}");
            Parameters[cName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualAParam<TM>(Expression<Func<TM, object>> prop, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)}=@{keyName.Name}");
                Parameters[keyName.Name] = value;
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualBParam(string cName, string pName, object value)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(cName)}=@{pName}");
            Parameters[pName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualConstant(string cName, object value, bool isQuot)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(cName)}={(isQuot ? $"'{value}'" : value)}");
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualConstant<TM>(Expression<Func<TM, object>> prop, object value, bool isQuot)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)}={(isQuot ? $"'{value}'" : value)}");
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndBetweenEqualConstant(string cName, object val1, object val2, bool isQuot)
        {
            AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(cName)} BETWEEN {(isQuot ? $"'{val1}'" : val1)} AND {(isQuot ? $"'{val2}'" : val2)})");
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndBetweenEqualConstant<TM>(Expression<Func<TM, object>> prop, object val1, object val2, bool isQuot)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(keyName.Key)} BETWEEN {(isQuot ? $"'{val1}'" : val1)} AND {(isQuot ? $"'{val2}'" : val2)})");
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndLikeAParam(string name, object value)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(name)} LIKE @{name}");
            Parameters[name] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndLikeAParam<TM>(Expression<Func<TM, object>> prop, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} LIKE @{keyName.Name}");
                Parameters[keyName.Name] = value;
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndLikelBParam(string cName, string pName, object value)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(cName)} LIKE @{pName}");
            Parameters[pName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndSymbolAParam(string name, string symbol, object value)
        {
            AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(name)} {symbol} @{name})");
            Parameters[name] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndSymbolAParam<TM>(Expression<Func<TM, object>> prop, string symbol, object value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(keyName.Key)} {symbol} @{keyName.Name})");
                Parameters[keyName.Name] = value;
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndSymbolBParam(string cName, string symbol, string pName, object value)
        {
            AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(cName)} {symbol} @{pName})");
            Parameters[pName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndSymbolConstant(string cName, string symbol, object value, bool isQuot)
        {
            AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(cName)} {symbol} {(isQuot ? $"'{value}'" : value)})");
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndSymbolConstant<TM>(Expression<Func<TM, object>> prop, string symbol, object value, bool isQuot)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"({CurrentTag.TAlias}.{GetQuot(keyName.Key)} {symbol} {(isQuot ? $"'{value}'" : value)})");
            }
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptWhereBuilder.InnerJoin<T>(string joinOn)
        {
            AddTable(typeof(T));
            AddInnerJoinTable($"{CurrentTag.Table} AS {CurrentTag.TAlias}", joinOn);
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptWhereBuilder.InnerJoin(Type type, string joinOn)
        {
            AddTable(type);
            AddInnerJoinTable($"{CurrentTag.Table} AS {CurrentTag.TAlias}", joinOn);
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptWhereBuilder.LeftJoin<T>(string joinOn)
        {
            AddTable(typeof(T));
            AddLeftJoinTable($"{CurrentTag.Table} AS {CurrentTag.TAlias}", joinOn);
            return this;
        }
        ISqlScriptSelectBuilder ISqlScriptWhereBuilder.LeftJoin(Type type, string joinOn)
        {
            AddTable(type);
            AddLeftJoinTable($"{CurrentTag.Table} AS {CurrentTag.TAlias}", joinOn);
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndInAParam(string name, IEnumerable value)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(name)} IN @{name}");
            Parameters[name] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndInAParam<TM>(Expression<Func<TM, object>> prop, IEnumerable value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} IN @{keyName.Name}");
                Parameters[keyName.Name] = value;
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndInBParam(string cName, string pName, IEnumerable value)
        {
            AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(cName)} IN @{pName}");
            Parameters[pName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndInBParam<TM>(Expression<Func<TM, object>> prop, string pName, IEnumerable value)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddWhereClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} IN @{pName}");
                Parameters[pName] = value;
            }
            return this;
        }
        #endregion // WHERE
        #region // GROUP/ORDER
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.GroupBy(string cName)
        {
            AddGroupClause($"{CurrentTag.TAlias}.{GetQuot(cName)}");
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.GroupBy<TM>(Expression<Func<TM, object>> prop)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddGroupClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)}");
            }
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.Having(string clause)
        {
            AddHavingClause(clause);
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.OrderBy(string cName, bool isDesc)
        {
            AddOrderClause($"{CurrentTag.TAlias}.{GetQuot(cName)} {(isDesc ? "DESC" : "ASC")}");
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.OrderBy<TM>(Expression<Func<TM, object>> prop, bool isDesc)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddOrderClause($"{CurrentTag.TAlias}.{GetQuot(keyName.Key)} {(isDesc ? "DESC" : "ASC")}");
            }
            return this;
        }
        ISqlScriptParameters ISqlScriptWhereBuilder.Limit(int take)
        {
            _take = take;
            return this;
        }
        ISqlScriptParameters ISqlScriptWhereBuilder.Limit(int skip, int take)
        {
            _take = take;
            _skip = skip;
            return this;
        }
        #endregion // Group/ORDER
    }
    #endregion // SqlScriptSimpleSelectBuilder
}
