using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 数字调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 四舍五入
        /// </summary>
        public static MidpointRounding DefaultMidPoint { get; set; } = MidpointRounding.ToEven;
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
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double? GetDouble(this double? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double? GetDouble(this decimal? value, int digit = 2)
        {
            if (value == null) { return null; }
            return (double)Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimal(this decimal? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimal(this double? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round((decimal)value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double? GetDouble(this float? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimal(this float? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round((decimal)value.Value, digit, DefaultMidPoint);
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
    }
}

namespace System.Data.Extter
{
    /// <summary>
    /// 数字调用
    /// </summary>
    public static partial class ExtterCaller
    {
        #region // 读取字节
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this Int16 number, bool isBigEndian = true)
        {
            byte[] array = new byte[2];
            fixed (byte* ptr = array)
            {
                *(short*)ptr = number;
            }
            return isBigEndian ? new byte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this Int16 number)
        {
            return new byte[2]
            {
                (byte)(number>>8),
                (byte)number,
            };
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this UInt16 number, bool isBigEndian = true)
        {
            byte[] array = new byte[2];
            fixed (byte* ptr = array)
            {
                *(ushort*)ptr = number;
            }
            return isBigEndian ? new byte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this UInt16 number)
        {
            return new byte[2]
            {
                (byte)(number>>8),
                (byte)number,
            };
        }
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this Int16 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[2];
            fixed (sbyte* ptr = array)
            {
                *(short*)ptr = number;
            }
            return isBigEndian ? new sbyte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this Int16 number)
        {
            return new sbyte[2]
            {
                (sbyte)(number>>8),
                (sbyte)number,
            };
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this UInt16 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[2];
            fixed (sbyte* ptr = array)
            {
                *(ushort*)ptr = number;
            }
            return isBigEndian ? new sbyte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this UInt16 number)
        {
            return new sbyte[2]
            {
                (sbyte)(number>>8),
                (sbyte)number,
            };
        }
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this Int32 number, bool isBigEndian = true)
        {
            byte[] array = new byte[4];
            fixed (byte* ptr = array)
            {
                *(int*)ptr = number;
            }
            return isBigEndian ? new byte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取整型字节(高位开始)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this Int32 number)
        {
            return new Byte[4]
            {
                (byte)(number>>24),
                (byte)(number>>16),
                (byte)(number>>8),
                (byte)(number>>0),
            };
        }
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this Int32 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[4];
            fixed (sbyte* ptr = array)
            {
                *(int*)ptr = number;
            }
            return isBigEndian ? new sbyte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this Int32 number)
        {
            return new SByte[4]
            {
                (sbyte)(number>>24),
                (sbyte)(number>>16),
                (sbyte)(number>>8),
                (sbyte)(number>>0),
            };
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this UInt32 number, bool isBigEndian = true)
        {
            byte[] array = new byte[4];
            fixed (byte* ptr = array)
            {
                *(uint*)ptr = number;
            }
            return isBigEndian ? new byte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this UInt32 number)
        {
            return new Byte[4]
            {
                (byte)(number>>24),
                (byte)(number>>16),
                (byte)(number>>8),
                (byte)(number>>0),
            };
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this UInt32 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[4];
            fixed (sbyte* ptr = array)
            {
                *(uint*)ptr = number;
            }
            return isBigEndian ? new sbyte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this UInt32 number)
        {
            return new SByte[4]
            {
                (sbyte)(number>>24),
                (sbyte)(number>>16),
                (sbyte)(number>>8),
                (sbyte)(number>>0),
            };
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this Int64 number, bool isBigEndian = true)
        {
            byte[] array = new byte[8];
            fixed (byte* ptr = array)
            {
                *(long*)ptr = number;
            }
            return isBigEndian ? new byte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this Int64 number)
        {
            var num1 = number >> 32;
            var num2 = (int)number;
            return new byte[8]
            {
                (byte)(num1>>24),
                (byte)(num1>>16),
                (byte)(num1>>8),
                (byte)num1,
                (byte)(num2>>24),
                (byte)(num2>>16),
                (byte)(num2>>8),
                (byte)num2,
            };
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this Int64 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[8];
            fixed (sbyte* ptr = array)
            {
                *(long*)ptr = number;
            }
            return isBigEndian ? new sbyte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this Int64 number)
        {
            var num1 = number >> 32;
            var num2 = (int)number;
            return new sbyte[8]
            {
                (sbyte)(num1>>24),
                (sbyte)(num1>>16),
                (sbyte)(num1>>8),
                (sbyte)num1,
                (sbyte)(num2>>24),
                (sbyte)(num2>>16),
                (sbyte)(num2>>8),
                (sbyte)num2,
            };
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this UInt64 number, bool isBigEndian = true)
        {
            byte[] array = new byte[8];
            fixed (byte* ptr = array)
            {
                *(ulong*)ptr = number;
            }
            return isBigEndian ? new byte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this UInt64 number)
        {
            UInt32 num1 = (UInt32)(number >> 32);
            UInt32 num2 = (UInt32)number;
            return new byte[8]
            {
                (byte)(num1>>24),
                (byte)(num1>>16),
                (byte)(num1>>8),
                (byte)num1,
                (byte)(num2>>24),
                (byte)(num2>>16),
                (byte)(num2>>8),
                (byte)num2,
            };
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this UInt64 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[8];
            fixed (sbyte* ptr = array)
            {
                *(ulong*)ptr = number;
            }
            return isBigEndian ? new sbyte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this UInt64 number)
        {
            UInt32 num1 = (UInt32)(number >> 32);
            UInt32 num2 = (UInt32)number;
            return new sbyte[8]
            {
                (sbyte)(num1>>24),
                (sbyte)(num1>>16),
                (sbyte)(num1>>8),
                (sbyte)num1,
                (sbyte)(num2>>24),
                (sbyte)(num2>>16),
                (sbyte)(num2>>8),
                (sbyte)num2,
            };
        }
        #endregion 读取字节
        #region // 转换类型
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> CastAs<TIn, TOut>(this IEnumerable<TIn> list)
            where TIn : class
            where TOut : class
        {
            foreach (var item in list)
            {
                yield return item as TOut;
            }
        }
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="list"></param>
        /// <param name="GetValue"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> CastAs<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> GetValue)
        {
            foreach (var item in list)
            {
                yield return GetValue(item);
            }
        }
        /// <summary>
        /// 强制转换(单字节转换)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Char> CastTo(this IEnumerable<Byte> list)
        {
            foreach (var item in list)
            {
                yield return (char)item;
            }
        }
        /// <summary>
        /// 强制转换(双字节转换)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Char> CastTo2(this IEnumerable<Byte> list)
        {
            var array = list.ToArray();
            if (array.Length % 2 == 1)
            {
                var arrList = list.ToList();
                arrList.Insert(0, (byte)0);
                array = arrList.ToArray();
            }
            for (int i = 1; i < array.Length; i++)
            {
                if (i % 2 == 0)
                {
                    yield return (char)(array[i - 1] * 256 + array[i]);
                }
            }
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> CastTo(this IEnumerable<Char> list)
        {
            foreach (var item in list)
            {
                yield return (byte)item;
            }
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> CastTo2(this IEnumerable<Char> list)
        {
            foreach (var item in list)
            {
                yield return (byte)(item >> 8);
                yield return (byte)item;
            }
        }
        #endregion 转换类型
        #region // 16进制
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this Int32 val)
        {
            return val.ToString("X8");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this UInt32 val)
        {
            return val.ToString("X8");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this Int16 val)
        {
            return val.ToString("X4");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this UInt16 val)
        {
            return val.ToString("X4");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this Int64 val)
        {
            return val.ToString("X16");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this UInt64 val)
        {
            return val.ToString("X16");
        }
        #endregion // 16进制
        /// <summary>
        /// 半进位1.2=>1.5或1.8=>2.0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static double GetHalfBitDouble(this double value, int digit)
        {
            return Cobber.CobberCaller.GetDouble((Math.Ceiling(value * Math.Pow(10, digit - 1) * 2) / 2.0 * Math.Pow(10, -digit + 1)), digit);
        }
        /// <summary>
        /// 判断版本范围,不合适直接抛异常
        /// </summary>
        /// <param name="version">在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GetThrowVersionRange(this long version) => GetVersionRange(version);
        /// <summary>
        /// 判断版本范围,不合适根据配置抛异常
        /// </summary>
        /// <param name="version">在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间</param>
        /// <param name="isFixed">是否修正成当天版本【yyyy-MM-dd 00:00:00】</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GetVersionRange(this long version, bool isFixed = false)
        {
            if (version >= 19491001000000L && version <= 99991231235959L) { return version; }
            return isFixed ? Int64.Parse(DateTime.Now.ToString("yyyyMMdd") + "000000")
            : throw new ArgumentOutOfRangeException("迁移版本号【Version】必须在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间");
        }
    }
}
