using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace System
{
    #region // 接口定义
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
        /// <summary>
        /// 添加错误信息
        /// (设置IsSuccess为false)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        IAlertMsg AddError(string msg);
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
    public interface IAlertMsgs<T> : IAlert<IEnumerable<T>>, IAlertMsg<IEnumerable<T>>, IAlertMsg
    {
        /// <summary>
        /// 数据
        /// </summary>
        new IEnumerable<T> Data { get; set; }
    }
    /// <summary>
    /// 异常提示接口
    /// </summary>
    public interface IAlertException : IAlertMsg
    {
        /// <summary>
        /// 异常信息
        /// </summary>
        Exception Exception { get; set; }
    }
    /// <summary>
    /// 提示Json接口
    /// </summary>
    public interface IAlertJson : IAlertMsg<string> { }
    #endregion 接口定义
    #region // 通用实现类
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
        public AlertMsg() : base(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertMsg(bool isSuccess) : base(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMsg(bool isSuccess, string message) : base(isSuccess, message) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMsg(string message) : base(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMsg(string fmt, params object[] param) : base(false, fmt, param) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="ex"></param>
        public AlertMsg(Exception ex) : base(false, ex.Message) { Data = ex; }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMsg(bool isSuccess, string fmt, params object[] param) : base(isSuccess, fmt, param) { }
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
        /// <summary>
        /// 未找到可操作内容
        /// </summary>
        public static AlertMsg NotFound { get => new AlertMsg(false, "未找到可操作内容") { Code = 404 }; }
        /// <summary>
        /// 不支持可操作内容
        /// </summary>
        public static AlertMsg NotSupported { get => new AlertMsg(false, "不支持可操作内容") { Code = 404 }; }
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
        public AlertMessage() : base(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertMessage(bool isSuccess) : base(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMessage(bool isSuccess, string message) : base(isSuccess, message) { Data = new ExpandoObject(); }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMessage(string message) : base(false, message) { Data = new ExpandoObject(); }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMessage(string fmt, params object[] param) : base(false, fmt, param) { Data = new ExpandoObject(); }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMessage(bool isSuccess, string fmt, params object[] param) : base(isSuccess, fmt, param) { Data = new ExpandoObject(); }
        /// <summary>
        /// 异常信息构造
        /// </summary>
        /// <param name="ex"></param>
        public AlertMessage(Exception ex) : base(ex) { }
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
        public AlertMsg() : this(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertMsg(bool isSuccess) : this(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMsg(bool isSuccess, string message)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMsg(string message) : this(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMsg(string fmt, params object[] param) : this(false, fmt, param) { }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMsg(bool isSuccess, string fmt, params object[] param) : this(isSuccess, string.Format(fmt, param)) { }
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
        /// <inheritdoc />
        public IAlertMsg AddError(string msg)
        {
            this.IsSuccess = false;
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
        public static IAlertMsg<Exception> Get(Exception ex)
        {
            return new AlertMsg<Exception>(false, ex.Message) { Data = ex };
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
        public AlertMsgs() : this(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertMsgs(bool isSuccess) : this(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertMsgs(bool isSuccess, string message) : base(isSuccess, message)
        {
            this.Data = new List<T>();
        }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertMsgs(string message) : this(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertMsgs(string fmt, params object[] param) : this(false, fmt, param) { }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertMsgs(bool isSuccess, string fmt, params object[] param) : this(isSuccess, string.Format(fmt, param)) { }
        /// <summary>
        /// 操作未实现
        /// </summary>
        /// <returns></returns>
        public new static AlertMsgs<T> NotImplement => new AlertMsgs<T>(false, "操作未实现");
    }
    /// <summary>
    /// 提示异常信息
    /// </summary>
    public class AlertException : AlertMsg, IAlertException
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertException() : base(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertException(bool isSuccess) : base(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertException(bool isSuccess, string message) : base(isSuccess, message) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="message"></param>
        public AlertException(string message) : base(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertException(string fmt, params object[] param) : base(false, fmt, param) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="ex"></param>
        public AlertException(Exception ex) : this(ex.Message, ex) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public AlertException(string message, Exception ex) : base(false, message)
        {
            Exception = ex;
        }
        /// <summary>
        /// 异常信息
        /// </summary>
        public virtual Exception Exception { get; set; }
    }
    /// <summary>
    /// 提示异常信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlertException<T> : AlertMsg<T>, IAlertException
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertException() : base(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertException(bool isSuccess) : base(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="message"></param>
        public AlertException(string message) : base(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertException(string fmt, params object[] param) : base(false, fmt, param) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="ex"></param>
        public AlertException(Exception ex) : this(ex.Message, ex) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public AlertException(string message, Exception ex) : base(false, message)
        {
            Exception = ex;
        }
        /// <summary>
        /// 异常信息
        /// </summary>
        public virtual Exception Exception { get; set; }
    }
    /// <summary>
    /// 提示异常信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlertExceptions<T> : AlertMsgs<T>, IAlertException
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertExceptions() : base(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertExceptions(bool isSuccess) : base(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertExceptions(bool isSuccess, string message) : base(isSuccess, message) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="message"></param>
        public AlertExceptions(string message) : base(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertExceptions(string fmt, params object[] param) : base(false, fmt, param) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="ex"></param>
        public AlertExceptions(Exception ex) : this(ex.Message, ex) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public AlertExceptions(string message, Exception ex) : base(false, message)
        {
            Exception = ex;
        }
        /// <summary>
        /// 异常信息
        /// </summary>
        public virtual Exception Exception { get; set; }
    }
    /// <summary>
    /// 提示Json信息
    /// </summary>
    public class AlertJson : AlertMsg<String>, IAlertJson
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertJson() : base(false, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        public AlertJson(bool isSuccess) : base(isSuccess, string.Empty) { }
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertJson(bool isSuccess, string message) : base(isSuccess, message) { }
        /// <summary>
        /// 错误消息构造
        /// </summary>
        /// <param name="message"></param>
        public AlertJson(string message) : this(false, message) { }
        /// <summary>
        /// 错误消息格式化构造
        /// </summary>
        public AlertJson(string fmt, params object[] param) : this(false, fmt, param) { }
        /// <summary>
        /// 格式化构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="fmt"></param>
        /// <param name="param"></param>
        public AlertJson(bool isSuccess, string fmt, params object[] param) : this(isSuccess, String.Format(fmt, param)) { }
        /// <summary>
        /// 异常构造
        /// </summary>
        /// <param name="ex"></param>
        public AlertJson(Exception ex) : this(false, ex.Message)
        {
            Data = ex.GetJsonString();
        }
    }
    #endregion 通用实现类
}