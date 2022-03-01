using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Cobber
{
    /// <summary>
    /// 默认SQL语句属性
    /// 自动生成SQL语句标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
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
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool TryGetAttribute(PropertyInfo prop, out DbColAttribute colAttr, bool inherit = false)
        {
            foreach (var item in prop.GetCustomAttributes(inherit))
            {
                string typeName = item.GetType().Name;
                switch (typeName)
                {
                    case "NotMappedAttribute":
                    case "IgnoreAttribute":
                        colAttr = null;
                        return false;
                    case nameof(DbColAttribute):
                        colAttr = item as DbColAttribute;
                        colAttr.Name ??= prop.Name;
                        return colAttr != null && !colAttr.Ignore;
                    default:
                        break;
                }
            }
            colAttr = null;
            return false;
        }
        /// <summary>
        /// 获取属性内容
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static DbColAttribute GetAttribute(PropertyInfo prop, bool inherit = false)
        {
            foreach (var item in prop.GetCustomAttributes(inherit))
            {
                var typeName = item.GetType().Name;
                switch (typeName)
                {
                    case "NotMappedAttribute":
                    case "IgnoreAttribute":
                        return GetIgnore(prop.Name);
                    case nameof(DbColAttribute):
                        var colAttr = item as DbColAttribute;
                        colAttr.Name ??= prop.Name;
                        return colAttr;
                    default:
                        break;
                }
            }
            return GetIgnore(prop.Name);
        }
        /// <summary>
        /// 获取属性内容
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static DbColAttribute GetAttribute(Type type, bool inherit = false)
        {
            foreach (var item in type.GetCustomAttributes(inherit))
            {
                var typeName = item.GetType().Name;
                switch (typeName)
                {
                    case "NotMappedAttribute":
                    case "IgnoreAttribute":
                        return GetIgnore(type.Name);
                    case nameof(DbColAttribute):
                        var result = item as DbColAttribute;
                        result.Name ??= type.Name;
                        return result;
                    default:
                        break;
                }
            }
            return GetIgnore(type.Name);
        }
        /// <summary>
        /// 尝试获取列名
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="keyName"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool TryGetColName(PropertyInfo prop, out Extter.Tuble2KeyName keyName, bool inherit = false)
        {
            if (prop == null)
            {
                keyName = null;
                return false;
            }
            foreach (var item in prop.GetCustomAttributes(inherit))
            {
                string typeName = item.GetType().Name;
                switch (typeName)
                {
                    case "NotMappedAttribute":
                    case "IgnoreAttribute":
                        keyName = null;
                        return false;
                    case nameof(DbColAttribute):
                        var attr = item as DbColAttribute;
                        attr.Name ??= prop.Name;
                        if (attr.Ignore)
                        {
                            keyName = null;
                            return false;
                        }
                        keyName = new Extter.Tuble2KeyName(attr.Name ?? prop.Name, prop.Name);
                        return true;
                    default:
                        break;
                }
            }
            keyName = null;
            return false;
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
        public EDisplayAttribute(string display, string name = null)
        {
            if (display == null)
            {
                Name = Display = name ?? String.Empty;
            }
            else
            {
                Display = display;
                Name = name ?? display;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EDisplayAttribute(string display, Type type)
        {
            Name = type.Name;
            Display = display;
            Type = type;
        }
        /// <summary>
        /// 名称(内部别名/Key名)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 描述内容
        /// </summary>
        public string Description { get; set; }
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
    /// <summary>
    /// 枚举显示泛型类
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EDisplayAttr<TEnum> : IJavaEnum<TEnum>
        where TEnum : Enum
    {
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<String> AllNames { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<TEnum> AllEnums { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<int> AllValues { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<string> AllEnumNames { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<EDisplayAttr<TEnum>> AllAttrs { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<String> Names { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<TEnum> Enums { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<int> Values { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<String> EnumNames { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static IReadOnlyCollection<EDisplayAttr<TEnum>> Attrs { get; }
        /// <summary>
        /// 未知值(枚举的默认第一个)
        /// </summary>
        public static EDisplayAttr<TEnum> Unknown { get; }
        static EDisplayAttr()
        {
            var type = typeof(TEnum);
            var values = System.Enum.GetValues(type);
            var allEnums = (TEnum[])values;
            var allValues = (int[])values;
            var allEnumNames = System.Enum.GetNames(type);
            AllEnums = new ReadOnlyCollection<TEnum>(allEnums);
            AllValues = new ReadOnlyCollection<int>(allValues);
            AllEnumNames = new ReadOnlyCollection<string>(allEnumNames);
            var allNames = new List<String>();
            var allAttrs = new List<EDisplayAttr<TEnum>>();
            for (int i = 0; i < allEnumNames.Length; i++)
            {
                var ename = allEnumNames[i];
                var eval = allValues[i];
                var enumber = allEnums[i];
                var prop = type.GetField(ename);
                var attr = prop.GetCustomAttribute<EDisplayAttribute>();
                if (attr == null)
                {
                    var dispName = ename;
                    var descrName = ename;
                    Type etype = null;
                    var disp = prop.GetCustomAttribute<DisplayAttribute>();
                    if (disp == null)
                    {
                        var descr = prop.GetCustomAttribute<DescriptionAttribute>();
                        if (descr != null)
                        {
                            dispName = descr.Description;
                            descrName = descr.Description;
                        }
                    }
                    else
                    {
                        dispName = disp.Name;
                        descrName = disp.Description;
                        etype = disp.ResourceType;
                    }
                    attr = new EDisplayAttribute(descrName, dispName)
                    {
                        Type = etype,
                    };
                }
                allAttrs.Add(new EDisplayAttr<TEnum>(enumber, eval, attr));
                allNames.Add(attr.Name);
            }
            AllAttrs = new ReadOnlyCollection<EDisplayAttr<TEnum>>(allAttrs);
            AllNames = new ReadOnlyCollection<String>(allNames);

            Names = new ReadOnlyCollection<String>(AllNames.Skip(1).ToList());
            Enums = new ReadOnlyCollection<TEnum>(AllEnums.Skip(1).ToList());
            Values = new ReadOnlyCollection<int>(AllValues.Skip(1).ToList());
            EnumNames = new ReadOnlyCollection<String>(AllEnumNames.Skip(1).ToList());
            Attrs = new ReadOnlyCollection<EDisplayAttr<TEnum>>(allAttrs.Skip(1).ToList());
            Unknown = AllAttrs.First();
        }
        /// <summary>
        /// 枚举值
        /// </summary>
        public TEnum Enum { get; }
        /// <summary>
        /// 值
        /// </summary>
        public Int32 Value { get; }
        /// <summary>
        /// 枚举名
        /// </summary>
        public String EnumName { get; }
        /// <summary>
        /// 名
        /// </summary>
        public String Name { get; }
        /// <summary>
        /// 显示名
        /// </summary>
        public String Display { get; }
        /// <summary>
        /// 属性类
        /// </summary>
        public EDisplayAttribute Attribute { get; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        private EDisplayAttr(TEnum enumValue, int value, EDisplayAttribute attr)
        {
            Enum = enumValue;
            Name = attr.Name;
            Display = attr.Display;
            Attribute = attr;
            Value = value;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this == null) { return Unknown.Name; }
            return this.Name;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToEnumString()
        {
            if (this == null) { return Unknown.EnumName; }
            return this.EnumName;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public virtual TEnum ToEnum()
        {
            if (this == null) { return Unknown.Enum; }
            return this.Enum;
        }
        #region // 转换内容
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator TEnum(EDisplayAttr<TEnum> enumValue)
        {
            return enumValue.Enum;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator String(EDisplayAttr<TEnum> enumValue)
        {
            return enumValue.Name;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator int(EDisplayAttr<TEnum> enumValue)
        {
            return enumValue.Value;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator EDisplayAttr<TEnum>(TEnum enumValue)
        {
            foreach (var item in Attrs)
            {
                if (item.Enum.Equals(enumValue)) { return item; }
            }
            return Unknown;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator EDisplayAttr<TEnum>(int enumValue)
        {
            foreach (var item in Attrs)
            {
                if (item.Value == enumValue) { return item; }
            }
            return Unknown;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator EDisplayAttr<TEnum>(String enumValue)
        {
            // 枚举优先1
            foreach (var item in Attrs)
            {
                if (item.EnumName == enumValue) { return item; }
            }
            // 枚举名称2
            foreach (var item in Attrs)
            {
                if (item.Name == enumValue) { return item; }
            }
            // 枚举值3
            if (int.TryParse(enumValue, out var value))
            {
                foreach (var item in Attrs)
                {
                    if (item.Value == value) { return item; }
                }
            }
            return Unknown;
        }
        #endregion
    }
}
