using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Dabber;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// Java枚举接口
    /// </summary>
    [Obsolete("替代方案:INEnumerable")]
    public interface IJavaEnum<TEnum> : INEnumerable<TEnum> where TEnum : struct, Enum { }
    /// <summary>
    /// Java枚举接口
    /// </summary>
    [Obsolete("替代方案:INEnumerable")]
    public interface IJavaEnum : INEnumerable { }
    /// <summary>
    /// 抽象Java枚举
    /// </summary>
    [Obsolete("替代方案:NEnumerable")]
    public struct JavaEnum : IJavaEnum
    {
        /// <summary>
        /// 枚举值
        /// </summary>
        public int Value { get; }
        /// <summary>
        /// 枚举名
        /// </summary>
        public string EnumName { get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }

        object INEnumerable.Enum => throw new NotImplementedException();

        DescriptionAttribute INEnumerable.Description => throw new NotImplementedException();

        DisplayNameAttribute INEnumerable.DisplayName => throw new NotImplementedException();

        EDisplayAttribute INEnumerable.EDisplay => throw new NotImplementedException();

        DisplayAttribute INEnumerable.Display => throw new NotImplementedException();

        FieldInfo INEnumerable.Field => throw new NotImplementedException();

        /// <summary>
        /// 构造
        /// </summary>
        private JavaEnum(int value, string name, string enumName)
        {
            Value = value;
            Name = name;
            EnumName = enumName;
        }
        /// <summary>
        /// 获取枚举字典
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDictionary<int, KeyValuePair<string, string>> GetEnumDictionary(Type type)
        {
            var result = new SortedDictionary<int, KeyValuePair<string, string>>();
            if (!type.IsEnum) { return result; }
            var vals = Enum.GetValues(type).ToArray();
            var names = Enum.GetNames(type);
            for (int i = 0; i < vals.Length; i++)
            {
                var val = vals[i];
                var name = names[i];
                var display = name;
                var attrs = type.GetField(name).GetCustomAttributes(false);
                if (attrs.IsEmpty()) { continue; }
                foreach (var attr in attrs)
                {
                    if (attr is EDisplayAttribute disp)
                    {
                        display = disp.Display;
                        break;
                    }
                    if (attr is DisplayAttribute displ)
                    {
                        display = displ.Name ?? displ.Description;
                        break;
                    }
                    if (attr is DescriptionAttribute description)
                    {
                        display = description.Description;
                        break;
                    }
                }
                result.Add(val, new KeyValuePair<string, string>(name, display));
            }
            return result;
        }
        /// <summary>
        /// 获取枚举类显示名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClassDisplay(Type type)
        {
            var attrs = type.GetCustomAttributes(false);
            string display = type.Name;
            foreach (var item in attrs)
            {
                if (item is EDisplayAttribute disp)
                {
                    display = disp.Display;
                    break;
                }
                if (item is DescriptionAttribute description)
                {
                    display = description.Description;
                    break;
                }
            }
            return display;
        }
        /// <summary>
        /// 是逻辑枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFlags(Type type)
        {
            if (!type.IsEnum) { return false; }
            var attrs = type.GetCustomAttributes(inherit: false);
            foreach (var item in attrs)
            {
                if (item is FlagsAttribute)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取Java枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<JavaEnum> GetJavaEnum(Type type)
        {
            var result = new List<JavaEnum>();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in props)
            {
                try
                {
                    if (prop.GetValue(null, null) is JavaEnum propModel)
                    {
                        result.Add(propModel);
                    }
                }
                catch { continue; }
            }
            return result;
        }
        /// <summary>
        /// 创建内容
        /// </summary>
        /// <returns></returns>
        public static StringBuilder BuilderContent(Type type)
        {
            var sb = GetUsingContent();
            sb.AppendLine($"namespace {type.Namespace}").AppendLine("{");
            sb.Append(GetClassContent(type));
            sb.AppendLine("}");
            return sb;
        }
        /// <summary>
        /// 创建内容
        /// </summary>
        /// <returns></returns>
        public static StringBuilder BuildSelfContent(Type type)
        {
            var sb = GetUsingContent();
            sb.AppendLine($"namespace {type.Namespace}").AppendLine("{");
            sb.Append(GetEnumContent(type));
            sb.Append(GetClassContent(type));
            sb.AppendLine("}");
            return sb;
        }
        /// <summary>
        /// 创建内容
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static StringBuilder BuilderContent(params Type[] types)
        {
            if (types.IsEmpty()) { return new StringBuilder(); }
            var sb = GetUsingContent();
            foreach (var item in types.GroupBy(s => s.Namespace))
            {
                sb.AppendLine($"namespace {item.Key}").AppendLine("{");
                foreach (Type type in item)
                {
                    sb.Append(GetClassContent(type));
                }
                sb.AppendLine("}");
            }
            return sb;
        }
        private static StringBuilder GetClassContent(Type type)
        {
            var className = $"JEnum{type.Name}";
            var typeDescr = JavaEnum.GetClassDisplay(type);
            var block4 = "    ";
            var preblock = block4;
            var sb = new StringBuilder()
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// {typeDescr}")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public partial struct {className} : IJavaEnum")
              .AppendLine($"{preblock}{{");
            var props = JavaEnum.GetEnumDictionary(type);
            preblock += block4;
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}/// <summary>")
                    .AppendLine($"{preblock}/// {item.Value.Value}")
                    .AppendLine($"{preblock}/// </summary>")
                    .AppendLine($"{preblock}public static {className} {item.Value.Key} {{ get; }} = new {className}({item.Key}, \"{item.Value.Value.Replace("\"", "\\\"")}\", nameof({item.Value.Key}));");
            }
            var first = props.First();
            var used = props.Skip(1);
            sb.AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 所有项列表")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static ReadOnlyCollection<{className}> AllItems {{ get; }} = new ReadOnlyCollection<{className}>(new List<{className}>")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    {string.Join(", ", props.Select(s => s.Value.Key))}")
              .AppendLine($"{preblock}}});")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 项列表")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static ReadOnlyCollection<{className}> Items {{ get; }} = new ReadOnlyCollection<{className}>(new List<{className}>")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    {string.Join(", ", used.Select(s => s.Value.Key))}")
              .AppendLine($"{preblock}}});")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 值列表")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static ReadOnlyCollection<Int32> Values {{ get; }} = new ReadOnlyCollection<Int32>(new List<Int32>")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    {string.Join(", ", used.Select(s => s.Value.Key + ".Value"))}")
              .AppendLine($"{preblock}}});")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 名称列表")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static ReadOnlyCollection<String> Names {{ get; }} = new ReadOnlyCollection<String>(new List<String>")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    {string.Join(", ", used.Select(s => s.Value.Key + ".Name"))}")
              .AppendLine($"{preblock}}});")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 枚举列表")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static ReadOnlyCollection<String> EnumNames {{ get; }} = new ReadOnlyCollection<String>(new List<String>")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    {string.Join(", ", used.Select(s => s.Value.Key + ".EnumName"))}")
              .AppendLine($"{preblock}}});")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 枚举值列表")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static ReadOnlyCollection<{type.Name}> Enums {{ get; }} = new ReadOnlyCollection<{type.Name}>(new List<{type.Name}>")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    {string.Join(", ", used.Select(s => type.Name + "." + s.Value.Key))}")
              .AppendLine($"{preblock}}});")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 值")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public Int32 Value {{ get; }}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 枚举值")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public {type.Name} Enum {{ get; }}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 枚举名")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public String EnumName {{ get; }}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 名")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public String Name {{ get; }}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 构造")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}private {className}(int value, string name, [CallerMemberName] string propName = null)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    Value = value;")
              .AppendLine($"{preblock}    Enum = ({type.Name})value;")
              .AppendLine($"{preblock}    Name = name;")
              .AppendLine($"{preblock}    EnumName = propName;")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成字符串")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}/// <returns></returns>")
              .AppendLine($"{preblock}public override string ToString()")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    return Name;")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成枚举字符串")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}/// <returns></returns>")
              .AppendLine($"{preblock}public string ToEnumString()")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    return EnumName;")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成类型")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator {className}(int value)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    switch (value)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}        case {item.Key}: return {item.Value.Key};");
            }
            sb.AppendLine($"{preblock}        default: return {first.Value.Key};")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成类型")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator {className}(int? value)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    if (!value.HasValue) {{ return {first.Value.Key}; }}")
              .AppendLine($"{preblock}    switch (value)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}        case {item.Key}: return {item.Value.Key};");
            }
            sb.AppendLine($"{preblock}        default: return {first.Value.Key};")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成整型")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator int({className} type) => type.Value;")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成类型")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator {className}(string name)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    switch (name)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                if (item.Key.ToString() != item.Value.Key)
                {
                    sb.AppendLine($"{preblock}        case \"{item.Key.ToString()}\":");
                }
                if (item.Value.Key.ToLower() != item.Value.Key)
                {
                    sb.AppendLine($"{preblock}        case \"{item.Value.Key.ToLower().Replace("\"", "\\\"")}\":");
                }
                if (item.Value.Key.ToUpper() != item.Value.Key)
                {
                    sb.AppendLine($"{preblock}        case \"{item.Value.Key.ToUpper().Replace("\"", "\\\"")}\":");
                }
                sb.AppendLine($"{preblock}        case \"{item.Value.Key}\": return {item.Value.Key};");
            }
            sb.AppendLine($"{preblock}        default: break;")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}    foreach (var item in Items)")
              .AppendLine($"{preblock}    {{")
              .AppendLine($"{preblock}        if (item.Name == name || item.EnumName.Equals(name, StringComparison.OrdinalIgnoreCase)) {{ return item; }}")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}    return {first.Value.Key};")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成类型")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator {className}({type.Name} value)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    switch (value)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}        case {type.Name}.{item.Value.Key}: return {item.Value.Key};");
            }
            sb.AppendLine($"{preblock}        default: return {first.Value.Key};")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成枚举")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator {type.Name}({className} type) => type.Enum;")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 转换成字符串")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static implicit operator string({className} type) => type.Name;")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 包含名称")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static bool Contains(string name) => Items.Any(i => i.Name == name || i.EnumName.Equals(name, StringComparison.OrdinalIgnoreCase));")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 包含值")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static bool Contains(int value) => Values.Contains(value);")
              .AppendLine($"{block4}}}");
            preblock = block4;
            sb.AppendLine($"{preblock}/*")
              .AppendLine($"{preblock}// {typeDescr}")
              .AppendLine($"{preblock}public enum {type.Name}{{");
            preblock += block4;
            sb.AppendLine(preblock + string.Join(",\r\n    	", props.Select(s => s.Value.Key + "(" + s.Key + ",\"" + s.Value.Value + "\")")) + ";")
              .AppendLine()
              .AppendLine($"{preblock}private String name;")
              .AppendLine($"{preblock}private int index;")
              .AppendLine()
              .AppendLine($"{preblock}private {type.Name}(int index,String name) {{")
              .AppendLine($"{preblock}    this.index = index;")
              .AppendLine($"{preblock}    this.name = name;")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}public String getName() {{")
              .AppendLine($"{preblock}    return name;")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}public int getIndex() {{")
              .AppendLine($"{preblock}    return index;")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}public static String getNameByIndex(int index) {{")
              .AppendLine($"{preblock}    for({type.Name} item : {type.Name}.values()) {{")
              .AppendLine($"{preblock}        if(item.index == index){{")
              .AppendLine($"{preblock}            return item.name;")
              .AppendLine($"{preblock}        }}")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}    return \"\";")
              .AppendLine($"{preblock}}}");
            preblock = block4;
            sb.AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}*/")
              .AppendLine($"{preblock}public static partial class {className}Caller")
              .AppendLine($"{preblock}{{");
            preblock += block4;
            sb.AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 获取名称")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static string GetName(this {type.Name} value)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    switch (value)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}        case {type.Name}.{item.Value.Key}: return {className}.{item.Value.Key}.Name;");
            }
            sb.AppendLine($"{preblock}        default: return {className}.{first.Value.Key}.Name;")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 获取枚举定义名称")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static string GetEnumName(this {type.Name} value)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    switch (value)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}        case {type.Name}.{item.Value.Key}: return nameof({type.Name}.{item.Value.Key});");
            }
            sb.AppendLine($"{preblock}        default: return nameof({type.Name}.{first.Value.Key});")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// 获取枚举")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}public static {type.Name} Get{type.Name}Enum(string value)")
              .AppendLine($"{preblock}{{")
              .AppendLine($"{preblock}    switch (value)")
              .AppendLine($"{preblock}    {{");
            foreach (var item in props)
            {
                if (item.Key.ToString() != item.Value.Key)
                {
                    sb.AppendLine($"{preblock}        case \"{item.Key}\":");
                }
                if (item.Value.Key.ToLower() != item.Value.Key)
                {
                    sb.AppendLine($"{preblock}        case \"{item.Value.Key.ToLower().Replace("\"", "\\\"")}\":");
                }
                if (item.Value.Key.ToUpper() != item.Value.Key)
                {
                    sb.AppendLine($"{preblock}        case \"{item.Value.Key.ToUpper().Replace("\"", "\\\"")}\":");
                }
                sb.AppendLine($"{preblock}        case nameof({type.Name}.{item.Value.Key}): return {type.Name}.{item.Value.Key};");
            }
            sb.AppendLine($"{preblock}        default: return {type.Name}.{first.Value.Key};")
              .AppendLine($"{preblock}    }}")
              .AppendLine($"{preblock}}}")
              .AppendLine($"{block4}}}");
            return sb;
        }
        private static StringBuilder GetEnumContent(Type type)
        {
            var className = type.Name;
            var typeDescr = JavaEnum.GetClassDisplay(type);
            var block4 = "    ";
            var preblock = block4;
            var sb = new StringBuilder()
              .AppendLine($"{preblock}/// <summary>")
              .AppendLine($"{preblock}/// {typeDescr}")
              .AppendLine($"{preblock}/// </summary>")
              .AppendLine($"{preblock}[EDisplay(\"{typeDescr.Replace("\"", "\\\"")}\")]")
              .AppendLine($"{preblock}public enum {className}")
              .AppendLine($"{preblock}{{");
            var props = JavaEnum.GetEnumDictionary(type);
            preblock += block4;
            foreach (var item in props)
            {
                sb.AppendLine($"{preblock}/// <summary>")
                    .AppendLine($"{preblock}/// {item.Value.Value}")
                    .AppendLine($"{preblock}/// </summary>")
                    .AppendLine($"{preblock}[EDisplay(\"{item.Value.Value.Replace("\"", "\\\"")}\")]")
                    .AppendLine($"{preblock}{item.Value.Key} = {item.Key},");
            }
            sb.AppendLine($"{block4}}}");
            return sb;
        }
        private static StringBuilder GetUsingContent()
        {
            return new StringBuilder()
                .AppendLine("using System;")
                .AppendLine("using System.Collections.Generic;")
                .AppendLine("using System.Collections.ObjectModel;")
                .AppendLine("using System.Data.Cobber;")
                .AppendLine("using System.Linq;")
                .AppendLine("using System.Runtime.CompilerServices;")
                .AppendLine("using System.Text;")
                .AppendLine()
                .AppendLine("#pragma warning disable");
        }
    }
}
