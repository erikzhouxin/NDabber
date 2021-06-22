using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// 自动SQL模板
    /// </summary>
    public interface IAutoSqlModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 创建表SQL
        /// CREATE TABLE IF NOT EXISTS ...(?...)
        /// </summary>
        string Create { get; }
        /// <summary>
        /// 插入SQL
        /// INSERT INTO ...(?...) VALUES(@...)
        /// </summary>
        string Insert { get; }
        /// <summary>
        /// 无添有换
        /// REPLACE INTO ...(?...) VALUES(@...)
        /// </summary>
        string Replace { get; }
        /// <summary>
        /// 更新Sql
        /// UPDATE ... SET ?...=@... WHERE ?...=@...
        /// </summary>
        string UpdateID { get; }
        /// <summary>
        /// 更新Sql不包含Where
        /// UPDATE ... SET ?...=@... 
        /// </summary>
        string Update { get; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?...=@...
        /// </summary>
        string DeleteID { get; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?... IN @...
        /// </summary>
        string DeleteInID { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ...
        /// </summary>
        string Select { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?...=@...
        /// </summary>
        string SelectID { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?... IN @... 
        /// </summary>
        string SelectInID { get; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... LIMIT @Skip,@Take
        /// </summary>
        string SelectLimit { get; }
        /// <summary>
        /// 计数
        /// SELECT COUNT(*) FROM ...
        /// </summary>
        string SelectCount { get; }
        /// <summary>
        /// 获取列的集合
        /// </summary>
        string[] Cols { get; }
        /// <summary>
        /// 标记名称(不包括分隔符)
        /// </summary>
        String TagName { get; }
        /// <summary>
        /// 标记主键名称(不包括分隔符)
        /// </summary>
        String TagID { get; }
        /// <summary>
        /// 主键WHERE(不包括WHERE关键字)[?...=@...]
        /// </summary>
        string WhereID { get; }
    }
    /// <summary>
    /// 自动SQL类型
    /// 限制:
    /// 只支持单一主键,不支持复合主键
    /// 未完成索引添加
    /// </summary>
    public class AutoSqlModel : IAutoSqlModel
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        /// <param name="type"></param>
        public AutoSqlModel(Type type)
        {
            Type = type;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// 创建表SQL
        /// CREATE TABLE IF NOT EXISTS ...(?...)
        /// </summary>
        public string Create { get; set; }
        /// <summary>
        /// 插入SQL
        /// INSERT INTO ...(?...) VALUES(@...)
        /// </summary>
        public string Insert { get; set; }
        /// <summary>
        /// 无添有换
        /// REPLACE INTO ...(?...) VALUES(@...)
        /// </summary>
        public string Replace { get; set; }
        /// <summary>
        /// 更新Sql
        /// UPDATE ... SET ?...=@... WHERE ?...=@...
        /// </summary>
        public string UpdateID { get; set; }
        /// <summary>
        /// 更新Sql不包含Where
        /// UPDATE ... SET ?...=@... WHERE ?...=@...
        /// </summary>
        public string Update { get; set; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?...=@...
        /// </summary>
        public string DeleteID { get; set; }
        /// <summary>
        /// 删除Sql
        /// DELETE ... WHERE ?... IN @...
        /// </summary>
        public string DeleteInID { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ...
        /// </summary>
        public string Select { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?... IN @... 
        /// </summary>
        public string SelectInID { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... WHERE ?...=@...
        /// </summary>
        public string SelectID { get; set; }
        /// <summary>
        /// 查询
        /// SELECT ?... FROM ... LIMIT @Skip,@Take
        /// </summary>
        public string SelectLimit { get; set; }
        /// <summary>
        /// 计数
        /// SELECT COUNT(*) FROM ...
        /// </summary>
        public string SelectCount { get; set; }
        /// <summary>
        /// 获取列的集合
        /// </summary>
        public string[] Cols { get; set; }
        /// <summary>
        /// 标记名称(不包括分隔符)
        /// </summary>
        public virtual String TagName { get; set; }
        /// <summary>
        /// 标记主键名称(不包括分隔符)
        /// </summary>
        public virtual String TagID { get; set; }
        /// <summary>
        /// 主键WHERE(不包括WHERE关键字)[?...=@...]
        /// </summary>
        public virtual string WhereID { get; set; }

        #region // 参数模型
        /// <summary>
        /// 百分号
        /// </summary>
        public const string PERCENT = "%";
        /// <summary>
        /// 下划线
        /// </summary>
        public const string ULINE = "_";
        /// <summary>
        /// 下划线
        /// </summary>
        public const string UNDERLINE = "_";
        /// <summary>
        /// 百分号
        /// </summary>
        public const String PAH = "%";
        /// <summary>
        /// 字符串(SQL的LIKE)
        /// </summary>
        public class StringLike
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            public StringLike() : this(string.Empty, PAH, string.Empty) { }
            /// <summary>
            /// 初始值构造
            /// </summary>
            /// <param name="value"></param>
            public StringLike(string value) : this(PAH, value, PAH) { }
            /// <summary>
            /// 完整构造
            /// </summary>
            public StringLike(string left, string value, string right)
            {
                Value = value;
                Left = left;
                Right = right;
            }
            /// <summary>
            /// 值
            /// </summary>
            public String Value { get; set; }
            /// <summary>
            /// 左边
            /// </summary>
            public String Left { get; set; }
            /// <summary>
            /// 右边
            /// </summary>
            public String Right { get; set; }
            /// <summary>
            /// 转换成字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Left + Value + Right;
            }
            /// <summary>
            /// 隐式转换成StringLike
            /// </summary>
            public static implicit operator StringLike(string value) => new StringLike(value);
            /// <summary>
            /// 隐式转换成StringLike
            /// </summary>
            public static implicit operator StringLike(Tuple<string, string, string> item) => new StringLike(item.Item1, item.Item2, item.Item3);
            /// <summary>
            /// 隐式转换成字符串
            /// </summary>
            public static implicit operator string(StringLike val) => val.ToString();
        }
        /// <summary>
        /// 数组
        /// </summary>
        public class ArrayIn
        {
            /// <summary>
            /// 构造
            /// </summary>
            public ArrayIn() : this(new object[] { }) { }
            /// <summary>
            /// 初始值构造
            /// </summary>
            /// <param name="value"></param>
            public ArrayIn(Array value)
            {
                Value = value;
            }
            /// <summary>
            /// 值
            /// </summary>
            public Array Value { get; set; }
            /// <summary>
            /// 左间隔
            /// </summary>
            public string LSplit { get; set; } = "'";
            /// <summary>
            /// 中间隔
            /// </summary>
            public string MSplit { get; set; } = ",";
            /// <summary>
            /// 右间隔
            /// </summary>
            public string RSplit { get; set; } = "'";
            /// <summary>
            /// 转换成字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Value == null) { return null; }
                if (Value.Length == 0) { return string.Empty; }
                if (Value.Length == 1) { return $"({LSplit}{Value.GetValue(0)}{RSplit})"; }
                var joinString = LSplit + MSplit + RSplit;
                return $"({LSplit}{string.Join(joinString, Value)}{RSplit})";
            }
            /// <summary>
            /// 隐式转换成ArrayIn
            /// </summary>
            public static implicit operator ArrayIn(Array value) => new ArrayIn(value as Array);
            /// <summary>
            /// 隐式转换成字符串
            /// </summary>
            public static implicit operator string(ArrayIn val) => val.ToString();
        }
        /// <summary>
        /// 比较接口
        /// </summary>
        public interface ICompare
        {
            /// <summary>
            /// 标记(>-1:小于,-1:小于等于,0:相等,1大于等于,>1大于)
            /// </summary>
            int Sign { get; }
            /// <summary>
            /// 值
            /// </summary>
            object Value { get; }
            /// <summary>
            /// 获取符号
            /// </summary>
            /// <returns></returns>
            string GetSign();
        }
        /// <summary>
        /// 比较泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Compare<T> : ICompare
        {
            /// <summary>
            /// 构造
            /// </summary>
            protected Compare(T value, int sign)
            {
                Value = value;
                Sign = sign;
            }
            /// <summary>
            /// 值
            /// </summary>
            public virtual T Value { get; }
            /// <summary>
            /// 标记(>-1:小于,-1:小于等于,0:相等,1大于等于,>1大于)
            /// </summary>
            public virtual int Sign { get; }

            object ICompare.Value => this.Value;
            /// <summary>
            /// 获取比较符号
            /// </summary>
            /// <returns></returns>
            public string GetSign()
            {
                switch (Sign)
                {
                    case -2: return "<";
                    case -1: return "<=";
                    case 0: return "=";
                    case 1: return ">=";
                    case 2: return ">";
                    default: return Sign < 0 ? "<" : ">";
                }
            }
            /// <summary>
            /// 隐式转
            /// </summary>
            public static implicit operator Compare<T>(Tuple<T, int> item) => new Compare<T>(item.Item1, item.Item2);
        }
        /// <summary>
        /// 比较区域
        /// </summary>
        public interface ICompareRange
        {
            /// <summary>
            /// 或关系
            /// </summary>
            bool IsOr { get; }
            /// <summary>
            /// 为空
            /// </summary>
            bool IsEmpty { get; }
            /// <summary>
            /// 比较值列表
            /// </summary>
            List<ICompare> Compares { get; }
            /// <summary>
            /// 添加比较
            /// </summary>
            /// <param name="compare"></param>
            /// <returns></returns>
            List<ICompare> Add(ICompare compare);
        }
        /// <summary>
        /// 比较区域
        /// </summary>
        public class CompareRange : ICompareRange
        {
            /// <summary>
            /// 构造
            /// </summary>
            public CompareRange() : this(null) { }
            /// <summary>
            /// 构造
            /// </summary>
            public CompareRange(List<ICompare> compares) : this(compares, false) { }
            /// <summary>
            /// 构造
            /// </summary>
            public CompareRange(List<ICompare> compares, bool isOr)
            {
                Compares = compares ?? new List<ICompare>();
                IsOr = isOr;
            }
            /// <summary>
            /// 比较值列表
            /// </summary>
            public List<ICompare> Compares { get; }
            /// <summary>
            /// 为空
            /// </summary>
            public bool IsEmpty => Compares.Count() == 0;
            /// <summary>
            /// 或关系(默认False)
            /// </summary>
            public bool IsOr { get; set; }

            /// <summary>
            /// 添加到比较值列表
            /// </summary>
            /// <param name="compare"></param>
            /// <returns></returns>
            public List<ICompare> Add(ICompare compare)
            {
                Compares.Add(compare);
                return Compares;
            }
        }
        /// <summary>
        /// 在.. 和 .. 之间
        /// </summary>
        public interface ICompareBetween
        {
            /// <summary>
            /// 获取开始值
            /// </summary>
            /// <returns></returns>
            object Begin { get; }
            /// <summary>
            /// 获取结束值
            /// </summary>
            /// <returns></returns>
            object End { get; }
        }
        /// <summary>
        /// 在.. 和 .. 之间
        /// </summary>
        public class CompareBetween<T> : ICompareBetween
        {
            /// <summary>
            /// 开始值
            /// </summary>
            public T Begin { get; }
            /// <summary>
            /// 结束值
            /// </summary>
            public T End { get; }

            object ICompareBetween.Begin => this.Begin;

            object ICompareBetween.End => this.End;

            /// <summary>
            /// 初始值构造
            /// </summary>
            public CompareBetween(T begin, T end)
            {
                Begin = begin;
                End = end;
            }
            /// <summary>
            /// 隐式转
            /// </summary>
            public static implicit operator CompareBetween<T>(Tuple<T, T> item) => new CompareBetween<T>(item.Item1, item.Item2);
        }
        #endregion
    }
    /// <summary>
    /// 创建模型接口(绕过类型字典使用泛型)
    /// </summary>
    public interface ISqlBuilderModel
    {
        /// <summary>
        /// 获取SQL创建者
        /// </summary>
        /// <returns></returns>
        AutoSqlBuilder GetSqlModel();
    }
    /// <summary>
    /// 自动SQL创建类
    /// </summary>
    public class AutoSqlBuilder : AutoSqlModel, IAutoSqlModel
    {
        /// <summary>
        /// 缓存字典
        /// </summary>
        protected ConcurrentDictionary<string, string> CacheDic = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public AutoSqlBuilder(Type type) : base(type)
        {

        }
    }
}
