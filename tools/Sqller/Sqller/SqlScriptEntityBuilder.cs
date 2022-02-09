using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Sqller.Sqller
{
    /// <summary>
    /// SQL创建实体
    /// </summary>
    public interface ISqlScriptEntityBuilder
    {
        /// <summary>
        /// 获取实体代码
        /// </summary>
        /// <returns></returns>
        StringBuilder GetEntityCodes();
    }
    /// <summary>
    /// SQL脚本实体创建者
    /// </summary>
    internal class SqlScriptEntityBuilder
    {
    }
}
