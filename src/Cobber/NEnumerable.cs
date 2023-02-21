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
        /// 显示名(EDisplay?.Name)
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 显示描述名(EDisplay?.Display)
        /// </summary>
        String DName { get; }
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
    public interface INEnumerable<out TEnum> : INEnumerable
        where TEnum : struct, Enum
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
        public static ReadOnlyCollection<long> AllVals { get; }
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
            var allValues = new int[values.Length];
            var allVals = new long[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                allValues[i] = (int)Convert.ChangeType(values.GetValue(i), typeof(int));
                allVals[i] = (long)Convert.ChangeType(values.GetValue(i), typeof(long));
            }
            var allEnumNames = System.Enum.GetNames(type);
            AllEnums = new ReadOnlyCollection<TEnum>(allEnums);
            AllValues = new ReadOnlyCollection<int>(allValues);
            AllVals = new ReadOnlyCollection<long>(allVals);
            AllEnumNames = new ReadOnlyCollection<string>(allEnumNames);
            var allNames = new List<String>();
            var allAttrs = new List<NEnumerable<TEnum>>();
            var getEnumCases = new List<SwitchCase>();
            var getEnumNameCases = new Dictionary<string, SwitchCase>();
            var getNameCases = new Dictionary<string, SwitchCase>();
            var getValueCases = new Dictionary<int, SwitchCase>();
            var getValCases = new Dictionary<long, SwitchCase>();
            var getNameCases2 = new Dictionary<string, SwitchCase>();
            var getNameCases3 = new Dictionary<string, SwitchCase>();
            for (int i = 0; i < allEnumNames.Length; i++)
            {
                var ename = allEnumNames[i];
                var eval = allValues[i];
                var eva = allVals[i];
                var enumber = allEnums[i];
                var prop = type.GetField(ename);
                var attr = prop.GetCustomAttribute<EDisplayAttribute>();
                var disp = prop.GetCustomAttribute<DisplayAttribute>();
                var dispn = prop.GetCustomAttribute<DisplayNameAttribute>();
                var descr = prop.GetCustomAttribute<DescriptionAttribute>();
                attr ??= (disp == null ?
                    new EDisplayAttribute(descr == null ? ename : descr.Description, dispn == null ? ename : disp.Name) :
                    new EDisplayAttribute(disp.Description, disp.Name) { Type = disp.ResourceType, });
                disp ??= new DisplayAttribute()
                {
                    Name = attr.Name,
                    Description = attr.Description,
                    Order = i,
                    ShortName = ename,
                    Prompt = $"{ename}:{attr.Name}:{attr.Description}"
                };
                descr ??= new DescriptionAttribute(attr.Description);
                dispn ??= new DisplayNameAttribute(attr.Name);
                var model = new NEnumerable<TEnum>(enumber)
                {
                    Display = disp,
                    Value = eval,
                    Val = eva,
                    EDisplay = attr,
                    DisplayName = dispn,
                    Description = descr,
                    EnumName = ename,
                    Field = prop,
                };
                allAttrs.Add(model);
                allNames.Add(attr.Name);

                var resValue = Expression.Constant(model, typeof(NEnumerable<TEnum>));

                getEnumCases.Add(Expression.SwitchCase(resValue, Expression.Constant(enumber, typeof(TEnum))));
                getEnumNameCases.Add(prop.Name, Expression.SwitchCase(resValue, Expression.Constant(prop.Name, typeof(string))));
                getNameCases.Add(prop.Name, Expression.SwitchCase(resValue, Expression.Constant(prop.Name, typeof(string))));
                if (!getValueCases.ContainsKey(eval))
                {
                    getValueCases.Add(eval, Expression.SwitchCase(resValue, Expression.Constant(eval, typeof(int))));
                    getNameCases.Add(eval.ToString(), Expression.SwitchCase(resValue, Expression.Constant(eval.ToString(), typeof(string))));
                }
                if (!getValCases.ContainsKey(eva))
                {
                    getValCases.Add(eva, Expression.SwitchCase(resValue, Expression.Constant(eva, typeof(long))));
                    if (eva != eval)
                    {
                        getNameCases.Add(eva.ToString(), Expression.SwitchCase(resValue, Expression.Constant(eva.ToString(), typeof(string))));
                    }
                }
                if (!getNameCases2.ContainsKey(prop.Name.ToLower()))
                {
                    getNameCases2.Add(prop.Name.ToLower(), Expression.SwitchCase(resValue, Expression.Constant(prop.Name.ToLower(), typeof(string))));
                }
                if (!getNameCases2.ContainsKey(prop.Name.ToUpper()))
                {
                    getNameCases2.Add(prop.Name.ToUpper(), Expression.SwitchCase(resValue, Expression.Constant(prop.Name.ToUpper(), typeof(string))));
                }
                if (!getNameCases3.ContainsKey(model.Name))
                {
                    getNameCases3.Add(model.Name, Expression.SwitchCase(resValue, Expression.Constant(model.Name, typeof(string))));
                }
            }
            foreach (var item in getNameCases2)
            {
                if (!getNameCases.ContainsKey(item.Key)) { getNameCases.Add(item.Key, item.Value); }
            }
            foreach (var item in getNameCases3)
            {
                if (!getNameCases.ContainsKey(item.Key)) { getNameCases.Add(item.Key, item.Value); }
            }
            AllAttrs = new ReadOnlyCollection<NEnumerable<TEnum>>(allAttrs);
            AllNames = new ReadOnlyCollection<String>(allNames);

            Names = new ReadOnlyCollection<String>(AllNames.Skip(1).ToList());
            Enums = new ReadOnlyCollection<TEnum>(AllEnums.Skip(1).ToList());
            Values = new ReadOnlyCollection<int>(AllValues.Skip(1).ToList());
            EnumNames = new ReadOnlyCollection<String>(AllEnumNames.Skip(1).ToList());
            Attrs = new ReadOnlyCollection<NEnumerable<TEnum>>(allAttrs.Skip(1).ToList());
            Unknown = AllAttrs.First();

            var enumVal = Expression.Parameter(typeof(TEnum), "enumVal");
            GetFromEnum = Expression.Lambda<Func<TEnum, NEnumerable<TEnum>>>(Expression.Switch(enumVal, Expression.Constant(Unknown), getEnumCases.ToArray()), enumVal).Compile();

            var enumName = Expression.Parameter(typeof(string), "enumName");
            GetFromEnumName = Expression.Lambda<Func<String, NEnumerable<TEnum>>>(Expression.Switch(enumName, Expression.Constant(Unknown), getEnumNameCases.Values.ToArray()), enumName).Compile();

            var enumValue = Expression.Parameter(typeof(int), "enumValue");
            GetFromInt32 = Expression.Lambda<Func<Int32, NEnumerable<TEnum>>>(Expression.Switch(enumValue, Expression.Constant(Unknown), getValueCases.Values.ToArray()), enumValue).Compile();

            var enumVal64 = Expression.Parameter(typeof(long), "enumValue");
            GetFromInt64 = Expression.Lambda<Func<Int64, NEnumerable<TEnum>>>(Expression.Switch(enumVal64, Expression.Constant(Unknown), getValCases.Values.ToArray()), enumVal64).Compile();

            var nameValue = Expression.Parameter(typeof(string), "nameValue");
            GetFromName = Expression.Lambda<Func<string, NEnumerable<TEnum>>>(Expression.Switch(nameValue, Expression.Constant(Unknown), getNameCases.Values.ToArray()), nameValue).Compile();
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
        /// 值
        /// </summary>
        public long Val { get; private set; }
        /// <summary>
        /// 枚举名
        /// </summary>
        public string EnumName { get; private set; }
        /// <summary>
        /// 显示名(EDisplay?.Name)
        /// </summary>
        public string Name { get => EDisplay?.Name; }
        /// <summary>
        /// 显示描述名(EDisplay?.Display)
        /// </summary>
        public String DName { get => EDisplay?.Display; }
        /// <summary>
        /// 枚举值
        /// </summary>
        public TEnum Enum { get; }
        /// <summary>
        /// 字段属性信息
        /// </summary>
        public FieldInfo Field { get; private set; }
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
        public static implicit operator long(NEnumerable<TEnum> enumValue)
        {
            return enumValue.Val;
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
            return GetFromInt32(enumValue);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator NEnumerable<TEnum>(long enumValue)
        {
            return GetFromInt64(enumValue);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="enumValue"></param>
        public static implicit operator NEnumerable<TEnum>(String enumValue)
        {
            return GetFromName(enumValue);
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
        /// <summary>
        /// 获取值内容
        /// </summary>
        public static Func<Int64, NEnumerable<TEnum>> GetFromInt64 { get; }
        /// <summary>
        /// 获取内容
        /// </summary>
        public static Func<String, NEnumerable<TEnum>> GetFromName { get; }
        #endregion
    }
}
