using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Mabber
{
    /// <summary>
    /// 用户映射
    /// </summary>
    public static class UserMapper
    {
        /// <summary>
        /// 映射(非内部类)
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="srcObj"></param>
        /// <returns></returns>
        public static TOut Map<TIn, TOut>(this TIn srcObj) where TOut : new()
        {
            return UserMapper<TIn, TOut>.Map(srcObj);
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public static TOut Map<TIn, TOut>(this TIn srcObj, TOut tagObj)
        {
            return UserMapper<TIn, TOut>.Map(srcObj, tagObj);
        }
        /// <summary>
        /// 映射(非内部类)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> srcObjs)
        {
            foreach (var item in srcObjs)
            {
                yield return UserMapper<TIn, TOut>.Map(item);
            }
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> srcObjs, Func<TOut> GetModel)
        {
            foreach (var item in srcObjs)
            {
                yield return UserMapper<TIn, TOut>.Map(item, GetModel());
            }
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> srcObjs, bool isNew, params object[] args)
        {
            if (isNew)
            {
                return UserMapper<TIn, TOut>.Map(srcObjs, () => (TOut)Activator.CreateInstance(typeof(TOut), args));
            }
            var result = new List<TOut>();
            foreach (var item in srcObjs)
            {
                result.Add(UserMapper<TIn, TOut>.Map(item));
            }
            return result;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        public static void Regist<TIn, TOut>()
        {
            UserMapper<TIn, TOut>.DoNoting();
        }
        /// <summary>
        /// 注册(推荐使用泛型注册方法)
        /// </summary>
        public static void Regist(Type typeIn, Type typeOut)
        {
            var type = typeof(UserMapper<,>).MakeGenericType(typeIn, typeOut);
            type.GetMethod("DoNoting", Reflection.BindingFlags.Public | Reflection.BindingFlags.Static).Invoke(null, null);
        }
    }
    /// <summary>
    /// 用户映射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UserMapper<T> : UserMapper<T, T> { }
    /// <summary>
    /// 用户映射
    /// </summary>
    /// <typeparam name="Tin"></typeparam>
    /// <typeparam name="Tout"></typeparam>
    public class UserMapper<Tin, Tout>
    {
        static UserMapper()
        {
            TinyMapper.Bind<Tin, Tout>();
        }
        /// <summary>
        /// 添加绑定
        /// </summary>
        /// <param name="config"></param>
        public static void Bind(Action<IBindingConfig<Tin, Tout>> config)
        {
            TinyMapper.Bind(config);
        }
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="config"></param>
        public static void Config(Action<ITinyMapperConfig> config)
        {
            TinyMapper.Config(config);
        }
        /// <summary>
        /// 映射(非内部类)
        /// </summary>
        /// <returns></returns>
        public static Tout Map(Tin srcObj)
        {
            return TinyMapper.Map<Tin, Tout>(srcObj);
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public static Tout Map(Tin srcObj, Tout tagObj)
        {
            return TinyMapper.Map(srcObj, tagObj);
        }
        /// <summary>
        /// 映射(非内部类)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tout> Map(IEnumerable<Tin> srcObjs)
        {
            foreach (var item in srcObjs)
            {
                yield return TinyMapper.Map<Tin, Tout>(item);
            }
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tout> Map(IEnumerable<Tin> srcObjs, Func<Tout> GetModel)
        {
            foreach (var item in srcObjs)
            {
                yield return TinyMapper.Map(item, GetModel());
            }
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tout> Map(IEnumerable<Tin> srcObjs, bool isNew, params object[] args)
        {
            if (isNew)
            {
                return Map(srcObjs, () => (Tout)Activator.CreateInstance(typeof(Tout), args));
            }
            var result = new List<Tout>();
            foreach (var item in srcObjs)
            {
                result.Add(TinyMapper.Map<Tin, Tout>(item));
            }
            return result;
        }
        /// <summary>
        /// 映射(非内部类)
        /// </summary>
        /// <returns></returns>
        public virtual Tout Get(Tin srcObj)
        {
            return TinyMapper.Map<Tin, Tout>(srcObj);
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public virtual Tout Get(Tin srcObj, Tout tagObj)
        {
            return TinyMapper.Map(srcObj, tagObj);
        }
        /// <summary>
        /// 映射(非内部类)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Tout> Get(IEnumerable<Tin> srcObjs)
        {
            foreach (var item in srcObjs)
            {
                yield return TinyMapper.Map<Tin, Tout>(item);
            }
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Tout> Get(IEnumerable<Tin> srcObjs, Func<Tout> GetModel)
        {
            foreach (var item in srcObjs)
            {
                yield return TinyMapper.Map(item, GetModel());
            }
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Tout> Get(IEnumerable<Tin> srcObjs, bool isNew, params object[] args)
        {
            if (isNew)
            {
                return Map(srcObjs, () => (Tout)Activator.CreateInstance(typeof(Tout), args));
            }
            var result = new List<Tout>();
            foreach (var item in srcObjs)
            {
                result.Add(TinyMapper.Map<Tin, Tout>(item));
            }
            return result;
        }
        /// <summary>
        /// 什么也不做
        /// </summary>
        public static void DoNoting() { }
        /*
         * todo: 
         * 1.多重接口集成时映射当前接口内容，没有追溯接口链
         * 
         */
    }
}
