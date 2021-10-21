using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 默认SQL语句属性
    /// 自动生成SQL语句标签
    /// </summary>
    public class DbColAttribute : Attribute, IDbColInfo
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public DbColAttribute(string display)
        {
            Display = display ?? string.Empty;
        }
        /// <summary>
        /// 显示名默认类名
        /// </summary>
        public string Display { get; private set; }
        /// <summary>
        /// 列名默认是属性名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public DbColType Type { get; set; }
        /// <summary>
        /// 长度默认64位
        /// </summary>
        public long Len { get; set; } = 64;
        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsReq { get; set; } = true;
        /// <summary>
        /// 默认值
        /// </summary>
        public object Default { get; set; }
        /// <summary>
        /// 是否主键
        /// 0=>无
        /// 10=>主键
        /// 20=>主键+外键
        /// 30=>外键
        /// </summary>
        public DbIxType Key { get; set; }
        /// <summary>
        /// 索引名,使用|分割
        /// </summary>
        public string Index { get; set; }
        /// <summary>
        /// 忽略映射
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 精度
        /// </summary>
        public int Digit { get; set; }
        /// <summary>
        /// 获取属性注解
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="colAttr"></param>
        /// <returns></returns>
        public static bool TryGetAttribute(PropertyInfo prop, out DbColAttribute colAttr)
        {
            colAttr = null;
            foreach (var item in prop.GetCustomAttributes(false))
            {
                var typeName = item.GetType().Name;
                switch (typeName)
                {
                    case "NotMappedAttribute":
                    case "IgnoreAttribute":
                        return false;
                    case "DbColAttribute":
                        colAttr = item as DbColAttribute;
                        break;
                    default:
                        break;
                }

            }
            return colAttr != null && !colAttr.Ignore;
        }
        /// <summary>
        /// 获取属性内容
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static DbColAttribute GetAttribute(PropertyInfo prop)
        {
            foreach (var item in prop.GetCustomAttributes(false))
            {
                var typeName = item.GetType().Name;
                switch (typeName)
                {
                    case "NotMappedAttribute":
                    case "IgnoreAttribute":
                        return GetIgnore(prop.Name);
                    case "DbColAttribute":
                        return item as DbColAttribute;
                    default:
                        break;
                }

            }
            return GetIgnore(prop.Name);
        }

        private static DbColAttribute GetIgnore(String display)
        {
            return new DbColAttribute(display) { Ignore = true };
        }
    }
    /// <summary>
    /// 数据库列类型
    /// </summary>
    [EDisplay("数据库列类型")]
    public enum DbColType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        [Display(Name = "字符串")]
        String = 0,
        /// <summary>
        /// 最长字符串
        /// MySql(LONGTEXT):4294967295 (2^32 - 1) 约4Gb
        /// Mssql(nvarchar(max)):
        /// </summary>
        [Display(Name = "最长字符串")]
        StringMax,
        /// <summary>
        /// 布尔
        /// </summary>
        [Display(Name = "布尔型")]
        Boolean,
        /// <summary>
        /// Byte
        /// </summary>
        [Display(Name = "字节")]
        Byte,
        /// <summary>
        /// Byte
        /// </summary>
        [Display(Name = "字符")]
        Char,
        /// <summary>
        /// short
        /// </summary>
        [Display(Name = "短整型")]
        Int16,
        /// <summary>
        /// int
        /// </summary>
        [Display(Name = "整型")]
        Int32,
        /// <summary>
        /// long
        /// </summary>
        [Display(Name = "长整型")]
        Int64,
        /// <summary>
        /// FLOAT
        /// </summary>
        [Display(Name = "浮点型")]
        Single,
        /// <summary>
        /// Double
        /// </summary>
        [Display(Name = "双精度浮点型")]
        Double,
        /// <summary>
        /// Double
        /// </summary>
        [Display(Name = "货币型")]
        Decimal,
        /// <summary>
        /// 日期时间
        /// </summary>
        [Display(Name = "日期时间")]
        DateTime,
        /// <summary>
        /// Json数据类型
        /// </summary>
        [Display(Name = "Json数据类型")]
        JsonString,
        /// <summary>
        /// 枚举类型
        /// </summary>
        [Display(Name = "枚举类型")]
        Enum,
        /// <summary>
        /// 集合类型
        /// </summary>
        [Display(Name = "集合类型")]
        Set,
        /// <summary>
        /// 对象类型
        /// </summary>
        [Display(Name = "对象类型")]
        Blob,
        /// <summary>
        /// byte
        /// </summary>
        [Display(Name = "无符号字节")]
        UByte,
        /// <summary>
        /// short
        /// </summary>
        [Display(Name = "无符号短整型")]
        UInt16,
        /// <summary>
        /// int
        /// </summary>
        [Display(Name = "无符号整型")]
        UInt32,
        /// <summary>
        /// long
        /// </summary>
        [Display(Name = "无符号长整型")]
        UInt64,
        /// <summary>
        /// Guid UUID
        /// </summary>
        [Display(Name = "全局唯一标识符")]
        Guid,
        /// <summary>
        /// 通用长度
        /// MySql(Text):65535 (2^16 - 1)约64KB
        /// </summary>
        [Display(Name = "字符串")]
        StringNormal,
        /// <summary>
        /// 中等长度
        /// MySql(MediumText):16777215 (2^24 - 1)约16MB
        /// </summary>
        [Display(Name = "字符串")]
        StringMedium,
        /// <summary>
        /// XML数据类型
        /// </summary>
        [Display(Name = "XML数据类型")]
        XmlString,
        /// <summary>
        /// 字符数组
        /// </summary>
        [Display(Name = "字符数组")]
        CharArray,
    }
    /// <summary>
    /// 数据库索引类型
    /// </summary>
    [EDisplay("数据库索引类型")]
    public enum DbIxType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = "未知")]
        Unknown = 0,
        /// <summary>
        /// 主键
        /// </summary>
        [Display(Name = "主键")]
        PK = 10,
        /// <summary>
        /// 自增主键
        /// </summary>
        [Display(Name = "自增主键")]
        APK = 11,
        /// <summary>
        /// 主键外键
        /// </summary>
        [Display(Name = "主键外键")]
        PFK = 20,
        /// <summary>
        /// 外键
        /// </summary>
        [Display(Name = "外键")]
        FK = 30,

        /// <summary>
        /// 普通索引
        /// </summary>
        [Display(Name = "普通索引")]
        IX = 40,
        /// <summary>
        /// 唯一索引
        /// </summary>
        [Display(Name = "唯一索引")]
        UIX = 50
    }
    /// <summary>
    /// 显示标记属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class EDisplayAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EDisplayAttribute(string display) : this(display, string.Empty)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EDisplayAttribute(string display, string name)
        {
            Name = name;
            Display = display;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EDisplayAttribute(string display, Type type)
        {
            Name = type.Name;
            Display = display;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }
    }
    /// <summary>
    /// 默认SQL语句属性
    /// 加载SQL语句标签
    /// </summary>
    public class DbSqlAttribute : Attribute
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public DbSqlAttribute(string value) : this(null, value, DateTime.Now.DayOfYear)
        {
        }
        /// <summary>
        /// 参数构造
        /// </summary>
        public DbSqlAttribute(string value, int version) : this(null, value, version)
        {
        }
        /// <summary>
        /// 参数构造
        /// </summary>
        public DbSqlAttribute(string display, string value) : this(display, value, DateTime.Now.DayOfYear)
        {
        }
        /// <summary>
        /// 参数构造
        /// </summary>
        public DbSqlAttribute(string display, string value, int version)
        {
            Display = display;
            Value = value;
            Version = version;
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// SQL语句
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public long Version { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
    }
    /// <summary>
    /// 数据列信息
    /// </summary>
    public interface IDbColInfo
    {
        /// <summary>
        /// 显示名默认类名
        /// </summary>
        string Display { get; }
        /// <summary>
        /// 列名默认是属性名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 类型
        /// </summary>
        DbColType Type { get; }
        /// <summary>
        /// 长度默认64位
        /// </summary>
        long Len { get; }
        /// <summary>
        /// 必填项
        /// </summary>
        bool IsReq { get; }
        /// <summary>
        /// 默认值
        /// </summary>
        object Default { get; }
        /// <summary>
        /// 是否主键
        /// 0=>无
        /// 10=>主键
        /// 20=>主键+外键
        /// 30=>外键
        /// </summary>
        DbIxType Key { get; }
        /// <summary>
        /// 索引名,使用|分割
        /// </summary>
        string Index { get; }
        /// <summary>
        /// 忽略映射
        /// </summary>
        bool Ignore { get; }
        /// <summary>
        /// 精度
        /// </summary>
        int Digit { get; }
    }
}
