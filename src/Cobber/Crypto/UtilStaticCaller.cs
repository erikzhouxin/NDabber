using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 内部静态调用类
    /// </summary>
    public static class UtilStaticCaller
    {
        /// <summary>
        /// 转换成字符串唯一值
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static string GetString(this Guid guid, string fmt = "N") => guid.ToString(fmt);
        #region // 日期时间型
        /// <summary>
        /// 转换成秒时间字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToSecondString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 转换成日期字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToDateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 获取当前天是星期几
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String GetDayOfWeek(this DateTime time)
        {
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Sunday: return "星期日";
                case DayOfWeek.Monday: return "星期一";
                case DayOfWeek.Tuesday: return "星期二";
                case DayOfWeek.Wednesday: return "星期三";
                case DayOfWeek.Thursday: return "星期四";
                case DayOfWeek.Friday: return "星期五";
                case DayOfWeek.Saturday: return "星期六";
                default: return "周鑫";
            }
        }
        #endregion
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> Append<T>(this List<T> model, IEnumerable<T> list) => AppendRange(model, list);
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> AppendRange<T>(this List<T> model, IEnumerable<T> list)
        {
            model.AddRange(list);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> Append<T>(this List<T> model, T item)
        {
            model.Add(item);
            return model;
        }
        #region // 序列化与反序列化
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 获取格式化的Json代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetJsonFormatString<T>(this T value)
        {
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            //格式化json字符串
            new JsonSerializer().Serialize(jsonWriter, value);
            return textWriter.ToString();
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic GetJsonObject(this string json)
        {
            return JsonConvert.DeserializeObject(json, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetJsonObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, DefaultNewtonsoftSetting);
        }
        /// <summary>
        /// 默认设置
        /// </summary>
        public static JsonSerializerSettings DefaultNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        /// <summary>
        /// 默认设置
        /// </summary>
        public static JsonSerializerSettings CurrentNewtonsoftSetting { get; set; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 帕斯卡(大驼峰)命名
        /// </summary>
        public static JsonSerializerSettings PascalCaseNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
        };
        /// <summary>
        /// 小驼峰命名
        /// </summary>
        public static JsonSerializerSettings CamelCaseNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
        };
        /// <summary>
        /// 小写属性命名
        /// </summary>
        public static JsonSerializerSettings LowerNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new LowercaseContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
        };
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetWebJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, CurrentNewtonsoftSetting);
        }
        /// <summary>
        /// 获取对象的Json字符串(小写属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLowerJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, LowerNewtonsoftSetting);
        }
        #endregion
        /// <summary>
        /// 转换成标题的大小写
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetTitleCase(this string val)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(val);
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> list)
        {
            return new ObservableCollection<T>(list);
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this Array array)
        {
            foreach (T item in array)
            {
                yield return item;
            }
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T GetOrDefault<T>(this IEnumerable<T> list, Func<T, bool> condition, T defVal = default(T))
        {
            foreach (var item in list)
            {
                if (condition(item))
                {
                    return item;
                }
            }
            return defVal;
        }
        /// <summary>
        /// 转换成整型数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int[] ToArray(this Array array)
        {
            if (array == null || array.Length == 0) { return new int[] { }; }
            var result = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (int)array.GetValue(i);
            }
            return result;
        }
        /// <summary>
        /// 转换成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this Array array)
        {
            if (array == null || array.Length == 0) { return new T[] { }; }
            var result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (T)array.GetValue(i);
            }
            return result;
        }
        /// <summary>
        /// 转换成列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable list)
        {
            foreach (T item in list)
            {
                yield return item;
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in list)
            {
                action(item, index++);
            }
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> model, IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return model; }
            foreach (var item in list)
            {
                model.Add(item);
            }
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> Append<T>(this ObservableCollection<T> model, IEnumerable<T> list) => AppendRange(model, list);
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> AppendRange<T>(this ObservableCollection<T> model, IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return model; }
            foreach (var item in list)
            {
                model.Add(item);
            }
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> Append<T>(this ObservableCollection<T> model, T item)
        {
            model.Add(item);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        public static ObservableCollection<TResult> ToObservable<TModel, TResult>(this IEnumerable<TModel> list, Func<TModel, TResult> func)
        {
            return new ObservableCollection<TResult>(list.Select(func));
        }
        /// <summary>
        /// 获取空的默认值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static String GetEmptyDefault(this string value, string defVal)
        {
            return string.IsNullOrWhiteSpace(value) ? defVal : value;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static String JoinString<T>(this IEnumerable<T> list, string split = ",")
        {
            return string.Join(split, list);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String JoinLine<T>(this IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder(string.Format("{0}", list.First()));
            foreach (var item in list.Skip(1))
            {
                sb.AppendLine().AppendFormat("{0}", item);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string content) => GetBytes(content, Encoding.UTF8);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this String content, Encoding encoding)
        {
            return encoding.GetBytes(content);
        }
        #region // Null Or Empty
        /// <summary>
        /// 将字符串转换成布尔值
        /// </summary>
        /// <param name="value">待转换字符串,是否,对错,真假等</param>
        /// <param name="defVal">出现其他值后返回值,默认为False</param>
        /// <returns></returns>
        public static bool GetBoolean(this string value, bool defVal = false)
        {
            if (value == null) { return defVal; }
            switch (value.ToUpper())
            {
                case "Y":
                case "T":
                case "TRUE":
                case "YES":
                case "是":
                case "对":
                case "真":
                case "合格":
                case "同意":
                case "行":
                case "可以":
                    return true;
                case "N":
                case "F":
                case "FALSE":
                case "NO":
                case "否":
                case "错":
                case "假":
                case "不":
                case "不合格":
                case "不同意":
                case "不行":
                case "不可以":
                    return false;
                default:
                    return defVal;
            }
        }
        /// <summary>
        /// 可为空的是True
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(this bool? value)
        {
            return value.HasValue && value.Value;
        }
        /// <summary>
        /// 可为空的是False
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalse(this bool? value)
        {
            return value.HasValue && !value.Value;
        }
        /// <summary>
        /// 是为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T model)
        {
            return model == null;
        }
        /// <summary>
        /// 不是为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(this T model)
        {
            return model != null;
        }
        /// <summary>
        /// 为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        /// <summary>
        /// 为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value == null || value.Count() == 0;
#else
            return value == null || !value.Any();
#endif
        }
        /// <summary>
        /// 不为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        /// <summary>
        /// 为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value != null && value.Count() > 0;
#else
            return value != null && value.Any();
#endif
        }
        /// <summary>
        /// 为NULL或空白字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 不为NULL或空白字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 为NULL/空/空白
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 不为NULL/空/空白
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 为空串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value == null || value.Count() == 0;
#else
            return value == null || !value.Any();
#endif
        }
        /// <summary>
        /// 不为空串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value != null && value.Count() > 0;
