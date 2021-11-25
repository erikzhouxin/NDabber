using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 静态调用扩展类
    /// </summary>
    public static partial class CobberCaller
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
