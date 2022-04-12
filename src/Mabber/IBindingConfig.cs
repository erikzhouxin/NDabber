using System;
using System.Linq.Expressions;

namespace System.Data.Mabber
{
    /// <summary>
    /// 绑定配置
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IBindingConfig<TSource, TTarget>
    {
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Bind(Expression<Func<TSource, object>> source, Expression<Func<TTarget, object>> target);
        // void Bind<TField>(Expression<Func<TTarget, TField>> target, TField value); not working yet
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        void Bind(Expression<Func<TTarget, object>> target, Type targetType);
        /// <summary>
        /// 忽略
        /// </summary>
        /// <param name="expression"></param>
        void Ignore(Expression<Func<TSource, object>> expression);
    }
}
