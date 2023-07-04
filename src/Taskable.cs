using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Extter;

namespace System
{
    /// <summary>
    /// 可取消的任务
    /// </summary>
    public class TaskRevokable : IDisposable
    {
        CancellationTokenSource _cancelSource;
        CancellationToken _cancelToken;
        Task _task;
        /// <summary>
        /// 显示异常
        /// </summary>
        public virtual Action<Exception> ShowException { get; set; }
        /// <summary>
        /// 是错误后停止
        /// </summary>
        public virtual bool IsErrorStopped { get; set; }
        /// <summary>
        /// 回调
        /// </summary>
        public virtual Action Callback { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        public TaskRevokable()
        {
            _cancelSource = new CancellationTokenSource();
            _cancelToken = _cancelSource.Token;
            _task = new Task(StartTask, _cancelToken);
        }
        /// <summary>
        /// 任务
        /// </summary>
        public TaskRevokable(Action task) : this()
        {
            Callback = task;
        }
        private void StartTask()
        {
            while (!_cancelToken.IsCancellationRequested)
            {
                try
                {
                    Callback?.Invoke();
                }
                catch (Exception ex)
                {
                    ShowException?.Invoke(ex);
                    if (IsErrorStopped) { return; }
                }
            }
        }
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <returns></returns>
        public virtual TaskRevokable Start()
        { _task.Start(); return this; }
        /// <summary>
        /// 取消
        /// </summary>
        public virtual void Cancel()
        { _cancelSource.Cancel(); }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            TestTry.Try(_cancelSource.Cancel);
            try
            {
                _task.Dispose();
                _cancelSource.Dispose();
            }
            catch { }
        }
        #region // 静态内容
        /// <summary>
        /// 启动新
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static TaskRevokable StartNew(Action callback)
        { return new TaskRevokable(callback).Start(); }
        #endregion 静态内容
    }
    /// <summary>
    /// 任务加锁模型
    /// </summary>
    public class TaskTryEnterModel
    {
        /// <summary>
        /// 锁
        /// </summary>
        public virtual object Locker { get; }
        /// <summary>
        /// 获取锁等待时间
        /// </summary>
        public virtual TimeSpan? Timer { get; }
        /// <summary>
        /// 关于锁的键
        /// </summary>
        public virtual string Key { get; }
        /// <summary>
        /// 未取到锁
        /// </summary>
        public virtual Action Unenter { get; }
        /// <summary>
        /// 构造
        /// </summary>
        public TaskTryEnterModel()
        {
            Locker = new object();
            Unenter = TestTry.DoNothing;
        }
        /// <summary>
        /// 键构造
        /// </summary>
        public TaskTryEnterModel(string key, TimeSpan? timer = null, Action unenter = null)
        {
            Key = key;
            Timer = timer;
            Locker = CacheLockModel<TaskTryEnterModel>.Get(key);
            Unenter = unenter ?? TestTry.DoNothing;
        }
        /// <summary>
        /// 对象构造
        /// </summary>
        public TaskTryEnterModel(object locker, TimeSpan? timer = null, Action unenter = null)
        {
            Locker = locker;
            Timer = timer;
            Unenter = unenter ?? TestTry.DoNothing;
        }
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <returns></returns>
        public virtual bool TryEnter()
        {
            if (Timer.HasValue)
            {
                if (Monitor.TryEnter(Locker, Timer.Value)) { return true; }
                Unenter?.Invoke();
                return false;
            }
            if (Monitor.TryEnter(Locker)) { return true; }
            Unenter?.Invoke();
            return false;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public virtual void Exit()
        {
            Monitor.Exit(Locker);
        }
    }
    /// <summary>
    /// 任务加锁模型
    /// </summary>
    public class TaskTryEnterModel<T>
    {
        /// <summary>
        /// 锁
        /// </summary>
        public virtual object Locker { get; }
        /// <summary>
        /// 获取锁等待时间
        /// </summary>
        public virtual TimeSpan? Timer { get; }
        /// <summary>
        /// 关于锁的键
        /// </summary>
        public virtual string Key { get; }
        /// <summary>
        /// 未取到锁
        /// </summary>
        public virtual Func<T> Unenter { get; }
        /// <summary>
        /// 结果
        /// </summary>
        public virtual T UnenterResult { get; private set; }
        /// <summary>
        /// 构造
        /// </summary>
        public TaskTryEnterModel()
        {
            Locker = new object();
            Unenter = GetDefault;
        }
        /// <summary>
        /// 键构造
        /// </summary>
        public TaskTryEnterModel(string key, TimeSpan? timer = null, Func<T> unenter = null)
        {
            Key = key;
            Timer = timer;
            Locker = CacheLockModel<TaskTryEnterModel>.Get(key);
            Unenter = unenter ?? GetDefault;
        }
        /// <summary>
        /// 对象构造
        /// </summary>
        public TaskTryEnterModel(object locker, TimeSpan? timer = null, Func<T> unenter = null)
        {
            Locker = locker;
            Timer = timer;
            Unenter = unenter ?? GetDefault;
        }
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <returns></returns>
        public virtual bool TryEnter()
        {
            if (Timer.HasValue)
            {
                if (Monitor.TryEnter(Locker, Timer.Value)) { return true; }
                UnenterResult = Unenter.Invoke();
                return false;
            }
            if (Monitor.TryEnter(Locker)) { return true; }
            UnenterResult = Unenter.Invoke();
            return false;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public virtual void Exit()
        {
            Monitor.Exit(Locker);
        }
        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        internal static T GetDefault() => default;
    }
}
