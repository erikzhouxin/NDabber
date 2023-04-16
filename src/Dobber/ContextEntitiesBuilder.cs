using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Common;
using System.Data.Dabber;
using System.Data.Extter;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Data.Dobber.ContextEntitiesBuilder;

namespace System.Data.Dobber
{
    /// <summary>
    /// 上下文实体创建者
    /// </summary>
    public static class ContextEntitiesBuilder
    {
        /// <summary>
        /// 创建实体环境接口
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static IContextEntitiesBuilder Create(StoreType storeType)
        {
            return storeType switch
            {
                StoreType.SQLite => new ContextEntitiesSQLiteBuilder(),
                StoreType.SqlServer => new ContextEntitiesSqlServerBuilder(),
                StoreType.MySQL => new ContextEntitiesMySQLBuilder(),
                StoreType.Oracle => new ContextEntitiesOracleBuilder(),
                StoreType.PostgreSQL => new ContextEntitiesPostgreSQLBuilder(),
                StoreType.Redis => throw new NotSupportedException("不支持[Redis]的创建方式"),
                StoreType.Access => throw new NotSupportedException("不支持[Access]的创建方式"),
                StoreType.Excel => throw new NotSupportedException("不支持[Excel]的创建方式"),
                StoreType.Xml => throw new NotSupportedException("不支持[Xml]的创建方式"),
                StoreType.Memory => throw new NotSupportedException("不支持[Memory]的创建方式"),
                StoreType.Unknown => throw new NotSupportedException("不支持[Unknown]的创建方式"),
                _ => throw new NotSupportedException("不支持[默认]的创建方式"),
            };
        }
        /// <summary>
        /// 特殊类型枚举
        /// </summary>
        [Flags]
        public enum SpecialTypeEnum
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// char(2)为int32
            /// </summary>
            Char2Int32 = 2,
        }
    }
    /// <summary>
    /// 创建实体
    /// </summary>
    public interface IContextEntitiesBuilder
    {
        /// <summary>
        /// 设置使用的命名空间
        /// </summary>
        /// <param name="usings"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetUsings(params string[] usings);
        /// <summary>
        /// 设置命名空间
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetNamespace(string nameSpace);
        /// <summary>
        /// 设置表名前置字符串
        /// </summary>
        /// <param name="preTable"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetPreTable(string preTable);
        /// <summary>
        /// 设置视图前置字符串
        /// </summary>
        /// <param name="preTable"></param>
        /// <param name="preView"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetPreTable(string preTable, string preView);
        /// <summary>
        /// 设置忽略表或列(列名请使用[表.列])
        /// </summary>
        /// <param name="ignoreList"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetIgnoreTableOrColumn(params string[] ignoreList);
        /// <summary>
        /// 添加接口定义
        /// </summary>
        /// <returns></returns>
        IContextEntitiesBuilder SetHasInterface(bool hasInterface = true);
        /// <summary>
        /// 添加模型定义
        /// </summary>
        /// <param name="nameSpace">不为空时有此项</param>
        /// <returns></returns>
        IContextEntitiesBuilder SetHasModel(string nameSpace);
        /// <summary>
        /// 设置忽略表或列(列名请使用[表.列])
        /// </summary>
        /// <param name="ignoreList"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetIgnoreTableOrColumn(IEnumerable<string> ignoreList);
        /// <summary>
        /// 设置特殊类型转换
        /// </summary>
        /// <param name="char2Int32"></param>
        /// <returns></returns>
        IContextEntitiesBuilder SetSpecialType(ContextEntitiesBuilder.SpecialTypeEnum char2Int32);
        /// <summary>
        /// 创建单个字符串文本
        /// </summary>
        /// <returns></returns>
        StringBuilder GetCodeSingle(DbConnection conn);
        /// <summary>
        /// 创建多个字符串文本
        /// </summary>
        /// <returns></returns>
        StringBuilder[] GetCodeMultiple(DbConnection conn);
    }
    internal class ContextEntitiesSQLiteBuilder : IContextEntitiesBuilder
    {
        public ContextEntitiesSQLiteBuilder()
        {

        }

        public StringBuilder[] GetCodeMultiple(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetCodeSingle(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(params string[] ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(IEnumerable<string> ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetNamespace(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable, string preView)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetUsings(params string[] usings)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasInterface(bool hasInterface = true)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasModel(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetSpecialType(ContextEntitiesBuilder.SpecialTypeEnum char2Int32)
        {
            throw new NotImplementedException();
        }
    }
    internal class ContextEntitiesSqlServerBuilder : IContextEntitiesBuilder
    {
        public StringBuilder[] GetCodeMultiple(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetCodeSingle(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(params string[] ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(IEnumerable<string> ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetNamespace(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable, string preView)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetUsings(params string[] usings)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasInterface(bool hasInterface = true)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasModel(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetSpecialType(SpecialTypeEnum char2Int32)
        {
            throw new NotImplementedException();
        }
    }
    internal class ContextEntitiesMySQLBuilder : IContextEntitiesBuilder
    {
        private HashSet<String> _usings = new HashSet<String>()
        {
            "using System;",
            "using System.Collections.Generic;",
            "using System.ComponentModel.DataAnnotations;",
            "using System.ComponentModel.DataAnnotations.Schema;",
            "using System.Data.Cobber;",
            "using System.Text;",
        };
        private string _nameSpace = "System.Data.Entities";
        private string _modelNamespace = "System.Data.Models";
        private string _dbName = "mysql";
        private bool _ignoreDbName = false;
        private bool _hasInterface = false;
        private bool _hasModel = false;
        private List<string> _ignoreList = new List<string>();
        private string _preTable = "T";
        private String _preView = "V";
        private SpecialTypeEnum _spacialType = SpecialTypeEnum.Unknown;
        /// <summary>
        /// 构造
        /// </summary>
        public ContextEntitiesMySQLBuilder()
        {

        }
        public StringBuilder[] GetCodeMultiple(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetCodeSingle(DbConnection conn)
        {
            _dbName = conn.Database;
            var tables = conn.Query($"select * from information_schema.tables where table_schema='{_dbName}'").ToList();
            var columns = conn.Query($"select * from information_schema.columns where table_schema='{_dbName}'").ToList();
            Func<String, string> GetTableName = (s) => s.SnakeToPascalCase();
            Func<string, String> GetViewName = (s) => s.SnakeToPascalCase();
            if (!String.IsNullOrEmpty(_preTable))
            {
                GetTableName = (s) =>
                {
                    var tableName = s.SnakeToPascalCase();
                    if (tableName.StartsWith(_preTable, StringComparison.OrdinalIgnoreCase))
                    {
                        return tableName;
                    }
                    return _preTable + tableName;
                };
            }
            if (!String.IsNullOrEmpty(_preView))
            {
                GetViewName = (s) =>
                {
                    var tableName = s.SnakeToPascalCase();
                    if (tableName.StartsWith(_preView, StringComparison.OrdinalIgnoreCase))
                    {
                        return tableName;
                    }
                    return _preView + tableName;
                };
            }
            var tableModels = new List<TableModel>();
            var copyTableReg = new Regex(@"\w+(_copy)\d+$");
            foreach (var tabItem in tables)
            {
                string tableName = tabItem.TABLE_NAME.ToLower();
                if (_ignoreList.Contains(tableName)) { continue; }
                if (copyTableReg.IsMatch(tableName)) { continue; }
                string tableComment = tabItem.TABLE_COMMENT;
                var className = GetTableName(tableName);
                if ("VIEW".Equals(tabItem.TABLE_TYPE, StringComparison.OrdinalIgnoreCase))
                {
                    className = GetViewName(tableName);
                }
                var tableColumns = new List<ColumnModel>();
                foreach (var colItem in columns.Where(s => tableName.Equals(s.TABLE_NAME)).OrderBy(s => s.ORDINAL_POSITION))
                {
                    string colName = colItem.COLUMN_NAME;
                    if (_ignoreList.Contains($"{tableName}.{colName.ToLower()}")) { continue; }
                    var propName = colName.SnakeToPascalCase();
                    string colComment = colItem.COLUMN_COMMENT;
                    string colType = colItem.COLUMN_TYPE;
                    bool colIsPK = "PRI".Equals(colItem.COLUMN_KEY);
                    bool colIsAuto = "auto_increment".Equals(colItem.EXTRA);
                    bool colIsNull = "YES".Equals(colItem.IS_NULLABLE);
                    DbColType cType;
                    String pType;
                    int colLen = 64;
                    int colDigit = 2;
                    if (colType == "bigint")
                    {
                        cType = DbColType.Int64;
                        pType = colIsNull ? "Int64?" : "Int64";
                    }
                    else if (colType == "bigint unsigned")
                    {
                        cType = DbColType.UInt64;
                        pType = colIsNull ? "UInt64?" : "UInt64";
                    }
                    else if (Regex.IsMatch(colType, "bit(\\(\\d+\\))?"))
                    {
                        cType = DbColType.Boolean;
                        pType = colIsNull ? "Boolean?" : "Boolean";
                    }
                    else if (colType == "date" || colType == "datetime")
                    {
                        cType = DbColType.DateTime;
                        pType = colIsNull ? "DateTime?" : "DateTime";
                    }
                    else if (colType == "decimal")
                    {
                        cType = DbColType.Decimal;
                        pType = colIsNull ? "Decimal?" : "Decimal";
                    }
                    else if (Regex.IsMatch(colType, "decimal(\\(\\S+\\))"))
                    {
                        cType = DbColType.Decimal;
                        pType = colIsNull ? "Decimal?" : "Decimal";
                        var number = Regex.Replace(colType, "decimal(\\(\\S+\\))", "$1").TrimStart('(').TrimEnd(')');
                        if (string.IsNullOrWhiteSpace(number))
                        {
                            var numArr = number.Split(',');
                            if (numArr.Length == 1)
                            {
                                colLen = Convert.ToInt32(numArr[0]);
                            }
                            else if (numArr.Length == 2)
                            {
                                colLen = Convert.ToInt32(numArr[0]);
                                colDigit = Convert.ToInt32(numArr[1]);
                            }
                        }
                    }
                    else if (colType == "double")
                    {
                        cType = DbColType.Double;
                        pType = colIsNull ? "Double?" : "Double";
                    }
                    else if (Regex.IsMatch(colType, "double(\\(\\S+\\))?"))
                    {
                        cType = DbColType.Double;
                        pType = colIsNull ? "Double?" : "Double";
                        var number = Regex.Replace(colType, "double(\\(\\S+\\))", "$1").TrimStart('(').TrimEnd(')');
                        if (string.IsNullOrWhiteSpace(number))
                        {
                            var numArr = number.Split(',');
                            if (numArr.Length == 1)
                            {
                                colLen = Convert.ToInt32(numArr[0]);
                            }
                            else if (numArr.Length == 2)
                            {
                                colLen = Convert.ToInt32(numArr[0]);
                                colDigit = Convert.ToInt32(numArr[1]);
                            }
                        }
                    }
                    else if (colType == "int" || colType == "tinyint" || colType == "mediumint" || colType == "smallint")
                    {
                        cType = DbColType.Int32;
                        pType = colIsNull ? "Int32?" : "Int32";
                    }
                    else if (colType == "int unsigned" || colType == "tinyint unsigned" || colType == "mediumint unsigned" || colType == "smallint unsigned")
                    {
                        cType = DbColType.UInt32;
                        pType = colIsNull ? "UInt32?" : "UInt32";
                    }
                    else if (colType == "longtext")
                    {
                        cType = DbColType.StringMax;
                        pType = "String";
                    }
                    else if (colType == "mediumtext")
                    {
                        cType = DbColType.StringMedium;
                        pType = "String";
                    }
                    else if (colType == "text")
                    {
                        cType = DbColType.StringNormal;
                        pType = "String";
                    }
                    else if (Regex.IsMatch(colType, "varchar(\\(\\d+\\))"))
                    {
                        cType = DbColType.String;
                        pType = "String";
                        colLen = Convert.ToInt32(Regex.Replace(colType, "varchar(\\(\\S+\\))", "$1").TrimStart('(').TrimEnd(')'));
                    }
                    else if (Regex.IsMatch(colType, "char(\\(\\d+\\))"))
                    {
                        cType = DbColType.String;
                        pType = "String";
                        colLen = Convert.ToInt32(Regex.Replace(colType, "char(\\(\\S+\\))", "$1").TrimStart('(').TrimEnd(')'));
                        if (colLen <= 2 && _spacialType.HasFlag(SpecialTypeEnum.Char2Int32))
                        {
                            cType = DbColType.Int32;
                            colType = "Int32";
                        }
                    }
                    else
                    {
                        cType = DbColType.String;
                        pType = "String";
                    }

                    tableColumns.Add(new ColumnModel(Regex.Replace(colComment, "\\s+", "").Replace("\"", ""))
                    {
                        Name = colName,
                        Property = propName,
                        Type = cType,
                        PType = pType,
                        Key = colIsAuto ? DbIxType.APK : (colIsPK ? DbIxType.PK : DbIxType.Unknown),
                        IsReq = !colIsNull,
                        Len = colLen,
                        Digit = colDigit,
                    });
                }
                tableModels.Add(new TableModel
                {
                    Name = tableName,
                    Clazz = className,
                    Comment = Regex.Replace(tableComment, "\\s+", "").Replace("\"", ""),
                    Columns = tableColumns,
                });
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in _usings)
            {
                sb.AppendLine(item);
            }
            sb
            .AppendLine($"/*********************************************************")
            .AppendLine($"* 代码使用工具自动生成，手工修改会被覆盖，使用【partial】")
            .AppendLine($"* 创 建 人：周鑫")
            .AppendLine($"* 创建时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}")
            .AppendLine($"* 描    述：自动创建MySQL生成上下文实体代码内容")
            .AppendLine($"* 创 建 类：{nameof(IContextEntitiesBuilder)}")
            .AppendLine($"* 代码使用工具自动生成，手工修改会被覆盖，使用【partial】")
            .AppendLine($"*********************************************************/");
            if (_ignoreDbName)
            {
                sb.AppendLine($"namespace {_nameSpace}");
            }
            else
            {
                sb.AppendLine($"namespace {_nameSpace}.{_dbName.PascalToSnakeCase()}");
            }
            sb.AppendLine("{").AppendLine("#pragma warning disable CS1570 // XML 注释出现 XML 格式错误");
            var black4 = "    ";
            var black8 = "        ";
            foreach (var tabItem in tableModels)
            {
                if (_hasInterface) { AppendTableInterface(sb, black4, black8, tabItem); }
                AppendTableClass(sb, black4, black8, tabItem, _hasInterface);
            }
            sb.AppendLine("#pragma warning restore CS1570 // XML 注释出现 XML 格式错误").AppendLine("}");
            if (_hasModel)
            {
                if (_ignoreDbName)
                {
                    sb.AppendLine($"namespace {_modelNamespace}");
                }
                else
                {
                    sb.AppendLine($"namespace {_modelNamespace}.{_dbName.PascalToSnakeCase()}");
                }
                sb.AppendLine("{").AppendLine("#pragma warning disable CS1570 // XML 注释出现 XML 格式错误");
                if (_modelNamespace != _nameSpace)
                {
                    sb.AppendLine($"{black4}using {_nameSpace};");
                }
                foreach (var tabItem in tableModels)
                {
                    AppendTableModel(sb, black4, black8, tabItem, _hasInterface);
                }
                sb.AppendLine("#pragma warning restore CS1570 // XML 注释出现 XML 格式错误").AppendLine("}");
            }
            return sb;
        }
        private static void AppendTableInterface(StringBuilder sb, string black4, string black8, TableModel tabItem)
        {
            sb.AppendLine($"{black4}/// <summary>").AppendLine($"{black4}/// {tabItem.Comment}").AppendLine($"{black4}/// </summary>");
            sb.AppendLine($"{black4}[DbCol(\"{tabItem.Comment}\", Name = \"{tabItem.Name}\")]");
            sb.AppendLine($"{black4}public partial interface I{tabItem.Clazz} : ICloneable");
            sb.AppendLine($"{black4}{{");
            foreach (var colItem in tabItem.Columns)
            {
                sb.AppendLine($"{black8}/// <summary>")
                  .AppendLine($"{black8}/// {colItem.Display}")
                  .AppendLine($"{black8}/// </summary>");
                if (colItem.Key == DbIxType.PK)
                {
                    sb.AppendLine($"{black8}[Key]");
                }
                else if (colItem.Key == DbIxType.APK)
                {
                    sb.AppendLine($"{black8}[Key]").AppendLine($"{black8}[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }
                sb.AppendLine($"{black8}[Display(Name = \"{colItem.Display}\")]");
                sb.AppendLine($"{black8}[Column(\"{colItem.Name}\")]");
                var colBuilder = new StringBuilder($"[DbCol(\"{colItem.Display}\"");
                colBuilder.Append($", Name = \"{colItem.Name}\"");
                if (colItem.Key == DbIxType.APK)
                {
                    colBuilder.Append($", Key = DbIxType.APK");
                }
                else if (colItem.Key == DbIxType.PK)
                {
                    colBuilder.Append($", Key = DbIxType.PK");
                }
                if (colItem.Type != DbColType.String)
                {
                    colBuilder.Append($", Type = DbColType.{colItem.Type.GetEnumName()}");
                }
                if (colItem.Len != 64)
                {
                    colBuilder.Append($", Len = {colItem.Len}");
                }
                if (!colItem.IsReq)
                {
                    colBuilder.Append($", IsReq = false");
                }
                colBuilder.Append(")]");
                sb.AppendLine($"{black8}{colBuilder}");
                sb.AppendLine($"{black8}{colItem.PType} {colItem.Property} {{ get; set; }}");
            }
            sb.AppendLine($"{black4}}}");
        }

        private static void AppendTableModel(StringBuilder sb, string black4, string black8, TableModel tabItem, bool hasInterface)
        {
            sb.AppendLine($"{black4}/// <summary>").AppendLine($"{black4}/// {tabItem.Comment}").AppendLine($"{black4}/// </summary>");
            sb.AppendLine($"{black4}[EDisplay(\"{tabItem.Comment}\")]");
            sb.AppendLine($"{black4}public partial class {tabItem.Clazz}Model : {(hasInterface ? "I" + tabItem.Clazz + ", " : "")}ICloneable");
            sb.AppendLine($"{black4}{{");
            foreach (var colItem in tabItem.Columns)
            {
                sb.AppendLine($"{black8}/// <summary>")
                  .AppendLine($"{black8}/// {colItem.Display}")
                  .AppendLine($"{black8}/// </summary>");
                sb.AppendLine($"{black8}[EDisplay(\"{colItem.Display}\")]");
                sb.AppendLine($"{black8}public virtual {colItem.PType} {colItem.Property} {{ get; set; }}");
            }
            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 默认构造")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public {tabItem.Clazz}Model() {{ }}");
            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 实体构造")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public {tabItem.Clazz}Model({tabItem.Clazz} entity) {{ SetModel(entity); }}");
            sb.AppendLine($"{black8}object ICloneable.Clone() {{ return this.Clone(); }}");
            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 浅表复制")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public virtual {tabItem.Clazz}Model Clone() {{ return ({tabItem.Clazz}Model)this.MemberwiseClone(); }}");
            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 获取实体类")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public virtual {tabItem.Clazz} GetEntity()");
            sb.AppendLine($"{black8}{{ ");
            sb.AppendLine($"{black8}{black4} return new {tabItem.Clazz}");
            sb.AppendLine($"{black8}{black4}{{ ");
            foreach (var colItem in tabItem.Columns)
            {
                sb.AppendLine($"{black8}{black8}{colItem.Property} = this.{colItem.Property},");
            }
            sb.AppendLine($"{black8}{black4}}};");
            sb.AppendLine($"{black8}}}");

            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 设置模型")
              .AppendLine($"{black8}/// </summary>");
            if (hasInterface)
            {
                sb.AppendLine($"{black8}public virtual {tabItem.Clazz}Model SetModel(I{tabItem.Clazz} entity)");
            }
            else
            {
                sb.AppendLine($"{black8}public virtual {tabItem.Clazz}Model SetModel({tabItem.Clazz} entity)");
            }
            sb.AppendLine($"{black8}{{ ");
            foreach (var colItem in tabItem.Columns)
            {
                sb.AppendLine($"{black8}{black4}this.{colItem.Property} = entity.{colItem.Property};");
            }
            sb.AppendLine($"{black8}{black4}return this;");
            sb.AppendLine($"{black8}}}");

            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 隐式转换成模型")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public static implicit operator {tabItem.Clazz}Model({tabItem.Clazz} entity) => new {tabItem.Clazz}Model(entity);");

            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 隐式转换成实体")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public static implicit operator {tabItem.Clazz}({tabItem.Clazz}Model model) => model.GetEntity();");

            sb.AppendLine($"{black4}}}");
        }

        private static void AppendTableClass(StringBuilder sb, string black4, string black8, TableModel tabItem, bool hasInterface)
        {
            sb.AppendLine($"{black4}/// <summary>").AppendLine($"{black4}/// {tabItem.Comment}").AppendLine($"{black4}/// </summary>");
            sb.AppendLine($"{black4}[Table(\"{tabItem.Name}\")]");
            sb.AppendLine($"{black4}[DbCol(\"{tabItem.Comment}\", Name = \"{tabItem.Name}\")]");
            sb.AppendLine($"{black4}public partial class {tabItem.Clazz} : {(hasInterface ? "I" + tabItem.Clazz + ", " : "")}ICloneable");
            sb.AppendLine($"{black4}{{");
            foreach (var colItem in tabItem.Columns)
            {
                sb.AppendLine($"{black8}/// <summary>")
                  .AppendLine($"{black8}/// {colItem.Display}")
                  .AppendLine($"{black8}/// </summary>");
                if (colItem.Key == DbIxType.PK)
                {
                    sb.AppendLine($"{black8}[Key]");
                }
                else if (colItem.Key == DbIxType.APK)
                {
                    sb.AppendLine($"{black8}[Key]").AppendLine($"{black8}[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }
                sb.AppendLine($"{black8}[Display(Name = \"{colItem.Display}\")]");
                sb.AppendLine($"{black8}[Column(\"{colItem.Name}\")]");
                var colBuilder = new StringBuilder($"[DbCol(\"{colItem.Display}\"");
                colBuilder.Append($", Name = \"{colItem.Name}\"");
                if (colItem.Key == DbIxType.APK)
                {
                    colBuilder.Append($", Key = DbIxType.APK");
                }
                else if (colItem.Key == DbIxType.PK)
                {
                    colBuilder.Append($", Key = DbIxType.PK");
                }
                if (colItem.Type != DbColType.String)
                {
                    colBuilder.Append($", Type = DbColType.{colItem.Type.GetEnumName()}");
                }
                if (colItem.Len != 64)
                {
                    colBuilder.Append($", Len = {colItem.Len}");
                }
                if (!colItem.IsReq)
                {
                    colBuilder.Append($", IsReq = false");
                }
                colBuilder.Append(")]");
                sb.AppendLine($"{black8}{colBuilder}");
                sb.AppendLine($"{black8}public virtual {colItem.PType} {colItem.Property} {{ get; set; }}");
            }
            sb.AppendLine($"{black8}object ICloneable.Clone() {{ return this.Clone(); }}");
            sb.AppendLine($"{black8}/// <summary>")
              .AppendLine($"{black8}/// 浅表复制")
              .AppendLine($"{black8}/// </summary>");
            sb.AppendLine($"{black8}public virtual {tabItem.Clazz} Clone() {{ return ({tabItem.Clazz})this.MemberwiseClone(); }}");
            sb.AppendLine($"{black4}}}");
        }

        internal class TableModel
        {
            public string Name { get; set; }
            public string Clazz { get; set; }
            public String Comment { get; set; }
            public List<ColumnModel> Columns { get; set; }
        }
        internal class ColumnModel
        {
            public ColumnModel(string display)
            {
                Display = display;
            }
            //
            // 摘要:
            //     显示名默认类名
            public string Display { get; }
            //
            // 摘要:
            //     列名默认是属性名
            public string Name { get; set; }
            public String Property { get; set; }
            //
            // 摘要:
            //     类型
            public DbColType Type { get; set; }
            public string PType { get; set; }
            //
            // 摘要:
            //     长度默认64位
            public long Len { get; set; }
            //
            // 摘要:
            //     必填项
            public bool IsReq { get; set; }
            //
            // 摘要:
            //     默认值
            public object Default { get; set; }
            //
            // 摘要:
            //     是否主键 0=>无 10=>主键 20=>主键+外键 30=>外键
            public DbIxType Key { get; set; }
            //
            // 摘要:
            //     索引名,使用|分割
            public string Index { get; set; }
            //
            // 摘要:
            //     忽略映射
            public bool Ignore { get; set; }
            //
            // 摘要:
            //     精度
            public int Digit { get; set; }
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(params string[] ignoreList)
        {
            if (ignoreList != null)
            {
                foreach (var ignore in ignoreList)
                {
                    _ignoreList.Add(ignore);
                }
            }
            return this;
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(IEnumerable<string> ignoreList)
        {
            if (ignoreList != null)
            {
                foreach (var ignore in ignoreList)
                {
                    _ignoreList.Add(ignore);
                }
            }
            return this;
        }

        public IContextEntitiesBuilder SetNamespace(string nameSpace)
        {
            _nameSpace = nameSpace ?? "System.Data.Entities";
            _ignoreDbName = true;
            return this;
        }
        public IContextEntitiesBuilder SetUsings(params string[] usings)
        {
            if (usings != null)
            {
                foreach (var item in usings)
                {
                    var val = item?.TrimStart() ?? String.Empty;
                    if (val.StartsWith("$"))
                    {
                        _usings.Add(val.Substring(1));
                        continue;
                    }
                    if (val.StartsWith("#"))
                    {
                        _usings.Add(val);
                        continue;
                    }
                    if (!val.StartsWith("using "))
                    {
                        val = $"using {val};";
                    }
                    _usings.Add(val);
                }
            }
            return this;
        }

        public IContextEntitiesBuilder SetPreTable(string preTable)
        {
            _preTable = preTable ?? String.Empty;
            return this;
        }

        public IContextEntitiesBuilder SetPreTable(string preTable, string preView)
        {
            _preTable = preTable ?? String.Empty;
            _preView = preView ?? string.Empty;
            return this;
        }

        public IContextEntitiesBuilder SetHasInterface(bool hasInterface = true)
        {
            _hasInterface = hasInterface;
            return this;
        }
        public IContextEntitiesBuilder SetHasModel(string modelNameSpace)
        {
            _hasModel = !string.IsNullOrEmpty(modelNameSpace);
            _modelNamespace = modelNameSpace;
            return this;
        }

        public IContextEntitiesBuilder SetSpecialType(SpecialTypeEnum char2Int32)
        {
            if (char2Int32 == SpecialTypeEnum.Unknown)
            { _spacialType = SpecialTypeEnum.Unknown; }
            _spacialType |= char2Int32;
            return this;
        }
    }
    internal class ContextEntitiesOracleBuilder : IContextEntitiesBuilder
    {
        public StringBuilder[] GetCodeMultiple(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetCodeSingle(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(params string[] ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(IEnumerable<string> ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetNamespace(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable, string preView)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetUsings(string[] usings)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasInterface(bool hasInterface = true)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasModel(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetSpecialType(SpecialTypeEnum char2Int32)
        {
            throw new NotImplementedException();
        }
    }
    internal class ContextEntitiesPostgreSQLBuilder : IContextEntitiesBuilder
    {
        public StringBuilder[] GetCodeMultiple(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetCodeSingle(DbConnection conn)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(params string[] ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetIgnoreTableOrColumn(IEnumerable<string> ignoreList)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetNamespace(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetPreTable(string preTable, string preView)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetUsings(params string[] usings)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasInterface(bool hasInterface = true)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetHasModel(string nameSpace)
        {
            throw new NotImplementedException();
        }

        public IContextEntitiesBuilder SetSpecialType(SpecialTypeEnum char2Int32)
        {
            throw new NotImplementedException();
        }
    }
}
