using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Sqller
{
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
}
