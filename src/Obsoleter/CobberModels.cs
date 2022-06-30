using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 自动Microsoft SQL Server创建者
    /// 依赖于[DbMarkable/DbAutoable]
    /// 推荐使用[AutoSqlServerBuilder]
    /// </summary>
    [Obsolete("替代方案:AutoSqlServerBuilder")]
    public class AutoMssqlBuilder
    {
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AutoSqlBuilder Builder(Type type) => AutoSqlServerBuilder.EntitySqlDic.GetOrAdd(type, (k) => AutoSqlServerBuilder.CreateSqlModel(k));
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AutoSqlBuilder Builder<T>() => AutoSqlServerBuilder.EntitySqlDic.GetOrAdd(typeof(T), (k) => AutoSqlServerBuilder.CreateSqlModel(k));
    }
    /// <summary>
    /// 枚举显示泛型类
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Obsolete("替代方案:NEnumerable<TEnum>")]
    public class EDisplayAttr<TEnum> : NEnumerable<TEnum>
        where TEnum : struct, Enum
    {
        private EDisplayAttr() : base(default(TEnum)) { }
    }
    public partial class UserPassword
    {
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        [Obsolete("替代方案:UserCrypto.GetHexString")]
        public static string GetHexString(byte[] hashData) => UserCrypto.GetHexString(hashData, DefaultCase);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <param name="isLower">是小写</param>
        /// <returns>16进制字符串(大写字母)</returns>
        [Obsolete("替代方案:UserCrypto.GetHexString")]
        public static string GetHexString(byte[] hashData, bool isLower) => UserCrypto.GetHexString(hashData, isLower);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        [Obsolete("替代方案:UserCrypto.GetHexString")]
        public static string GetByte16String(byte[] hashData) => UserCrypto.GetHexString(hashData, false);
        /// <summary>
        /// 原码字符
        /// </summary>
        [Obsolete("替代方案:Origin")]
        public string OPass { get => Origin; }
        /// <summary>
        /// 加密字符
        /// </summary>
        [Obsolete("替代方案:Hash")]
        public string HPass { get => Hash; }
    }
}
