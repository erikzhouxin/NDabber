using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 自动MySql创建者
    /// 依赖于[DbMarkable/DbAutoable]
    /// </summary>
    public class AutoMySqlBuilder
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
        /// <summary>
        /// 获取带括号的内容
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetQuot(string text)
        {
            return $"`{text}`";
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
                    keyPart = $"`{key}` LIKE @{key}";
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
                    keyPart = $"(`{key}` {objItem.GetSign()} @{key}Item0";
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
                    keyPart = $"`{key}` BETWEEN @{key}{nameof(compareObj.Begin)} AND @{key}{nameof(compareObj.End)}";
                    args[key] = null;
                    args[$"{key}{nameof(compareObj.Begin)}"] = compareObj.Begin;
                    args[$"{key}{nameof(compareObj.End)}"] = compareObj.End;
                }
                else if (value is Array)
                {
                    keyPart = $"`{key}` IN @{key}";
                }
                else
                {
                    keyPart = $"`{key}`=@{key}";
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
            var createBuilder = new StringBuilder($"CREATE TABLE IF NOT EXISTS `{tableName}`(").AppendLine();
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
                            createBuilder.AppendLine($"\t{(item.IsUnique ? " UNIQUE " : " ")}KEY `IX_{tableName}_{item.IndexName.Replace("|", "_")}` (`{item.IndexName.Replace("|", "`,`")}`) USING BTREE{split}");
                        }
                    }
                    if (!prop.IsAuto)
                    {
                        insertIgApkColumns.Append($"`{prop.ColumnName}`");
                        insertIgApkValues.Append($"@{prop.PropertyName}");
                    }
                    if (!prop.IsPK)
                    {
                        updateSetValue.Append($"`{prop.ColumnName}`=@{prop.PropertyName}");
                    }
                    selectAsValue.Append($"`{prop.ColumnName}` AS `{prop.PropertyName}`");
                }
                else
                {
                    createBuilder.AppendLine($"\t{colDefine},");
                    if (!prop.IsAuto)
                    {
                        insertIgApkColumns.Append($"`{prop.ColumnName}`,");
                        insertIgApkValues.Append($"@{prop.PropertyName},");
                    }
                    if (!prop.IsPK)
                    {
                        updateSetValue.Append($"`{prop.ColumnName}`=@{prop.PropertyName},");
                    }
                    selectAsValue.Append($"`{prop.ColumnName}` AS `{prop.PropertyName}`,");
                }
            }
            createBuilder.Append($")ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='{tableComment}';");

            result.Create = createBuilder.ToString();
            result.Insert = string.Format("INSERT INTO `{0}`({1}) VALUES({2})", tableName, insertIgApkColumns, insertIgApkValues); // 无自增主键
            result.Replace = string.Format("REPLACE INTO `{0}`({1}) VALUES({2})", tableName, insertIgApkColumns, insertIgApkValues); // 无自增主键
            result.DeleteID = string.Format("DELETE FROM `{0}` WHERE `{1}`=@{2}", tableName, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.DeleteInID = string.Format("DELETE FROM `{0}` WHERE `{1}` IN @{2}", tableName, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.UpdateID = string.Format("UPDATE `{0}` SET {1} WHERE `{2}`=@{3}", tableName, updateSetValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.Update = string.Format("UPDATE `{0}` SET {1}", tableName, updateSetValue);
            result.Select = string.Format("SELECT {1} FROM `{0}`", tableName, selectAsValue);
            result.SelectID = string.Format("SELECT {1} FROM `{0}` WHERE `{2}`=@{3} LIMIT 1", tableName, selectAsValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.SelectInID = string.Format("SELECT {1} FROM `{0}` WHERE `{2}` IN @{3}", tableName, selectAsValue, result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);
            result.SelectLimit = string.Format("SELECT {1} FROM `{0}` LIMIT @Skip,@Take", tableName, selectAsValue);
            result.SelectCount = string.Format("SELECT COUNT(*) FROM `{0}`", tableName);
            result.WhereID = string.Format("`{0}`=@{1}", result.Table.DefaultTableKey, result.Table.DefaultPropertyKey);

            return result;
        }
        /// <summary>
        /// 获取MySQL类型定义
        /// </summary>
        /// <returns></returns>
        public static StringBuilder GetDefinitionType(DbColAttribute dbCol)
        {
            var defType = GetDbType(dbCol);
            return new StringBuilder()
                .AppendFormat("`{0}` ", dbCol.Name)
                .Append(defType)
                .Append(dbCol.Key == DbIxType.PK ? " PRIMARY KEY" : "")
                .Append(dbCol.Key == DbIxType.APK ? " PRIMARY KEY AUTO_INCREMENT" : "")
                .Append(dbCol.IsReq ? " NOT NULL" : " NULL")
                .Append(dbCol.Default == null ? "" : string.Format(" DEFAULT '{0}'", dbCol.Default))
                .AppendFormat(" COMMENT '{0}'", GetComment(dbCol.Display));
        }

        private static string GetComment(string display)
        {
            return display.Replace("'", "''").Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// 获取MySql类型
        /// </summary>
        /// <param name="dbCol"></param>
        /// <returns></returns>
        public static string GetDbType(DbColAttribute dbCol)
        {
            switch (dbCol.Type)
            {
                case DbColType.String: return string.Format("varchar({0})", dbCol.Len);
                case DbColType.StringMax: return string.Format("longtext");
                case DbColType.StringMedium: return string.Format("mediumtext");
                case DbColType.StringNormal: return string.Format("text");
                case DbColType.Boolean: return string.Format("bit");
                case DbColType.Byte: return string.Format("tinyint");
                case DbColType.Char: return string.Format("tinyint");
                case DbColType.Int16: return string.Format("smallint");
                case DbColType.Int32: return string.Format("int");
                case DbColType.Int64: return string.Format("bigint");
                case DbColType.UByte: return string.Format("tinyint unsigned");
                case DbColType.UInt16: return string.Format("smallint unsigned");
                case DbColType.UInt32: return string.Format("int unsigned");
                case DbColType.UInt64: return string.Format("bigint unsigned");
                case DbColType.Single:
                case DbColType.Double: return string.Format("double");
                case DbColType.Decimal: return string.Format("decimal({0},{1})", dbCol.Len, dbCol.Digit);
                case DbColType.DateTime: return string.Format("datetime");
                case DbColType.JsonString: return string.Format("text");
                case DbColType.Guid: return string.Format("guid");
                case DbColType.Blob: return string.Format("blob");
                case DbColType.XmlString: return string.Format("text");
                case DbColType.Enum: return string.Format("varchar(10)");
                case DbColType.Set: return string.Format("varchar(10)");
                default: throw new Exception("未确定解决方案");
            }
        }
        #endregion
    }
}
