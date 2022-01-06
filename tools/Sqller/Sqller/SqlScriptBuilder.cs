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
}
