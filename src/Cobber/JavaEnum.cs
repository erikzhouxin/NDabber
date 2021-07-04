using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Dabber;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// Java枚举接口
    /// </summary>
    public interface IJavaEnum
    {
        /// <summary>
        /// 值
        /// </summary>
        Int32 Value { get; }
        /// <summary>
        /// 枚举名
        /// </summary>
        String EnumName { get; }
        /// <summary>
        /// 名
        /// </summary>
        String Name { get; }
    }
    /// <summary>
    /// 抽象Java枚举
    /// </summary>
    public struct JavaEnum : IJavaEnum
    {
        /// <summary>
        /// 枚举值
        /// </summary>
        public int Value { get; }
        /// <summary>
        /// 枚举名
        /// </summary>
        public string EnumName { get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 构造
        /// </summary>
        private JavaEnum(int value, string name, string enumName)
        {
            Value = value;
            Name = name;
            EnumName = enumName;
        }
        /// <summary>
        /// 获取枚举字典
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDictionary<int, KeyValuePair<string, string>> GetEnumDictionary(Type type)
        {
            var result = new SortedDictionary<int, KeyValuePair<string, string>>();
            if (!type.IsEnum) { return result; }
            var vals = Enum.GetValues(type).ToArray();
            var names = Enum.GetNames(type);
            for (int i = 0; i < vals.Length; i++)
            {
                var val = vals[i];
                var name = names[i];
                var display = name;
                var attrs = type.GetField(name).GetCustomAttributes(false);
                if (attrs.IsEmpty()) { continue; }
                foreach (var attr in attrs)
                {
                    if (attr is EDisplayAttribute disp)
                    {
                        display = disp.Display;
                        break;
                    }
                    if (attr is DisplayAttribute displ)
                    {
                        display = displ.Name ?? displ.Description;
                        break;
                    }
                    if (attr is DescriptionAttribute description)
                    {
                        display = description.Description;
                        break;
                    }
                }
                result.Add(val, new KeyValuePair<string, string>(name, display));
            }
            return result;
        }
        /// <summary>
        /// 获取枚举类显示名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClassDisplay(Type type)
        {
            var attrs = type.GetCustomAttributes(false);
            string display = type.Name;
            foreach (var item in attrs)
            {
                if (item is EDisplayAttribute disp)
                {
                    display = disp.Display;
                    break;
                }
                if (item is DescriptionAttribute description)
                {
                    display = description.Description;
                    break;
                }
            }
            return display;
        }
        /// <summary>
        /// 是逻辑枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFlags(Type type)
        {
            if (!type.IsEnum) { return false; }
            var attrs = type.GetCustomAttributes(inherit: false);
            foreach (var item in attrs)
            {
                if (item is FlagsAttribute)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取Java枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<JavaEnum> GetJavaEnum(Type type)
        {
            var result = new List<JavaEnum>();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in props)
            {
                try
                {
                    if (prop.GetValue(null, null) is JavaEnum propModel)
                    {
                        result.Add(propModel);
                    }
                }
                catch { continue; }
            }
            return result;
        }
    }
}
