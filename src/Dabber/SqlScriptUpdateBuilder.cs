using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// SQL脚本更新From部分
    /// </summary>
    public interface ISqlScriptUpdateFromBuilder
    {
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder SetForm<T>(string tag = null);
    }
    /// <summary>
    /// SQL脚本更新Set部分
    /// </summary>
    public interface ISqlScriptUpdateSetBuilder
    {

    }
    /// <summary>
    /// SQL脚本更新Where部分
    /// </summary>
    public interface ISqlScriptUpdateWhereBuilder
    {

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
        public SqlScriptUpdateBuilder(StoreType storeType) : base(storeType)
        {
        }
        /// <summary>
        /// 设置修改表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlScriptUpdateSetBuilder ISqlScriptUpdateFromBuilder.SetForm<T>(string tag = null)
        {
            return this;
        }
        /// <summary>
        /// 获取SQL脚本
        /// </summary>
        /// <returns></returns>
        public override string GetSqlScript()
        {
            return new StringBuilder("UPDATE ")
                .Append(CurrentTag.TKey).Append(" SET ")
                .Append(_setClause)
                .Append(_whereClause == null ? "" : $" WHERE {_whereClause}")
                .ToString();
        }
    }
}
