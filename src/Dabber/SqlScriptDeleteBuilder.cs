using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// 删除脚本From部分
    /// </summary>
    public interface ISqlScriptDeleteFromBuilder
    {
        /// <summary>
        /// 使用逻辑删除
        /// </summary>
        /// <returns></returns>
        ISqlScriptDeleteLogicalFromBuilder UseLogical();
        /// <summary>
        /// 删除条件
        /// </summary>
        ISqlScriptDeleteWhereBuilder Where { get; }
    }
    /// <summary>
    /// 逻辑删除逻辑
    /// </summary>
    public interface ISqlScriptDeleteLogicalFromBuilder
    {
        /// <summary>
        /// 删除条件
        /// </summary>
        ISqlScriptDeleteWhereBuilder Where { get; }
    }
    /// <summary>
    /// 删除脚本Where部分
    /// </summary>
    public interface ISqlScriptDeleteWhereBuilder
    {

    }
    /// <summary>
    /// SQL脚本删除创建
    /// </summary>
    internal class SqlScriptDeleteBuilder : ISqlScriptDeleteFromBuilder, ISqlScriptDeleteLogicalFromBuilder, ISqlScriptDeleteWhereBuilder
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
        public SqlScriptDeleteBuilder(StoreType storeType)
        {
            SetStoreType(storeType);
        }
        /// <summary>
        /// 获取查询模型
        /// </summary>
        public Func<Type, AutoSqlBuilder> GetSqlModel { get; private set; }
        /// <summary>
        /// 获取分隔号
        /// </summary>
        public Func<String, string> GetQuot { get; private set; }
        /// <summary>
        /// 存储类型
        /// </summary>
        public StoreType StoreType { get; private set; }
        /// <summary>
        /// 括号开始
        /// </summary>
        public string QuotStart => "(";
        /// <summary>
        /// 括号结束
        /// </summary>
        public string QuotEnd => ")";
        private bool _isLogical;
        private StringBuilder _columnsClause;
        private StringBuilder _fromClause;
        private StringBuilder _whereClause;
        /// <summary>
        /// 设置存储类型
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public ISqlScriptDeleteFromBuilder SetStoreType(StoreType storeType)
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
            return this;
        }
        /// <summary>
        /// 使用逻辑删除
        /// </summary>
        /// <returns></returns>
        public ISqlScriptDeleteLogicalFromBuilder UseLogical()
        {
            _isLogical = true;
            return this;
        }
        /// <summary>
        /// 删除条件
        /// </summary>
        public ISqlScriptDeleteWhereBuilder Where => this;
    }
}
