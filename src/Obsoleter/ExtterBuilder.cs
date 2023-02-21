using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    public static partial class ExtterBuilder
    {
        /// <summary>
        /// 创建内容
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        [Obsolete("替代方案:BuildExpressionClassContent")]
        public static StringBuilder BuilderContent(params Type[] types) => BuildExpressionClassContent(types);
    }
}
