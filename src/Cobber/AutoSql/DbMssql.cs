using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 自动Microsoft SQL Server创建者
    /// 依赖于[DbMarkable/DbAutoable]
    /// 推荐使用[AutoSqlServerBuilder]
    /// </summary>
    public class AutoMssqlBuilder
    {
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AutoSqlBuilder Builder(Type type) => AutoSqlServerBuilder.EntitySqlDic.GetOrAdd(type, (k) => AutoSqlServerBuilder.CreateSqlModel(k));
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AutoSqlBuilder Builder<T>() => AutoSqlServerBuilder.EntitySqlDic.GetOrAdd(typeof(T), (k) => AutoSqlServerBuilder.CreateSqlModel(k));
    }
    /// <summary>
    /// 自动SqlServer创建者
    /// </summary>
    public class AutoSqlServerBuilder
    {
        #region // 类型定义
        /// <summary>
        /// 实体SQL语句
        /// </summary>
        internal static ConcurrentDictionary<Type, AutoSqlBuilder> EntitySqlDic { get; } = new ConcurrentDictionary<Type, AutoSqlBuilder>();
        internal sealed class BuilderModel<T> : ISqlBuilderModel
        {
            /// <summary>
            /// SQL模型
            /// </summary>
            public static AutoSqlBuilder SqlModel { get; } = CreateSqlModel<T>();
            /// <summary>
            /// 获取SQL创建模型
            /// </summary>
            /// <returns></returns>
            public AutoSqlBuilder GetSqlModel()
            {
                return SqlModel;
            }
        }
        #endregion
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AutoSqlBuilder Builder(Type type) => EntitySqlDic.GetOrAdd(type, (k) => CreateSqlModel(k));
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AutoSqlBuilder Builder<T>() => BuilderModel<T>.SqlModel;
        /// <summary>
        /// 获取带括号的内容
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetQuot(string text)
        {
            return $"[{text}]";
        }
        #region // 辅助方法        
        /// <summary>
        /// 获取标记参数的Where及处理后的参数字典
        /// </summary>
        public static IDictionary<string, object> GetArgsParams(Tuple<String, object>[] args, StringBuilder builder) => GetArgsParams(args.ToDictionary(s => s.Item1, s => s.Item2), builder);
        /// <summary>
        /// 获取标记参数的Where及处理后的参数字典
        /// </summary>
        public static IDictionary<string, object> GetArgsParams(KeyValuePair<string, object>[] args, StringBuilder builder) => GetArgsParams(args.ToDictionary(s => s.Key, s => s.Value), builder);
        /// <summary>
        /// 获取标记参数的Where及处理后的参数字典
        /// </summary>
        /// <param name="args"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetArgsParams(IDictionary<string, object> args, StringBuilder builder)
        {
            var keys = args.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var value = args[key];
                string keyPart;
                if (value is AutoSqlModel.StringLike)
                {
                    args[key] = value.ToString();
                    keyPart = $"[{key}] LIKE @{key}";
                }
                else if (value is AutoSqlModel.ICompare)
                {
                    var compareObj = value as AutoSqlModel.ICompare;
                    keyPart = $"[{key}] {compareObj.GetSign()} @{key}";
                    args[key] = compareObj.Value;
                }
                else if (value is AutoSqlModel.ICompareRange)
                {
                    var compareObj = value as AutoSqlModel.ICompareRange;
                    args[key] = null;
                    if (compareObj.IsEmpty) { continue; }
                    var objItem = compareObj.Compares[0];
                    keyPart = $"([{key}] {objItem.GetSign()} @{key}Item0";
                    args[$"{key}Item0"] = objItem.Value;
                    if (compareObj.Compares.Count() >= 1)
                    {
                        var sb = new StringBuilder(keyPart);
                        var relateString = compareObj.IsOr ? " OR " : " AND ";
                        for (int j = 1; j < compareObj.Compares.Count(); j++)
                        {
                            objItem = compareObj.Compares[j];
                            sb.Append(relateString).Append($"[{key}] {objItem.GetSign()} @{key}Item{j}");
                            args[$"{key}Item{j}"] = objItem.Value;
                        }
                        keyPart = sb.ToString();
                    }
                    keyPart += ")";
                }
                else if (value is AutoSqlModel.ICompareBetween)
                {
                    var compareObj = value as AutoSqlModel.ICompareBetween;
                    keyPart = $"[{key}] BETWEEN @{key}{nameof(compareObj.Begin)} AND @{key}{nameof(compareObj.End)}";
                    args[key] = null;
                    args[$"{key}{nameof(compareObj.Begin)}"] = compareObj.Begin;
                    args[$"{key}{nameof(compareObj.End)}"] = compareObj.End;
                }
                else if (value is Array)
                {
                    keyPart = $"[{key}] IN @{key}";
                }
                else
                {
                    keyPart = $"[{key}]=@{key}";
                }
                builder.Append(keyPart + " AND ");
            }
            builder.Remove(builder.Length - 5, 5);
            return args;
        }
        /// <summary>
        /// 创建模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AutoSqlBuilder CreateSqlModel<T>() => EntitySqlDic.GetOrAdd(typeof(T), (k) => CreateSqlModel(k));
        /// <summary>
        /// 获取实体的创建SQL语句
        /// </summary>
        /// <returns></returns>
        public static AutoSqlBuilder CreateSqlModel(Type type)
        {
            var result = new AutoSqlBuilder(type);
            var tableName = result.Table.TableName;
            var tableComment = result.Table.TableComment;
            var createBuilder = new StringBuilder($"CREATE TABLE [{tableName}](");
            var colLength = result.Table.Columns.Length;
            StringBuilder insertIgApkColumns = new StringBuilder();
            StringBuilder insertIgApkValues = new StringBuilder();
            StringBuilder updateSetValue = new StringBuilder();
            StringBuilder selectAsValue = new StringBuilder();
            for (int i = 0; i < colLength; i++)
            {
                var prop = result.Table.Columns[i];
                var colDefine = GetDefinitionType(prop.DbCol);
                if (i + 1 == colLength) // 最后一个进行特殊处理
                {
                    if (result.Table.Indexes.Length == 0)
                    {
                        createBuilder.AppendLine($"\t{colDefine}");
                    }
                    else
                    {
                        var idxLen = result.Table.Indexes.Length;
                        createBuilder.AppendLine($"\t{colDefine},");
                        for (int j = 0; j < idxLen;)
                        {
                            var item = result.Table.Indexes[j];
                            var split = ++j == idxLen ? "" : ",";
                            if (item.IsUnique)
                            {
                                createBuilder.AppendLine($"\tCONSTRAINT [IX_{tableName}_{item.IndexName.Replace("|", "_")}] UNIQUE ([{item.IndexName.Replace("|", "],[")}])");
                            }
                            else
                            {
                                createBuilder.AppendLine($"\tINDEX [IX_{tableName}_{item.IndexName.Replace("|", "_")}] NONCLUSTERED ([{item.IndexName.Replace("|", "],[")}])");
                            }
                        }
                    }
                    if (!prop.IsAuto)
                    {
                        insertIgApkColumns.Append($"[{prop.ColumnName}]");
                        insertIgApkValues.Append($"@{prop.PropertyName}");
                    }
                    if (!prop.IsPK)
                    {
                        updateSetValue.Append($"[{prop.ColumnName}]=@{prop.PropertyName}");
                    }
                    selectAsValue.Append($"[{prop.ColumnName}] AS [{prop.PropertyName}]");
                }
                else
                {
                    createBuilder.AppendLine($"\t{colDefine},");
                    if (!prop.IsAuto)
                    {
                        insertIgApkColumns.Append($"[{prop.ColumnName}],");
                        insertIgApkValues.Append($"@{prop.PropertyName},");
                    }
                    if (!prop.IsPK)
                    {
                        updateSetValue.Append($"[{prop.ColumnName}]=@{prop.PropertyName},");
                    }
                    selectAsValue.Append($"[{prop.ColumnName}] AS [{prop.PropertyName}],");
                }
            }
            createBuilder.Append(");");

            result.Create = createBuilder.ToString();
            result.Insert = string.Format("INSERT INTO [{0}]({1}) VALUES({2})", tableName, insertIgApkColumns, insertIgApkValues); // 无自增主键
            result.Replace = string.Format("REPLACE INTO [{0}]({1}) VALUES({2})", tableName, insertIgApkColumns, insertIgApkValues); // 无自增主键
            result.DeleteID = string.Format("DELETE FROM [{0}] WHERE [{1}]=@{2}", tableName, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.DeleteInID = string.Format("DELETE FROM [{0}] WHERE [{1}] IN @{2}", tableName, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.UpdateID = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{3}", tableName, updateSetValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.Update = string.Format("UPDATE [{0}] SET {1}", tableName, updateSetValue);
            result.Select = string.Format("SELECT {1} FROM [{0}]", tableName, selectAsValue);
            result.SelectID = string.Format("SELECT TOP 1 {1} FROM [{0}] WHERE [{2}]=@{3}", tableName, selectAsValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.SelectInID = string.Format("SELECT {1} FROM [{0}] WHERE [{2}] IN @{3}", tableName, selectAsValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.SelectLimit = string.Format("SELECT {1} FROM [{0}] OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY", tableName, selectAsValue);
            result.SelectCount = string.Format("SELECT COUNT(*) FROM [{0}]", tableName);
            result.WhereID = string.Format("[{0}]=@{1}", result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);

            return result;
        }
        /// <summary>
        /// 获取数据库类型定义
        /// </summary>
        /// <returns></returns>
        public static string GetDefinitionType(DbColAttribute dbCol)
        {
            var defType = GetDbType(dbCol);
            return new StringBuilder()
                .AppendFormat("[{0}] ", dbCol.Name)
                .Append(defType)
                .Append(dbCol.Key == DbIxType.PK ? " PRIMARY KEY" : "")
                .Append(dbCol.Key == DbIxType.APK ? " PRIMARY KEY AUTOINCREMENT" : "")
                .Append(dbCol.IsReq ? " NOT NULL" : " NULL")
                .Append(dbCol.Default == null ? "" : string.Format(" DEFAULT '{0}'", dbCol.Default)).ToString();
            //.AppendFormat(" COMMENT '{0}'", dbCol.Display.GetMySqlComment());
        }
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="dbCol"></param>
        /// <returns></returns>
        public static string GetDbType(DbColAttribute dbCol)
        {
            switch (dbCol.Type)
            {
                case DbColType.String:
                case DbColType.StringMax:
                case DbColType.StringMedium:
                case DbColType.StringNormal: return string.Format("TEXT");
                case DbColType.Boolean:
                case DbColType.Byte:
                case DbColType.Char:
                case DbColType.Int16:
                case DbColType.Int32:
                case DbColType.Int64:
                case DbColType.UByte:
                case DbColType.UInt16:
                case DbColType.UInt32:
                case DbColType.UInt64: return string.Format("INTEGER");
                case DbColType.Single:
                case DbColType.Double: return string.Format("REAL");
                case DbColType.Decimal: return string.Format("DECIMAL({0},{1})", dbCol.Len, dbCol.Digit);
                case DbColType.DateTime:
                case DbColType.JsonString: return string.Format("TEXT");
                case DbColType.Guid:
                case DbColType.Blob: return string.Format("BLOB");
                case DbColType.Enum:
                case DbColType.Set:
                default: throw new Exception("未确定解决方案");
            }
        }
        #endregion
    }
}
