using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// Sql脚本创建者
    /// </summary>
    public static class SqlScriptBuilder
    {
        /// <summary>
        /// 创建简单查询
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public static ISqlScriptFromBuilder CreateSimpleSelect(StoreType storeType)
        {
            return SqlScriptSimpleSelectBuilder.Create(storeType);
        }
    }
}
