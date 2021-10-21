using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Cobber
{
    /// <summary>
    /// 数据库表模型
    /// </summary>
    public class DbTableModel
    {
        /// <summary>
        /// 类型构造
        /// </summary>
        /// <param name="type"></param>
        public DbTableModel(Type type)
        {
            ClassName = type.Name;
            ClassType = type;
            DbCol = type.GetCustomAttribute<DbColAttribute>() ?? new DbColAttribute(type.Name);
            TableName = DbCol.Name = DbCol.Name ?? type.Name;
            TableComment = Regex.Replace(DbCol.Display, "\\s+", " ");
            var cols = new List<DbColumnModel>();
            var props = new List<PropertyInfo>();
            var colNames = new List<string>();
            var propNames = new List<string>();
            var indexes = new List<DbIndexModel>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!DbColAttribute.TryGetAttribute(prop, out DbColAttribute colAttr)) { continue; }
                var colModel = new DbColumnModel(prop, colAttr);
                switch (colAttr.Key)
                {
                    case DbIxType.PK:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        DefaultTableKey = colModel.ColumnName;
                        DefaultPropertyKey = colModel.PropertyName;
                        break;
                    case DbIxType.APK: // 自增主键
                        DefaultTableKey = colModel.ColumnName;
                        DefaultPropertyKey = colModel.PropertyName;
                        break;
                    case DbIxType.PFK:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        DefaultTableKey = colModel.ColumnName;
                        DefaultPropertyKey = colModel.PropertyName;
                        break;
                    case DbIxType.FK:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        break;
                    case DbIxType.IX:
                    case DbIxType.UIX:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        if (string.IsNullOrWhiteSpace(colModel.Index))
                        {
                            colModel.Index = colModel.ColumnName;
                        }
                        indexes.Add(new DbIndexModel(colModel));
                        break;
                    default:
                        colNames.Add(colModel.ColumnName);
                        propNames.Add(colModel.PropertyName);
                        break;
                }
                cols.Add(colModel);
                props.Add(prop);
            }
            Columns = cols.ToArray();
            Properties = props.ToArray();
            ColumnNames = colNames.ToArray();
            PropertyNames = propNames.ToArray();
            Indexes = indexes.ToArray();
        }
        /// <summary>
        /// 注解值
        /// </summary>
        public DbColAttribute DbCol { get; }
        /// <summary>
        /// 类名
        /// </summary>
        public virtual string ClassName { get; }
        /// <summary>
        /// 类类型
        /// </summary>
        public virtual Type ClassType { get; }
        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName { get; }
        /// <summary>
        /// 表注释
        /// </summary>
        public virtual String TableComment { get; }

        /// <summary>
        /// 表标识字段
        /// </summary>
        public virtual String[] TableID { get; }
        /// <summary>
        /// 默认数据库字段标识
        /// </summary>
        public virtual string DefaultTableKey { get; }
        /// <summary>
        /// 默认类属性标识
        /// </summary>
        public virtual string DefaultPropertyKey { get; }
        /// <summary>
        /// 列序列
        /// </summary>
        public virtual DbColumnModel[] Columns { get; }
        /// <summary>
        /// 属性列表
        /// </summary>
        public virtual PropertyInfo[] Properties { get; }
        /// <summary>
        /// 列名字段
        /// </summary>
        public virtual string[] ColumnNames { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public virtual string[] PropertyNames { get; }
        /// <summary>
        /// 索引序列
        /// </summary>
        public virtual DbIndexModel[] Indexes { get; }
    }
    /// <summary>
    /// 数据库列模型
    /// </summary>
    public class DbColumnModel : IDbColInfo
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="prop"></param>
        public DbColumnModel(PropertyInfo prop) : this(prop, DbColAttribute.GetAttribute(prop))
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="dbCol"></param>
        public DbColumnModel(PropertyInfo prop, DbColAttribute dbCol)
        {
            dbCol.Name = dbCol.Name ?? prop.Name;
            Property = prop;
            DbCol = dbCol;
            ColumnName = dbCol.Name = dbCol.Name ?? prop.Name;
            ColumnComment = dbCol.Display;
            PropertyName = prop.Name;
            IsPK = dbCol.Key == DbIxType.APK || dbCol.Key == DbIxType.FK || dbCol.Key == DbIxType.PFK;
            IsAuto = dbCol.Key == DbIxType.APK;

            #region // DbCol
            Display = dbCol.Display;
            Name = dbCol.Name ?? prop.Name;
            Type = dbCol.Type;
            Len = dbCol.Len;
            IsReq = dbCol.IsReq;
            Default = dbCol.Default;
            Key = dbCol.Key;
            Index = dbCol.Index;
            Ignore = dbCol.Ignore;
            Digit = dbCol.Digit;
            #endregion
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        public virtual PropertyInfo Property { get; }
        /// <summary>
        /// 列属性
        /// </summary>
        public virtual DbColAttribute DbCol { get; }
        /// <summary>
        /// 列名
        /// </summary>
        public virtual string ColumnName { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public virtual string PropertyName { get; }
        /// <summary>
        /// 列注释
        /// </summary>
        public virtual String ColumnComment { get; }
        /// <summary>
        /// 是主键
        /// </summary>
        public virtual bool IsPK { get; }
        /// <summary>
        /// 是自增
        /// </summary>
        public virtual bool IsAuto { get; }
        #region // DbCol
        /// <summary>
        /// 显示名默认类名
        /// </summary>
        public string Display { get; }
        /// <summary>
        /// 列名默认是属性名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 类型
        /// </summary>
        public DbColType Type { get; }
        /// <summary>
        /// 长度默认64位
        /// </summary>
        public long Len { get; }
        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsReq { get; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object Default { get; }
        /// <summary>
        /// 是否主键
        /// 0=>无
        /// 10=>主键
        /// 20=>主键+外键
        /// 30=>外键
        /// </summary>
        public DbIxType Key { get; }
        /// <summary>
        /// 索引名,使用|分割
        /// </summary>
        public string Index { get; internal set; }
        /// <summary>
        /// 忽略映射
        /// </summary>
        public bool Ignore { get; }
        /// <summary>
        /// 精度
        /// </summary>
        public int Digit { get; }
        #endregion
    }
    /// <summary>
    /// 数据库索引模型
    /// </summary>
    public class DbIndexModel
    {
        /// <summary>
        /// 标记列信息
        /// </summary>
        public DbColumnModel Column { get; }
        /// <summary>
        /// 使用列信息
        /// </summary>
        public DbColumnModel[] Columns { get; private set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string[] ColumnNames { get; }
        /// <summary>
        /// 索引名
        /// </summary>
        public String IndexName { get; }
        /// <summary>
        /// 是唯一
        /// </summary>
        public bool IsUnique { get; }
        /// <summary>
        /// 构造
        /// </summary>
        public DbIndexModel(DbColumnModel column)
        {
            Column = column;
            ColumnNames = column.Index.Split('|');
            IndexName = column.Index;
            IsUnique = column.Key == DbIxType.UIX;
        }
    }
}
