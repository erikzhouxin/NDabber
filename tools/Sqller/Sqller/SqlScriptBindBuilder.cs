﻿using System;
using System.Collections.Generic;
using System.Data.Dabber;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Text;

namespace System.Data.Sqller
{
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
        /// 自定义表别名
        /// </summary>
        public string TNamed { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public int TCount { get; set; }
        /// <summary>
        /// 获取命名的标记
        /// </summary>
        /// <returns></returns>
        public string GetTableTag()
        {
            return TNamed ?? TAlias;
        }
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
        /// <param name="tag"></param>
        /// <exception cref="NotSupportedException" />
        /// <returns></returns>
        protected virtual SqlScriptTagModel AddTable(Type type, string tag = null)
        {
            if (type == null)
            {
                throw new NotSupportedException("添加表类型不能为空");
            }
            Tags.Add(CurrentTag = new SqlScriptTagModel
            {
                Type = type,
                IsType = type != null,
                SqlModel = GetSqlModel(type),
                TCount = Tags.Count,
            });
            CurrentTag.Table = GetQuot(CurrentTag.SqlModel.TagName);
            CurrentTag.TAlias = GetQuot($"t{CurrentTag.TCount}");
            CurrentTag.TNamed = tag;
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
    }
}