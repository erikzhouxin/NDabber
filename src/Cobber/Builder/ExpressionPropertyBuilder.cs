using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 表达式属性创建者
    /// </summary>
    public static class ExpressionPropertyBuilder
    {
        /// <summary>
        /// 创建内容
        /// </summary>
        /// <returns></returns>
        public static StringBuilder BuilderContent(Type type)
        {
            return BuilderContent(new Type[] { type });
        }
        /// <summary>
        /// 创建内容
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static StringBuilder BuilderContent(params Type[] types)
        {
            if (types.IsEmpty()) { return new StringBuilder(); }
            var result = new StringBuilder();
            var usingList = new HashSet<string>()
            {
                "System","System.Linq.Expressions"
            };
            var typeBuilder = new StringBuilder();
            var block4 = "    ";
            var block8 = block4 + block4;
            foreach (var grouper in types.GroupBy(s => s.Namespace))
            {
                typeBuilder.AppendLine($"namespace {grouper.Key}").AppendLine("{");
                foreach (var type in grouper)
                {
                    var typeAttr = DbColAttribute.GetAttribute(type);
                    if (typeAttr.Ignore) { continue; }
                    var propInfos = type.GetProperties(Reflection.BindingFlags.Public | Reflection.BindingFlags.Instance);
                    typeBuilder
                        .AppendLine($"{block4}/// <summary>")
                        .AppendLine($"{block4}/// 表达式类:{typeAttr.Display}")
                        .AppendLine($"{block4}/// <see cref=\"{type.Name}\"/>")
                        .AppendLine($"{block4}/// </summary>")
                        .AppendLine($"{block4}public partial class Expression{type.Name}")
                        .AppendLine($"{block4}{{");
                    foreach (var item in propInfos)
                    {
                        var attr = DbColAttribute.GetAttribute(item);
                        if (attr.Ignore) { continue; }
                        if (!string.IsNullOrEmpty(item.PropertyType.Namespace))
                        {
                            usingList.Add(item.PropertyType.Namespace);
                        }
                        typeBuilder
                            .AppendLine($"{block8}/// <summary>")
                            .AppendLine($"{block8}/// {attr.Display}")
                            .AppendLine($"{block8}/// </summary>")
                            .AppendLine($"{block8}public static Expression<Func<{type.Name}, {item.PropertyType.Name}>> {item.Name} => m => m.{item.Name};");
                    }
                }
                typeBuilder.AppendLine($"{block4}}}").AppendLine("}");
            }
            foreach (var item in usingList.OrderBy(s => s))
            {
                result.AppendLine($"using {item};");
            }
            result
                .AppendLine()
                .AppendLine("#pragma warning disable")
                .Append(typeBuilder);
            return result;
        }

    }
}
