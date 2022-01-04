using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Text;

namespace System.Data.Sqller
{
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
}
