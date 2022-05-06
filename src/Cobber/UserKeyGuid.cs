using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// Guid字符类
    /// </summary>
    public class UserKeyGuid
    {
        /// <summary>
        /// 32进制数据
        /// </summary>
        public static readonly char[] Data32 = "0123456789ABCDEFGHJKMNPRSTUVWXYZ".ToCharArray();
        /// <summary>
        /// 64进制数据
        /// </summary>
        public static readonly char[] Data64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+-".ToCharArray();
        /// <summary>
        /// 初始构造函数
        /// </summary>
        public UserKeyGuid(bool is32 = true) : this(Guid.NewGuid(), is32) { }
        /// <summary>
        /// 初始构造函数
        /// </summary>
        public UserKeyGuid(long high, long low, bool is32 = true) : this(GetULong(high), GetULong(low), is32)
        {
        }
        /// <summary>
        /// 初始构造函数
        /// </summary>
        public UserKeyGuid(ulong high, ulong low, bool is32 = true)
        {
            UUID = GetGuid(high, low);
            HValue = high;
            LValue = low;
            if (is32)
            {
                HText = Get32String(high);
                LText = Get32String(low);
            }
            else
            {
                HText = Get64String(high);
                LText = Get64String(low);
            }
            Text = HText + LText;
        }
        /// <summary>
        /// 初始构造函数
        /// </summary>
        public UserKeyGuid(Guid dat, bool is32 = true) : this(dat.ToString("N"), is32)
        {
        }
        /// <summary>
        /// 初始构造函数
        /// </summary>
        public UserKeyGuid(string text, bool is32 = true)
        {
            if (text.Length == 26)
            {
                Text = text;
                HText = text.Substring(0, 13);
                LText = text.Substring(13, 13);
                HValue = Get32Value(HText);
                LValue = Get32Value(LText);
                UUID = GetGuid(HValue, LValue);
            }
            else if (text.Length == 22)
            {
                Text = text;
                HText = text.Substring(0, 11);
                LText = text.Substring(11, 11);
                HValue = Get64Value(HText);
                LValue = Get64Value(LText);
                UUID = GetGuid(HValue, LValue);
            }
            else
            {
                UUID = new Guid(text);
                var mString = UUID.ToString("N");
                if (is32)
                {
                    HValue = Get32Value(mString.Substring(0, 16));
                    LValue = Get32Value(mString.Substring(16, 16));
                    HText = Get32String(HValue);
                    LText = Get32String(LValue);
                }
                else
                {
                    HValue = Get64Value(mString.Substring(0, 16));
                    LValue = Get64Value(mString.Substring(16, 16));
                    HText = Get64String(HValue);
                    LText = Get64String(LValue);
                }
                Text = HText + LText;
            }
        }

        /// <summary>
        /// 源数据
        /// </summary>
        public Guid UUID { get; protected set; }
        /// <summary>
        /// 高位值
        /// </summary>
        public ulong HValue { get; protected set; }
        /// <summary>
        /// 高位简字串
        /// </summary>
        public string HText { get; protected set; }
        /// <summary>
        /// 低位值
        /// </summary>
        public ulong LValue { get; protected set; }
        /// <summary>
        /// 低位简字串
        /// </summary>
        public string LText { get; protected set; }

        /// <summary>
        /// 简字串
        /// </summary>
        public virtual string Text { get; protected set; }

        /// <summary>
        /// 返回64进制字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }
        #region // 辅助方法
        private static Guid GetGuid(ulong hvalue, ulong lvalue)
        {
            return new Guid(GetHexString(hvalue, 16) + GetHexString(lvalue, 16));
        }
        private static ulong Get64Value(string val)
        {
            return KeyGet64Value(val);
        }

        private static string Get64String(ulong val)
        {
            return KeyGet64String(val);
        }
        private static ulong Get32Value(string val)
        {
            return KeyGet32Value(val);
        }

        private static string Get32String(ulong val)
        {
            return KeyGet32String(val);
        }
        /// <summary>
        /// 获取一个时间日期的字符串
        /// </summary>
        /// <param name="is32"></param>
        /// <returns></returns>
        public static string GetGuidText(bool is32 = true)
        {
            return GetGuidText(Guid.NewGuid().ToString("N"), is32);
        }
        /// <summary>
        /// 转换Guid字符串到KeyGuid
        /// </summary>
        /// <param name="text"></param>
        /// <param name="is32"></param>
        /// <returns></returns>
        public static string GetGuidText(string text, bool is32)
        {
            string hText;
            string lText;
            if (is32)
            {
                var hValue = Get32Value(text.Substring(0, 16));
                var lValue = Get32Value(text.Substring(16, 16));
                hText = Get32String(hValue);
                lText = Get32String(lValue);
            }
            else
            {
                var hValue = Get64Value(text.Substring(0, 16));
                var lValue = Get64Value(text.Substring(16, 16));
                hText = Get64String(hValue);
                lText = Get64String(lValue);
            }
            return hText + lText;
        }
        /// <summary>
        /// 转换Guid到KeyGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="is32"></param>
        /// <returns></returns>
        public static string GetGuidText(Guid guid, bool is32)
        {
            var text = guid.ToString("N");
            return GetGuidText(text, is32);
        }

        /// <summary>
        /// 还原一个KeyGuid字符串到Guid
        /// </summary>
        /// <returns></returns>
        public static Guid GetGuid(string text, bool is32 = true)
        {
            Guid uuid;
            if (text.Length == 26)
            {
                var hText = text.Substring(0, 13);
                var lText = text.Substring(13, 13);
                var hValue = Get32Value(hText);
                var lValue = Get32Value(lText);
                uuid = GetGuid(hValue, lValue);
            }
            else if (text.Length == 22)
            {
                var hText = text.Substring(0, 11);
                var lText = text.Substring(11, 11);
                var hValue = Get64Value(hText);
                var lValue = Get64Value(lText);
                uuid = GetGuid(hValue, lValue);
            }
            else
            {
                uuid = Guid.Empty;
            }
            return uuid;
        }
        /// <summary>
        /// 还原一个KeyGuid字符串到Guid字符串
        /// </summary>
        /// <returns></returns>
        public static string GetGuidString(string text, bool is32 = true)
        {
            return GetGuid(text, is32).ToString("N");
        }
        #endregion
        #region // 外部方法
        /// <summary>
        /// 获取无符号值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ulong GetULong(long value)
        {
            return value >= 0 ? (ulong)value : (ulong)(-value) + long.MaxValue;
        }
        /// <summary>
        /// 获取十六进制字符串
        /// </summary>
        /// <param name="value">无符号长整型</param>
        /// <param name="len">补齐长度</param>
        /// <returns></returns>
        internal static string GetHexString(ulong value, int len)
        {
            var result = string.Format("{0:X}", value);
            if (len > 0) { return result.PadLeft(len, '0'); }
            return result;
        }

        /// <summary>
        /// 通过基本计算获取字符串
        /// </summary>
        /// <param name="val">计算数值</param>
        /// <param name="len">填充长度</param>
        /// <returns></returns>
        internal static string KeyGet32String(ulong val, int len = 13)
        {
            string result = "";
            if (val == 0) { result = "0"; }
            else
            {
                while (val >= 1)
                {
                    result = Data32[val - (val / 32) * 32] + result;
                    val = val / 32;
                }
            }
            return len <= 0 ? result : result.PadLeft(len, '0');
        }
        /// <summary>
        /// 通过基本计算获取字符串
        /// </summary>
        /// <param name="val">计算数值</param>
        /// <param name="len">填充长度</param>
        /// <returns></returns>
        internal static string KeyGet64String(ulong val, int len = 11)
        {
            string result = "";
            if (val == 0)
            {
                result = "0";
            }
            else
            {
                while (val >= 1)
                {
                    result = Data64[val - (val / 64) * 64] + result;
                    val = val / 64;
                }
            }
            return len <= 0 ? result : result.PadLeft(len, '0');
        }
        /// <summary>
        /// 获取计算得到的数值
        /// </summary>
        /// <param name="text">简字串</param>
        /// <returns></returns>
        internal static ulong KeyGet32Value(string text)
        {
            var bit = text.Length - 1;
            ulong result = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ulong val = Get32Num(text[i]);
                result += val * (ulong)Math.Pow(32, bit--);
            }
            return result;
        }
        /// <summary>
        /// 获取计算得到的数值
        /// </summary>
        /// <param name="text">简字串</param>
        /// <returns></returns>
        internal static ulong KeyGet64Value(string text)
        {
            var bit = text.Length - 1;
            ulong result = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ulong val = Get64Num(text[i]);
                result += val * (ulong)Math.Pow(64, bit--);
            }
            return result;
        }
        /// <summary>
        /// 获取字符串的序号
        /// </summary>
        /// <param name="x">获取字符</param>
        /// <returns></returns>
        internal static byte Get32Num(char x)
        {
            switch (x)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a':
                case 'A': return 10;
                case 'b':
                case 'B': return 11;
                case 'c':
                case 'C': return 12;
                case 'd':
                case 'D': return 13;
                case 'e':
                case 'E': return 14;
                case 'f':
                case 'F': return 15;
                case 'g':
                case 'G': return 16;
                case 'h':
                case 'H': return 17;
                case 'i':
                case 'I':
                case 'j':
                case 'J': return 18;
                case 'k': case 'K': return 19;
                case 'l':
                case 'L':
                case 'm':
                case 'M': return 20;
                case 'n': case 'N': return 21;
                case 'o':
                case 'O':
                case 'p':
                case 'P': return 22;
                case 'q':
                case 'Q':
                case 'r':
                case 'R': return 23;
                case 's':
                case 'S': return 24;
                case 't':
                case 'T': return 25;
                case 'u':
                case 'U': return 26;
                case 'v':
                case 'V': return 27;
                case 'w':
                case 'W': return 28;
                case 'x':
                case 'X': return 29;
                case 'y':
                case 'Y': return 30;
                case 'z':
                case 'Z': return 31;
                default: return 0;
            }
        }
        /// <summary>
        /// 获取字符串的序号
        /// </summary>
        /// <param name="x">获取字符</param>
        /// <returns></returns>
        internal static byte Get64Num(char x)
        {
            switch (x)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                case 'G': return 16;
                case 'H': return 17;
                case 'I': return 18;
                case 'J': return 19;
                case 'K': return 20;
                case 'L': return 21;
                case 'M': return 22;
                case 'N': return 23;
                case 'O': return 24;
                case 'P': return 25;
                case 'Q': return 26;
                case 'R': return 27;
                case 'S': return 28;
                case 'T': return 29;
                case 'U': return 30;
                case 'V': return 31;
                case 'W': return 32;
                case 'X': return 33;
                case 'Y': return 34;
                case 'Z': return 35;
                case 'a': return 36;
                case 'b': return 37;
                case 'c': return 38;
                case 'd': return 39;
                case 'e': return 40;
                case 'f': return 41;
                case 'g': return 42;
                case 'h': return 43;
                case 'i': return 44;
                case 'j': return 45;
                case 'k': return 46;
                case 'l': return 47;
                case 'm': return 48;
                case 'n': return 49;
                case 'o': return 50;
                case 'p': return 51;
                case 'q': return 52;
                case 'r': return 53;
                case 's': return 54;
                case 't': return 55;
                case 'u': return 56;
                case 'v': return 57;
                case 'w': return 58;
                case 'x': return 59;
                case 'y': return 60;
                case 'z': return 61;
                case '+': return 62;
                case '-': return 63;
                case '/': return 63;
                default: return 0;
            }
        }
        #endregion
    }
}
