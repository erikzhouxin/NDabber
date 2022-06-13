using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 仿照元组(Tuple)进行创建的类
    /// </summary>
    public static class Tuble
    {
        /// <summary>
        /// 创建一元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1> Create<T1>(T1 model1)
            => new Tuble<T1>(model1);
        /// <summary>
        /// 创建二元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2> Create<T1, T2>(T1 model1, T2 model2)
            => new Tuble<T1, T2>(model1, model2);
        /// <summary>
        /// 创建三元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3> Create<T1, T2, T3>(T1 model1, T2 model2, T3 model3)
            => new Tuble<T1, T2, T3>(model1, model2, model3);
        /// <summary>
        /// 创建四元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 model1, T2 model2, T3 model3, T4 model4)
            => new Tuble<T1, T2, T3, T4>(model1, model2, model3, model4);
        /// <summary>
        /// 创建五元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5)
            => new Tuble<T1, T2, T3, T4, T5>(model1, model2, model3, model4, model5);
        /// <summary>
        /// 创建六元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6)
            => new Tuble<T1, T2, T3, T4, T5, T6>(model1, model2, model3, model4, model5, model6);
        /// <summary>
        /// 创建七元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7)
            => new Tuble<T1, T2, T3, T4, T5, T6, T7>(model1, model2, model3, model4, model5, model6, model7);
        /// <summary>
        /// 创建八元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7, T8 model8)
            => new Tuble<T1, T2, T3, T4, T5, T6, T7, T8>(model1, model2, model3, model4, model5, model6, model7, model8);
    }
    #region // 扩展类
    /// <summary>
    /// 一元组
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class Tuble<T1> : ITuble<T1>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble()
        {

        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="item1"></param>
        public Tuble(T1 item1)
        {
            Item1 = item1;
        }
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1> Clone() => new Tuble<T1>(Item1);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class Tuble<T1, T2> : Tuble<T1>, ITuble<T1, T2>, ICloneable
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
        /// 项二
        /// </summary>
        public virtual T2 Item2 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2> Clone() => new Tuble<T1, T2>(Item1, Item2);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class TubleInt32 : Tuble<int, int>, ICloneable
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
    public class Tuble2Int32 : TubleInt32, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2Int32() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tuble2Int32(int key, int value) : base(key, value) { }
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class TubleString : Tuble<string, string>, ICloneable
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
    public class Tuble2CheckString : Tuble<bool, string>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2CheckString() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tuble2CheckString(bool key, string value) : base(key, value) { }
    }
    /// <summary>
    /// 二元组
    /// </summary>
    public class Tuble2String : TubleString
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2String() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tuble2String(string key, string value) : base(key, value) { }
    }
    /// <summary>
    /// 对象二元组
    /// </summary>
    public class Tuble2Object : Tuble<object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public Tuble2Object(object m1, object m2) : base(m1, m2) { }
    }
    /// <summary>
    /// 对象二元组
    /// </summary>
    public class Tuble2CheckObject : Tuble<bool, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public Tuble2CheckObject(bool m1, object m2) : base(m1, m2) { }
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
    public class Tuble2StringInt : TubleStringInt
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2StringInt() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tuble2StringInt(string key, int value) : base(key, value) { }
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
    /// 二元组
    /// </summary>
    public class Tuble2IntString : TubleIntString
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble2IntString() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tuble2IntString(int key, string value) : base(key, value) { }
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
    public class Tuble<T1, T2, T3> : Tuble<T1, T2>, ITuble<T1, T2, T3>, ICloneable
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
        public Tuble(T1 m1, T2 m2, T3 m3)
        {
            Item1 = m1;
            Item2 = m2;
            Item3 = m3;
        }
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2, T3> Clone() => new Tuble<T1, T2, T3>(Item1, Item2, Item3);
        object ICloneable.Clone() => Clone();
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
    /// 三元组对象
    /// </summary>
    public class Tuble3Object : Tuble<object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble3Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public Tuble3Object(object m1, object m2, object m3) : base(m1, m2, m3) { }
    }
    /// <summary>
    /// 三元组检查对象
    /// </summary>
    public class Tuble3String : Tuble<String, String, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3String() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Tuble3String(string id, string name, string value) : base(id, name, value) { }
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
        public string Value { get => base.Item3; set => base.Item3 = value; }
    }
    /// <summary>
    /// 三元组对象
    /// </summary>
    public class Tuble3CheckObject : Tuble<bool, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble3CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public Tuble3CheckObject(bool m1, object m2, object m3) : base(m1, m2, m3) { }
    }
    /// <summary>
    /// 三元组检查字符串
    /// </summary>
    public class Tuble3CheckString : Tuble<bool, String, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3CheckString() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public Tuble3CheckString(bool m1, string m2, string m3) : base(m1, m2, m3) { }
    }
    /// <summary>
    /// 三元组[标识:int:ID,名称:int:Name,值:int:Value]
    /// </summary>
    public class Tuble3Int32 : Tuble<int, int, int>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3Int32() { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Tuble3Int32(int id, int name, int value) : base(id, name, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public int ID { get => base.Item1; set => base.Item1 = value; }
        /// <summary>
        /// 名称
        /// </summary>
        public int Name { get => base.Item2; set => base.Item2 = value; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get => base.Item3; set => base.Item3 = value; }
    }
    /// <summary>
    /// 三元组[标识:string:ID,名称:string:Name,值:int:Value]
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
    /// 三元组[标识:string:ID,编码:string:Code,值:int:Value]
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
    /// 三元组[标识:string:ID,名称:string:Name,值:double:Value]
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
    /// 三元组[标识:string:ID,编码:string:Code,值:double:Value]
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
    /// 三元组[标识:int:ID,名称:string:Name,值:double:Value]
    /// </summary>
    public class Tuble3C001 : Tuble<int, String, int>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3C001() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Tuble3C001(int id, string name, int value) : base(id, name, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public int ID { get => base.Item1; set => base.Item1 = value; }
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
    /// 三元组[标识:int:ID,编码:string:Code,值:double:Value]
    /// </summary>
    public class Tuble3C002 : Tuble<int, String, double>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble3C002() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public Tuble3C002(int id, string code, double value) : base(id, code, value) { }
        /// <summary>
        /// 标识
        /// </summary>
        public int ID { get => base.Item1; set => base.Item1 = value; }
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
    public class Tuble<T1, T2, T3, T4> : Tuble<T1, T2, T3>, ITuble<T1, T2, T3, T4>, ICloneable
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
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4)
        {
            Item1 = m1;
            Item2 = m2;
            Item3 = m3;
            Item4 = m4;
        }
        /// <summary>
        /// 项四
        /// </summary>
        public virtual T4 Item4 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2, T3, T4> Clone() => new Tuble<T1, T2, T3, T4>(Item1, Item2, Item3, Item4);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 4元组检查对象
    /// </summary>
    public class Tuble4CheckObject : Tuble<bool, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble4CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public Tuble4CheckObject(bool m1, object m2, object m3, object m4) : base(m1, m2, m3, m4) { }
    }
    /// <summary>
    /// 4元组对象
    /// </summary>
    public class Tuble4Object : Tuble<object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble4Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public Tuble4Object(object m1, object m2, object m3, object m4) : base(m1, m2, m3, m4) { }
    }
    /// <summary>
    /// 4元组检查字符串
    /// </summary>
    public class Tuble4CheckString : Tuble<bool, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble4CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public Tuble4CheckString(bool m1, string m2, string m3, string m4) : base(m1, m2, m3, m4) { }
    }
    /// <summary>
    /// 4元组字符串
    /// </summary>
    public class Tuble4String : Tuble<string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble4String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public Tuble4String(string m1, string m2, string m3, string m4) : base(m1, m2, m3, m4) { }
    }
    /// <summary>
    /// 五元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5> : Tuble<T1, T2, T3, T4>, ITuble<T1, T2, T3, T4, T5>, ICloneable
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
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5)
        {
            Item1 = m1;
            Item2 = m2;
            Item3 = m3;
            Item4 = m4;
            Item5 = m5;
        }
        /// <summary>
        /// 项五
        /// </summary>
        public virtual T5 Item5 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2, T3, T4, T5> Clone() => new Tuble<T1, T2, T3, T4, T5>(Item1, Item2, Item3, Item4, Item5);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 5元组检查对象
    /// </summary>
    public class Tuble5CheckObject : Tuble<bool, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble5CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public Tuble5CheckObject(bool m1, object m2, object m3, object m4, object m5) : base(m1, m2, m3, m4, m5) { }
    }
    /// <summary>
    /// 5元组对象
    /// </summary>
    public class Tuble5Object : Tuble<object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble5Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public Tuble5Object(object m1, object m2, object m3, object m4, object m5) : base(m1, m2, m3, m4, m5) { }
    }
    /// <summary>
    /// 5元组检查字符串
    /// </summary>
    public class Tuble5CheckString : Tuble<bool, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble5CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public Tuble5CheckString(bool m1, string m2, string m3, string m4, string m5) : base(m1, m2, m3, m4, m5) { }
    }
    /// <summary>
    /// 5元组字符串
    /// </summary>
    public class Tuble5String : Tuble<string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble5String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public Tuble5String(string m1, string m2, string m3, string m4, string m5) : base(m1, m2, m3, m4, m5) { }
    }
    /// <summary>
    /// 六元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5, T6> : Tuble<T1, T2, T3, T4, T5>, ITuble<T1, T2, T3, T4, T5, T6>, ICloneable
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
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6)
        {
            Item1 = m1;
            Item2 = m2;
            Item3 = m3;
            Item4 = m4;
            Item5 = m5;
            Item6 = m6;
        }
        /// <summary>
        /// 项六
        /// </summary>
        public virtual T6 Item6 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2, T3, T4, T5, T6> Clone() => new Tuble<T1, T2, T3, T4, T5, T6>(Item1, Item2, Item3, Item4, Item5, Item6);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 6元组检查对象
    /// </summary>
    public class Tuble6CheckObject : Tuble<bool, object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble6CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public Tuble6CheckObject(bool m1, object m2, object m3, object m4, object m5, object m6) : base(m1, m2, m3, m4, m5, m6) { }
    }
    /// <summary>
    /// 6元组对象
    /// </summary>
    public class Tuble6Object : Tuble<object, object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble6Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public Tuble6Object(object m1, object m2, object m3, object m4, object m5, object m6) : base(m1, m2, m3, m4, m5, m6) { }
    }
    /// <summary>
    /// 6元组检查字符串
    /// </summary>
    public class Tuble6CheckString : Tuble<bool, string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble6CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public Tuble6CheckString(bool m1, string m2, string m3, string m4, string m5, string m6) : base(m1, m2, m3, m4, m5, m6) { }
    }
    /// <summary>
    /// 6元组字符串
    /// </summary>
    public class Tuble6String : Tuble<string, string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble6String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public Tuble6String(string m1, string m2, string m3, string m4, string m5, string m6) : base(m1, m2, m3, m4, m5, m6) { }
    }
    /// <summary>
    /// 七元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5, T6, T7> : Tuble<T1, T2, T3, T4, T5, T6>, ITuble<T1, T2, T3, T4, T5, T6, T7>, ICloneable
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
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6, T7 m7)
        {
            Item1 = m1;
            Item2 = m2;
            Item3 = m3;
            Item4 = m4;
            Item5 = m5;
            Item6 = m6;
            Item7 = m7;
        }
        /// <summary>
        /// 项七
        /// </summary>
        public virtual T7 Item7 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2, T3, T4, T5, T6, T7> Clone() => new Tuble<T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, Item3, Item4, Item5, Item6, Item7);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 7元组检查对象
    /// </summary>
    public class Tuble7CheckObject : Tuble<bool, object, object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble7CheckObject() { }
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
        public Tuble7CheckObject(bool m1, object m2, object m3, object m4, object m5, object m6, object m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
    }
    /// <summary>
    /// 7元组对象
    /// </summary>
    public class Tuble7Object : Tuble<object, object, object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble7Object() { }
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
        public Tuble7Object(object m1, object m2, object m3, object m4, object m5, object m6, object m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
    }
    /// <summary>
    /// 7元组检查字符串
    /// </summary>
    public class Tuble7CheckString : Tuble<bool, string, string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble7CheckString() { }
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
        public Tuble7CheckString(bool m1, string m2, string m3, string m4, string m5, string m6, string m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
    }
    /// <summary>
    /// 7元组字符串
    /// </summary>
    public class Tuble7String : Tuble<string, string, string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble7String() { }
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
        public Tuble7String(string m1, string m2, string m3, string m4, string m5, string m6, string m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
    }
    /// <summary>
    /// 八元组
    /// </summary>
    public class Tuble<T1, T2, T3, T4, T5, T6, T7, T8> : Tuble<T1, T2, T3, T4, T5, T6, T7>, ITuble<T1, T2, T3, T4, T5, T6, T7, T8>, ICloneable
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
        public Tuble(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6, T7 m7, T8 m8)
        {
            Item1 = m1;
            Item2 = m2;
            Item3 = m3;
            Item4 = m4;
            Item5 = m5;
            Item6 = m6;
            Item7 = m7;
            Item8 = m8;
        }
        /// <summary>
        /// 项八
        /// </summary>
        public virtual T8 Item8 { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public new ITuble<T1, T2, T3, T4, T5, T6, T7, T8> Clone() => new Tuble<T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 8元组检查对象
    /// </summary>
    public class Tuble8CheckObject : Tuble<bool, object, object, object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble8CheckObject() { }
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
        public Tuble8CheckObject(bool m1, object m2, object m3, object m4, object m5, object m6, object m7, object m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
    }
    /// <summary>
    /// 8元组对象
    /// </summary>
    public class Tuble8Object : Tuble<object, object, object, object, object, object, object, object>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble8Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public Tuble8Object(object m1, object m2) : base(m1, m2, null, null, null, null, null, null) { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public Tuble8Object(object m1, object m2, object m3) : base(m1, m2, m3, null, null, null, null, null) { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public Tuble8Object(object m1, object m2, object m3, object m4) : base(m1, m2, m3, m4, null, null, null, null) { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public Tuble8Object(object m1, object m2, object m3, object m4, object m5) : base(m1, m2, m3, m4, m5, null, null, null) { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public Tuble8Object(object m1, object m2, object m3, object m4, object m5, object m6) : base(m1, m2, m3, m4, m5, m6, null, null) { }
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
        public Tuble8Object(object m1, object m2, object m3, object m4, object m5, object m6, object m7) : base(m1, m2, m3, m4, m5, m6, m7, null) { }
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
        public Tuble8Object(object m1, object m2, object m3, object m4, object m5, object m6, object m7, object m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
    }
    /// <summary>
    /// 8元组检查字符串
    /// </summary>
    public class Tuble8CheckString : Tuble<bool, string, string, string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble8CheckString() { }
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
        public Tuble8CheckString(bool m1, string m2, string m3, string m4, string m5, string m6, string m7, string m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
    }
    /// <summary>
    /// 8元组字符串
    /// </summary>
    public class Tuble8String : Tuble<string, string, string, string, string, string, string, string>
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public Tuble8String() { }
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
        public Tuble8String(string m1, string m2, string m3, string m4, string m5, string m6, string m7, string m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
    }
    /// <summary>
    /// 属性改变触发模型
    /// </summary>
    /// <typeparam name="TM"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public sealed class Tuble4Changed<TM, TV> : Tuble<TM, TV, TV, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Tuble4Changed() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="model"></param>
        /// <param name="newVal"></param>
        /// <param name="oldVal"></param>
        /// <param name="pname"></param>
        public Tuble4Changed(TM model, TV newVal, TV oldVal, string pname)
        {
            this.Item1 = model;
            this.Item2 = newVal;
            this.Item3 = oldVal;
            this.Item4 = pname;
        }
        /// <summary>
        /// 模型
        /// </summary>
        public TM Model { get => Item1; set => Item1 = value; }
        /// <summary>
        /// 新值
        /// </summary>
        public TV NewVal { get => Item2; set => Item2 = value; }
        /// <summary>
        /// 旧值
        /// </summary>
        public TV OldVal { get => Item3; set => Item3 = value; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public String PropertyName { get => Item4; set => Item4 = value; }
    }
    #endregion
    #region // 扩展接口
    /// <summary>
    /// 1元组接口
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface ITuble<out T1> : ICloneable
    {
        /// <summary>
        /// 项1
        /// </summary>
        T1 Item1 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1> Clone();
    }
    /// <summary>
    /// 二元组接口
    /// </summary>
    public interface ITuble<out T1, out T2> : ITuble<T1>, ICloneable
    {
        /// <summary>
        /// 项2
        /// </summary>
        T2 Item2 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2> Clone();
    }
    /// <summary>
    /// 3元组接口
    /// </summary>
    public interface ITuble<out T1, out T2, out T3> : ITuble<T1, T2>, ICloneable
    {
        /// <summary>
        /// 项3
        /// </summary>
        T3 Item3 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2, T3> Clone();
    }
    /// <summary>
    /// 4元组接口
    /// </summary>
    public interface ITuble<out T1, out T2, out T3, out T4> : ITuble<T1, T2, T3>, ICloneable
    {
        /// <summary>
        /// 项4
        /// </summary>
        T4 Item4 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2, T3, T4> Clone();
    }
    /// <summary>
    /// 5元组接口
    /// </summary>
    public interface ITuble<out T1, out T2, out T3, out T4, out T5> : ITuble<T1, T2, T3, T4>, ICloneable
    {
        /// <summary>
        /// 项5
        /// </summary>
        T5 Item5 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2, T3, T4, T5> Clone();
    }
    /// <summary>
    /// 6元组接口
    /// </summary>
    public interface ITuble<out T1, out T2, out T3, out T4, out T5, out T6> : ITuble<T1, T2, T3, T4, T5>, ICloneable
    {
        /// <summary>
        /// 项6
        /// </summary>
        T6 Item6 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2, T3, T4, T5, T6> Clone();
    }
    /// <summary>
    /// 7元组接口
    /// </summary>
    public interface ITuble<out T1, out T2, out T3, out T4, out T5, out T6, out T7> : ITuble<T1, T2, T3, T4, T5, T6>, ICloneable
    {
        /// <summary>
        /// 项7
        /// </summary>
        T7 Item7 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2, T3, T4, T5, T6, T7> Clone();
    }
    /// <summary>
    /// 8元组接口
    /// </summary>
    public interface ITuble<out T1, out T2, out T3, out T4, out T5, out T6, out T7, out T8> : ITuble<T1, T2, T3, T4, T5, T6, T7>, ICloneable
    {
        /// <summary>
        /// 项8
        /// </summary>
        T8 Item8 { get; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new ITuble<T1, T2, T3, T4, T5, T6, T7, T8> Clone();
    }
    #endregion
    #region // 扩展触发类
    /// <summary>
    /// 触发元组改变代理
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TM"></typeparam>
    /// <param name="args"></param>
    public delegate void TubleTriggerChanged<TM, TV>(Tuble4Changed<TM, TV> args);
    /// <summary>
    /// 触发元组属性改变抽象类
    /// </summary>
    public abstract class TubleTriggerPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性更改
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变化时
        /// </summary>
        public void OnPropertyChanged<T>(string propertyName, ref T model, T value)
        {
            model = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 属性变化时
        /// </summary>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 属性变化时
        /// </summary>
        public void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /// <summary>
    /// 触发一元组
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class TubleTrigger<T1> : TubleTriggerPropertyChanged, ITuble<T1>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        public TubleTrigger(T1 m1)
        {
            this._item1 = m1;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1> Clone() => new TubleTrigger<T1>(Item1);
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发一元组字符串
    /// </summary>
    public class TubleTrigger1String : TubleTrigger<String>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger1String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        public TubleTrigger1String(string m1) : base(m1) { }
    }
    /// <summary>
    /// 触发二元组
    /// </summary>
    public class TubleTrigger<T1, T2> : TubleTriggerPropertyChanged, ITuble<T1, T2>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public TubleTrigger(T1 m1, T2 m2)
        {
            this._item1 = m1;
            this._item2 = m2;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2>(TubleTrigger<T1, T2> model) => new Tuble<T1, T2>(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2>(Tuble<T1, T2> model) => new TubleTrigger<T1, T2>(model.Item1, model.Item2);
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2> Clone() => new TubleTrigger<T1, T2>(Item1, Item2);
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发2元组检查对象
    /// </summary>
    public class TubleTrigger2CheckObject : TubleTrigger<bool, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger2CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public TubleTrigger2CheckObject(bool m1, object m2) : base(m1, m2) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble2CheckObject(TubleTrigger2CheckObject model)
            => new Tuble2CheckObject(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2CheckObject(Tuble2CheckObject model)
            => new TubleTrigger2CheckObject(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2CheckObject(Tuble<bool, object> model)
            => new TubleTrigger2CheckObject(model.Item1, model.Item2);
    }
    /// <summary>
    /// 触发2元组检查对象
    /// </summary>
    public class TubleTrigger2Object : TubleTrigger<object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger2Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public TubleTrigger2Object(object m1, object m2) : base(m1, m2) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble2Object(TubleTrigger2Object model)
            => new Tuble2Object(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2Object(Tuble2Object model)
            => new TubleTrigger2Object(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2Object(Tuble<object, object> model)
            => new TubleTrigger2Object(model.Item1, model.Item2);
    }
    /// <summary>
    /// 触发2元组检查字符串
    /// </summary>
    public class TubleTrigger2String : TubleTrigger<string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger2String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public TubleTrigger2String(string m1, string m2) : base(m1, m2) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble2String(TubleTrigger2String model)
            => new Tuble2String(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2String(Tuble2String model)
            => new TubleTrigger2String(model.Item1, model.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2String(Tuble<string, string> model)
            => new TubleTrigger2String(model.Item1, model.Item2);
    }
    /// <summary>
    /// 触发2元组检查字符串
    /// </summary>
    public class TubleTrigger2CheckString : TubleTrigger<bool, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger2CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public TubleTrigger2CheckString(bool m1, string m2) : base(m1, m2) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble2CheckString(TubleTrigger2CheckString model)
        {
            return new Tuble2CheckString(model.Item1, model.Item2);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2CheckString(Tuble2CheckString model)
        {
            return new TubleTrigger2CheckString(model.Item1, model.Item2);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger2CheckString(Tuble<bool, string> model)
        {
            return new TubleTrigger2CheckString(model.Item1, model.Item2);
        }
    }
    /// <summary>
    /// 触发三元组
    /// </summary>
    public class TubleTrigger<T1, T2, T3> : TubleTriggerPropertyChanged, ITuble<T1, T2, T3>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public TubleTrigger(T1 m1, T2 m2, T3 m3)
        {
            this._item1 = m1;
            this._item2 = m2;
            this._item3 = m3;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 项三改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3>, T3> Item3Changed;
        private T3 _item3;
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3
        {
            get => _item3;
            set
            {
                var oldVal = _item3;
                OnPropertyChanged(nameof(Item3), ref _item3, value);
                Item3Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3>, T3>(this, value, oldVal, nameof(Item3)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2, T3>(TubleTrigger<T1, T2, T3> model)
        {
            return new Tuble<T1, T2, T3>(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2, T3>(Tuble<T1, T2, T3> model)
        {
            return new TubleTrigger<T1, T2, T3>(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2, T3> Clone() => new TubleTrigger<T1, T2, T3>(Item1, Item2, Item3);
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        ITuble<T1, T2> ITuble<T1, T2>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发3元组检查对象
    /// </summary>
    public class TubleTrigger3CheckObject : TubleTrigger<bool, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger3CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public TubleTrigger3CheckObject(bool m1, object m2, object m3) : base(m1, m2, m3) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble3CheckObject(TubleTrigger3CheckObject model)
        {
            return new Tuble3CheckObject(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3CheckObject(Tuble3CheckObject model)
        {
            return new TubleTrigger3CheckObject(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3CheckObject(Tuble<bool, object, object> model)
        {
            return new TubleTrigger3CheckObject(model.Item1, model.Item2, model.Item3);
        }
    }
    /// <summary>
    /// 触发3元组检查对象
    /// </summary>
    public class TubleTrigger3Object : TubleTrigger<object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger3Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public TubleTrigger3Object(object m1, object m2, object m3) : base(m1, m2, m3) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble3Object(TubleTrigger3Object model)
        {
            return new Tuble3Object(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3Object(Tuble3Object model)
        {
            return new TubleTrigger3Object(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3Object(Tuble<object, object, object> model)
        {
            return new TubleTrigger3Object(model.Item1, model.Item2, model.Item3);
        }
    }
    /// <summary>
    /// 触发3元组检查字符串
    /// </summary>
    public class TubleTrigger3String : TubleTrigger<string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger3String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public TubleTrigger3String(string m1, string m2, string m3) : base(m1, m2, m3) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble3String(TubleTrigger3String model)
        {
            return new Tuble3String(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3String(Tuble3String model)
        {
            return new TubleTrigger3String(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3String(Tuble<string, string, string> model)
        {
            return new TubleTrigger3String(model.Item1, model.Item2, model.Item3);
        }
    }
    /// <summary>
    /// 触发3元组检查字符串
    /// </summary>
    public class TubleTrigger3CheckString : TubleTrigger<bool, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger3CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        public TubleTrigger3CheckString(bool m1, string m2, string m3) : base(m1, m2, m3) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble3CheckString(TubleTrigger3CheckString model)
        {
            return new Tuble3CheckString(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3CheckString(Tuble3CheckString model)
        {
            return new TubleTrigger3CheckString(model.Item1, model.Item2, model.Item3);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger3CheckString(Tuble<bool, string, string> model)
        {
            return new TubleTrigger3CheckString(model.Item1, model.Item2, model.Item3);
        }
    }
    /// <summary>
    /// 触发四元组
    /// </summary>
    public class TubleTrigger<T1, T2, T3, T4> : TubleTriggerPropertyChanged, ITuble<T1, T2, T3, T4>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public TubleTrigger(T1 m1, T2 m2, T3 m3, T4 m4)
        {
            this._item1 = m1;
            this._item2 = m2;
            this._item3 = m3;
            this._item4 = m4;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 项三改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4>, T3> Item3Changed;
        private T3 _item3;
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3
        {
            get => _item3;
            set
            {
                var oldVal = _item3;
                OnPropertyChanged(nameof(Item3), ref _item3, value);
                Item3Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4>, T3>(this, value, oldVal, nameof(Item3)));
            }
        }
        /// <summary>
        /// 项4改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4>, T4> Item4Changed;
        private T4 _item4;
        /// <summary>
        /// 项4
        /// </summary>
        public virtual T4 Item4
        {
            get => _item4;
            set
            {
                var oldVal = _item4;
                OnPropertyChanged(nameof(Item4), ref _item4, value);
                Item4Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4>, T4>(this, value, oldVal, nameof(Item4)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2, T3, T4>(TubleTrigger<T1, T2, T3, T4> model)
        {
            return new Tuble<T1, T2, T3, T4>(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2, T3, T4>(Tuble<T1, T2, T3, T4> model)
        {
            return new TubleTrigger<T1, T2, T3, T4>(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2, T3, T4> Clone() => new TubleTrigger<T1, T2, T3, T4>(Item1, Item2, Item3, Item4);
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        ITuble<T1, T2> ITuble<T1, T2>.Clone() => Clone();
        ITuble<T1, T2, T3> ITuble<T1, T2, T3>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发4元组检查对象
    /// </summary>
    public class TubleTrigger4CheckObject : TubleTrigger<bool, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger4CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public TubleTrigger4CheckObject(bool m1, object m2, object m3, object m4) : base(m1, m2, m3, m4) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble4CheckObject(TubleTrigger4CheckObject model)
        {
            return new Tuble4CheckObject(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4CheckObject(Tuble4CheckObject model)
        {
            return new TubleTrigger4CheckObject(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4CheckObject(Tuble<bool, object, object, object> model)
        {
            return new TubleTrigger4CheckObject(model.Item1, model.Item2, model.Item3, model.Item4);
        }
    }
    /// <summary>
    /// 触发4元组检查对象
    /// </summary>
    public class TubleTrigger4Object : TubleTrigger<object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger4Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public TubleTrigger4Object(object m1, object m2, object m3, object m4) : base(m1, m2, m3, m4) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble4Object(TubleTrigger4Object model)
        {
            return new Tuble4Object(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4Object(Tuble4Object model)
        {
            return new TubleTrigger4Object(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4Object(Tuble<object, object, object, object> model)
        {
            return new TubleTrigger4Object(model.Item1, model.Item2, model.Item3, model.Item4);
        }
    }
    /// <summary>
    /// 触发4元组检查字符串
    /// </summary>
    public class TubleTrigger4String : TubleTrigger<string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger4String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public TubleTrigger4String(string m1, string m2, string m3, string m4) : base(m1, m2, m3, m4) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble4String(TubleTrigger4String model)
        {
            return new Tuble4String(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4String(Tuble4String model)
        {
            return new TubleTrigger4String(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4String(Tuble<string, string, string, string> model)
        {
            return new TubleTrigger4String(model.Item1, model.Item2, model.Item3, model.Item4);
        }
    }
    /// <summary>
    /// 触发4元组检查字符串
    /// </summary>
    public class TubleTrigger4CheckString : TubleTrigger<bool, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger4CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        public TubleTrigger4CheckString(bool m1, string m2, string m3, string m4) : base(m1, m2, m3, m4) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble4CheckString(TubleTrigger4CheckString model)
        {
            return new Tuble4CheckString(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4CheckString(Tuble4CheckString model)
        {
            return new TubleTrigger4CheckString(model.Item1, model.Item2, model.Item3, model.Item4);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger4CheckString(Tuble<bool, string, string, string> model)
        {
            return new TubleTrigger4CheckString(model.Item1, model.Item2, model.Item3, model.Item4);
        }
    }
    /// <summary>
    /// 触发5元组
    /// </summary>
    public class TubleTrigger<T1, T2, T3, T4, T5> : TubleTriggerPropertyChanged, ITuble<T1, T2, T3, T4, T5>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public TubleTrigger(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5)
        {
            this._item1 = m1;
            this._item2 = m2;
            this._item3 = m3;
            this._item4 = m4;
            this._item5 = m5;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 项三改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5>, T3> Item3Changed;
        private T3 _item3;
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3
        {
            get => _item3;
            set
            {
                var oldVal = _item3;
                OnPropertyChanged(nameof(Item3), ref _item3, value);
                Item3Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5>, T3>(this, value, oldVal, nameof(Item3)));
            }
        }
        /// <summary>
        /// 项4改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5>, T4> Item4Changed;
        private T4 _item4;
        /// <summary>
        /// 项4
        /// </summary>
        public virtual T4 Item4
        {
            get => _item4;
            set
            {
                var oldVal = _item4;
                OnPropertyChanged(nameof(Item4), ref _item4, value);
                Item4Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5>, T4>(this, value, oldVal, nameof(Item4)));
            }
        }
        /// <summary>
        /// 项5改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5>, T5> Item5Changed;
        private T5 _item5;
        /// <summary>
        /// 项5
        /// </summary>
        public virtual T5 Item5
        {
            get => _item5;
            set
            {
                var oldVal = _item5;
                OnPropertyChanged(nameof(Item5), ref _item5, value);
                Item5Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5>, T5>(this, value, oldVal, nameof(Item5)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2, T3, T4, T5>(TubleTrigger<T1, T2, T3, T4, T5> model)
        {
            return new Tuble<T1, T2, T3, T4, T5>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2, T3, T4, T5>(Tuble<T1, T2, T3, T4, T5> model)
        {
            return new TubleTrigger<T1, T2, T3, T4, T5>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2, T3, T4, T5> Clone() => new TubleTrigger<T1, T2, T3, T4, T5>(Item1, Item2, Item3, Item4, Item5);
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        ITuble<T1, T2> ITuble<T1, T2>.Clone() => Clone();
        ITuble<T1, T2, T3> ITuble<T1, T2, T3>.Clone() => Clone();
        ITuble<T1, T2, T3, T4> ITuble<T1, T2, T3, T4>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发5元组检查对象
    /// </summary>
    public class TubleTrigger5CheckObject : TubleTrigger<bool, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger5CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public TubleTrigger5CheckObject(bool m1, object m2, object m3, object m4, object m5) : base(m1, m2, m3, m4, m5) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble5CheckObject(TubleTrigger5CheckObject model)
        {
            return new Tuble5CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5CheckObject(Tuble5CheckObject model)
        {
            return new TubleTrigger5CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5CheckObject(Tuble<bool, object, object, object, object> model)
        {
            return new TubleTrigger5CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
    }
    /// <summary>
    /// 触发5元组检查对象
    /// </summary>
    public class TubleTrigger5Object : TubleTrigger<object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger5Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public TubleTrigger5Object(object m1, object m2, object m3, object m4, object m5) : base(m1, m2, m3, m4, m5) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble5Object(TubleTrigger5Object model)
        {
            return new Tuble5Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5Object(Tuble5Object model)
        {
            return new TubleTrigger5Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5Object(Tuble<object, object, object, object, object> model)
        {
            return new TubleTrigger5Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
    }
    /// <summary>
    /// 触发5元组检查字符串
    /// </summary>
    public class TubleTrigger5String : TubleTrigger<string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger5String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public TubleTrigger5String(string m1, string m2, string m3, string m4, string m5) : base(m1, m2, m3, m4, m5) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble5String(TubleTrigger5String model)
        {
            return new Tuble5String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5String(Tuble5String model)
        {
            return new TubleTrigger5String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5String(Tuble<string, string, string, string, string> model)
        {
            return new TubleTrigger5String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
    }
    /// <summary>
    /// 触发5元组检查字符串
    /// </summary>
    public class TubleTrigger5CheckString : TubleTrigger<bool, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger5CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        public TubleTrigger5CheckString(bool m1, string m2, string m3, string m4, string m5) : base(m1, m2, m3, m4, m5) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble5CheckString(TubleTrigger5CheckString model)
        {
            return new Tuble5CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5CheckString(Tuble5CheckString model)
        {
            return new TubleTrigger5CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger5CheckString(Tuble<bool, string, string, string, string> model)
        {
            return new TubleTrigger5CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
        }
    }
    /// <summary>
    /// 触发6元组
    /// </summary>
    public class TubleTrigger<T1, T2, T3, T4, T5, T6> : TubleTriggerPropertyChanged, ITuble<T1, T2, T3, T4, T5, T6>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public TubleTrigger(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6)
        {
            this._item1 = m1;
            this._item2 = m2;
            this._item3 = m3;
            this._item4 = m4;
            this._item5 = m5;
            this._item6 = m6;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 项三改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6>, T3> Item3Changed;
        private T3 _item3;
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3
        {
            get => _item3;
            set
            {
                var oldVal = _item3;
                OnPropertyChanged(nameof(Item3), ref _item3, value);
                Item3Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6>, T3>(this, value, oldVal, nameof(Item3)));
            }
        }
        /// <summary>
        /// 项4改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6>, T4> Item4Changed;
        private T4 _item4;
        /// <summary>
        /// 项4
        /// </summary>
        public virtual T4 Item4
        {
            get => _item4;
            set
            {
                var oldVal = _item4;
                OnPropertyChanged(nameof(Item4), ref _item4, value);
                Item4Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6>, T4>(this, value, oldVal, nameof(Item4)));
            }
        }
        /// <summary>
        /// 项5改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6>, T5> Item5Changed;
        private T5 _item5;
        /// <summary>
        /// 项5
        /// </summary>
        public virtual T5 Item5
        {
            get => _item5;
            set
            {
                var oldVal = _item5;
                OnPropertyChanged(nameof(Item5), ref _item5, value);
                Item5Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6>, T5>(this, value, oldVal, nameof(Item5)));
            }
        }
        /// <summary>
        /// 项6改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6>, T6> Item6Changed;
        private T6 _item6;
        /// <summary>
        /// 项6
        /// </summary>
        public virtual T6 Item6
        {
            get => _item6;
            set
            {
                var oldVal = _item6;
                OnPropertyChanged(nameof(Item6), ref _item6, value);
                Item6Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6>, T6>(this, value, oldVal, nameof(Item6)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2, T3, T4, T5, T6>(TubleTrigger<T1, T2, T3, T4, T5, T6> model)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2, T3, T4, T5, T6>(Tuble<T1, T2, T3, T4, T5, T6> model)
        {
            return new TubleTrigger<T1, T2, T3, T4, T5, T6>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2, T3, T4, T5, T6> Clone() => new TubleTrigger<T1, T2, T3, T4, T5, T6>(Item1, Item2, Item3, Item4, Item5, Item6);
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        ITuble<T1, T2> ITuble<T1, T2>.Clone() => Clone();
        ITuble<T1, T2, T3> ITuble<T1, T2, T3>.Clone() => Clone();
        ITuble<T1, T2, T3, T4> ITuble<T1, T2, T3, T4>.Clone() => Clone();
        ITuble<T1, T2, T3, T4, T5> ITuble<T1, T2, T3, T4, T5>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发6元组检查对象
    /// </summary>
    public class TubleTrigger6CheckObject : TubleTrigger<bool, object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger6CheckObject() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public TubleTrigger6CheckObject(bool m1, object m2, object m3, object m4, object m5, object m6) : base(m1, m2, m3, m4, m5, m6) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble6CheckObject(TubleTrigger6CheckObject model)
        {
            return new Tuble6CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6CheckObject(Tuble6CheckObject model)
        {
            return new TubleTrigger6CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6CheckObject(Tuble<bool, object, object, object, object, object> model)
        {
            return new TubleTrigger6CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
    }
    /// <summary>
    /// 触发6元组检查对象
    /// </summary>
    public class TubleTrigger6Object : TubleTrigger<object, object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger6Object() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public TubleTrigger6Object(object m1, object m2, object m3, object m4, object m5, object m6) : base(m1, m2, m3, m4, m5, m6) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble6Object(TubleTrigger6Object model)
        {
            return new Tuble6Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6Object(Tuble6Object model)
        {
            return new TubleTrigger6Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6Object(Tuble<object, object, object, object, object, object> model)
        {
            return new TubleTrigger6Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
    }
    /// <summary>
    /// 触发6元组检查字符串
    /// </summary>
    public class TubleTrigger6String : TubleTrigger<string, string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger6String() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public TubleTrigger6String(string m1, string m2, string m3, string m4, string m5, string m6) : base(m1, m2, m3, m4, m5, m6) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble6String(TubleTrigger6String model)
        {
            return new Tuble6String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6String(Tuble6String model)
        {
            return new TubleTrigger6String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6String(Tuble<string, string, string, string, string, string> model)
        {
            return new TubleTrigger6String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
    }
    /// <summary>
    /// 触发6元组检查字符串
    /// </summary>
    public class TubleTrigger6CheckString : TubleTrigger<bool, string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger6CheckString() { }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="m3"></param>
        /// <param name="m4"></param>
        /// <param name="m5"></param>
        /// <param name="m6"></param>
        public TubleTrigger6CheckString(bool m1, string m2, string m3, string m4, string m5, string m6) : base(m1, m2, m3, m4, m5, m6) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble6CheckString(TubleTrigger6CheckString model)
        {
            return new Tuble6CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6CheckString(Tuble6CheckString model)
        {
            return new TubleTrigger6CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger6CheckString(Tuble<bool, string, string, string, string, string> model)
        {
            return new TubleTrigger6CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
        }
    }
    /// <summary>
    /// 触发7元组
    /// </summary>
    public class TubleTrigger<T1, T2, T3, T4, T5, T6, T7> : TubleTriggerPropertyChanged, ITuble<T1, T2, T3, T4, T5, T6, T7>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
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
        public TubleTrigger(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6, T7 m7)
        {
            this._item1 = m1;
            this._item2 = m2;
            this._item3 = m3;
            this._item4 = m4;
            this._item5 = m5;
            this._item6 = m6;
            this._item7 = m7;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 项三改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T3> Item3Changed;
        private T3 _item3;
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3
        {
            get => _item3;
            set
            {
                var oldVal = _item3;
                OnPropertyChanged(nameof(Item3), ref _item3, value);
                Item3Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T3>(this, value, oldVal, nameof(Item3)));
            }
        }
        /// <summary>
        /// 项4改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T4> Item4Changed;
        private T4 _item4;
        /// <summary>
        /// 项4
        /// </summary>
        public virtual T4 Item4
        {
            get => _item4;
            set
            {
                var oldVal = _item4;
                OnPropertyChanged(nameof(Item4), ref _item4, value);
                Item4Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T4>(this, value, oldVal, nameof(Item4)));
            }
        }
        /// <summary>
        /// 项5改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T5> Item5Changed;
        private T5 _item5;
        /// <summary>
        /// 项5
        /// </summary>
        public virtual T5 Item5
        {
            get => _item5;
            set
            {
                var oldVal = _item5;
                OnPropertyChanged(nameof(Item5), ref _item5, value);
                Item5Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T5>(this, value, oldVal, nameof(Item5)));
            }
        }
        /// <summary>
        /// 项6改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T6> Item6Changed;
        private T6 _item6;
        /// <summary>
        /// 项6
        /// </summary>
        public virtual T6 Item6
        {
            get => _item6;
            set
            {
                var oldVal = _item6;
                OnPropertyChanged(nameof(Item6), ref _item6, value);
                Item6Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T6>(this, value, oldVal, nameof(Item6)));
            }
        }
        /// <summary>
        /// 项7改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T7> Item7Changed;
        private T7 _item7;
        /// <summary>
        /// 项7
        /// </summary>
        public virtual T7 Item7
        {
            get => _item7;
            set
            {
                var oldVal = _item7;
                OnPropertyChanged(nameof(Item7), ref _item7, value);
                Item7Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7>, T7>(this, value, oldVal, nameof(Item7)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2, T3, T4, T5, T6, T7>(TubleTrigger<T1, T2, T3, T4, T5, T6, T7> model)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2, T3, T4, T5, T6, T7>(Tuble<T1, T2, T3, T4, T5, T6, T7> model)
        {
            return new TubleTrigger<T1, T2, T3, T4, T5, T6, T7>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2, T3, T4, T5, T6, T7> Clone() => new TubleTrigger<T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, Item3, Item4, Item5, Item6, Item7);
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        ITuble<T1, T2> ITuble<T1, T2>.Clone() => Clone();
        ITuble<T1, T2, T3> ITuble<T1, T2, T3>.Clone() => Clone();
        ITuble<T1, T2, T3, T4> ITuble<T1, T2, T3, T4>.Clone() => Clone();
        ITuble<T1, T2, T3, T4, T5> ITuble<T1, T2, T3, T4, T5>.Clone() => Clone();
        ITuble<T1, T2, T3, T4, T5, T6> ITuble<T1, T2, T3, T4, T5, T6>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发7元组检查对象
    /// </summary>
    public class TubleTrigger7CheckObject : TubleTrigger<bool, object, object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger7CheckObject() { }
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
        public TubleTrigger7CheckObject(bool m1, object m2, object m3, object m4, object m5, object m6, object m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble7CheckObject(TubleTrigger7CheckObject model)
        {
            return new Tuble7CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7CheckObject(Tuble7CheckObject model)
        {
            return new TubleTrigger7CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7CheckObject(Tuble<bool, object, object, object, object, object, object> model)
        {
            return new TubleTrigger7CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
    }
    /// <summary>
    /// 触发7元组检查对象
    /// </summary>
    public class TubleTrigger7Object : TubleTrigger<object, object, object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger7Object() { }
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
        public TubleTrigger7Object(object m1, object m2, object m3, object m4, object m5, object m6, object m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble7Object(TubleTrigger7Object model)
        {
            return new Tuble7Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7Object(Tuble7Object model)
        {
            return new TubleTrigger7Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7Object(Tuble<object, object, object, object, object, object, object> model)
        {
            return new TubleTrigger7Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
    }
    /// <summary>
    /// 触发7元组检查字符串
    /// </summary>
    public class TubleTrigger7String : TubleTrigger<string, string, string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger7String() { }
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
        public TubleTrigger7String(string m1, string m2, string m3, string m4, string m5, string m6, string m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble7String(TubleTrigger7String model)
        {
            return new Tuble7String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7String(Tuble7String model)
        {
            return new TubleTrigger7String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7String(Tuble<string, string, string, string, string, string, string> model)
        {
            return new TubleTrigger7String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
    }
    /// <summary>
    /// 触发7元组检查字符串
    /// </summary>
    public class TubleTrigger7CheckString : TubleTrigger<bool, string, string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger7CheckString() { }
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
        public TubleTrigger7CheckString(bool m1, string m2, string m3, string m4, string m5, string m6, string m7) : base(m1, m2, m3, m4, m5, m6, m7) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble7CheckString(TubleTrigger7CheckString model)
        {
            return new Tuble7CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7CheckString(Tuble7CheckString model)
        {
            return new TubleTrigger7CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger7CheckString(Tuble<bool, string, string, string, string, string, string> model)
        {
            return new TubleTrigger7CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
        }
    }
    /// <summary>
    /// 触发8元组
    /// </summary>
    public class TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8> : TubleTriggerPropertyChanged, ITuble<T1, T2, T3, T4, T5, T6, T7, T8>, ICloneable
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger() { }
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
        public TubleTrigger(T1 m1, T2 m2, T3 m3, T4 m4, T5 m5, T6 m6, T7 m7, T8 m8)
        {
            this._item1 = m1;
            this._item2 = m2;
            this._item3 = m3;
            this._item4 = m4;
            this._item5 = m5;
            this._item6 = m6;
            this._item7 = m7;
            this._item8 = m8;
        }
        /// <summary>
        /// 项一改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T1> Item1Changed;
        private T1 _item1;
        /// <summary>
        /// 项一
        /// </summary>
        public virtual T1 Item1
        {
            get => _item1;
            set
            {
                var oldVal = _item1;
                OnPropertyChanged(nameof(Item1), ref _item1, value);
                Item1Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T1>(this, value, oldVal, nameof(Item1)));
            }
        }
        /// <summary>
        /// 项二改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T2> Item2Changed;
        private T2 _item2;
        /// <summary>
        /// 项二
        /// </summary>
        public virtual T2 Item2
        {
            get => _item2;
            set
            {
                var oldVal = _item2;
                OnPropertyChanged(nameof(Item2), ref _item2, value);
                Item2Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T2>(this, value, oldVal, nameof(Item2)));
            }
        }
        /// <summary>
        /// 项三改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T3> Item3Changed;
        private T3 _item3;
        /// <summary>
        /// 项三
        /// </summary>
        public virtual T3 Item3
        {
            get => _item3;
            set
            {
                var oldVal = _item3;
                OnPropertyChanged(nameof(Item3), ref _item3, value);
                Item3Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T3>(this, value, oldVal, nameof(Item3)));
            }
        }
        /// <summary>
        /// 项4改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T4> Item4Changed;
        private T4 _item4;
        /// <summary>
        /// 项4
        /// </summary>
        public virtual T4 Item4
        {
            get => _item4;
            set
            {
                var oldVal = _item4;
                OnPropertyChanged(nameof(Item4), ref _item4, value);
                Item4Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T4>(this, value, oldVal, nameof(Item4)));
            }
        }
        /// <summary>
        /// 项5改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T5> Item5Changed;
        private T5 _item5;
        /// <summary>
        /// 项5
        /// </summary>
        public virtual T5 Item5
        {
            get => _item5;
            set
            {
                var oldVal = _item5;
                OnPropertyChanged(nameof(Item5), ref _item5, value);
                Item5Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T5>(this, value, oldVal, nameof(Item5)));
            }
        }
        /// <summary>
        /// 项6改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T6> Item6Changed;
        private T6 _item6;
        /// <summary>
        /// 项6
        /// </summary>
        public virtual T6 Item6
        {
            get => _item6;
            set
            {
                var oldVal = _item6;
                OnPropertyChanged(nameof(Item6), ref _item6, value);
                Item6Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T6>(this, value, oldVal, nameof(Item6)));
            }
        }
        /// <summary>
        /// 项7改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T7> Item7Changed;
        private T7 _item7;
        /// <summary>
        /// 项7
        /// </summary>
        public virtual T7 Item7
        {
            get => _item7;
            set
            {
                var oldVal = _item7;
                OnPropertyChanged(nameof(Item7), ref _item7, value);
                Item7Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T7>(this, value, oldVal, nameof(Item7)));
            }
        }
        /// <summary>
        /// 项8改变值
        /// </summary>
        public event TubleTriggerChanged<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T8> Item8Changed;
        private T8 _item8;
        /// <summary>
        /// 项8
        /// </summary>
        public virtual T8 Item8
        {
            get => _item8;
            set
            {
                var oldVal = _item8;
                OnPropertyChanged(nameof(Item8), ref _item8, value);
                Item8Changed?.Invoke(new Tuble4Changed<TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>, T8>(this, value, oldVal, nameof(Item8)));
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble<T1, T2, T3, T4, T5, T6, T7, T8>(TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8> model)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7, T8>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>(Tuble<T1, T2, T3, T4, T5, T6, T7, T8> model)
        {
            return new TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual ITuble<T1, T2, T3, T4, T5, T6, T7, T8> Clone()
            => new TubleTrigger<T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);
        ITuble<T1> ITuble<T1>.Clone() => Clone();
        ITuble<T1, T2> ITuble<T1, T2>.Clone() => Clone();
        ITuble<T1, T2, T3> ITuble<T1, T2, T3>.Clone() => Clone();
        ITuble<T1, T2, T3, T4> ITuble<T1, T2, T3, T4>.Clone() => Clone();
        ITuble<T1, T2, T3, T4, T5> ITuble<T1, T2, T3, T4, T5>.Clone() => Clone();
        ITuble<T1, T2, T3, T4, T5, T6> ITuble<T1, T2, T3, T4, T5, T6>.Clone() => Clone();
        ITuble<T1, T2, T3, T4, T5, T6, T7> ITuble<T1, T2, T3, T4, T5, T6, T7>.Clone() => Clone();
        object ICloneable.Clone() => Clone();
    }
    /// <summary>
    /// 触发8元组检查对象
    /// </summary>
    public class TubleTrigger8CheckObject : TubleTrigger<bool, object, object, object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger8CheckObject() { }
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
        public TubleTrigger8CheckObject(bool m1, object m2, object m3, object m4, object m5, object m6, object m7, object m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble8CheckObject(TubleTrigger8CheckObject model)
        {
            return new Tuble8CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8CheckObject(Tuble8CheckObject model)
        {
            return new TubleTrigger8CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8CheckObject(Tuble<bool, object, object, object, object, object, object, object> model)
        {
            return new TubleTrigger8CheckObject(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
    }
    /// <summary>
    /// 触发8元组检查字符串
    /// </summary>
    public class TubleTrigger8CheckString : TubleTrigger<bool, string, string, string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger8CheckString() { }
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
        public TubleTrigger8CheckString(bool m1, string m2, string m3, string m4, string m5, string m6, string m7, string m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble8CheckString(TubleTrigger8CheckString model)
        {
            return new Tuble8CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8CheckString(Tuble8CheckString model)
        {
            return new TubleTrigger8CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8CheckString(Tuble<bool, string, string, string, string, string, string, string> model)
        {
            return new TubleTrigger8CheckString(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
    }
    /// <summary>
    /// 触发8元组检查对象
    /// </summary>
    public class TubleTrigger8Object : TubleTrigger<object, object, object, object, object, object, object, object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger8Object() { }
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
        public TubleTrigger8Object(object m1, object m2, object m3, object m4, object m5, object m6, object m7, object m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble8Object(TubleTrigger8Object model)
        {
            return new Tuble8Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8Object(Tuble8Object model)
        {
            return new TubleTrigger8Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8Object(Tuble<object, object, object, object, object, object, object, object> model)
        {
            return new TubleTrigger8Object(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
    }
    /// <summary>
    /// 触发8元组检查字符串
    /// </summary>
    public class TubleTrigger8String : TubleTrigger<string, string, string, string, string, string, string, string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TubleTrigger8String() { }
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
        public TubleTrigger8String(string m1, string m2, string m3, string m4, string m5, string m6, string m7, string m8) : base(m1, m2, m3, m4, m5, m6, m7, m8) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator Tuble8String(TubleTrigger8String model)
        {
            return new Tuble8String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8String(Tuble8String model)
        {
            return new TubleTrigger8String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator TubleTrigger8String(Tuble<string, string, string, string, string, string, string, string> model)
        {
            return new TubleTrigger8String(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7, model.Item8);
        }
    }
    #endregion
}
