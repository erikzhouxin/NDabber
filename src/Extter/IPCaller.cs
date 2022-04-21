using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// IP地址
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 取本机主机ip
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetCurrent()
        {
            try
            {
                IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in ipEntry.AddressList)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip;
                    }
                }
                foreach (var ip in ipEntry.AddressList)
                {
                    //从IP地址列表中筛选出IPv6类型的IP地址
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return ip;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return IPAddress.Parse("127.0.0.1");
        }
        /// <summary>
        /// 取本机主机ip
        /// </summary>
        /// <returns></returns>
        public static IPAddress[] GetList()
        {
            try
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return new IPAddress[] { IPAddress.Parse("127.0.0.1") };
        }
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
    public static partial class ExtterCaller
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
