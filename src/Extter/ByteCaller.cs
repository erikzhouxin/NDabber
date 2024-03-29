﻿using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 字节编码
    /// </summary>
    public static partial class CobberCaller
    {
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
        /// <summary>
        /// 比较两个字节数组是否相同内容
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="tag"></param>
        /// <param name="isNull">同时为NULL时是否相等,默认相等</param>
        /// <returns></returns>
        public static bool EqualBytes(this byte[] curr, byte[] tag, bool isNull = true)
        {
            if (curr == null && tag == null) { return isNull; }
            if (curr == null || tag == null) { return false; }
            if (curr.Length != tag.Length) { return false; }
            for (int i = 0; i < curr.Length; i++)
            {
                if (curr[i] != tag[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 字节调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GetMd5(this byte[] bytes)
        {
            return MD5.Create().ComputeHash(bytes);
        }
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetMd5String(this byte[] bytes)
        {
            return GetHexString(GetMd5(bytes));
        }
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetHexString(this byte[] hashData) => GetHexString(hashData, false);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <param name="isLower">是小写</param>
        /// <returns>16进制字符串</returns>
        public static string GetHexString(this byte[] hashData, bool isLower)
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
        public static byte[] GetHexByte(this string hexString)
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
        public static byte[] GetHexBytes(this string hexString)
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
        public static byte[] GetHexBytes(this string hexString, char fix, char replace = ' ')
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
        public static byte[] GetHexBytes(this string hexString, char fix = ' ', string replace = " ")
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
        public static byte[] GetHexBytes(this string hexString, char fix, params char[] replaces)
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
        unsafe public static Int16 GetInt16(this byte[] bytes, bool isBigEndian = true)
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
        public static Int16 ReadInt16(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            return (Int16)((bytes[0] << 8) + bytes[1]);
        }
        /// <summary>
        /// 读无符号短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt16 GetUInt16(this byte[] bytes, bool isBigEndian = true)
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
        public static UInt16 ReadUInt16(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return bytes[0]; }
            return (UInt16)((bytes[0] << 8) + bytes[1]);
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int32 GetInt32(this byte[] bytes, bool isBigEndian = true)
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
        /// <returns></returns>
        unsafe public static UInt32 GetUInt32(this byte[] bytes, bool isBigEndian = true)
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
        /// <returns></returns>
        unsafe public static Int64 GetInt64(this byte[] bytes, bool isBigEndian = true)
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
        /// <returns></returns>
        unsafe public static UInt64 GetUInt64(this byte[] bytes, bool isBigEndian = true)
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
        /// <returns></returns>
        unsafe public static Int16 GetInt16(this sbyte[] bytes, bool isBigEndian = true)
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
        public static Int16 ReadInt16(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (Int16)((byte)bytes[0]); }
            return (Int16)((((byte)bytes[0]) << 8) + ((byte)bytes[1]));
        }
        /// <summary>
        /// 读无符号短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt16 GetUInt16(this sbyte[] bytes, bool isBigEndian = true)
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
        public static UInt16 ReadUInt16(this sbyte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) { return 0; }
            if (bytes.Length == 1) { return (UInt16)((byte)bytes[0]); }
            return (UInt16)((((byte)bytes[0]) << 8) + ((byte)bytes[1]));
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int32 GetInt32(this sbyte[] bytes, bool isBigEndian = true)
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
        /// <returns></returns>
        unsafe public static UInt32 GetUInt32(this sbyte[] bytes, bool isBigEndian = true)
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
        /// <returns></returns>
        unsafe public static Int64 GetInt64(this sbyte[] bytes, bool isBigEndian = true)
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
        /// 读无符号长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt64 GetUInt64(this sbyte[] bytes, bool isBigEndian = true)
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
        #endregion
        #region // 压缩
        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] data)
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
        public static byte[] Decompress(this byte[] data)
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
        public static String GetGB2312String(this byte[] value) => _gb2312Encoding.Value.GetString(value);
        /// <summary>
        /// 转换GBK成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetGB2312Bytes(this string content) => _gb2312Encoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GB2312进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetGB2312HexString(this string content, bool isLower = false) => GetGB2312Bytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的GB2312字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexGB2312String(this string hexString) => GetGB2312String(GetHexByte(hexString));
        /// <summary>
        /// 获取GBK编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetGBKString(this byte[] value) => _gbkEncoding.Value.GetString(value);
        /// <summary>
        /// 转换GBK成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetGBKBytes(this string content) => _gbkEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GBK16进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetGBKHexString(this string content, bool isLower = false) => GetGBKBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的GBK字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexGBKString(this string hexString) => GetGBKString(GetHexByte(hexString));
        /// <summary>
        /// 获取Utf8编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetUTF8String(this byte[] value) => _utf8Encoding.Value.GetString(value);
        /// <summary>
        /// 转换UTF8成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetUTF8Bytes(this string content) => _utf8Encoding.Value.GetBytes(content);
        /// <summary>
        /// 获取UTF8进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetUTF8HexString(this string content, bool isLower = false) => GetUTF8Bytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的UTF8字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexUTF8String(this string hexString) => GetUTF8String(GetHexByte(hexString));
        /// <summary>
        /// 获取Unicode编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetUnicodeString(this byte[] value) => _unicodeEncoding.Value.GetString(value);
        /// <summary>
        /// 转换Unicode成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetUnicodeBytes(this string content) => _unicodeEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取Unicode进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetUnicodeHexString(this string content, bool isLower = false) => GetUnicodeBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的Unicode字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexUnicodeString(this string hexString) => GetUnicodeString(GetHexByte(hexString));
        /// <summary>
        /// 获取编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetEncodingString(this byte[] value) => _utf8Encoding.Value.GetString(value);
        /// <summary>
        /// 获取编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String GetEncodingString(this byte[] value, Encoding encoding) => encoding.GetString(value);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytes(this string content) => CobberCaller.GetBytes(content, _utf8Encoding.Value);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytes(this string content, Encoding encoding) => CobberCaller.GetBytes(content, encoding);
        /// <summary>
        /// 获取Encoding16进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetEncodingHexString(this string content, Encoding encoding, bool isLower = false) => GetEncodingBytes(content, encoding).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串Encoding字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String GetHexEncodingString(this string hexString, Encoding encoding) => GetEncodingString(GetHexByte(hexString), encoding);
        #endregion
    }
}
