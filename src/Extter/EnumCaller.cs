using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 枚举调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 转换成枚举名称字符串
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this T enumValue) where T : Enum
        {
            return Enum.GetName(typeof(T), enumValue);
        }
    }
}

namespace System.Data.Extter
{
    /// <summary>
    /// 枚举调用
    /// </summary>
    public static class EnumCaller
    {
    }
}
