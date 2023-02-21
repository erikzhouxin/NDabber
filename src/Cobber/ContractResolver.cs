using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 小写转换
    /// </summary>
    public class LowercaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 转换属性名
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
    /// <summary>
    /// 大写转换
    /// </summary>
    public class UppercaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 转换属性名
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToUpper();
        }
    }
}
