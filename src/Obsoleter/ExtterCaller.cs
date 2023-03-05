using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
using System.Data.Impeller;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Extter
{
    /// <summary>
    /// 程序集类型调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class AssemblyTypeCaller
    {
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static Type[] GetTypes(Type type, bool isExport = true) => type.GetTypes();
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nSpace"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(Type type, string nSpace, bool isExport = true) => type.GetTypes(nSpace, isExport);
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetNamespaceTypes(Type type, bool isExport = true) => type.GetNamespaceTypes(isExport);
        /// <summary>
        /// 获取类型所在程序集的名称类型
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="name">全称时使用[Type.Assembly.GetType],非全称使用遍历</param>
        /// <returns></returns>
        public static Type GetSameAssemblyType(Type type, string name) => type.GetSameAssemblyType(name);
    }
    /// <summary>
    /// 布尔调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class BooleanCaller
    {
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetTrue"></param>
        /// <param name="GetFalse"></param>
        /// <returns></returns>
        public static T If<T>(T model, Func<T, bool> FuncIf, Func<T, T> GetTrue, Func<T, T> GetFalse) => model.If(FuncIf, GetTrue, GetFalse);
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="trueValue"></param>
        /// <param name="GetFalse"></param>
        /// <returns></returns>
        public static T If<T>(T model, Func<T, bool> FuncIf, T trueValue, Func<T, T> GetFalse) => model.If(FuncIf, trueValue, GetFalse);
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetTrue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static T If<T>(T model, Func<T, bool> FuncIf, Func<T, T> GetTrue, T falseValue) => model.If(FuncIf, GetTrue, falseValue);
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static T IfTrue<T>(T model, Func<T, bool> FuncIf, T falseValue) => model.IfTrue(FuncIf, falseValue);
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetFalse"></param>
        /// <returns></returns>
        public static T IfTrue<T>(T model, Func<T, bool> FuncIf, Func<T, T> GetFalse) => model.IfTrue(FuncIf, GetFalse);
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="trueValue"></param>
        /// <returns></returns>
        public static T IfFalse<T>(T model, Func<T, bool> FuncIf, T trueValue) => model.IfFalse(FuncIf, trueValue);
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetTrue"></param>
        /// <returns></returns>
        public static T IfFalse<T>(T model, Func<T, bool> FuncIf, Func<T, T> GetTrue) => model.IfFalse(FuncIf, GetTrue);
    }
    /// <summary>
    /// 字节调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class ByteCaller
    {
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GetMd5(byte[] bytes)
        {
            return MD5.Create().ComputeHash(bytes);
        }
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetMd5String(byte[] bytes)
        {
            return GetHexString(GetMd5(bytes));
        }
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetHexString(byte[] hashData) => GetHexString(hashData, false);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <param name="isLower">是小写</param>
        /// <returns>16进制字符串</returns>
        public static string GetHexString(byte[] hashData, bool isLower)
        {
            StringBuilder sBuilder = new StringBuilder();
            var fmt = isLower ? "x2" : "X2";
            foreach (var hash in hashData)
            {
                sBuilder.Append(hash.ToString(fmt));
            }
            return sBuilder.ToString();
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetHexByte(string hexString)
        {
            if ((hexString.Length % 2) != 0) { hexString += " "; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetHexBytes(string hexString)
        {
            if ((hexString.Length % 2) != 0) { hexString += " "; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="fix"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static byte[] GetHexBytes(string hexString, char fix, char replace = ' ')
        {
            hexString.Replace(replace.ToString(), "");
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetHexBytes(string hexString, char fix = ' ', string replace = " ")
        {
            hexString.Replace(replace, "");
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetHexBytes(string hexString, char fix, params char[] replaces)
        {
            foreach (var item in replaces)
            {
                hexString.Replace(item.ToString(), "");
            }
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        #region // 转数字
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int16 GetInt16(byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(short*)ptr;
            }
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt16))]
        public static Int16 ReadInt16(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            return (Int16)((bytes[0] << 8) + bytes[1]);
        }
        /// <summary>
        /// 读无符号短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt16 GetUInt16(byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(ushort*)ptr;
            }
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt16))]
        public static UInt16 ReadUInt16(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            return (UInt16)((bytes[0] << 8) + bytes[1]);
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int32 GetInt32(byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(int*)ptr;
            }
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt32))]
        public static Int32 ReadInt32(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            if (bytes.Length == 2) { return (bytes[0] << 8) + bytes[1]; }
            if (bytes.Length == 3) { return (bytes[0] << 16) + (bytes[1] << 8) + bytes[2]; }
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt32 GetUInt32(byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(uint*)ptr;
            }
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt32))]
        public static UInt32 ReadUInt32(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            if (bytes.Length == 2) { return ((uint)bytes[0] << 8) + bytes[1]; }
            if (bytes.Length == 3) { return ((uint)bytes[0] << 16) + ((uint)bytes[1] << 8) + bytes[2]; }
            return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + bytes[3];
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int64 GetInt64(byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(long*)ptr;
            }
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt64))]
        public static Int64 ReadInt64(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return bytes[0]; }
                if (bytes.Length == 2) { return ((uint)bytes[0] << 8) + bytes[1]; }
                if (bytes.Length == 3) { return ((uint)bytes[0] << 16) + ((uint)bytes[1] << 8) + bytes[2]; }
                return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + bytes[3];
            }
            if (bytes.Length == 5) { return ((long)bytes[0] << 32) + ((uint)bytes[1] << 24) + (bytes[2] << 16) + (bytes[3] << 8) + bytes[4]; }
            if (bytes.Length == 6) { return ((long)bytes[0] << 40) + ((long)bytes[1] << 32) + ((uint)bytes[2] << 24) + (bytes[3] << 16) + (bytes[4] << 8) + bytes[5]; }
            if (bytes.Length == 7) { return ((long)bytes[0] << 48) + ((long)bytes[1] << 40) + ((long)bytes[2] << 32) + ((uint)bytes[3] << 24) + (bytes[4] << 16) + (bytes[5] << 8) + bytes[6]; }
            return ((long)bytes[0] << 56) + ((long)bytes[1] << 48) + ((long)bytes[2] << 40) + ((long)bytes[3] << 32) + ((uint)bytes[4] << 24) + (bytes[5] << 16) + (bytes[6] << 8) + bytes[7];
        }
        /// <summary>
        /// 读无符号长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt64 GetUInt64(byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(ulong*)ptr;
            }
        }
        /// <summary>
        /// 读无符号长整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt64))]
        public static UInt64 ReadUInt64(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return bytes[0]; }
                if (bytes.Length == 2) { return ((uint)bytes[0] << 8) + bytes[1]; }
                if (bytes.Length == 3) { return ((uint)bytes[0] << 16) + ((uint)bytes[1] << 8) + bytes[2]; }
                return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + bytes[3];
            }
            if (bytes.Length == 5) { return ((ulong)bytes[0] << 32) + ((uint)bytes[1] << 24) + ((uint)bytes[2] << 16) + ((uint)bytes[3] << 8) + bytes[4]; }
            if (bytes.Length == 6) { return ((ulong)bytes[0] << 40) + ((ulong)bytes[1] << 32) + ((uint)bytes[2] << 24) + ((uint)bytes[3] << 16) + ((uint)bytes[4] << 8) + bytes[5]; }
            if (bytes.Length == 7) { return ((ulong)bytes[0] << 48) + ((ulong)bytes[1] << 40) + ((ulong)bytes[2] << 32) + ((uint)bytes[3] << 24) + ((uint)bytes[4] << 16) + ((uint)bytes[5] << 8) + bytes[6]; }
            return ((ulong)bytes[0] << 56) + ((ulong)bytes[1] << 48) + ((ulong)bytes[2] << 40) + ((ulong)bytes[3] << 32) + ((uint)bytes[4] << 24) + ((uint)bytes[5] << 16) + ((uint)bytes[6] << 8) + bytes[7];
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int16 GetInt16(sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(short*)ptr;
            }
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt16))]
        public static Int16 ReadInt16(sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (Int16)((byte)bytes[0]); }
            return (Int16)((((byte)bytes[0]) << 8) + ((byte)bytes[1]));
        }
        /// <summary>
        /// 读无符号短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt16 GetUInt16(sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(ushort*)ptr;
            }
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt16))]
        public static UInt16 ReadUInt16(sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (UInt16)((byte)bytes[0]); }
            return (UInt16)((((byte)bytes[0]) << 8) + ((byte)bytes[1]));
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int32 GetInt32(sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(int*)ptr;
            }
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt32))]
        public static Int32 ReadInt32(sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (byte)bytes[0]; }
            if (bytes.Length == 2) { return (((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
            if (bytes.Length == 3) { return (((byte)bytes[0]) << 16) + (((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
            return (((byte)bytes[0]) << 24) + (((byte)bytes[1]) << 16) + (((byte)bytes[2]) << 8) + ((byte)bytes[3]);
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt32 GetUInt32(sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(uint*)ptr;
            }
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt32))]
        public static UInt32 ReadUInt32(sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return ((byte)bytes[0]); }
            if (bytes.Length == 2) { return ((uint)((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
            if (bytes.Length == 3) { return ((uint)((byte)bytes[0]) << 16) + ((uint)((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
            return ((uint)((byte)bytes[0]) << 24) + ((uint)((byte)bytes[1]) << 16) + ((uint)((byte)bytes[2]) << 8) + ((byte)bytes[3]);
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int64 GetInt64(sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(long*)ptr;
            }
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt64))]
        public static Int64 ReadInt64(sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return ((byte)bytes[0]); }
                if (bytes.Length == 2) { return ((uint)((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
                if (bytes.Length == 3) { return ((uint)((byte)bytes[0]) << 16) + ((uint)((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
                return ((uint)((byte)bytes[0]) << 24) + ((uint)((byte)bytes[1]) << 16) + ((uint)((byte)bytes[2]) << 8) + ((byte)bytes[3]);
            }
            if (bytes.Length == 5) { return ((long)((byte)bytes[0]) << 32) + ((uint)((byte)bytes[1]) << 24) + (((byte)bytes[2]) << 16) + (((byte)bytes[3]) << 8) + ((byte)bytes[4]); }
            if (bytes.Length == 6) { return ((long)((byte)bytes[0]) << 40) + ((long)((byte)bytes[1]) << 32) + ((uint)((byte)bytes[2]) << 24) + (((byte)bytes[3]) << 16) + (((byte)bytes[4]) << 8) + ((byte)bytes[5]); }
            if (bytes.Length == 7) { return ((long)((byte)bytes[0]) << 48) + ((long)((byte)bytes[1]) << 40) + ((long)((byte)bytes[2]) << 32) + ((uint)((byte)bytes[3]) << 24) + (((byte)bytes[4]) << 16) + (((byte)bytes[5]) << 8) + ((byte)bytes[6]); }
            return ((long)((byte)bytes[0]) << 56) + ((long)((byte)bytes[1]) << 48) + ((long)((byte)bytes[2]) << 40) + ((long)((byte)bytes[3]) << 32) + ((uint)((byte)bytes[4]) << 24) + (((byte)bytes[5]) << 16) + (((byte)bytes[6]) << 8) + ((byte)bytes[7]);
        }
        /// <summary>
        /// 读无符号长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt64 GetUInt64(sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(ulong*)ptr;
            }
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt64))]
        public static UInt64 ReadUInt64(sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return ((byte)bytes[0]); }
                if (bytes.Length == 2) { return ((uint)((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
                if (bytes.Length == 3) { return ((uint)((byte)bytes[0]) << 16) + ((uint)((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
                return ((uint)((byte)bytes[0]) << 24) + ((uint)((byte)bytes[1]) << 16) + ((uint)((byte)bytes[2]) << 8) + ((byte)bytes[3]);
            }
            if (bytes.Length == 5) { return ((ulong)((byte)bytes[0]) << 32) + ((uint)((byte)bytes[1]) << 24) + ((uint)((byte)bytes[2]) << 16) + ((uint)((byte)bytes[3]) << 8) + ((byte)bytes[4]); }
            if (bytes.Length == 6) { return ((ulong)((byte)bytes[0]) << 40) + ((ulong)((byte)bytes[1]) << 32) + ((uint)((byte)bytes[2]) << 24) + ((uint)((byte)bytes[3]) << 16) + ((uint)((byte)bytes[4]) << 8) + ((byte)bytes[5]); }
            if (bytes.Length == 7) { return ((ulong)((byte)bytes[0]) << 48) + ((ulong)((byte)bytes[1]) << 40) + ((ulong)((byte)bytes[2]) << 32) + ((uint)((byte)bytes[3]) << 24) + ((uint)((byte)bytes[4]) << 16) + ((uint)((byte)bytes[5]) << 8) + ((byte)bytes[6]); }
            return ((ulong)((byte)bytes[0]) << 56) + ((ulong)((byte)bytes[1]) << 48) + ((ulong)((byte)bytes[2]) << 40) + ((ulong)((byte)bytes[3]) << 32) + ((uint)((byte)bytes[4]) << 24) + ((uint)((byte)bytes[5]) << 16) + ((uint)((byte)bytes[6]) << 8) + ((byte)bytes[7]);
        }
        #endregion
        #region // 压缩
        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// 解压字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            var ms = new MemoryStream(data);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var msreader = new MemoryStream();
            var buffer = new byte[0x1000];
            while (true)
            {
                var reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }
        #endregion
        #region // 转编码
        private static LazyBone<Encoding> _gb2312Encoding = new LazyBone<Encoding>(() => Encoding.GetEncoding("GB2312"), true);
        private static LazyBone<Encoding> _gbkEncoding = new LazyBone<Encoding>(() => Encoding.GetEncoding("GBK"), true);
        private static LazyBone<Encoding> _utf8Encoding = new LazyBone<Encoding>(() => Encoding.UTF8, true);
        private static LazyBone<Encoding> _unicodeEncoding = new LazyBone<Encoding>(() => Encoding.Unicode, true);
        /// <summary>
        /// GB2312编码
        /// </summary>
        public static Encoding GB2312Encoding { get => _gb2312Encoding.Value; }
        /// <summary>
        /// GBK编码
        /// </summary>
        public static Encoding GBKEncoding { get => _gbkEncoding.Value; }
        /// <summary>
        /// UTF8编码
        /// </summary>
        public static Encoding UTF8Encoding { get => _utf8Encoding.Value; }
        /// <summary>
        /// Unicode编码
        /// </summary>
        public static Encoding UnicodeEncoding { get => _unicodeEncoding.Value; }
        /// <summary>
        /// 获取GB2312编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetGB2312String(byte[] value) => _gb2312Encoding.Value.GetString(value);
        /// <summary>
        /// 转换GBK成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetGB2312Bytes(string content) => _gb2312Encoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GB2312进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetGB2312HexString(string content, bool isLower = false) => GetGB2312Bytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的GB2312字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexGB2312String(string hexString) => GetGB2312String(GetHexByte(hexString));
        /// <summary>
        /// 获取GBK编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetGBKString(byte[] value) => _gbkEncoding.Value.GetString(value);
        /// <summary>
        /// 转换GBK成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetGBKBytes(string content) => _gbkEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GBK16进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetGBKHexString(string content, bool isLower = false) => GetGBKBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的GBK字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexGBKString(string hexString) => GetGBKString(GetHexByte(hexString));
        /// <summary>
        /// 获取Utf8编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetUTF8String(byte[] value) => _utf8Encoding.Value.GetString(value);
        /// <summary>
        /// 转换UTF8成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetUTF8Bytes(string content) => _utf8Encoding.Value.GetBytes(content);
        /// <summary>
        /// 获取UTF8进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetUTF8HexString(string content, bool isLower = false) => GetUTF8Bytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的UTF8字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexUTF8String(string hexString) => GetUTF8String(GetHexByte(hexString));
        /// <summary>
        /// 获取Unicode编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetUnicodeString(byte[] value) => _unicodeEncoding.Value.GetString(value);
        /// <summary>
        /// 转换Unicode成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetUnicodeBytes(string content) => _unicodeEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取Unicode进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetUnicodeHexString(string content, bool isLower = false) => GetUnicodeBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的Unicode字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexUnicodeString(string hexString) => GetUnicodeString(GetHexByte(hexString));
        /// <summary>
        /// 获取编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetEncodingString(byte[] value) => _utf8Encoding.Value.GetString(value);
        /// <summary>
        /// 获取编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String GetEncodingString(byte[] value, Encoding encoding) => encoding.GetString(value);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytes(string content) => CobberCaller.GetBytes(content, _utf8Encoding.Value);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytes(string content, Encoding encoding) => CobberCaller.GetBytes(content, encoding);
        /// <summary>
        /// 获取Encoding16进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetEncodingHexString(string content, Encoding encoding, bool isLower = false) => GetEncodingBytes(content, encoding).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串Encoding字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String GetHexEncodingString(string hexString, Encoding encoding) => GetEncodingString(GetHexByte(hexString), encoding);
        #endregion
    }
    /// <summary>
    /// 复制克隆
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class CopyClone
    {
        /// <summary>
        /// 深度表达式树复制
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="original">Object to copy.</param>
        /// <param name="copiedReferencesDict">Dictionary of already copied objects (Keys: original objects, Values: their copies).</param>
        /// <returns></returns>
        public static T DeepExpressionCopy<T>(T original, Dictionary<object, object> copiedReferencesDict = null)
        {
            return (T)ExtterCaller.DeepExpressionTreeObjCopy(original, false, copiedReferencesDict ?? new Dictionary<object, object>(new ExtterCaller.ReferenceEqualityComparer()));
        }
        /// <summary>
        /// 深度Memory的Serialize复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepMemoryCopy<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// 深度反射复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepReflectionCopy<T>(T original)
        {
            return (T)ExtterCaller.ReflectionInternalCopy((object)original, new Dictionary<Object, object>(new ExtterCaller.ReferenceEqualityComparer()));
        }
    }
    /// <summary>
    /// 成员调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class MemberCaller
    {
        /// <summary>
        /// 字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (FieldInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TM, TP>(Expression<Func<TM, TP>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TM>(Expression<Func<TM, object>> expression)
        {
            var body = expression.Body;
            if (body is MemberExpression member)
            {
                return (PropertyInfo)member.Member;
            }
            if (body is UnaryExpression unary)
            {
                return (PropertyInfo)((MemberExpression)unary.Operand).Member;
            }
            return null;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String GetFullName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            var member = body.Member;
            return $"{member.DeclaringType.FullName}.{member.Name}";
        }
        /// <summary>
        /// 获取成员全称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static String GetFullName(MemberInfo method)
        {
            return $"{method.DeclaringType?.FullName}.{method.Name}";
        }
        #region // 查询模型内容
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="value">查询字符串,为空时,true</param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static bool SearchModel<T>(T model, string value, char split = default)
        {
            if (model == null) { return false; }
            if (string.IsNullOrWhiteSpace(value)) { return true; }
            if (split == default)
            {
                return SearchContains(model, new string[] { value.Trim() }, PropertyAccess.GetAccess(model).FuncGetDic.Values);
            }
            return SearchModel(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,0</param>
        /// <param name="value">查找字符串,为空时,1</param>
        /// <param name="split">查找分隔符</param>
        /// <returns></returns>
        public static int SearchOrModel<T>(T model, string value, char split = default)
        {
            if (model == null) { return 0; }
            if (string.IsNullOrEmpty(value)) { return 1; }
            if (split == default)
            {
                return SearchContainsCount<T>(model, new string[] { value.Trim() }, PropertyAccess.GetAccess(model).FuncGetDic.Values);
            }
            return SearchOrModel<T>(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="values">查询值,为空时,true</param>
        /// <returns></returns>
        public static bool SearchModel<T>(T model, params string[] values)
        {
            if (model == null) { return false; }
            if (values.IsEmpty()) { return true; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToArray();
            if (valItems.Length == 0) { return true; }
            return SearchContains(model, valItems, PropertyAccess.GetAccess(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(T model, params string[] values)
        {
            if (model == null || values == null) { return 0; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return 1; }
            return SearchContainsCount(model, valItems, PropertyAccess.GetAccess(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <param name="values">查询值,为空时,true</param>
        /// <returns></returns>
        public static bool SearchModel<T>(T model, string[] properties, string[] values)
        {
            if (model == null || properties.IsEmpty()) { return false; }
            if (values.IsEmpty()) { return true; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return true; }
            var funcGetList = PropertyAccess.GetAccess(model).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return SearchContains<T>(model, valItems, funcGetList);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(T model, string[] properties, string[] values)
        {
            if (model == null || values == null) { return 0; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return 1; }
            var funcGetList = PropertyAccess.GetAccess(model).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return SearchContainsCount(model, valItems, funcGetList);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="values">查询值,为空时,源列表</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty()) { return new List<T>(); }
            if (values.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var funcGetList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Values;
            return models.Where(m => SearchContains<T>(m, valItems, funcGetList));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            var getFuncList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Values;
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems, getFuncList);
                if (count > 0)
                {
                    dic[count].Add(model);
                }
            }
            var result = new List<T>();
            foreach (var item in dic.OrderByDescending(s => s.Key))
            {
                result.AddRange(item.Value);
            }
            return result;
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(IEnumerable<T> models, string searchContent, string[] properties) => SearchModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(IEnumerable<T> models, string searchContent, string[] properties) => SearchOrModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型(指定字段,指定查找值)
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="properties">查找属性,为空时,空列表</param>
        /// <param name="values">查找值,为空时,源列表</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(IEnumerable<T> models, string[] values, string[] properties)
        {
            if (models.IsEmpty()) { return new List<T>(); }
            if (values.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var funcGetList = properties.IsEmpty() ? PropertyAccess.GetAccess(models.First()).FuncGetDic.Values : PropertyAccess.GetAccess(models.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return models.Where(m => SearchContains<T>(m, valItems, funcGetList));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(IEnumerable<T> models, string[] values, string[] properties)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            var getFuncList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems, getFuncList);
                if (count > 0)
                {
                    dic[count].Add(model);
                }
            }
            var result = new List<T>();
            foreach (var item in dic.OrderByDescending(s => s.Key))
            {
                result.AddRange(item.Value);
            }
            return result;
        }
        private static bool SearchContains<T>(T model, string[] valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var dic = valItems.ToDictionary(s => s, s => false);
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems)
                {
                    if (valString.Contains(vi))
                    {
                        dic[vi] = true;
                    }
                }
            }
            return dic.All(s => s.Value);
        }
        private static int SearchContainsCount<T>(T model, string[] valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var count = 0;
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems)
                {
                    if (valString.Contains(vi)) { count++; }
                }
            }
            return count;
        }
        /// <summary>
        /// 获取查询值,分隔符包括两种逗号/空格/竖线
        /// </summary>
        /// <param name="content"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] GetSearchKeys(string content, char[] split = null)
        {
            split ??= new char[] { ',', '，', '|', ' ' };
            return string.IsNullOrWhiteSpace(content) ? (new string[0]) : content.Trim().Split(split, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess{T}"/>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess<T>() => new PropertyAccess<T>();
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess.Get"/>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess(Type type) => PropertyAccess.Get(type);
        /// <summary>
        /// 获取静态的成员属性值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue(Type type, string memberName)
        {
            return PropertyAccess.Get(type).FuncGetValue.Invoke(null, memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue(Type type, string memberName, object newValue)
        {
            PropertyAccess.Get(type).FuncSetValue.Invoke(null, memberName, newValue);
        }
        /// <summary>
        /// 获取静态的成员属性值
        /// <see cref="PropertyAccess{T}.InternalGetValue"/>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue<T>(string memberName)
        {
            return PropertyAccess<T>.InternalGetValue(default(T), memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// <see cref="PropertyAccess{T}.InternalSetValue"/>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue<T>(string memberName, object newValue)
        {
            PropertyAccess<T>.InternalSetValue(default(T), memberName, newValue);
        }
        /// <summary>
        /// 转换成属性字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToParameters<T>(T model)
        {
            return PropertyAccess<T>.InternalGetDic.ToDictionary(s => s.Key, s => s.Value(model));
        }
        /// <summary>
        /// 获取静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalGetValue"/>
        /// </summary>
        /// <param name="instance">实例对象,为null时使用泛型类的静态内容</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue<T>(T instance, string memberName) where T : class
        {
            if (instance == null || instance.GetType() == typeof(T)) { return PropertyAccess<T>.InternalGetValue(instance, memberName); }
            return PropertyAccess.Get(instance.GetType()).FuncGetValue(instance, memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalSetValue"/>
        /// </summary>
        /// <param name="instance">实例对象,为null时使用泛型类的静态内容</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue<T>(T instance, string memberName, object newValue) where T : class
        {
            if (instance == null || instance.GetType() == typeof(T)) { PropertyAccess<T>.InternalSetValue(instance, memberName, newValue); return; }
            PropertyAccess.Get(instance.GetType()).FuncSetValue(instance, memberName, newValue);
        }
        #region // 获取Action/Func表达式的方法全称或方法信息
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName(Action expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0>(Func<T0> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0>(Action<T0> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0>(Expression<Func<T0>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0>(Expression<Action<T0>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1>(Func<T0, T1> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1>(Action<T0, T1> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1>(Expression<Func<T0, T1>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1>(Expression<Action<T0, T1>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2>(Func<T0, T1, T2> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2>(Action<T0, T1, T2> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2>(Expression<Func<T0, T1, T2>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2>(Expression<Action<T0, T1, T2>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3>(Func<T0, T1, T2, T3> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3>(Action<T0, T1, T2, T3> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3>(Expression<Func<T0, T1, T2, T3>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3>(Expression<Action<T0, T1, T2, T3>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4>(Func<T0, T1, T2, T3, T4> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4>(Expression<Func<T0, T1, T2, T3, T4>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4>(Expression<Action<T0, T1, T2, T3, T4>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5>(Func<T0, T1, T2, T3, T4, T5> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5>(Action<T0, T1, T2, T3, T4, T5> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5>(Expression<Func<T0, T1, T2, T3, T4, T5>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5>(Expression<Action<T0, T1, T2, T3, T4, T5>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6>(Func<T0, T1, T2, T3, T4, T5, T6> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6>(Action<T0, T1, T2, T3, T4, T5, T6> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6>(Expression<Func<T0, T1, T2, T3, T4, T5, T6>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6>(Expression<Action<T0, T1, T2, T3, T4, T5, T6>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7>(Func<T0, T1, T2, T3, T4, T5, T6, T7> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7>(Action<T0, T1, T2, T3, T4, T5, T6, T7> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        #endregion
    }
    /// <summary>
    /// 日期时间调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class DateTimeCaller
    {
        /// <summary>
        /// 通用日期格式
        /// </summary>
        public static String DateFormatter { get; set; } = "yyyy-MM-dd";
        /// <summary>
        /// 通用时间格式
        /// </summary>
        public static String TimeFormatter { get; set; } = "HH:mm:ss";
        /// <summary>
        /// 通用日期时间格式
        /// </summary>
        public static String DateTimeFormatter { get; set; } = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String ToDateString(DateTime date)
        {
            return date.ToString(DateFormatter);
        }
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToTimeString(DateTime time)
        {
            return time.ToString(TimeFormatter);
        }
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String ToDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormatter);
        }
    }
    /// <summary>
    /// 枚举调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class EnumCaller
    {
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag(int value, int tag)
        {
            return (value & tag) != 0;
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(T value, int tag) where T : struct, Enum
        {
            return value.HasFlag((T)(object)tag);
        }
    }
    /// <summary>
    /// 异常调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class ExceptionCaller
    {
        /// <summary>
        /// 获取异常的信息及跟踪信息文本
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetMessageStackTraceString(Exception exception)
        {
            var sb = new StringBuilder();
            sb.Append(exception.Message);
            if (exception.StackTrace != null)
            {
                sb.AppendLine().Append(exception.StackTrace);
            }
            return sb.ToString();
        }
    }
    /// <summary>
    /// 文件调用者
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class FileCaller
    {
        #region // 文件空间及大小
        /// <summary>
        /// 用于获取盘信息的api
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <param name="sectorsPerCluster"></param>
        /// <param name="bytesPerSector"></param>
        /// <param name="numberOfFreeClusters"></param>
        /// <param name="totalNumbeOfClusters"></param>
        /// <returns></returns>
        public static bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName, ref uint sectorsPerCluster, ref uint bytesPerSector, ref uint numberOfFreeClusters, ref uint totalNumbeOfClusters)
            => KERNEL32.GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumbeOfClusters);
        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static FolderSizeInfo GetFolderSize(DirectoryInfo directory)
        {
            FolderSizeInfo result = new FolderSizeInfo();
            if (!directory.Exists) { return result; }
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            long fileSize = 0;
            long spaceSize = 0;
            var diskInfo = directory.GetDiskInfo();
            foreach (var item in files)
            {
                fileSize += item.Length;
                spaceSize += CalcSpaceSize(item.Length, diskInfo.ClusterSize);
            }
            var dirs = directory.GetDirectories("*", SearchOption.AllDirectories);
            result.FolderCount = dirs.Length;
            result.FolderSize = spaceSize;
            result.FileCount = files.Length;
            result.FileSize = fileSize;
            return result;
        }
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(DirectoryInfo dir) => GetDiskInfo(dir.Root.FullName);
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(FileInfo file) => GetDiskInfo(file.Directory.Root.FullName);
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(string rootPathName)
        {
            uint sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
            return new DiskSizeInfo()
            {
                SectorsPerCluster = sectorsPerCluster,
                BytesPerSector = bytesPerSector
            };
        }
        private static long CalcSpaceSize(long fileSize, long clusterSize)
        {
            if (fileSize % clusterSize == 0) { return fileSize; }
            decimal res = fileSize / clusterSize;
            int clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
            return clusterSize * clu;
        }
        #endregion
        #region // 判断文件夹
        /// <summary>
        /// 尝试打开文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool TryOpenDirectory(string dir)
        {
            try
            {
                if (File.Exists(dir))
                {
                    Process.Start("explorer.exe", $"/e,/select,{dir}");
                }
                else
                {
                    Process.Start("explorer.exe", $"/e,{dir}");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 最后一个盘符
        /// </summary>
        public static String LastLocalDisk => GetLastLocalDisk();
        /// <summary>
        /// 最后一个盘符
        /// 如:F:\
        /// </summary>
        /// <returns></returns>
        public static string GetLastLocalDisk()
        {
            return DriveInfo.GetDrives().LastOrDefault(s => s.DriveType == DriveType.Fixed)?.Name;
        }
        /// <summary>
        /// 获取已存在的保存目录
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public static string GetExistSaveDir(string saveDir, string parent, string subDir, bool isRecursive)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(saveDir))
                {
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }
                    return saveDir;
                }
                saveDir = Path.GetFullPath(Path.Combine(parent, subDir));
                CreateDir(new DirectoryInfo(saveDir), isRecursive);
                return saveDir;
            }
            catch
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isRecursive">循环递归创建</param>
        /// <returns></returns>
        public static DirectoryInfo CreateDir(DirectoryInfo dir, bool isRecursive = false)
        {
            if (isRecursive)
            {
                return CreateRecursiveDir(dir);
            }
            if (!dir.Exists) { dir.Create(); }
            return dir;
        }
        /// <summary>
        /// 级联创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateRecursiveDir(DirectoryInfo dir)
        {
            if (dir.Exists) { return dir; }
            CreateRecursiveDir(dir.Parent);
            if (!dir.Exists) { dir.Create(); }
            return dir;
        }
        /// <summary>
        /// 级联检查目录是否存在
        /// </summary>
        /// <param name="dir"></param>
        public static void CheckRecursiveDir(DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                CheckRecursiveDir(dir.Parent);
                dir.Create();
            }
        }
        /// <summary>
        /// 存在内容
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool HasContent(DirectoryInfo dir)
        {
            if (dir == null) { return false; }
            return dir.Exists && (dir.GetFiles().Length > 0 || dir.GetDirectories().Length > 0);
        }
    }
    /// <summary>
    /// IP地址
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class IPCaller
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long GetValue(IPAddress ip)
        {
            byte[] byt = ip.GetAddressBytes();
            if (byt.Length == 4)
            {
                return System.BitConverter.ToUInt32(byt, 0);
            }
            return System.BitConverter.ToInt64(byt, 0);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static IPAddress GetIPAddress(Int64 ip)
        {
            return new IPAddress(ip);
        }
    }
    /// <summary>
    /// 集合和数组操作
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class ListArrayCaller
    {
        /// <summary>
        /// 集合左边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static List<T> PadLeft<T>(IEnumerable<T> list, int length, T defVal = default(T))
        {
            var result = new List<T>();
            if (list != null)
            {
                result.AddRange(list);
            }
            if (result.Count >= length)
            {
                return result;
            }
            var count = length - result.Count;
            for (int i = 0; i < count; i++)
            {
                result.Insert(0, defVal);
            }
            return result;
        }
        /// <summary>
        /// 集合右边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static List<T> PadRight<T>(IEnumerable<T> list, int length, T defVal = default(T))
        {
            var result = new List<T>();
            if (list != null)
            {
                result.AddRange(list);
            }
            if (result.Count >= length)
            {
                return result;
            }
            var count = length - result.Count;
            for (int i = 0; i < count; i++)
            {
                result.Add(defVal);
            }
            return result;
        }
        /// <summary>
        /// 设置ASCII码开头的内容
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] SetAsciiCharStart(byte[] tag, string val)
        {
            var len = tag.Length > val.Length ? val.Length : tag.Length;
            for (int i = 0; i < len; i++) { tag[i] = (byte)val[i]; }
            return tag;
        }
        /// <summary>
        /// 判断数组是否以内容开头
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool StartsWith(byte[] tag, byte[] val)
        {
            if (tag == null) { return false; }
            if (val == null) { return true; }
            if (tag.Length < val.Length) { return false; }
            for (int i = 0; i < val.Length; i++)
            {
                if (tag[i] != val[i]) { return false; }
            }
            return true;
        }
        /// <summary>
        /// 判断数组是否以内容开头
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool StartsWithAscii(byte[] tag, string val)
        {
            if (tag == null) { return false; }
            if (val == null) { return true; }
            if (tag.Length < val.Length) { return false; }
            for (int i = 0; i < val.Length; i++)
            {
                if (tag[i] != val[i]) { return false; }
            }
            return true;
        }
        /// <summary>
        /// 查找及其所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="EqualItem"></param>
        /// <param name="GetSubItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(IEnumerable<T> list, Func<T, bool> EqualItem, Func<T, IEnumerable<T>> GetSubItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                if (EqualItem(item))
                {
                    res.Add(item);
                    res.AddRange(FindAndSubItems(GetSubItems(item), GetSubItems));
                }
            }
            return res.ToArray();
        }
        /// <summary>
        /// 查找及其所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="tagModel"></param>
        /// <param name="FindItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(IEnumerable<T> list, T tagModel, Func<T, T, bool> FindItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                if (FindItems(tagModel, item))
                {
                    res.Add(item);
                    res.AddRange(FindAndSubItems(list, item, FindItems));
                }
            }
            return res.ToArray();
        }
        /// <summary>
        /// 查找及其所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="EqualItem"></param>
        /// <param name="FindItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(IEnumerable<T> list, Func<T, bool> EqualItem, Func<T, T, bool> FindItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                if (EqualItem(item))
                {
                    res.Add(item);
                    res.AddRange(FindAndSubItems(list, item, FindItems));
                }
            }
            return res.ToArray();
        }
        /// <summary>
        /// 查找所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="GetSubItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(IEnumerable<T> list, Func<T, IEnumerable<T>> GetSubItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                res.Add(item);
                res.AddRange(FindAndSubItems(GetSubItems(item), GetSubItems));
            }
            return res.ToArray();
        }
    }
    /// <summary>
    /// 媒介调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class MediaCaller
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytes(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin); //一定不要忘记将流的初始位置重置
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length); //如果上面流没有seek 则这里读取的数据全会为0
            ms.Dispose();
            return bytes;
        }
    }
    /// <summary>
    /// 数字调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class NumberCaller
    {
        #region // 读取字节
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(Int16 number, bool isBigEndian = true)
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
        public static byte[] ReadBytes(Int16 number)
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
        unsafe public static byte[] GetBytes(UInt16 number, bool isBigEndian = true)
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
        public static byte[] ReadBytes(UInt16 number)
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
        unsafe public static sbyte[] GetSBytes(Int16 number, bool isBigEndian = true)
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
        public static sbyte[] ReadSBytes(Int16 number)
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
        unsafe public static sbyte[] GetSBytes(UInt16 number, bool isBigEndian = true)
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
        public static sbyte[] ReadSBytes(UInt16 number)
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
        unsafe public static byte[] GetBytes(Int32 number, bool isBigEndian = true)
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
        public static byte[] ReadBytes(Int32 number)
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
        unsafe public static sbyte[] GetSBytes(Int32 number, bool isBigEndian = true)
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
        public static sbyte[] ReadSBytes(Int32 number)
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
        unsafe public static byte[] GetBytes(UInt32 number, bool isBigEndian = true)
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
        public static byte[] ReadBytes(UInt32 number)
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
        unsafe public static sbyte[] GetSBytes(UInt32 number, bool isBigEndian = true)
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
        public static sbyte[] ReadSBytes(UInt32 number)
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
        unsafe public static byte[] GetBytes(Int64 number, bool isBigEndian = true)
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
        public static byte[] ReadBytes(Int64 number)
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
        unsafe public static sbyte[] GetSBytes(Int64 number, bool isBigEndian = true)
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
        public static sbyte[] ReadSBytes(Int64 number)
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
        unsafe public static byte[] GetBytes(UInt64 number, bool isBigEndian = true)
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
        public static byte[] ReadBytes(UInt64 number)
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
        unsafe public static sbyte[] GetSBytes(UInt64 number, bool isBigEndian = true)
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
        public static sbyte[] ReadSBytes(UInt64 number)
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
        public static IEnumerable<TOut> CastAs<TIn, TOut>(IEnumerable<TIn> list)
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
        public static IEnumerable<TOut> CastAs<TIn, TOut>(IEnumerable<TIn> list, Func<TIn, TOut> GetValue)
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
        public static IEnumerable<Char> CastTo(IEnumerable<Byte> list)
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
        public static IEnumerable<Char> CastTo2(IEnumerable<Byte> list)
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
        public static IEnumerable<Byte> CastTo(IEnumerable<Char> list)
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
        public static IEnumerable<Byte> CastTo2(IEnumerable<Char> list)
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
        public static String ReadHex(Int32 val)
        {
            return val.ToString("X8");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(UInt32 val)
        {
            return val.ToString("X8");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(Int16 val)
        {
            return val.ToString("X4");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(UInt16 val)
        {
            return val.ToString("X4");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(Int64 val)
        {
            return val.ToString("X16");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(UInt64 val)
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
        public static double GetHalfBitDouble(double value, int digit)
        {
            return Cobber.CobberCaller.GetDouble((Math.Ceiling(value * Math.Pow(10, digit - 1) * 2) / 2.0 * Math.Pow(10, -digit + 1)), digit);
        }
    }
    /// <summary>
    /// 打开调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class OpenCaller
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFile(string fileName) => StartFile(new FileInfo(fileName));
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFile(FileInfo fileName)
        {
            Process.Start(new ProcessStartInfo(fileName.FullName) { UseShellExecute = true });
        }
    }
    /// <summary>
    /// 字符串调用扩展
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class StringCaller
    {
        /// <summary>
        /// 转换成蛇形
        /// ATest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToSnakeCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            list.Add(char.ToLower(str[0]));
            char curr;
            for (int i = 1; i < str.Length; i++)
            {
                curr = str[i];
                if (Char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(Char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 驼峰转换成蛇形
        /// aTest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string CamelToSnakeCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            char curr;
            for (int i = 0; i < str.Length; i++)
            {
                curr = str[i];
                if (Char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(Char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 帕斯卡(大驼峰)转换成蛇形
        /// ATest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string PascalToSnakeCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            var list = new List<char>(str.Length * 2);
            list.Add(char.ToLower(str[0]));
            char curr;
            for (int i = 1; i < str.Length; i++)
            {
                curr = str[i];
                if (char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 帕斯卡(大驼峰)转换成驼峰
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PascalToCamelCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            char curr = Char.ToLower(str[0]);
            StringBuilder sb = new StringBuilder().Append(curr);
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 驼峰转换成帕斯卡(大驼峰)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CamelToPascalCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            char curr = Char.ToUpper(str[0]);
            StringBuilder sb = new StringBuilder().Append(curr);
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 蛇形转换成驼峰
        /// a_test=>aTest
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string SnakeToCamelCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length);
            char curr;
            for (int i = 0; i < str.Length; i++)
            {
                curr = str[i];
                if (curr == '_')
                {
                    i++;
                    if (i >= str.Length) { break; }
                    curr = Char.ToUpper(str[i]);
                }
                list.Add(curr);
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 蛇形转换成帕斯卡(大驼峰)
        /// a_test=>ATest
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string SnakeToPascalCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length);
            char curr = str[0];
            int start;
            if (curr == '_')
            {
                if (str.Length == 1) { return str; }
                start = 2;
                list.Add(Char.ToUpper(curr));
            }
            else
            {
                start = 1;
                list.Add(char.ToUpper(curr));
            }
            for (int i = start; i < str.Length; i++)
            {
                curr = str[i];
                if (curr == '_')
                {
                    i++;
                    if (i >= str.Length) { break; }
                    curr = Char.ToUpper(str[i]);
                }
                list.Add(curr);
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 相等忽略大小写
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(string src, string tag)
        {
            if (src == null) { return src == tag; }
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 相等忽略大小写
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualIgnoreCase(string src, string tag)
        {
            if (src == null) { return src == tag; }
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 相等忽略大小写
        /// 无null判断
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase2(string src, string tag)
        {
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 相等忽略大小写
        /// 无null判断
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualIgnoreCase2(string src, string tag)
        {
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 获取文本的指定长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string TakeString(string text, int len)
        {
            if (text == null || len <= 0) { return text; }
            if (text.Length > len) { return text.Substring(0, len); }
            return text;
        }
        #region // Base64
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <returns></returns>
        public static String GetBase64(string json, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(json));
        }
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <see cref="SerialCaller.GetModelBytes{T}(T)"/>
        /// <returns></returns>
        public static String GetBase64<T>(T model)
        {
            return Convert.ToBase64String(model.GetModelBytes());
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <see cref="SerialCaller.GetBytesModel{T}(byte[])"/>
        /// <returns></returns>
        public static T GetDebase64<T>(string base64)
        {
            return Convert.FromBase64String(base64).GetBytesModel<T>();
        }
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <returns></returns>
        public static String GetBase64(string json) => GetBase64(json, Encoding.UTF8);
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <returns></returns>
        public static String GetDebase64(string base64, Encoding encoding)
        {
            var bytes = Convert.FromBase64String(base64);
            return encoding.GetString(bytes);
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <returns></returns>
        public static String GetDebase64(string base64) => GetDebase64(base64, Encoding.UTF8);
        #endregion
        #region // 压缩
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Compress(string str) => Compress(str, Encoding.UTF8);
        /// <summary>
        /// 根据编码压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Compress(string str, Encoding encoding)
        {
            var bytes = encoding.GetBytes(str);

            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(bytes, 0, bytes.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();

            return Convert.ToBase64String(buffer);
        }
        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decompress(string str) => Decompress(str, Encoding.UTF8);
        /// <summary>
        /// 根据编码解压字符串
        /// </summary>
        /// <returns></returns>
        public static string Decompress(string str, Encoding encoding)
        {
            var bytes = Convert.FromBase64String(str);

            var ms = new MemoryStream(bytes);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var msreader = new MemoryStream();
            var buffer = new byte[0x1000];
            while (true)
            {
                var reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();

            return encoding.GetString(buffer);
        }
        #endregion
    }
    /// <summary>
    /// 仿照元组(Tuple)进行创建的类
    /// </summary>
    [Obsolete("替代方案:Tuble")]
    public static class TubleCaller
    {
        /// <summary>
        /// 创建二元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2> Create<T1, T2>(T1 model1, T2 model2)
        {
            return new Tuble<T1, T2>(model1, model2);
        }
        /// <summary>
        /// 创建三元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3> Create<T1, T2, T3>(T1 model1, T2 model2, T3 model3)
        {
            return new Tuble<T1, T2, T3>(model1, model2, model3);
        }
        /// <summary>
        /// 创建四元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 model1, T2 model2, T3 model3, T4 model4)
        {
            return new Tuble<T1, T2, T3, T4>(model1, model2, model3, model4);
        }
        /// <summary>
        /// 创建五元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5)
        {
            return new Tuble<T1, T2, T3, T4, T5>(model1, model2, model3, model4, model5);
        }
        /// <summary>
        /// 创建六元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6>(model1, model2, model3, model4, model5, model6);
        }
        /// <summary>
        /// 创建七元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7>(model1, model2, model3, model4, model5, model6, model7);
        }
        /// <summary>
        /// 创建八元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7, T8 model8)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7, T8>(model1, model2, model3, model4, model5, model6, model7, model8);
        }
    }
    /// <summary>
    /// 网络调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class WebCaller
    {
        /// <summary>
        /// 获取一个请求
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IAlertMsg GetRequest(FtpRequestTypeModel model)
        {
            switch (model.Type)
            {
                case FtpRequestTypeModel.ReqType.Upload: return UploadFile(model, model.FileName);
                case FtpRequestTypeModel.ReqType.Delete: return DeleteFile(model);
                case FtpRequestTypeModel.ReqType.Download:
                default: return DownloadFile(model, model.FileName);
            }
        }
        /// <summary>
        /// 上传
        /// </summary>
        public static AlertMsg UploadFile(FtpRequestModel model, string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            string uri = model.GetUrl().TrimEnd('/') + fileInfo.Name;
            var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(model.Account, model.Password);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInfo.Length;

            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            using (FileStream fs = fileInfo.OpenRead())
            {
                using (Stream strm = reqFTP.GetRequestStream())
                {
                    try
                    {
                        contentLen = fs.Read(buff, 0, buffLength);
                        while (contentLen != 0)
                        {
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }
                        strm.Close();
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return new AlertMsg(false, ex.Message);
                    }
                    return AlertMsg.OperSuccess;
                }
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        public static IAlertMsg DownloadFile(FtpRequestModel model, string saveFileName)
        {
            try
            {
                FileStream outputStream = new FileStream(saveFileName, FileMode.Create);
                var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(model.GetUrl()));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(model.Account, model.Password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, ex.Message);
            }
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        public static IAlertMsg DeleteFile(FtpRequestModel model)
        {
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(model.GetUrl()));
                reqFTP.Credentials = new NetworkCredential(model.Account, model.Password);
                reqFTP.KeepAlive = false;
                reqFTP.Method = nameof(WebRequestType.DELE);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                var result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, ex.Message);
            }
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 获取枚举的Enum名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToEnumString(WebRequestType type)
        {
            return type switch
            {
                WebRequestType.GET => nameof(WebRequestType.GET),
                WebRequestType.PUT => nameof(WebRequestType.PUT),
                WebRequestType.RETR => nameof(WebRequestType.RETR),
                WebRequestType.NLST => nameof(WebRequestType.NLST),
                WebRequestType.STOR => nameof(WebRequestType.STOR),
                WebRequestType.DELE => nameof(WebRequestType.DELE),
                WebRequestType.APPE => nameof(WebRequestType.APPE),
                WebRequestType.SIZE => nameof(WebRequestType.SIZE),
                WebRequestType.STOU => nameof(WebRequestType.STOU),
                WebRequestType.MKD => nameof(WebRequestType.MKD),
                WebRequestType.RMD => nameof(WebRequestType.RMD),
                WebRequestType.LIST => nameof(WebRequestType.LIST),
                WebRequestType.MDTM => nameof(WebRequestType.MDTM),
                WebRequestType.PWD => nameof(WebRequestType.PWD),
                WebRequestType.RENAME => nameof(WebRequestType.RENAME),
                WebRequestType.CONNECT => nameof(WebRequestType.CONNECT),
                WebRequestType.HEAD => nameof(WebRequestType.HEAD),
                WebRequestType.POST => nameof(WebRequestType.POST),
                WebRequestType.MKCOL => nameof(WebRequestType.MKCOL),
                _ => nameof(WebRequestType.GET),
            };
        }
    }
    /// <summary>
    /// 序列化调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class SerialCaller
    {
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <see cref="BinaryFormatter.Deserialize(Stream)"/>
        /// <see cref="GetBytesModel"/>
        /// <returns></returns>
        [Obsolete("替代方案:GetBytesModel,原因:高版本.NET已弃用,请使用另外的方法")]
        public static T GetBinModel<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return (T)ExtterCaller.SerialByteBinder<T>.Formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <see cref="BinaryFormatter.Serialize(Stream, object)"/>
        /// <see cref="GetModelBytes"/>
        /// <returns></returns>
        [Obsolete("替代方案:GetModelBytes,原因:高版本.NET已弃用,请使用另外的方法")]
        public static byte[] GetBinBytes<T>(T model)
        {
            using (var stream = new MemoryStream())
            {
                ExtterCaller.SerialByteBinder<T>.Formatter.Serialize(stream, model);
                return stream.ToArray();
            }
        }
        /// <summary>
        /// 将对象转换为byte数组
        /// </summary>
        /// <param name="model">被转换对象</param>
        /// <returns>转换后byte数组</returns>
        public static byte[] GetModelBytes<T>(T model)
        {
            byte[] buff = new byte[Marshal.SizeOf(model)];
            Marshal.StructureToPtr(model, Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0), true);
            return buff;
        }

        /// <summary>
        /// 将byte数组转换成对象
        /// </summary>
        /// <param name="bytes">被转换byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static T GetBytesModel<T>(byte[] bytes)
        {
            return (T)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), typeof(T));
        }
        /// <summary>
        /// 将byte数组转换成对象
        /// </summary>
        /// <param name="bytes">被转换byte数组</param>
        /// <param name="type">转换成的类名</param>
        /// <returns>转换完成后的对象</returns>
        public static object GetBytesModel(byte[] bytes, Type type)
        {
            return Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), type);
        }
    }
    /// <summary>
    /// 单元测试调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class UnitTestCaller
    {
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestAction(string consoleFmt, Action action, int times = 10000)
        {
            action.Invoke();
            var now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                action.Invoke();
            }
            Console.WriteLine(consoleFmt, DateTime.Now - now);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestFunc<T>(string consoleFmt, Func<T> action, int times = 10000)
        {
            action.Invoke();
            var now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                action.Invoke();
            }
            Console.WriteLine(consoleFmt, DateTime.Now - now);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static async Task TestActionAsync(string consoleFmt, Action action, int times = 10000)
        {
            await Task.Factory.StartNew(() => TestAction(consoleFmt, action, times));
        }
    }
    /// <summary>
    /// 单元测试调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class ShortcutCaller
    {
        internal static readonly Guid CLSID_WshShell = new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8");
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="directory">快捷方式所处的文件夹</param>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks></remarks>
        /*
            <COMReference Include="IWshRuntimeLibrary">
                <WrapperTool>tlbimp</WrapperTool>
                <VersionMinor>0</VersionMinor>
                <VersionMajor>1</VersionMajor>
                <Guid>f935dc20-1cf0-11d0-adb9-00c04fd58a0b</Guid>
                <Lcid>0</Lcid>
                <Isolated>false</Isolated>
                <EmbedInteropTypes>true</EmbedInteropTypes>
            </COMReference>
        */
        public static void Create(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }
            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            if (System.IO.File.Exists(shortcutPath)) { IO.File.Delete(shortcutPath); }
            dynamic wshShell = null, shortcut = null;
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                wshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WshShell)); // WshShell shell = new WshShell();
#pragma warning restore CA1416 // 验证平台兼容性
                shortcut = wshShell.CreateShortcut(shortcutPath); // shortcut = (IWshShortcut)wshShell.CreateShortcut(shortcutPath); //创建快捷方式对象
                shortcut.TargetPath = targetPath; //指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath); //设置起始位置
                shortcut.WindowStyle = 1; //设置运行方式，默认为常规窗口
                shortcut.Description = description; //设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation; //设置图标路径
                shortcut.Arguments = args;
                shortcut.Save(); //保存快捷方式
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Marshal.ReleaseComObject(shortcut);
                Marshal.ReleaseComObject(wshShell);
            }

        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"</param>
        /// <param name="args"></param>
        /// <remarks>参数</remarks>
        public static void CreateDesktop(string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); //获取桌面文件夹路径
            Create(desktop, shortcutName, targetPath, description, iconLocation, args);
        }
    }
    /// <summary>
    /// Window操作系统调用
    /// </summary>
    [Obsolete("替代方案:ExtterCaller")]
    public static class WindowCmdCaller
    {
        /// <summary>
        /// C:\Windows
        /// </summary>
        public static String WindowDir => Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
        /// <summary>
        /// 获取特殊目录全路径
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        public static String GetFullPath(Environment.SpecialFolder special)
        {
            return Path.GetFullPath(Environment.GetFolderPath(special));
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="exeFile"></param>
        /// <param name="startDir"></param>
        /// <param name="args"></param>
        public static IAlertMsg ExecHidden(string exeFile, string startDir, string args)
        {
            var result = new AlertMsg(true, "");
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    FileName = exeFile,
                    Arguments = args,
                    WorkingDirectory = Path.GetFullPath(startDir),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                }
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null) { return; }
                result.AddMsg(e.Data);
            };
            p.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null) { return; }
                result.AddMsg(e.Data);
            };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();

            return result;
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        public static IAlertMsg ExecHidden(string command) => ExecHidden("cmd.exe", WindowDir, command);
        /// <summary>
        /// 执行命令行
        /// </summary>
        public static IAlertMsg ExecHidden(string exeFile, string command) => ExecHidden(exeFile, WindowDir, command);
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IAlertMsg NetStart(string serviceName) => ExecHidden("net", WindowDir, $" start {serviceName}");
        /// <summary>
        /// 关闭服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IAlertMsg NetStop(string serviceName) => ExecHidden("net", WindowDir, $" stop {serviceName}");
    }
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 深度Memory的Serialize复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("替代方案:DeepJsonCopy,原因是BinaryFormatter部分已弃用")]
        public static T DeepMemoryCopy<T>(T obj)
        {
            return CopyClone.DeepMemoryCopy(obj);
        }
        /// <summary>
        /// 用于获取盘信息的api
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <param name="sectorsPerCluster"></param>
        /// <param name="bytesPerSector"></param>
        /// <param name="numberOfFreeClusters"></param>
        /// <param name="totalNumbeOfClusters"></param>
        /// <returns></returns>
        [Obsolete("替代方案:【Impeller.KERNEL32.GetDiskFreeSpace】")]
        public static bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName, ref uint sectorsPerCluster, ref uint bytesPerSector, ref uint numberOfFreeClusters, ref uint totalNumbeOfClusters)
            => Impeller.KERNEL32.GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumbeOfClusters);

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <see cref="BinaryFormatter.Deserialize(Stream)"/>
        /// <see cref="GetBytesModel"/>
        /// <returns></returns>
        [Obsolete("替代方案:GetBytesModel,原因:高版本.NET已弃用,请使用另外的方法代替,不相互兼容")]
        public static T GetBinModel<T>(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return (T)SerialByteBinder<T>.Formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <see cref="SerializableAttribute"/>
        /// <see cref="BinaryFormatter.Serialize(Stream, object)"/>
        /// <see cref="GetModelBytes"/>
        /// <returns></returns>
        [Obsolete("替代方案:GetModelBytes,原因:高版本.NET已弃用,请使用另外的方法代替,不相互兼容")]
        public static byte[] GetBinBytes<T>(this T model)
        {
            using (var stream = new MemoryStream())
            {
                SerialByteBinder<T>.Formatter.Serialize(stream, model);
                return stream.ToArray();
            }
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt16))]
        public static Int16 ReadInt16(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            return (Int16)((bytes[0] << 8) + bytes[1]);
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt16))]
        public static UInt16 ReadUInt16(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            return (UInt16)((bytes[0] << 8) + bytes[1]);
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt32))]
        public static Int32 ReadInt32(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            if (bytes.Length == 2) { return (bytes[0] << 8) + bytes[1]; }
            if (bytes.Length == 3) { return (bytes[0] << 16) + (bytes[1] << 8) + bytes[2]; }
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt32))]
        public static UInt32 ReadUInt32(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            if (bytes.Length == 2) { return ((uint)bytes[0] << 8) + bytes[1]; }
            if (bytes.Length == 3) { return ((uint)bytes[0] << 16) + ((uint)bytes[1] << 8) + bytes[2]; }
            return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + bytes[3];
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt64))]
        public static Int64 ReadInt64(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return bytes[0]; }
                if (bytes.Length == 2) { return ((uint)bytes[0] << 8) + bytes[1]; }
                if (bytes.Length == 3) { return ((uint)bytes[0] << 16) + ((uint)bytes[1] << 8) + bytes[2]; }
                return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + bytes[3];
            }
            if (bytes.Length == 5) { return ((long)bytes[0] << 32) + ((uint)bytes[1] << 24) + (bytes[2] << 16) + (bytes[3] << 8) + bytes[4]; }
            if (bytes.Length == 6) { return ((long)bytes[0] << 40) + ((long)bytes[1] << 32) + ((uint)bytes[2] << 24) + (bytes[3] << 16) + (bytes[4] << 8) + bytes[5]; }
            if (bytes.Length == 7) { return ((long)bytes[0] << 48) + ((long)bytes[1] << 40) + ((long)bytes[2] << 32) + ((uint)bytes[3] << 24) + (bytes[4] << 16) + (bytes[5] << 8) + bytes[6]; }
            return ((long)bytes[0] << 56) + ((long)bytes[1] << 48) + ((long)bytes[2] << 40) + ((long)bytes[3] << 32) + ((uint)bytes[4] << 24) + (bytes[5] << 16) + (bytes[6] << 8) + bytes[7];
        }
        /// <summary>
        /// 读无符号长整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt64))]
        public static UInt64 ReadUInt64(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return bytes[0]; }
                if (bytes.Length == 2) { return ((uint)bytes[0] << 8) + bytes[1]; }
                if (bytes.Length == 3) { return ((uint)bytes[0] << 16) + ((uint)bytes[1] << 8) + bytes[2]; }
                return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + bytes[3];
            }
            if (bytes.Length == 5) { return ((ulong)bytes[0] << 32) + ((uint)bytes[1] << 24) + ((uint)bytes[2] << 16) + ((uint)bytes[3] << 8) + bytes[4]; }
            if (bytes.Length == 6) { return ((ulong)bytes[0] << 40) + ((ulong)bytes[1] << 32) + ((uint)bytes[2] << 24) + ((uint)bytes[3] << 16) + ((uint)bytes[4] << 8) + bytes[5]; }
            if (bytes.Length == 7) { return ((ulong)bytes[0] << 48) + ((ulong)bytes[1] << 40) + ((ulong)bytes[2] << 32) + ((uint)bytes[3] << 24) + ((uint)bytes[4] << 16) + ((uint)bytes[5] << 8) + bytes[6]; }
            return ((ulong)bytes[0] << 56) + ((ulong)bytes[1] << 48) + ((ulong)bytes[2] << 40) + ((ulong)bytes[3] << 32) + ((uint)bytes[4] << 24) + ((uint)bytes[5] << 16) + ((uint)bytes[6] << 8) + bytes[7];
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt16))]
        public static Int16 ReadInt16(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (Int16)((byte)bytes[0]); }
            return (Int16)((((byte)bytes[0]) << 8) + ((byte)bytes[1]));
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt16))]
        public static UInt16 ReadUInt16(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (UInt16)((byte)bytes[0]); }
            return (UInt16)((((byte)bytes[0]) << 8) + ((byte)bytes[1]));
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt32))]
        public static Int32 ReadInt32(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (byte)bytes[0]; }
            if (bytes.Length == 2) { return (((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
            if (bytes.Length == 3) { return (((byte)bytes[0]) << 16) + (((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
            return (((byte)bytes[0]) << 24) + (((byte)bytes[1]) << 16) + (((byte)bytes[2]) << 8) + ((byte)bytes[3]);
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt32))]
        public static UInt32 ReadUInt32(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return ((byte)bytes[0]); }
            if (bytes.Length == 2) { return ((uint)((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
            if (bytes.Length == 3) { return ((uint)((byte)bytes[0]) << 16) + ((uint)((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
            return ((uint)((byte)bytes[0]) << 24) + ((uint)((byte)bytes[1]) << 16) + ((uint)((byte)bytes[2]) << 8) + ((byte)bytes[3]);
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetInt64))]
        public static Int64 ReadInt64(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return ((byte)bytes[0]); }
                if (bytes.Length == 2) { return ((uint)((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
                if (bytes.Length == 3) { return ((uint)((byte)bytes[0]) << 16) + ((uint)((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
                return ((uint)((byte)bytes[0]) << 24) + ((uint)((byte)bytes[1]) << 16) + ((uint)((byte)bytes[2]) << 8) + ((byte)bytes[3]);
            }
            if (bytes.Length == 5) { return ((long)((byte)bytes[0]) << 32) + ((uint)((byte)bytes[1]) << 24) + (((byte)bytes[2]) << 16) + (((byte)bytes[3]) << 8) + ((byte)bytes[4]); }
            if (bytes.Length == 6) { return ((long)((byte)bytes[0]) << 40) + ((long)((byte)bytes[1]) << 32) + ((uint)((byte)bytes[2]) << 24) + (((byte)bytes[3]) << 16) + (((byte)bytes[4]) << 8) + ((byte)bytes[5]); }
            if (bytes.Length == 7) { return ((long)((byte)bytes[0]) << 48) + ((long)((byte)bytes[1]) << 40) + ((long)((byte)bytes[2]) << 32) + ((uint)((byte)bytes[3]) << 24) + (((byte)bytes[4]) << 16) + (((byte)bytes[5]) << 8) + ((byte)bytes[6]); }
            return ((long)((byte)bytes[0]) << 56) + ((long)((byte)bytes[1]) << 48) + ((long)((byte)bytes[2]) << 40) + ((long)((byte)bytes[3]) << 32) + ((uint)((byte)bytes[4]) << 24) + (((byte)bytes[5]) << 16) + (((byte)bytes[6]) << 8) + ((byte)bytes[7]);
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetUInt64))]
        public static UInt64 ReadUInt64(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length <= 4)
            {
                if (bytes.Length == 1) { return ((byte)bytes[0]); }
                if (bytes.Length == 2) { return ((uint)((byte)bytes[0]) << 8) + ((byte)bytes[1]); }
                if (bytes.Length == 3) { return ((uint)((byte)bytes[0]) << 16) + ((uint)((byte)bytes[1]) << 8) + ((byte)bytes[2]); }
                return ((uint)((byte)bytes[0]) << 24) + ((uint)((byte)bytes[1]) << 16) + ((uint)((byte)bytes[2]) << 8) + ((byte)bytes[3]);
            }
            if (bytes.Length == 5) { return ((ulong)((byte)bytes[0]) << 32) + ((uint)((byte)bytes[1]) << 24) + ((uint)((byte)bytes[2]) << 16) + ((uint)((byte)bytes[3]) << 8) + ((byte)bytes[4]); }
            if (bytes.Length == 6) { return ((ulong)((byte)bytes[0]) << 40) + ((ulong)((byte)bytes[1]) << 32) + ((uint)((byte)bytes[2]) << 24) + ((uint)((byte)bytes[3]) << 16) + ((uint)((byte)bytes[4]) << 8) + ((byte)bytes[5]); }
            if (bytes.Length == 7) { return ((ulong)((byte)bytes[0]) << 48) + ((ulong)((byte)bytes[1]) << 40) + ((ulong)((byte)bytes[2]) << 32) + ((uint)((byte)bytes[3]) << 24) + ((uint)((byte)bytes[4]) << 16) + ((uint)((byte)bytes[5]) << 8) + ((byte)bytes[6]); }
            return ((ulong)((byte)bytes[0]) << 56) + ((ulong)((byte)bytes[1]) << 48) + ((ulong)((byte)bytes[2]) << 40) + ((ulong)((byte)bytes[3]) << 32) + ((uint)((byte)bytes[4]) << 24) + ((uint)((byte)bytes[5]) << 16) + ((uint)((byte)bytes[6]) << 8) + ((byte)bytes[7]);
        }
        /// <summary>
        /// 序列化绑定
        /// </summary>
        [Obsolete("BinaryFormatter不允许写入内存序列化后的二进制数据,所以此类已弃用,请使用其他方式代替,相互兼容")]
        internal class SerialByteBinder<T> : SerializationBinder
        {
            /// <summary>
            /// 实例
            /// </summary>
            public static SerializationBinder Instance { get; }
            /// <summary>
            /// 格式化
            /// </summary>
            public static BinaryFormatter Formatter { get; }
            /// <summary>
            /// 静态构造
            /// </summary>
            static SerialByteBinder()
            {
                Formatter = new BinaryFormatter();
                Formatter.Binder = Instance = new SerialByteBinder<T>();
            }
            /// <summary>
            /// 绑定到类型
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            /// <returns></returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                return typeof(T);
            }
        }

    }
}
