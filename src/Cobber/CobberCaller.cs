using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// 静态调用扩展类
    /// </summary>
    public static class CobberCaller
    {
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataTable(this DbDataReader sdr, string tableName = "Result")
        {
            var table = new DataTable(tableName ?? "Result");
            for (int i = 0; i < sdr.FieldCount; i++)
            {
                var name = sdr.GetName(i);
                table.Columns.Add(name);
            }
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    var row = table.NewRow();
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        var val = sdr.GetValue(i);
                        //if (val.Equals(DBNull.Value))
                        //{
                        //    val = null;
                        //}
                        row[i] = val;
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }
        /// <summary>
        /// 包含忽略属性
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasIgnoreAttribute(this PropertyInfo prop, bool inherit = false)
        {
            foreach (var item in prop.GetCustomAttributes(inherit))
            {
                if (item.GetType().Name == "NotMappedAttribute") { return true; }
            }
            return false;
        }
#if NET40
        /// <summary>
        /// 获取用户属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        internal static T GetCustomAttribute<T>(this PropertyInfo prop, bool inherit = false) where T : Attribute
        {
            foreach (var item in prop.GetCustomAttributes(inherit))
            {
                if (item is T) { return item as T; }
            }
            return default(T);
        }
        /// <summary>
        /// 获取用户属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        internal static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            foreach (var item in type.GetCustomAttributes(inherit))
            {
                if (item is T) { return item as T; }
            }
            return default(T);
        }
#endif
        /// <summary>
        /// 是主键
        /// </summary>
        /// <param name="ixt"></param>
        /// <returns></returns>
        public static bool IsPK(this DbIxType ixt)
        {
            switch (ixt)
            {
                case DbIxType.PK:
                case DbIxType.APK:
                case DbIxType.PFK:
                    return true;
                case DbIxType.FK:
                case DbIxType.IX:
                case DbIxType.UIX:
                default:
                    return false;
            }
        }
    }
}
