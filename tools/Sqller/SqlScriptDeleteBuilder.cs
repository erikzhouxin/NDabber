using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Sqller
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
}
