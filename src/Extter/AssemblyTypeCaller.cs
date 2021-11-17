using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 程序集类型调用
    /// </summary>
    public static class AssemblyTypeCaller
    {
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static Type[] GetTypes(this Type type, bool isExport = true)
        {
            return isExport ? type.Assembly.GetExportedTypes() : type.Assembly.GetTypes();
        }
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nSpace"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(this Type type, string nSpace, bool isExport = true)
        {
            var types = isExport ? type.Assembly.GetExportedTypes() : type.Assembly.GetTypes();
            if (string.IsNullOrEmpty(nSpace)) { return types; }
            return types.Where(s => nSpace.Equals(s.Namespace, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetNamespaceTypes(this Type type, bool isExport = true)
        {
            var types = isExport ? type.Assembly.GetExportedTypes() : type.Assembly.GetTypes();
            return types.Where(s => s.Namespace == type.Namespace);
        }
    }
}
