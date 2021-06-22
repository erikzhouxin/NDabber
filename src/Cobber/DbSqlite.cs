using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Dabber
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
            var eColAttr = type.GetCustomAttribute<DbColAttribute>() ?? new DbColAttribute(type.Name);
            result.TagName = eColAttr.Name = eColAttr.Name ?? type.Name;
            var pSql = new List<string>();
            var apSql = new List<string>();
            var pk = "id";
            var uixList = new List<string>();
            var colDefList = new List<String>();
            foreach (var prop in type.GetProperties())
            {
                if (!DbColAttribute.TryGetAttribute(prop, out DbColAttribute colAttr)) { continue; }
                colAttr.Name = colAttr.Name ?? prop.Name;
                switch (colAttr.Key)
                {
                    case DbIxType.PK:
                        pSql.Add(prop.Name);
                        pk = prop.Name;
                        break;
                    case DbIxType.APK: // 自增主键
                        pk = prop.Name;
                        break;
                    case DbIxType.PFK:
                        pSql.Add(prop.Name);
                        pk = prop.Name;
                        break;
                    case DbIxType.FK:
                        pSql.Add(prop.Name);
                        break;
                    case DbIxType.IX:
                        pSql.Add(prop.Name);
                        break;
                    case DbIxType.UIX:
                        pSql.Add(prop.Name);
                        uixList.Add(string.IsNullOrEmpty(colAttr.Index) ? prop.Name : colAttr.Index);
                        break;
                    default:
                        pSql.Add(prop.Name);
                        break;
                }
                String defType;
                switch (colAttr.Type)
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
                    case DbColType.Decimal: defType = string.Format("DECIMAL({0},{1})", colAttr.Len, colAttr.Digit); break;
                    case DbColType.DateTime: defType = string.Format("DATETIME"); break;
                    case DbColType.JsonString: defType = string.Format("TEXT"); break;
                    case DbColType.Set:
                    case DbColType.Blob: defType = string.Format("BLOB"); break;
                    default: defType = string.Format("TEXT"); break;
                }
                var nullable = colAttr.IsReq ? "NOT" : "";
                var keyable = colAttr.Key == DbIxType.PK ? " PRIMARY KEY" : (colAttr.Key == DbIxType.APK ? " PRIMARY KEY AUTOINCREMENT" : (colAttr.Default == null ? "" : $" DEFAULT '{colAttr.Default}'"));
                colDefList.Add($"[{colAttr.Name}] {defType} {nullable} NULL{keyable}{{0}} -- {colAttr.Display}");
                apSql.Add(prop.Name);
            }
            var createBuilder = new StringBuilder($"CREATE TABLE IF NOT EXISTS [{eColAttr.Name}]( -- {eColAttr.Display}").AppendLine();
            for (int i = 0; i < colDefList.Count; i++)
            {
                var spliter = ",";
                if (i + 1 == colDefList.Count) { spliter = ""; }
                createBuilder.AppendLine($"\t{string.Format(colDefList[i], spliter)}");
            }
            createBuilder.Append(");");
            foreach (var item in uixList.Distinct())
            {
                createBuilder.AppendLine().AppendFormat("CREATE UNIQUE INDEX IF NOT EXISTS [IX_{0}_{1}] ON [{0}]([{2}]);", eColAttr.Name, item.Replace("|", "_"), item.Replace("|", "],["));
            }

            var commaPSql = string.Join("],[", pSql);
            var commaAtPSql = string.Join(",@", pSql);
            var commaAPSql = string.Join("],[", apSql);
            var commaSetValSql = string.Join(",", pSql.Where(s => !s.Equals(pk)).Select(s => string.Format("[{0}]=@{0}", s)));

            result.Create = createBuilder.ToString();
            result.Insert = string.Format("INSERT INTO [{0}]([{1}]) VALUES(@{2})", eColAttr.Name, commaPSql, commaAtPSql); // 无自增主键
            result.Replace = string.Format("REPLACE INTO [{0}]([{1}]) VALUES(@{2})", eColAttr.Name, commaPSql, commaAtPSql); // 无自增主键
            result.DeleteID = string.Format("DELETE FROM [{0}] WHERE [{1}]=@{1}", eColAttr.Name, pk);
            result.DeleteInID = string.Format("DELETE FROM [{0}] WHERE [{1}] IN @{1}", eColAttr.Name, pk);
            result.UpdateID = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{2}", eColAttr.Name, commaSetValSql, pk);
            result.Update = string.Format("UPDATE [{0}] SET {1}", eColAttr.Name, commaSetValSql);
            result.Select = string.Format("SELECT [{1}] FROM [{0}]", eColAttr.Name, commaAPSql);
            result.SelectID = string.Format("SELECT [{1}] FROM [{0}] WHERE [{2}]=@{2} LIMIT 1", eColAttr.Name, commaAPSql, pk);
            result.SelectInID = string.Format("SELECT [{1}] FROM [{0}] WHERE [{2}] IN @{2}", eColAttr.Name, commaAPSql, pk);
            result.SelectLimit = string.Format("SELECT [{1}] FROM [{0}] LIMIT @Skip,@Take", eColAttr.Name, commaAPSql);
            result.SelectCount = string.Format("SELECT COUNT(*) FROM [{0}]", eColAttr.Name);
            result.Cols = apSql.ToArray();
            result.WhereID = string.Format("[{0}]=@{0}", pk);
            result.TagID = pk;

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
