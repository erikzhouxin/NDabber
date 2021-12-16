using System;
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
    /// Sql脚本创建者
    /// </summary>
    public static class SqlScriptBuilder
    {
        /// <summary>
        /// 创建简单查询
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlFromBuilder CreateSimpleSelect(StoreType storeType)
        {
            return SqlSimpleSelectBuilder.Create(storeType);
        }
    }
    /// <summary>
    /// SQL的参数字典
    /// </summary>
    public interface ISqlParameters
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
    }
    /// <summary>
    /// 查询创建
    /// </summary>
    public interface ISqlSelectBuilder : ISqlParameters
    {
        /// <summary>
        /// 添加查询分句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        ISqlSelectBuilder AddSelectClause(string clause);
        /// <summary>
        /// 添加名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISqlSelectBuilder Add(string name);
        /// <summary>
        /// 添加字段及属性名
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder Add(string cName, string pName);
        /// <summary>
        /// 添加别名及字段及属性名
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder Add(string tag, string cName, string pName);
        /// <summary>
        /// 添加一个类的所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlSelectBuilder Add<T>(string tag = null);
        /// <summary>
        /// 获取Where创建者
        /// </summary>
        ISqlWhereBuilder Where { get; }
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder Column<T>(Expression<Func<T>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName = null);
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder Column<TM>(Expression<Func<TM, object>> prop, string pName = null);
    }
    /// <summary>
    /// From创建者
    /// </summary>
    public interface ISqlFromBuilder : ISqlParameters
    {
        /// <summary>
        /// 添加自动类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlFromBuilder From<T>(string tag = null);
        /// <summary>
        /// 添加自动类
        /// </summary>
        /// <returns></returns>
        ISqlFromBuilder From(Type type, string tag = null);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlWhereBuilder FromWhere(Type type, string tag = null);
        /// <summary>
        /// 自动补全类型所有字段
        /// 直接跳转到Where字段添加
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlWhereBuilder FromWhere<T>(string tag = null);
        /// <summary>
        /// 获取一个查询
        /// </summary>
        /// <returns></returns>
        ISqlSelectBuilder Select { get; }
    }
    /// <summary>
    /// Where创建者
    /// </summary>
    public interface ISqlWhereBuilder : ISqlParameters
    {
        /// <summary>
        /// 添加Where分句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        ISqlWhereBuilder AndWhereClause(string clause);
        /// <summary>
        /// 添加Where参数
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlWhereBuilder AndWhereParameter(string pName, object value);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlWhereBuilder AndEqual(string cName, string pName, object value = null);
        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlWhereBuilder AndEqual(string cName, object value);
        /// <summary>
        /// 添加内置式
        /// </summary>
        /// <param name="clause"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlWhereBuilder AndEqualClause(string clause, string pName, object value);
        /// <summary>
        /// From创建者
        /// </summary>
        ISqlFromBuilder From { get; }
    }
}
