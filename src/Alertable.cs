using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace System.Data.Dabber
{
    #region // 原生Alert
    /// <summary>
    /// 提示信息动态实现类
    /// 三级基础动态初始类(Data=>(dynamic)null)
    /// </summary>
    public class AlertMsg : AlertMsg<dynamic>
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        public static string SplitString { get; } = "\r\n";
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMsg(bool isSuccess = false, string message = "") : base(isSuccess, message)
        {
        }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMsg(string message) : base(false, message)
        {
        }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMsg(string fmt, params object[] param) : base(false, fmt, param)
        {
        }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMsg(bool isSuccess, string fmt, params object[] param) : base(isSuccess, fmt, param)
        {
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator AlertMsg(Exception ex) => new AlertMsg(false, "操作失败,错误:{0}", ex.Message);
        /// <summary>
        /// 成功操作空
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static AlertMsg SuccessOperEmpty(string msg = "操作成功")
        {
            return new AlertMsg(true, msg)
            {
                Data = new
                {
                    EffLine = 0
                }
            };
        }
        /// <summary>
        /// 失败操作空
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static AlertMsg FailOperEmpty(string msg = "操作失败")
        {
            return new AlertMsg(false, msg)
            {
                Data = new
                {
                    EffLine = 0
                }
            };
        }
        /// <summary>
        /// 操作未实现
        /// </summary>
        /// <returns></returns>
        public static new AlertMsg NotImplement { get => new AlertMsg(false, "操作未实现"); }
        /// <summary>
        /// 操作失败
        /// </summary>
        /// <returns></returns>
        public static AlertMsg OperError { get => new AlertMsg(false, "操作失败"); }
        /// <summary>
        /// 操作成功
        /// </summary>
        /// <returns></returns>
        public static AlertMsg OperSuccess { get => new AlertMsg(true, "操作成功"); }
    }
    /// <summary>
    /// 提示信息动态实现类
    /// 三级基础动态初始类(Data=>new ExpandoObject())
    /// </summary>
    public class AlertMessage : AlertMsg
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMessage(bool isSuccess = false, string message = "") : base(isSuccess, message)
        {
            Data = new ExpandoObject();
        }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMessage(string message) : base(false, message)
        {
            Data = new ExpandoObject();
        }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMessage(string fmt, params object[] param) : base(false, fmt, param)
        {
            Data = new ExpandoObject();
        }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMessage(bool isSuccess, string fmt, params object[] param) : base(isSuccess, fmt, param)
        {
            Data = new ExpandoObject();
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <returns></returns>
        public AlertMessage SetData(string key, object value)
        {
            (Data as IDictionary<string, object>)[key] = value;
            return this;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <returns></returns>
        public AlertMessage SetError(string key, object value)
        {
            IsSuccess = false;
            (Data as IDictionary<string, object>)[key] = value;
            return this;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator AlertMessage(string msg)
        {
            return new AlertMessage(msg);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator AlertMessage(PagingResult data)
        {
            return new AlertMessage(true) { Data = data };
        }
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMessage(Tuple<Object, string> res)
        {
            if (res.Item1 == null)
            {
                return new AlertMessage(false, res.Item2 ?? "NotFound")
                {
                    Code = 404,
                };
            }
            return new AlertMessage(true, "数据已加载") { Data = res.Item1 };
        }
        /// <summary>
        /// 获取一个简单提示消息
        /// </summary>
        /// <returns></returns>
        public new static AlertMessage Get(bool isSuccess, string msg)
        {
            return new AlertMessage(isSuccess, msg);
        }
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMessage(Tuple<bool, string> res) => new AlertMessage(res.Item1, res.Item2);
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator AlertMessage(Exception ex) => new AlertMessage(false, "操作失败,错误:{0}", ex.Message);
    }
    /// <summary>
    /// 提示信息泛型实现类
    /// 三级基础泛型实现类(Data=>(T)null)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlertMsg<T> : IAlertMsg<T>, IAlert<T>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMsg(bool isSuccess = false, string message = "")
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMsg(string message) : this(false, message)
        {
        }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMsg(string fmt, params object[] param) : this(false, fmt, param)
        {
        }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMsg(bool isSuccess, string fmt, params object[] param) : this(isSuccess, string.Format(fmt, param))
        {
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public virtual bool IsSuccess { get; set; }
        /// <summary>
        /// 标识代码
        /// </summary>
        public virtual int Code { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public virtual string Message { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public virtual T Data { get; set; }
        dynamic IAlertMsg.Data { get => Data; set => Data = value; }
        dynamic IAlertResult.Data { get => Data; }
        object IAlert.Data { get => Data; }
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IAlertMsg AddMsg(string msg)
        {
            AddMessage(msg);
            return this;
        }
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="msg"></param>
        protected void AddMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg)) { return; }
            Message = string.IsNullOrEmpty(Message) ? msg : $"{Message}{AlertMsg.SplitString}{msg}";
        }

        /// <summary>
        /// 转换成列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ToList()
        {
            return Message?.Split(AlertMsg.SplitString.ToCharArray());
        }
        #region // 辅助方法
        /// <summary>
        /// 获取一个简单提示消息
        /// </summary>
        /// <returns></returns>
        public static IAlertMsg<T> Get(bool isSuccess, string msg)
        {
            return new AlertMsg<T>(isSuccess, msg);
        }
        /// <summary>
        /// 获取一个简单提示消息
        /// </summary>
        /// <returns></returns>
        public static IAlertMsg<T> Get(IAlert alert)
        {
            return new AlertMsg<T>(alert.IsSuccess, alert.Message);
        }
        /// <summary>
        /// 获取一个简单提示消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static AlertMsg<T> Get(T data, string msg) => new AlertMsg<T>(data != null, msg) { Data = data };
        /// <summary>
        /// 隐式转换
        /// (Data为空?NotFound|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>(T data) => data == null ? new AlertMsg<T>(false, "NotFound") { Code = 404 } : new AlertMsg<T>(true, "数据已加载") { Data = data };
#if NETFrame
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>(Tuple<bool, string> res) => new AlertMsg<T>(res.Item1, res.Item2);
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>(Tuple<bool, string, T> res) => new AlertMsg<T>(res.Item1, res.Item2) { Data = res.Item3 };
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>(Tuple<T, string> res)
        {
            if (res.Item1 == null)
            {
                return new AlertMsg<T>(false, res.Item2)
                {
                    Code = 404,
                };
            }
            return new AlertMsg<T>(true, "数据已加载") { Data = res.Item1 };
        }
#endif
#if NETFx
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>((bool IsSuccess, string Message) res) => new AlertMsg<T>(res.IsSuccess, res.Message);
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>((bool IsSuccess, string Message, T Data) res) => new AlertMsg<T>(res.IsSuccess, res.Message) { Data = res.Data };
        /// <summary>
        /// 隐式转换
        /// (data.Item1为空?data.Item2|数据已加载)
        /// </summary>
        public static implicit operator AlertMsg<T>((T Data, string Message) res)
        {
            if (res.Data == null)
            {
                return new AlertMsg<T>(false, res.Message)
                {
                    Code = 404,
                };
            }
            return new AlertMsg<T>(true, "数据已加载") { Data = res.Data };
        }
#endif
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator bool(AlertMsg<T> res) => res.IsSuccess;
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator AlertMsg<T>(bool res) => new AlertMsg<T>(res, res ? "成功" : "失败");
        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator AlertMsg<T>(Exception ex) => new AlertMsg<T>(false, "操作失败,错误:{0}", ex.Message);

        /// <summary>
        /// 操作未实现
        /// </summary>
        /// <returns></returns>
        public static AlertMsg<T> NotImplement => new AlertMsg<T>(false, "操作未实现");
        #endregion
    }
    /// <summary>
    /// 提示信息泛型实现类
    /// 三级基础泛型实现类(Data=>(T)null)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlertMsgs<T> : AlertMsg<IEnumerable<T>>, IAlertMsgs<T>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMsgs(bool isSuccess = false, string message = "")
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.Data = new List<T>();
        }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMsgs(string message) : this(false, message)
        {
        }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMsgs(string fmt, params object[] param) : this(false, fmt, param)
        {
        }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMsgs(bool isSuccess, string fmt, params object[] param) : this(isSuccess, string.Format(fmt, param))
        {
        }
    }
    /// <summary>
    /// 提示信息泛型接口
    /// 二级传递接口-有修改
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAlertMsg<T> : IAlert<T>, IAlertMsg
    {
        /// <summary>
        /// 数据
        /// </summary>
        new T Data { get; set; }
    }
    /// <summary>
    /// 提示信息泛型接口
    /// 二级传递接口-有修改
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAlertMsgs<T> : IAlert<IEnumerable<T>>, IAlertMsg
    {
        /// <summary>
        /// 数据
        /// </summary>
        new IEnumerable<T> Data { get; set; }
    }
    /// <summary>
    /// 提示信息接口
    /// 二级传递接口-有修改
    /// 方法返回推荐使用IAlertResult
    /// </summary>
    public interface IAlertMsg : IAlert, IAlertResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        new bool IsSuccess { get; set; }
        /// <summary>
        /// 标识代码
        /// </summary>
        new int Code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        new string Message { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        new dynamic Data { get; set; }
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        IAlertMsg AddMsg(string msg);
    }
    /// <summary>
    /// 提示信息接口
    /// 二级传递接口-无修改
    /// </summary>
    public interface IAlertResult : IAlert
    {
        /// <summary>
        /// 数据
        /// </summary>
        new dynamic Data { get; }
    }
    /// <summary>
    /// 提示信息泛型接口
    /// 一级传递泛型接口-无修改
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAlert<out T> : IAlert
    {
        /// <summary>
        /// 数据
        /// </summary>
        new T Data { get; }
    }
    /// <summary>
    /// 提示信息接口
    /// 一级传递接口-无修改
    /// 方法返回推荐使用IAlertResult
    /// </summary>
    public interface IAlert
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// 标识代码
        /// </summary>
        int Code { get; }
        /// <summary>
        /// 提示信息
        /// </summary>
        string Message { get; }
        /// <summary>
        /// 数据
        /// </summary>
        object Data { get; }
    }
    #endregion
    /// <summary>
    /// 对象帮助类
    /// </summary>
    public static partial class EModels
    {
        /// <summary>
        /// 获取泛型实例提示信息
        /// </summary>
        public static AlertMsg<T> GetAlert<T>(this T data) => data;
        /// <summary>
        /// 获取泛型实例提示信息
        /// </summary>
        public static AlertMsg<T> GetAlert<T>(this T data, string msg) => new AlertMsg<T>(data != null, msg) { Data = data };
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage<T>(this T data, string msg = null) => new AlertMessage(data != null, msg) { Data = data };
#if NETFrame
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this Tuple<bool, string> res) => new AlertMsg(res.Item1, res.Item2);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this Tuple<bool, string> res) => new AlertMessage(res.Item1, res.Item2);
#endif
#if NETFx
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this (bool IsSuccess, String Message) res) => new AlertMsg(res.IsSuccess, res.Message);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this (bool IsSuccess, String Message) res) => new AlertMessage(res.IsSuccess, res.Message);
#endif
    }
    #region // 分页查询结果
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPageResult<out T> : IPageResult
    {
        /// <summary>
        /// 结果项
        /// </summary>
        new IEnumerable<T> Items { get; }
    }
    /// <summary>
    /// 
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
        IEnumerable<object> Items { get; }
        /// <summary>
        /// 跳过
        /// </summary>
        int Skip { get; }
        /// <summary>
        /// 获取
        /// </summary>
        int Take { get; }

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

        IEnumerable<object> IPageResult.Items => Items as IEnumerable<object>;

        /// <summary>
        /// 进一除法
        /// </summary>
        /// <param name="devidend">被除数</param>
        /// <param name="divisor">除数</param>
        /// <returns></returns>
        public static int CeilDividend(int devidend, int divisor)
        {
            if (divisor == 0) { return 0; }
            return (int)Math.Ceiling(((decimal)devidend) / divisor);
        }
    }
    #endregion
}
