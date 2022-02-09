﻿using System;
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
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder Add<T>(string tag = null);
        /// <summary>
        /// 获取Where创建者
        /// </summary>
        ISqlScriptWhereBuilder Where { get; }
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
        ISqlScriptFromBuilder From<T>(string tag = null);
        /// <summary>
        /// 添加自动类
        /// </summary>
        /// <returns></returns>
        ISqlScriptFromBuilder From(Type type, string tag = null);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder FromWhere(Type type, string tag = null);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder FromWhere<T>(string tag = null);
        /// <summary>
        /// 获取一个查询
        /// </summary>
        /// <returns></returns>
        ISqlScriptSelectBuilder Select { get; }
    }
    /// <summary>
    /// Where创建者
    /// </summary>
    public interface ISqlScriptWhereBuilder : ISqlScriptParameters
    {
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualBParam(string cName, string pName, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndEqualAParam(string cName, object value = null);
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
        /// <param name="cName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="isQuot"></param>
        /// <returns></returns>
        ISqlScriptWhereBuilder AndBetweenEqualConstant(string cName, object val1, object val2, bool isQuot = false);
        /// <summary>
        /// From创建者
        /// </summary>
        ISqlScriptFromBuilder From { get; }
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
        public ISqlScriptSelectBuilder Select { get => StoreType == StoreType.Unknown ? throw new NotSupportedException() : this; }
        /// <summary>
        /// 获取Where语句
        /// </summary>
        public ISqlScriptWhereBuilder Where { get => StoreType == StoreType.Unknown ? throw new NotSupportedException() : this; }
        /// <summary>
        /// 获取From语句
        /// </summary>
        public ISqlScriptFromBuilder From { get => StoreType == StoreType.Unknown ? throw new NotSupportedException() : this; }

        public ISqlScriptWhereBuilder SetParameter(object args)
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
                .ToString();
        }
        #region // FROM
        /// <summary>
        /// 添加类
        /// </summary>
        /// <returns></returns>
        ISqlScriptFromBuilder ISqlScriptFromBuilder.From(Type type, string tag)
        {
            return AddFromTable(type, tag);
        }
        /// <summary>
        /// 添加类及标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptFromBuilder ISqlScriptFromBuilder.From<T>(string tag)
        {
            return AddFromTable(typeof(T), tag);
        }
        /// <summary>
        /// 添加类
        /// </summary>
        /// <returns></returns>
        ISqlScriptWhereBuilder ISqlScriptFromBuilder.FromWhere(Type type, string tag)
        {
            return AddFromSelect(type, tag);
        }
        /// <summary>
        /// 添加类及标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlScriptWhereBuilder ISqlScriptFromBuilder.FromWhere<T>(string tag)
        {
            return AddFromSelect(typeof(T), tag);
        }
        public ISqlScriptWhereBuilder AddFromSelect(Type type, string tag = null)
        {
            AddTable(type, tag);
            var sqlModel = GetSqlModel(CurrentTag.Type);
            foreach (var item in sqlModel.Table.Columns)
            {
                AddSelectClause($"{CurrentTag.TAlias}.{GetQuot(item.ColumnName)} AS {GetQuot(item.PropertyName)}");
            }
            AddFromTable($"{GetQuot(sqlModel.TagName)} AS {CurrentTag.GetTableTag()}");
            return this;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <returns></returns>
        public ISqlScriptFromBuilder AddFromTable(Type type, string tag)
        {
            AddTable(type, tag);
            var sqlModel = GetSqlModel(CurrentTag.Type);
            return AddFromTable($"{GetQuot(sqlModel.TagName)} AS {CurrentTag.GetTableTag()}");
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public ISqlScriptFromBuilder AddFromTable(string clause)
        {
            if (_fromClause == null) { _fromClause = new StringBuilder(clause); }
            else { _fromClause.Append($", { clause}"); }
            return this;
        }
        #endregion // FROM
        #region // SELECT
        /// <summary>
        /// 添加字段及属性名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add(string name)
        {
            AddSelectClause($"{CurrentTag.GetTableTag()}.{GetQuot(name)}");
            return this;
        }
        /// <summary>
        /// 添加字段及属性名
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add<T>(string tag)
        {
            var sqlModel = GetSqlModel(typeof(T));
            tag ??= CurrentTag.GetTableTag();
            foreach (var item in sqlModel.Table.Columns)
            {
                AddSelectClause($"{tag}.{GetQuot(item.ColumnName)} AS {GetQuot(item.PropertyName)}");
            }
            return this;
        }

        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add(string cName, string pName)
        {
            AddSelectClause($"{CurrentTag.GetTableTag()}.{GetQuot(cName)} AS {GetQuot(pName)}");
            return this;
        }

        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Add(string tag, string cName, string pName)
        {
            AddSelectClause($"{tag ?? CurrentTag.GetTableTag()}.{GetQuot(cName)} AS {GetQuot(pName)}");
            return this;
        }
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Column<T>(Expression<Func<T>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSelectClause($"{CurrentTag.GetTableTag()}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSelectClause($"{CurrentTag.GetTableTag()}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlScriptSelectBuilder ISqlScriptSelectBuilder.Column<TM>(Expression<Func<TM, object>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                AddSelectClause($"{CurrentTag.GetTableTag()}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        #endregion // SELECT
        #region // WHERE
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualBParam(string cName, string pName, object value)
        {
            AddWhereClause($"{CurrentTag.GetTableTag()}.{GetQuot(cName)}=@{pName}");
            Parameters[pName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualAParam(string cName, object value)
        {
            AddWhereClause($"{CurrentTag.GetTableTag()}.{GetQuot(cName)}=@{cName}");
            Parameters[cName] = value;
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndEqualConstant(string cName, object value, bool isQuot)
        {
            AddWhereClause($"{CurrentTag.GetTableTag()}.{GetQuot(cName)}={(isQuot ? $"'{value}'" : value)}");
            return this;
        }
        ISqlScriptWhereBuilder ISqlScriptWhereBuilder.AndBetweenEqualConstant(string cName, object val1, object val2, bool isQuot)
        {
            AddWhereClause($"({CurrentTag.GetTableTag()}.{GetQuot(cName)} BETWEEN {(isQuot ? $"'{val1}'" : val1)} AND {(isQuot ? $"'{val2}'" : val2)})");
            return this;
        }
        #endregion // WHERE
    }
}