#else
            return value != null && value.Any();
#endif
        }
        /// <summary>
        /// 为空数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this T[] value)
        {
            return value == null || value.Length == 0;
        }
        /// <summary>
        /// 不为空数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this T[] value)
        {
            return value != null && value.Length > 0;
        }
        /// <summary>
        /// 是否为默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T model)
        {
            return model == null ? default(T) == null : model.Equals(default(T));
        }
        /// <summary>
        /// 是否为默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsNotDefault<T>(this T model)
        {
            return model == null ? default(T) != null : !model.Equals(default(T));
        }
        #endregion
        #region // 数字
        /// <summary>
        /// 四舍五入
        /// </summary>
        public static MidpointRounding DefaultMidPoint { get; set; } = MidpointRounding.AwayFromZero;
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double GetDouble(this double value, int digit = 2)
        {
            return Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double GetDouble(this decimal value, int digit = 2)
        {
            return (double)Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal(this decimal value, int digit = 2)
        {
            return Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal(this double value, int digit = 2)
        {
            return Math.Round((decimal)value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double GetDouble(this float value, int digit = 2)
        {
            return Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal(this float value, int digit = 2)
        {
            return Math.Round((decimal)value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 逻辑且一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int AndValue(this int firstVal, int second)
        {
            return firstVal & second;
        }
        /// <summary>
        /// 逻辑且一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsAndValue(this int firstVal, int second)
        {
            return (firstVal & second) > 0;
        }
        /// <summary>
        /// 逻辑或一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsOrValue(this int firstVal, int second)
        {
            return (firstVal | second) > 0;
        }
        /// <summary>
        /// 逻辑或一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int OrValue(this int firstVal, int second)
        {
            return firstVal | second;
        }
        /// <summary>
        /// 尝试转换成整型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToInt32(this string key, out int value)
        {
            return int.TryParse(key, out value);
        }
        /// <summary>
        /// 尝试转换成浮点型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToDouble(this string key, out double value)
        {
            return double.TryParse(key, out value);
        }
        /// <summary>
        /// 尝试转换成浮点型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToDecimal(this string key, out decimal value)
        {
            return decimal.TryParse(key, out value);
        }
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <returns></returns>
        public static int ToCInt32(this string value, int defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToInt32(value);
        }
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <returns></returns>
        public static int ToPInt32(this string value, int defVal = 0)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static double ToCDouble(this string value, double defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToDouble(value);
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static double ToPDouble(this string value, double defVal = 0)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static double? ToNullableDouble(this string value, double? defVal = null)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static decimal ToCDecimal(this string value, decimal defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToDecimal(value);
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static decimal? ToNullableDecimal(this string value, decimal? defVal = null)
        {
            if (string.IsNullOrEmpty(value)) { return defVal; }
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static decimal ToPDecimal(this string value, decimal defVal = 0)
        {
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成布尔值
        /// </summary>
        /// <returns></returns>
        public static bool ToCBoolean(this string value, bool defVal = false)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToBoolean(value);
        }
        /// <summary>
        /// 转换成布尔值
        /// </summary>
        /// <returns></returns>
        public static bool ToPBoolean(this string value, bool defVal = false)
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            return defVal;
        }
        #endregion
        #region // 枚举

        /// <summary>
        /// 转换成枚举名称字符串
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this T enumValue) where T : Enum
        {
            return Enum.GetName(typeof(T), enumValue);
        }
        #endregion
        #region // 懒加载
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static Lazy<T> GetLazy<T>(this T model, bool isThreadSafe = true)
        {
            return new Lazy<T>(() => model, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static Lazy<T> GetLazy<T>(this T model, Func<T> GetValue, bool isThreadSafe = true)
        {
            return model == null ? new Lazy<T>(GetValue, isThreadSafe) : new Lazy<T>(() => model, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static Lazy<T> GetLazy<T>(this Func<T> GetValue, bool isThreadSafe = true)
        {
            return new Lazy<T>(GetValue, isThreadSafe);
        }
        #endregion
    }
}
