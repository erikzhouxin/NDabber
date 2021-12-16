using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Cobber
{
    /// <summary>
    /// 自动SQLite创建者
    /// 依赖于[DbMarkable/DbAutoable]
    /// </summary>
    public sealed class AutoSQLiteBuilder
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
                    keyPart = $"{key} {compareObj.GetSign()} @{key}";
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
                            sb.Append(relateString).Append($"`{key}` {objItem.GetSign()} @{key}Item{j}");
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
        /// <returns></returns>
        public static AutoSqlBuilder CreateSqlModel<T>() => EntitySqlDic.GetOrAdd(typeof(T), (k) => CreateSqlModel(k));
        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AutoSqlBuilder CreateSqlModel(Type type)
        {
            var result = new AutoSqlBuilder(type);
            var tableName = result.Table.TableName;
            var tableComment = result.Table.TableComment;
            var createBuilder = new StringBuilder($"CREATE TABLE IF NOT EXISTS [{tableName}]( -- {tableComment}").AppendLine();
            var colLength = result.Table.Columns.Length;
            StringBuilder insertIgApkColumns = new StringBuilder();
            StringBuilder insertIgApkValues = new StringBuilder();
            StringBuilder updateSetValue = new StringBuilder();
            StringBuilder selectAsValue = new StringBuilder();
            for (int i = 0; i < colLength; i++)
            {
                var prop = result.Table.Columns[i];
                String defType;
                switch (prop.DbCol.Type)
                {
                    case DbColType.Guid:
                    case DbColType.String:
                    case DbColType.XmlString:
                    case DbColType.StringMax:
                    case DbColType.StringMedium:
                    case DbColType.StringNormal: defType = string.Format("TEXT"); break;
                    case DbColType.Enum:
                    case DbColType.Boolean:
                    case DbColType.Byte:
                    case DbColType.Char:
                    case DbColType.Int16:
                    case DbColType.Int32:
                    case DbColType.Int64:
                    case DbColType.UByte:
                    case DbColType.UInt16:
                    case DbColType.UInt32:
                    case DbColType.UInt64: defType = string.Format("INTEGER"); break;
                    case DbColType.Single:
                    case DbColType.Double: defType = string.Format("REAL"); break;
                    case DbColType.Decimal: defType = string.Format("DECIMAL({0},{1})", prop.DbCol.Len, prop.DbCol.Digit); break;
                    case DbColType.DateTime: defType = string.Format("DATETIME"); break;
                    case DbColType.JsonString: defType = string.Format("TEXT"); break;
                    case DbColType.Set:
                    case DbColType.Blob: defType = string.Format("BLOB"); break;
                    default: defType = string.Format("TEXT"); break;
                }
                var nullable = prop.DbCol.IsReq ? "NOT" : "";
                var keyable = prop.DbCol.Key == DbIxType.PK ? " PRIMARY KEY" : (prop.DbCol.Key == DbIxType.APK ? " PRIMARY KEY AUTOINCREMENT" : (prop.DbCol.Default == null ? "" : $" DEFAULT '{prop.DbCol.Default}'"));

                if (i + 1 == colLength) // 最后一个进行特殊处理
                {
                    createBuilder.AppendLine($"\t[{prop.ColumnName}] {defType} {nullable} NULL{keyable} -- {prop.ColumnComment}");
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
                    createBuilder.AppendLine($"\t[{prop.ColumnName}] {defType} {nullable} NULL{keyable}, -- {prop.ColumnComment}");
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
            foreach (var item in result.Table.Indexes)
            {
                createBuilder.AppendLine().Append($"CREATE{(item.IsUnique ? " UNIQUE " : " ")}INDEX IF NOT EXISTS [IX_{tableName}_{item.IndexName.Replace("|", "_")}] ON [{tableName}]([{item.IndexName.Replace("|", "],[")}]);");
            }

            result.Create = createBuilder.ToString();
            result.Insert = string.Format("INSERT INTO [{0}]({1}) VALUES({2})", tableName, insertIgApkColumns, insertIgApkValues); // 无自增主键
            result.Replace = string.Format("REPLACE INTO [{0}]({1}) VALUES({2})", tableName, insertIgApkColumns, insertIgApkValues); // 无自增主键
            result.DeleteID = string.Format("DELETE FROM [{0}] WHERE [{1}]=@{2}", tableName, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.DeleteInID = string.Format("DELETE FROM [{0}] WHERE [{1}] IN @{2}", tableName, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.UpdateID = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{3}", tableName, updateSetValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.Update = string.Format("UPDATE [{0}] SET {1}", tableName, updateSetValue);
            result.Select = string.Format("SELECT {1} FROM [{0}]", tableName, selectAsValue);
            result.SelectID = string.Format("SELECT {1} FROM [{0}] WHERE [{2}]=@{3} LIMIT 1", tableName, selectAsValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.SelectInID = string.Format("SELECT {1} FROM [{0}] WHERE [{2}] IN @{3}", tableName, selectAsValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.SelectLimit = string.Format("SELECT {1} FROM [{0}] LIMIT @Skip,@Take", tableName, selectAsValue);
            result.SelectCount = string.Format("SELECT COUNT(*) FROM [{0}]", tableName);
            result.WhereID = string.Format("[{0}]=@{1}", result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);

            return result;
        }
        #endregion
        /// <summary>
        /// 创建连接字符串
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string BuilderConnString(FileInfo fileInfo, string password = null)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Format("Data Source={0};", fileInfo.FullName);
            }
            else
            {
                return string.Format("Data Source={0};Password={1};", fileInfo.FullName, password);
            }
        }
    }
}
