using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// IP地址
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 获取IPv4的值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Int32 GetIPv4Value(string ip)
        {
            string[] items = ip.Split(new char[] { '.' });
            return Int32.Parse(items[0]) << 24 | Int32.Parse(items[1]) << 16 | Int32.Parse(items[2]) << 8 | Int32.Parse(items[3]);
        }
        /// <summary>
        /// 获取IPv4的值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetIPv4Address(int ip)
        {
            return $"{(byte)(ip >> 24)}.{(byte)(ip >> 16)}.{(byte)(ip >> 8)}.{(byte)ip}";
        }
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// IP地址
    /// </summary>
    public static class IPCaller
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long GetValue(this IPAddress ip)
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
        public static IPAddress GetIPAddress(this Int64 ip)
        {
            return new IPAddress(ip);
        }
    }
}
