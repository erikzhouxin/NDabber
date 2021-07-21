using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 数字调用
    /// </summary>
    public static class NumberCaller
    {
        #region // 读取字节
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// 读取无符号整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// 读取长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// 读取无符号长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// 读取短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this UInt16 number)
        {
            return new byte[2]
            {
                (byte)(number>>8),
                (byte)number,
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
    }
}
