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
    /// SQL脚本创建者
    /// </summary>
    internal class SqlSimpleSelectBuilder : ISqlSelectBuilder, ISqlFromBuilder, ISqlWhereBuilder
    {
        #region // 静态定义
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlFromBuilder Create(StoreType storeType)
        {
            return new SqlSimpleSelectBuilder(storeType);
        }
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storeType"></param>
        protected SqlSimpleSelectBuilder(StoreType storeType)
        {
            SetStoreSelect(storeType);
        }
        /// <summary>
        /// 获取SQL模型
        /// </summary>
        public Func<Type, AutoSqlBuilder> GetSqlModel { get; private set; }
        /// <summary>
        /// 获取括号类型
        /// </summary>
        public Func<string, string> GetQuot { get; private set; }
        /// <summary>
        /// 存储类型
        /// </summary>
        public StoreType StoreType { get; private set; }
        /// <summary>
        /// 类型及标记字典
        /// </summary>
        public List<Tuble2TypeTag> TypeTags { get; } = new();
        /// <summary>
        /// 当前
        /// </summary>
        public Tuble2TypeTag CurrentTag { get; private set; }
        /// <summary>
        /// 参数列表
        /// </summary>

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        /// <summary>
        /// SQL脚本
        /// </summary>
        public String SqlScript { get => GetSqlScript(); }
        private StringBuilder _columnsClause;
        private StringBuilder _fromClause;
        private StringBuilder _whereClause;
        private ISqlFromBuilder SetStoreSelect(StoreType storeType)
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
        /// 获取查询内容语句
        /// </summary>
        public ISqlSelectBuilder Select { get => StoreType == StoreType.Unknown ? throw new NotSupportedException() : this; }
        /// <summary>
        /// 获取Where语句
        /// </summary>
        public ISqlWhereBuilder Where { get => StoreType == StoreType.Unknown ? throw new NotSupportedException() : this; }
        /// <summary>
        /// 获取From语句
        /// </summary>
        public ISqlFromBuilder From { get => StoreType == StoreType.Unknown ? throw new NotSupportedException() : this; }
        /// <summary>
        /// 获取懒加载内容
        /// </summary>
        /// <returns></returns>
        public Tuble2SqlArgs GetSqlWithArgs(object args)
        {
            return new Tuble2SqlArgs(GetSqlScript(), SetParameter(args).Parameters);
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
            return new Tuble2SqlArgs(GetSqlScript(), Parameters);
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
            return new Tuble2SqlArgs(GetSqlScript(), Parameters);
        }
        public ISqlWhereBuilder SetParameter(object args)
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
        public string GetSqlScript()
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
        ISqlFromBuilder ISqlFromBuilder.From(Type type, string tag)
        {
            return AddFromTable(type, tag);
        }
        /// <summary>
        /// 添加类及标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlFromBuilder ISqlFromBuilder.From<T>(string tag)
        {
            return AddFromTable(typeof(T), tag);
        }
        /// <summary>
        /// 添加类
        /// </summary>
        /// <returns></returns>
        ISqlWhereBuilder ISqlFromBuilder.FromWhere(Type type, string tag)
        {
            return AddFromSelect(type, tag);
        }
        /// <summary>
        /// 添加类及标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISqlWhereBuilder ISqlFromBuilder.FromWhere<T>(string tag)
        {
            return AddFromSelect(typeof(T), tag);
        }
        public ISqlWhereBuilder AddFromSelect(Type type, string tag = null)
        {
            TypeTags.Add(CurrentTag = new Tuble2TypeTag(type, tag ?? $"t{TypeTags.Count}"));
            var sqlModel = GetSqlModel(CurrentTag.Type);
            foreach (var item in sqlModel.Table.Columns)
            {
                AddSelectClause($"{CurrentTag.Tag}.{GetQuot(item.ColumnName)} AS {GetQuot(item.PropertyName)}");
            }
            AddFromTable($"{GetQuot(sqlModel.TagName)} AS {CurrentTag.Tag}");
            return this;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <returns></returns>
        public ISqlFromBuilder AddFromTable(Type type, string tag)
        {
            TypeTags.Add(CurrentTag = new Tuble2TypeTag(type, tag ?? $"t{TypeTags.Count}"));
            var sqlModel = GetSqlModel(CurrentTag.Type);
            return AddFromTable($"{GetQuot(sqlModel.TagName)} AS {CurrentTag.Tag}");
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public ISqlFromBuilder AddFromTable(string clause)
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
        ISqlSelectBuilder ISqlSelectBuilder.Add(string name)
        {
            return AddSelectClause($"{CurrentTag.Item2}.{GetQuot(name)}");
        }
        /// <summary>
        /// 添加字段及属性名
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ISqlSelectBuilder ISqlSelectBuilder.Add<T>(string tag)
        {
            var sqlModel = GetSqlModel(typeof(T));
            tag ??= CurrentTag.Tag;
            foreach (var item in sqlModel.Table.Columns)
            {
                AddSelectClause($"{tag}.{GetQuot(item.ColumnName)} AS {GetQuot(item.PropertyName)}");
            }
            return this;
        }
        /// <summary>
        /// 添加查询列
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public ISqlSelectBuilder AddSelectClause(string clause)
        {
            if (_columnsClause == null)
            {
                _columnsClause = new StringBuilder().Append(clause);
            }
            else
            {
                _columnsClause.Append($", {clause}");
            }
            return this;
        }

        ISqlSelectBuilder ISqlSelectBuilder.Add(string cName, string pName)
        {
            return AddSelectClause($"{CurrentTag.Tag}.{GetQuot(cName)} AS {GetQuot(pName)}");
        }

        ISqlSelectBuilder ISqlSelectBuilder.Add(string tag, string cName, string pName)
        {
            return AddSelectClause($"{tag ?? CurrentTag.Tag}.{GetQuot(cName)} AS {GetQuot(pName)}");
        }
        /// <summary>
        /// 列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder ISqlSelectBuilder.Column<T>(Expression<Func<T>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                return AddSelectClause($"{CurrentTag.Tag}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder ISqlSelectBuilder.Column<TM, TP>(Expression<Func<TM, TP>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                return AddSelectClause($"{CurrentTag.Tag}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        /// <summary>
        /// 类及列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        ISqlSelectBuilder ISqlSelectBuilder.Column<TM>(Expression<Func<TM, object>> prop, string pName)
        {
            if (DbColAttribute.TryGetColName(prop.GetPropertyInfo(), out Tuble2KeyName keyName))
            {
                return AddSelectClause($"{CurrentTag.Tag}.{GetQuot(keyName.Key)} AS {pName ?? GetQuot(keyName.Name)}");
            }
            return this;
        }
        #endregion // SELECT
        #region // WHERE
        ISqlWhereBuilder ISqlWhereBuilder.AndEqual(string cName, string pName, object value)
        {
            return AndEqualClause($"{CurrentTag.Tag}.{GetQuot(cName)}=@{pName}", pName, value);
        }
        ISqlWhereBuilder ISqlWhereBuilder.AndEqual(string cName, object value)
        {
            return AndEqualClause($"{CurrentTag.Tag}.{GetQuot(cName)}=@{cName}", cName, value);
        }
        /// <summary>
        /// 添加Where列及值
        /// </summary>
        /// <param name="clause"></param>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ISqlWhereBuilder AndEqualClause(string clause, string pName, object value)
        {
            AndWhereClause(clause);
            if (value != null)
            {
                AndWhereParameter(pName, value);
            }
            return this;
        }
        /// <summary>
        /// 添加Where参数
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ISqlWhereBuilder AndWhereParameter(string pName, object value)
        {
            Parameters[pName] = value;
            return this;
        }
        /// <summary>
        /// 添加Where子句
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public ISqlWhereBuilder AndWhereClause(string clause)
        {
            if (_whereClause == null)
            {
                _whereClause = new StringBuilder(clause);
            }
            else
            {
                _whereClause.Append($" AND {clause}");
            }
            return this;
        }
        #endregion // WHERE
    }
}
