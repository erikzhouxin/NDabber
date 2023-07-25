using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Hopper
{
#pragma warning disable CS1570 // XML 注释出现 XML 格式错误
    /// <summary>
    /// 压缩配置式方式(最新兼容式)
    /// </summary>
    public class HopperUrlModel : HopperUrlModelV1
    {
        /// <summary>
        /// 构造
        /// </summary>
        public HopperUrlModel() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="url"></param>
        public HopperUrlModel(String url) : base(url) { }
    }

    /// <summary>
    /// 压缩配置式方式(V1版本)有限的URL格式
    /// 格式:[protocal]?[host][port]?[path]?[args]?[anchor]
    /// 示例: socket://localhost:8080/api/test?id=12&status=3#tag
    /// 解析:
    /// protocol =>   socket
    /// host     =>   localhost
    /// port     =>   8080
    /// path     =>   /api/test
    /// args     =>   id=12&status=3
    /// anchor   =>   tag
    /// 注意:
    /// 1.斜杠两种都支持
    /// 2.参数值最好不带特殊字符或者进行自行编码(不可预料)
    /// </summary>
    public class HopperUrlModelV1
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        public static Regex DefaultRegex = new Regex("^((?<protocal>\\w+):((//)|(\\\\)))?(?<host>((((\\w)+)\\.?)+))(:(?<port>\\d+))?(?<path>(((\\)|(/))?(\\w*))*))(\\?(?<args>((&?(([\\w]+)=([^&#]*))?)*)))?(#(?<anchor>([\\w\\W]*)))?$");
        private static Dictionary<string, HopperUrlModelV1> ServerDic = new();
        /// <summary>
        /// 正则表达式字符串
        /// </summary>
        public virtual String Pattern { get => Regex.ToString(); }
        /// <summary>
        /// 正则表达式
        /// </summary>
        public virtual Regex Regex { get => DefaultRegex; }
        /// <summary>
        /// 协议
        /// </summary>
        public virtual String Protocal { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public virtual String Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public virtual Int32 Port { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public virtual String Path { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public virtual String Args { get; set; }
        /// <summary>
        /// 参数字典
        /// </summary>
        public virtual Dictionary<string, string> ArgsDic { get; set; }
        /// <summary>
        /// 锚
        /// </summary>
        public virtual String Anchor { get; set; }
        /// <summary>
        /// 参数字典
        /// </summary>
        public virtual Dictionary<string, string> AnchorDic { get; set; }
        /// <summary>
        /// 参数字典
        /// </summary>
        public virtual Dictionary<string, string> Dic { get; set; }
        /// <summary>
        /// 是否匹配
        /// </summary>
        public virtual bool IsMatch { get; set; }
        /// <summary>
        /// 原始字符串
        /// </summary>
        public virtual String Origin { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        public HopperUrlModelV1()
        {
            IsMatch = false;
            ArgsDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            AnchorDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="url"></param>
        public HopperUrlModelV1(String url) : this()
        {
            Origin = url ?? string.Empty;
            Set(url);
        }
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual HopperUrlModelV1 Set(string url) => Set(Convert(url));
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual HopperUrlModelV1 Set(HopperUrlModelV1 model)
        {
            this.Anchor = model.Anchor;
            this.AnchorDic = model.AnchorDic;
            this.Host = model.Host;
            this.Args = model.Args;
            this.ArgsDic = model.ArgsDic;
            this.Dic = model.Dic;
            this.IsMatch = model.IsMatch;
            this.Origin = model.Origin;
            this.Path = model.Path;
            this.Port = model.Port;
            this.Protocal = model.Protocal;
            return this;
        }
        /// <summary>
        /// 当字符串为null或为空时返回defVal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual string Get(string property, string defVal)
        {
            if (Dic.TryGetValue(property, out var val) && !string.IsNullOrEmpty(val))
            { return val; }
            return defVal;
        }
        /// <summary>
        /// 当字符串为null或为空时返回defVal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T Get<T>(string property, T defVal)
        {
            if (Dic.TryGetValue(property, out var val) && !string.IsNullOrEmpty(val))
            { return (T)TestTry.Try(System.Convert.ChangeType, val, typeof(T), defVal); }
            return defVal;
        }
        /// <summary>
        /// 当字符串为null或为空时返回defVal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T[] GetArray<T>(string property, T[] defVal)
        {
            if (Dic.TryGetValue(property, out var val) && !string.IsNullOrEmpty(val))
            { return ConvertArray<T>(val, defVal); }
            return defVal;
        }
        /// <summary>
        /// 获取Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T GetJson<T>(string property, T defVal)
        {
            return Get<T>(property, CobberCaller.GetJsonObject<T>, defVal);
        }
        /// <summary>
        /// 获取Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T GetJson<T>(string property, Func<Exception, T> defVal)
        {
            return Get<T>(property, CobberCaller.GetJsonObject<T>, defVal);
        }
        /// <summary>
        /// 获取Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="settings"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T GetJson<T>(string property, JsonSerializerSettings settings, T defVal)
        {
            return Get<T>(property, (s) => CobberCaller.GetJsonObject<T>(s, settings), defVal);
        }
        /// <summary>
        /// 获取Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="settings"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T GetJson<T>(string property, JsonSerializerSettings settings, Func<Exception, T> defVal)
        {
            return Get<T>(property, (s) => CobberCaller.GetJsonObject<T>(s, settings), defVal);
        }
        /// <summary>
        /// 转换字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T[] ConvertArray<T>(string val, T[] defVal)
        {
            if (string.IsNullOrEmpty(val)) { return defVal; }
            if (val.StartsWith("[")) { return CobberCaller.TryGetJsonObject<T[]>(val, defVal); }
            var splits = val.Split(new char[] { ',', '|', '，', ' ', });
            var resa = new T[splits.Length];
            for (int i = 0; i < splits.Length; i++)
            {
                resa[i] = (T)TestTry.Try(System.Convert.ChangeType, splits[i], typeof(T), default(T));
            }
            return resa;
        }
        /// <summary>
        /// 当字符串为null或为空时返回defVal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T Get<T>(string property, Func<string, T> converter, T defVal)
        {
            Dic.TryGetValue(property, out var val);
            return TestTry.Try(converter, val, (ex) => defVal);
        }
        /// <summary>
        /// 当字符串为null或为空时返回defVal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public virtual T Get<T>(string property, Func<string, T> converter, Func<Exception, T> defVal)
        {
            Dic.TryGetValue(property, out var val);
            return TestTry.Try(converter, val, defVal);
        }
        /// <summary>
        /// 当字符串为null或为空时返回defVal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public virtual T Get<T>(string property, Func<string, T> converter)
        {
            Dic.TryGetValue(property, out var val);
            return TestTry.Try(converter, val);
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HopperUrlModelV1 Convert(string url)
        {
            var res = new HopperUrlModelV1();
            if (string.IsNullOrWhiteSpace(url)) { return res; }
            if (ServerDic.ContainsKey(url)) { return ServerDic[url].Clone(); }
            var match = DefaultRegex.Match(url);
            if (!match.Success)
            {
                res.IsMatch = false;
                return res;
            }
            res.IsMatch = true;
            res.Dic[nameof(Protocal)] = res.Protocal = GetDefaultString(match.Groups["protocal"].Value, string.Empty);
            res.Dic[nameof(Host)] = res.Host = GetDefaultString(match.Groups["host"].Value, string.Empty);
            res.Port = (res.Dic[nameof(Port)] = GetDefaultString(match.Groups["port"].Value, "80")).ToPInt32(80);
            res.Dic[nameof(Path)] = res.Path = GetDefaultString(match.Groups["path"].Value, string.Empty).Trim('\\').Trim('/');
            res.Dic[nameof(Anchor)] = res.Anchor = GetDefaultString(match.Groups["anchor"].Value, string.Empty);
            res.Dic[nameof(Args)] = res.Args = GetDefaultString(match.Groups["args"].Value, string.Empty);
            var spliter = res.Args.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in spliter)
            {
                var ix = item.IndexOf('=');
                if (ix <= 0) { continue; }
                var property = item.Substring(0, ix);
                var value = string.Empty;
                if (ix + 1 < item.Length) { value = GetUrlArgs(item.Substring(ix + 1)); }
                res.ArgsDic[property] = value;
                res.Dic[property] = value;
            }
            ServerDic[url] = res.Clone();
            return res.Clone();
            static string GetDefaultString(string value, string defVal)
            {
                return string.IsNullOrWhiteSpace(value) ? defVal : value.Trim();
            }
        }
        /// <summary>
        /// 还原参数内容(解码URL)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string GetUrlArgs(string v)
        {
            return Uri.UnescapeDataString(v);
        }
        /// <summary>
        /// 设置参数内容(编码URL)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String SetUrlArgs(string v)
        {
            return Uri.EscapeDataString(v);
        }
        /// <summary>
        /// 转换模型为一个参数字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static String GetModelArgs<T>(T model, params string[] properties)
        {
            if (model == null) { return string.Empty; }
            if (properties == null) { properties = new string[0]; }
            var sb = new StringBuilder();
            foreach (var item in PropertyAccess.GetAccess(model).FuncInfoDic)
            {
                if (properties.Contains(item.Key)) { continue; }
                var val = item.Value.GetValue(model)?.ToString();
                sb.Append(item.Key).Append("=").Append(val == null ? "" : SetUrlArgs(val)).Append("&");
            }
            if (sb.Length > 0) { sb.Length--; }
            return sb.ToString();
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var res = new StringBuilder();
            if (!string.IsNullOrEmpty(Protocal)) { res.Append(Protocal).Append("://"); }
            if (!string.IsNullOrEmpty(Host)) { res.Append(Host); }
            if (Port > 0) { res.Append(":").Append(Port); }
            if (!string.IsNullOrEmpty(Path)) { res.Append("/").Append(Path.Trim('\\').Trim('/')); }
            if (!string.IsNullOrEmpty(Args)) { res.Append("?").Append(Args); }
            if (!string.IsNullOrEmpty(Anchor)) { res.Append("#").Append(Anchor); }
            return res.ToString();
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual HopperUrlModelV1 Clone()
        {
            return new HopperUrlModelV1
            {
                Anchor = Anchor,
                AnchorDic = AnchorDic,
                Host = Host,
                Args = Args,
                ArgsDic = ArgsDic,
                Dic = Dic,
                IsMatch = IsMatch,
                Origin = Origin,
                Path = Path,
                Port = Port,
                Protocal = Protocal,
            };
        }
    }
#pragma warning restore CS1570 // XML 注释出现 XML 格式错误
}
