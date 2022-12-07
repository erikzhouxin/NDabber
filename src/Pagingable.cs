using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 分页结果接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPageResult<out T> : IPageResult
    {
        /// <summary>
        /// 结果项
        /// </summary>
        new IEnumerable<T> Items { get; }
        /// <summary>
        /// 重置结果项
        /// </summary>
        /// <returns></returns>
        new IPageResult<T> Reset<TRes>(IEnumerable<TRes> items);
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        new IPageResult<T> Clone();
    }
    /// <summary>
    /// 分页结果接口
    /// </summary>
    public interface IPageResult
    {
        /// <summary>
        /// 页面
        /// </summary>
        int Page { get; set; }
        /// <summary>
        /// 每页长度
        /// </summary>
        int Size { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        int TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPage { get; }
        /// <summary>
        /// 查询条件
        /// </summary>
        string Search { get; set; }
        /// <summary>
        /// 结果项
        /// </summary>
        IEnumerable<object> Items { get; set; }
        /// <summary>
        /// 跳过
        /// </summary>
        int Skip { get; }
        /// <summary>
        /// 获取
        /// </summary>
        int Take { get; }
        /// <summary>
        /// 是客户端分页
        /// </summary>
        bool IsClient { get; set; }
        /// <summary>
        /// 重置结果项
        /// </summary>
        /// <returns></returns>
        IPageResult Reset();
        /// <summary>
        /// 重置结果项
        /// </summary>
        /// <returns></returns>
        IPageResult Reset<TRes>(IEnumerable<TRes> items);
        /// <summary>
        /// 复制结果
        /// </summary>
        /// <returns></returns>
        IPageResult Clone();
    }
    /// <summary>
    /// 查询结果
    /// </summary>
    public class PagingResult : PagingResult<object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public PagingResult() : base() { }
        /// <summary>
        /// 构造
        /// </summary>
        public PagingResult(int page, int size) : base(page, size) { }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="paging"></param>
        public static implicit operator PagingResult(Tuple<int, int> paging) => new PagingResult(paging.Item1, paging.Item2);
        /// <summary>
        /// 升级成泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PagingResult<T> Upgrade<T>()
        {
            return (this as PagingResult<T>) ?? new PagingResult<T>(Page, Size);
        }
    }
    /// <summary>
    /// 查询结果类
    /// </summary>
    public class PagingResult<T> : IPageResult<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public PagingResult() : this(1, 20) { }
        /// <summary>
        /// 模型构造
        /// </summary>
        public PagingResult(int page, int size)
        {
            Page = page;
            Size = size;
            Items = new List<T>();
        }
        /// <summary>
        /// 页面
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 每页长度
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get { return CeilDividend(TotalCount, Size); } }
        /// <summary>
        /// 查询条件
        /// </summary>
        public string Search { get; set; }
        /// <summary>
        /// 结果项
        /// </summary>
        public IEnumerable<T> Items { get; set; }
        /// <summary>
        /// 跳过
        /// </summary>
        public int Skip { get { return (Page - 1) * Size; } }
        /// <summary>
        /// 获取
        /// </summary>
        public int Take { get { return Size; } }
        /// <summary>
        /// 是客户端分页
        /// </summary>
        public bool IsClient { get; set; }

        IEnumerable<object> IPageResult.Items { get => Items as IEnumerable<object>; set => Items = value as IEnumerable<T>; }

        /// <summary>
        /// 进一除法
        /// </summary>
        /// <param name="devidend">被除数</param>
        /// <param name="divisor">除数</param>
        /// <returns></returns>
        public static int CeilDividend(int devidend, int divisor)
        {
            if (divisor == 0 || devidend == 0) { return 1; }
            return (int)Math.Ceiling(((decimal)devidend) / divisor);
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public IPageResult Reset()
        {
            Items = new List<T>();
            return this;
        }
        /// <summary>
        /// 重设项
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        IPageResult IPageResult.Reset<TRes>(IEnumerable<TRes> items)
        {
            Items = items as IEnumerable<T>;
            return this;
        }
        /// <summary>
        /// 重设项
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public IPageResult<T> Reset<TRes>(IEnumerable<TRes> items)
        {
            Items = items as IEnumerable<T>;
            return this;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="paging"></param>
        public static implicit operator List<T>(PagingResult<T> paging)
        {
            return paging.Items.AsList();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="items"></param>
        public static implicit operator PagingResult<T>(List<T> items)
        {
            return new PagingResult<T>(1, 20)
            {
                Items = items,
                IsClient = true,
                TotalCount = items.Count()
            };
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="items"></param>
        public static implicit operator PagingResult<T>(ObservableCollection<T> items)
        {
            return new PagingResult<T>(1, 20)
            {
                Items = items,
                IsClient = true,
                TotalCount = items.Count()
            };
        }
        /// <summary>
        /// 克隆内容
        /// </summary>
        /// <returns></returns>
        public virtual PagingResult<T> Clone()
        {
            return new PagingResult<T>(Page, Size)
            {
                Items = Items,
                IsClient = IsClient,
                TotalCount = TotalCount,
                Search = Search,
            };
        }
        IPageResult IPageResult.Clone() => this.Clone();
        IPageResult<T> IPageResult<T>.Clone() => this.Clone();
    }
}
