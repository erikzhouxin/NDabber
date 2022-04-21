using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 获取可枚举类型接口
    /// </summary>
    public interface INEnumerable
    {
        /// <summary>
        /// 值
        /// </summary>
        Int32 Value { get; }
        /// <summary>
        /// 名
        /// </summary>
        String Name { get; }
        /// <summary>
        /// 枚举值
        /// </summary>
        object Enum { get; }
        /// <summary>
        /// 枚举名
        /// </summary>
        String EnumName { get; }
        /// <summary>
        /// 描述属性[DescriptionAttribute]
        /// </summary>
        DescriptionAttribute Description { get; }
        /// <summary>
        /// 实现名称属性[DisplayNameAttribute]
        /// </summary>
        DisplayNameAttribute DisplayName { get; }
        /// <summary>
        /// 实现显示名称属性[EDisplayAttribute]
        /// </summary>
        EDisplayAttribute EDisplay { get; }
        /// <summary>
        /// 实现显示名称属性[DisplayAttribute]
        /// </summary>
        DisplayAttribute Display { get; }
        /// <summary>
        /// 字段属性信息
        /// </summary>
        FieldInfo Field { get; }
    }
    /// <summary>
    /// 获取可枚举类型接口
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public interface INEnumerable<TEnum> : INEnumerable where TEnum : struct, Enum
    {
        /// <summary>
        /// 枚举值
        /// </summary>
        new TEnum Enum { get; }
    }
    /// <summary>
    /// 获取可枚举类型内容
    /// </summary>
    public static class NEnumerable
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static INEnumerable<T> GetNEnumerable<T>(this T model) where T : struct, Enum
        {
            return NEnumerable<T>.GetFromEnum(model);
        }
    }
    /// <summary>
    /// 获取可枚举类型内容
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class NEnumerable<TEnum> : INEnumerable<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<String> AllNames { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<TEnum> AllEnums { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<int> AllValues { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<string> AllEnumNames { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<NEnumerable<TEnum>> AllAttrs { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<String> Names { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<TEnum> Enums { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<int> Values { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<String> EnumNames { get; }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        public static ReadOnlyCollection<NEnumerable<TEnum>> Attrs { get; }
        /// <summary>
        /// 位置向内容
        /// </summary>
        public static NEnumerable<TEnum> Unknown { get; }
        /// <summary>
        /// 构造
        /// </summary>
        static NEnumerable()
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
            var allAttrs = new List<NEnumerable<TEnum>>();
            var getEnumCases = new List<SwitchCase>();
            var getNameCases = new List<SwitchCase>();
            var getValueCases = new List<SwitchCase>();

            for (int i = 0; i < allEnumNames.Length; i++)
            {
                var ename = allEnumNames[i];
                var eval = allValues[i];
                var enumber = allEnums[i];
                var prop = type.GetField(ename);
                var attr = prop.GetCustomAttribute<EDisplayAttribute>();
                var disp = prop.GetCustomAttribute<DisplayAttribute>();
                var dispn = prop.GetCustomAttribute<DisplayNameAttribute>();
                var descr = prop.GetCustomAttribute<DescriptionAttribute>();
                if (attr == null)
                {
                    var dispName = ename;
                    var descrName = ename;
                    Type etype = null;
                    if (disp == null)
                    {
                        if (dispn != null)
                        {
                            dispName = dispn.DisplayName;
                        }
                        if (descr != null)
                        {
                            descrName = descr.Description;
                        }
                    }
                    else
                    {
                        dispName = disp.Name;
                        descrName = disp.Description;
                        etype = disp.ResourceType;
                    }
                    attr = new EDisplayAttribute(descrName, dispName) { Type = etype, };
                }
                if (disp == null)
                {
                    disp = new DisplayAttribute()
                    {
                        Name = attr.Name,
                        Description = attr.Description,
                        Order = i,
                        ShortName = ename,
                        Prompt = $"{ename}:{attr.Name}:{attr.Description}"
                    };
                }
                if (descr == null)
                {
                    descr = new DescriptionAttribute(attr.Description);
                }
                if (dispn == null)
                {
                    dispn = new DisplayNameAttribute(attr.Name);
                }
                var model = new NEnumerable<TEnum>(enumber)
                {
                    Name = attr.Name,
                    Display = disp,
                    Value = eval,
                    EDisplay = attr,
                    DisplayName = dispn,
                    Description = descr,
                    EnumName = ename,
                };
                allAttrs.Add(model);
                allNames.Add(attr.Name);

                var resValue = Expression.Constant(model, typeof(NEnumerable<TEnum>));
                var numValue = Expression.Constant(enumber, typeof(TEnum));
                var intValue = Expression.Constant(eval, typeof(int));
                var nameHashVal = Expression.Constant(prop.Name.GetHashCode(), typeof(int));

                getEnumCases.Add(Expression.SwitchCase(resValue, numValue));
                getNameCases.Add(Expression.SwitchCase(resValue, nameHashVal));
                getValueCases.Add(Expression.SwitchCase(resValue, intValue));
            }
            AllAttrs = new ReadOnlyCollection<NEnumerable<TEnum>>(allAttrs);
            AllNames = new ReadOnlyCollection<String>(allNames);

            Names = new ReadOnlyCollection<String>(AllNames.Skip(1).ToList());
            Enums = new ReadOnlyCollection<TEnum>(AllEnums.Skip(1).ToList());
            Values = new ReadOnlyCollection<int>(AllValues.Skip(1).ToList());
            EnumNames = new ReadOnlyCollection<String>(AllEnumNames.Skip(1).ToList());
            Attrs = new ReadOnlyCollection<NEnumerable<TEnum>>(allAttrs.Skip(1).ToList());
            Unknown = AllAttrs.First();

            Expression expressBody;
            var enumVal = Expression.Parameter(typeof(TEnum), "enumVal");
            GetFromEnum = Expression.Lambda<Func<TEnum, NEnumerable<TEnum>>>(Expression.Switch(enumVal, Expression.Constant(Unknown), getEnumCases.ToArray()), enumVal).Compile();

            var enumName = Expression.Parameter(typeof(string), "enumName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(enumName, typeof(object).GetMethod("GetHashCode")));
            expressBody = Expression.Block(typeof(NEnumerable<TEnum>), new[] { nameHash }, calHash, Expression.Switch(nameHash, Expression.Constant(Unknown), getNameCases.ToArray()));
            GetFromEnumName = Expression.Lambda<Func<string, NEnumerable<TEnum>>>(expressBody, enumName).Compile();

            var enumValue = Expression.Parameter(typeof(int), "enumValue");
            GetFromInt32 = Expression.Lambda<Func<Int32, NEnumerable<TEnum>>>(Expression.Switch(enumValue, Expression.Constant(Unknown), getValueCases.ToArray()), enumValue).Compile();
        }
        #region // 实现接口
        /// <summary>
        /// 枚举值
        /// </summary>
        object INEnumerable.Enum { get => this.Enum; }
        /// <summary>
        /// 描述内容
        /// </summary>
        public DescriptionAttribute Description { get; private set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public DisplayNameAttribute DisplayName { get; private set; }
        /// <summary>
        /// 显示
        /// </summary>
        public EDisplayAttribute EDisplay { get; private set; }
        /// <summary>
        /// 显示
        /// </summary>
        public EDisplayAttribute Attribute { get => EDisplay; }
        /// <summary>
        /// 显示属性
        /// </summary>
        public DisplayAttribute Display { get; private set; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get; private set; }
        /// <summary>
        /// 枚举名
        /// </summary>
        public string EnumName { get; private set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        public TEnum Enum { get; }
        /// <summary>
        /// 字段属性信息
        /// </summary>
        public FieldInfo Field { get; }
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        protected NEnumerable(TEnum tenum)
        {
            Enum = tenum;
        }
        #region // 转换内容
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator TEnum(NEnumerable<TEnum> enumValue)
        {
            return enumValue.Enum;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator String(NEnumerable<TEnum> enumValue)
        {
            return enumValue.Name;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator int(NEnumerable<TEnum> enumValue)
        {
            return enumValue.Value;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator NEnumerable<TEnum>(TEnum enumValue)
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
        public static implicit operator NEnumerable<TEnum>(int enumValue)
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
        public static implicit operator NEnumerable<TEnum>(String enumValue)
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
        #region // 转换方法
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
        #endregion
        #region // 静态方法
        /// <summary>
        /// 获取内容
        /// </summary>
        public static Func<TEnum, NEnumerable<TEnum>> GetFromEnum { get; }
        /// <summary>
        /// 获取内容
        /// </summary>
        public static Func<String, NEnumerable<TEnum>> GetFromEnumName { get; }
        /// <summary>
        /// 获取值内容
        /// </summary>
        public static Func<Int32, NEnumerable<TEnum>> GetFromInt32 { get; }
        #endregion
    }
    /// <summary>
    /// 枚举显示泛型类
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Obsolete("替代方案:NEnumerable<TEnum>")]
    public class EDisplayAttr<TEnum> : NEnumerable<TEnum>
        where TEnum : struct, Enum
    {
        private EDisplayAttr() : base(default(TEnum)) { }
    }
}
