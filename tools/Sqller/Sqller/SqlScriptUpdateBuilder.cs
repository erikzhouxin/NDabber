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
            Parameters[cName] = value;
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
}
