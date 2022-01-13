using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 仿照元组(Tuple)进行创建的类
    /// </summary>
    public static class TubleCaller
    {
        /// <summary>
        /// 创建二元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2> Create<T1, T2>(T1 model1, T2 model2)
        {
            return new Tuble<T1, T2>(model1, model2);
        }
        /// <summary>
        /// 创建三元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3> Create<T1, T2, T3>(T1 model1, T2 model2, T3 model3)
        {
            return new Tuble<T1, T2, T3>(model1, model2, model3);
        }
        /// <summary>
        /// 创建四元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 model1, T2 model2, T3 model3, T4 model4)
        {
            return new Tuble<T1, T2, T3, T4>(model1, model2, model3, model4);
        }
        /// <summary>
        /// 创建五元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5)
        {
            return new Tuble<T1, T2, T3, T4, T5>(model1, model2, model3, model4, model5);
        }
        /// <summary>
        /// 创建六元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6>(model1, model2, model3, model4, model5, model6);
        }
        /// <summary>
        /// 创建七元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7>(model1, model2, model3, model4, model5, model6, model7);
        }
        /// <summary>
        /// 创建八元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7, T8 model8)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7, T8>(model1, model2, model3, model4, model5, model6, model7, model8);
        }
    }
    #region // 扩展类
    /// <summary>
    /// 二元组
    /// </summary>
    public class Tuble<T1, T2>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public Tuble(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        /// <summary>
        /// 项一
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// 项二
        /// </summary>
        public T2 Item2 { get; set; }
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class TubleInt32 : Tuble<int, int>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleInt32() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TubleInt32(int key, int value) : base(key, value) { }
        /// <summary>
        /// 键
        /// </summary>
        public int Key { get => Item1; set => Item1 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get => Item2; set => Item2 = value; }
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class TubleString : Tuble<string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleString() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TubleString(string key, string value) : base(key, value) { }
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get => Item1; set => Item1 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get => Item2; set => Item2 = value; }
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class TubleStringInt : Tuble<string, int>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleStringInt() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TubleStringInt(string key, int value) : base(key, value) { }
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get => Item1; set => Item1 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get => Item2; set => Item2 = value; }
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class TubleIntString : Tuble<int, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleIntString() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TubleIntString(int key, string value) : base(key, value) { }
        /// <summary>
        /// 键
        /// </summary>
        public int Key { get => Item1; set => Item1 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get => Item2; set => Item2 = value; }
    }
    /// <summary>
    /// 二元组SQL脚本结果项
    /// </summary>
    public class Tuble2SqlArgs : Tuble<string, Dictionary<string, object>>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public Tuble2SqlArgs() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public Tuble2SqlArgs(string sql, Dictionary<string, object> parameters) : base(sql, parameters) { }
        /// <summary>
        /// SQL脚本
        /// </summary>
        public string Sql => Item1;
        /// <summary>
        /// 参数列表
        /// </summary>
        public Dictionary<string, object> Parameters => Item2;
    }
    /// <summary>
    /// 二元组类型标记
    /// </summary>
    public class Tuble2TypeTag : Tuble<Type, string>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public Tuble2TypeTag() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        public Tuble2TypeTag(Type type, string tag) : base(type, tag) { }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type => Item1;
        /// <summary>
        /// 标记
        /// </summary>
        public string Tag => Item2;
    }
    /// <summary>
    /// 二元组键名
    /// </summary>
    public class Tuble2KeyName : Tuble<string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2KeyName() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        public Tuble2KeyName(string key, string name) : base(key, name) { }
        /// <summary>
        /// 键
        /// </summary>
        public string Key => Item1;
        /// <summary>
        /// 名
        /// </summary>
        public String Name => Item2;
    }
    /// <summary>
    /// 三元组
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class Tuble<T1, T2, T3> : Tuble<T1, T2>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public Tuble(T1 m1, T2 m2, T3 m3) : base(m1, m2) { Item3 = m3; }
        /// <summary>
        /// 项三
        /// </summary>
        public T3 Item3 { get; set; }
    }
    /// <summary>
    /// 二元组类型标记
    /// </summary>
    public class Tuble3TypeKeyTag : Tuble<Type, string, string>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public Tuble3TypeKeyTag() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="tag"></param>
        public Tuble3TypeKeyTag(Type type, string key, string tag) : base(type, key, tag) { }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type => Item1;
        /// <summary>
        /// 键
        /// </summary>
        public string Key => Item2;
        /// <summary>
        /// 标记
        /// </summary>
        public string Tag => Item3;
    }
    /// <summary>
    /// 三元组[标识:ID,名称:Name,值:Value]
    /// </summary>
    public class Tuble3A001 : Tuble<String, String, int>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3A001() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Tuble3A001(string id, string name, int value) : base(id, name, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get => base.Item1; set => base.Item1 = value; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get => base.Item2; set => base.Item2 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get => base.Item3; set => base.Item3 = value; }
    }
    /// <summary>
    /// 三元组[标识:ID,编码:Code,值:Value]
    /// </summary>
    public class Tuble3B001 : Tuble<String, String, int>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3B001() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public Tuble3B001(string id, string code, int value) : base(id, code, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get => base.Item1; set => base.Item1 = value; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get => base.Item2; set => base.Item2 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get => base.Item3; set => base.Item3 = value; }
    }
    /// <summary>
    /// 三元组[标识:ID,名称:Name,值:Value]
    /// </summary>
    public class Tuble3A002 : Tuble<String, String, double>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3A002() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Tuble3A002(string id, string name, double value) : base(id, name, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get => base.Item1; set => base.Item1 = value; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get => base.Item2; set => base.Item2 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public double Value { get => base.Item3; set => base.Item3 = value; }
    }
    /// <summary>
    /// 三元组[标识:ID,编码:Code,值:Value]
    /// </summary>
    public class Tuble3B002 : Tuble<String, String, double>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3B002() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public Tuble3B002(string id, string code, double value) : base(id, code, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get => base.Item1; set => base.Item1 = value; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get => base.Item2; set => base.Item2 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public double Value { get => base.Item3; set => base.Item3 = value; }
    }
    /// <summary>
    /// 四元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4> : Tuble<T1, T2, T3>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4) : base(m1, m2, m3) { Item4 = m4; }
        /// <summary>
        /// 项四
        /// </summary>
        public T4 Item4 { get; set; }
    }
    /// <summary>
    /// 五元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5> : Tuble<T1, T2, T3, T4>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5) : base(m1, m2, m3, m4) { Item5 = m5; }
        /// <summary>
        /// 项五
        /// </summary>
        public T5 Item5 { get; set; }
    }
    /// <summary>
    /// 六元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5, T6> : Tuble<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6) : base(m1, m2, m3, m4, m5) { Item6 = m6; }
        /// <summary>
        /// 项六
        /// </summary>
        public T6 Item6 { get; set; }
    }
    /// <summary>
    /// 七元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5, T6, T7> : Tuble<T1, T2, T3, T4, T5, T6>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        /// <param name="m7"></param>
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6, T7 m7) : base(m1, m2, m3, m4, m5, m6) { Item7 = m7; }
        /// <summary>
        /// 项七
        /// </summary>
        public T7 Item7 { get; set; }
    }
    /// <summary>
    /// 八元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5, T6, T7, T8> : Tuble<T1, T2, T3, T4, T5, T6, T7>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        /// <param name="m7"></param>
        /// <param name="m8"></param>
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6, T7 m7, T8 m8) : base(m1, m2, m3, m4, m5, m6, m7) { Item8 = m8; }
        /// <summary>
        /// 项八
        /// </summary>
        public T8 Item8 { get; set; }
    }
    #endregion
}
