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
            return new Tuble<T1, T2>
            {
                Item1 = model1,
                Item2 = model2,
            };
        }
        /// <summary>
        /// 创建三元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3> Create<T1, T2, T3>(T1 model1, T2 model2, T3 model3)
        {
            return new Tuble<T1, T2, T3>
            {
                Item1 = model1,
                Item2 = model2,
                Item3 = model3,
            };
        }
        /// <summary>
        /// 创建四元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 model1, T2 model2, T3 model3, T4 model4)
        {
            return new Tuble<T1, T2, T3, T4>
            {
                Item1 = model1,
                Item2 = model2,
                Item3 = model3,
                Item4 = model4,
            };
        }
        /// <summary>
        /// 创建五元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5)
        {
            return new Tuble<T1, T2, T3, T4, T5>
            {
                Item1 = model1,
                Item2 = model2,
                Item3 = model3,
                Item4 = model4,
                Item5 = model5,
            };
        }
        /// <summary>
        /// 创建六元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6>
            {
                Item1 = model1,
                Item2 = model2,
                Item3 = model3,
                Item4 = model4,
                Item5 = model5,
                Item6 = model6,
            };
        }
        /// <summary>
        /// 创建七元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7>
            {
                Item1 = model1,
                Item2 = model2,
                Item3 = model3,
                Item4 = model4,
                Item5 = model5,
                Item6 = model6,
                Item7 = model7,
            };
        }
        /// <summary>
        /// 创建八元组
        /// </summary>
        /// <returns></returns>
        public static Tuble<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 model1, T2 model2, T3 model3, T4 model4, T5 model5, T6 model6, T7 model7, T8 model8)
        {
            return new Tuble<T1, T2, T3, T4, T5, T6, T7, T8>
            {
                Item1 = model1,
                Item2 = model2,
                Item3 = model3,
                Item4 = model4,
                Item5 = model5,
                Item6 = model6,
                Item7 = model7,
                Item8 = model8,
            };
        }
    }
    #region // 扩展类
    /// <summary>
    /// 二元组
    /// </summary>
    public class Tuble<T1, T2>
    {
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
    /// 三元组
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class Tuble<T1, T2, T3> : Tuble<T1, T2>
    {
        /// <summary>
        /// 项三
        /// </summary>
        public T3 Item3 { get; set; }
    }
    /// <summary>
    /// 三元组[标识:ID,名称:Name,值:Value]
    /// </summary>
    public class Tuble3A001 : Tuble<String, String, int>
    {
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
        /// 项八
        /// </summary>
        public T8 Item8 { get; set; }
    }
    #endregion
}
