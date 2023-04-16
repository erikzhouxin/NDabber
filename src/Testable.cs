global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 尝试测试
    /// </summary>
    public static class TestTry
    {
        #region // 定义内容
        /// <summary>
        /// 用户账号
        /// </summary>
        public const String Account = "erikzhouxin";
        /// <summary>
        /// 用户名称
        /// </summary>
        public const String UserName = "zhouxin";
        /// <summary>
        /// 用户账号
        /// </summary>
        public static string AuthorAccount { get; } = "ErikZhouXin";
        /// <summary>
        /// 用户名称
        /// </summary>
        public static string AuthorUserName { get; } = "周鑫";
        #endregion 定义内容
        #region // 缩减内容
        /// <summary>
        /// 创建数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public static T[] CreateArray<T>(params T[] models) => models;
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try(this Delegate action, Action<Exception> excep)
        {
            try
            {
                action.DynamicInvoke();
            }
            catch (Exception ex)
            {
                excep?.Invoke(ex);
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try(this Delegate action, Action<Exception> excep, params object[] args)
        {
            try
            {
                action.DynamicInvoke(args);
            }
            catch (Exception ex)
            {
                excep?.Invoke(ex);
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        /// <param name="action"></param>
        public static void Try(this Delegate action)
        {
            try
            {
                action.DynamicInvoke();
            }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static void Try(this Delegate action, params object[] args)
        {
            try
            {
                action.DynamicInvoke(args);
            }
            catch { }
        }
        #endregion
        #region // 反射内容 Reflection
        /// <summary>
        /// 当前程序集[NSystem.Data.Dabber]
        /// </summary>
        public static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();
        /// <summary>
        /// 入口程序集(运行程序*.exe)
        /// </summary>
        public static Assembly EntryAssembly { get; } = Assembly.GetEntryAssembly();
        /// <summary>
        /// 成员全称
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static String GetMemberFullName(this MemberInfo member)
        {
            return $"{member.DeclaringType?.FullName}.{member.Name}";
        }
        #endregion 反射内容 Reflection
        #region // 委托内容 Delegate
        /// <summary>
        /// 方法信息
        /// </summary>
        /// <returns></returns>
        public static string GetMethodFullName(this Delegate expression)
        {
            var member = expression.Method;
            return $"{member.DeclaringType?.FullName}.{member.Name}";
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Exception Call(this Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception ex) { return ex; }
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <returns></returns>
        public static Exception Call<T1>(this Action<T1> action, T1 t1 = default)
        {
            try
            {
                action(t1);
                return null;
            }
            catch (Exception ex) { return ex; }
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <returns></returns>
        public static Exception Call<T1, T2>(this Action<T1, T2> action, T1 t1 = default, T2 t2 = default)
        {
            try
            {
                action(t1, t2);
                return null;
            }
            catch (Exception ex) { return ex; }
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Call<T>(this Func<T> action)
        {
            try
            {
                return action();
            }
            catch { }
            return default;
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <returns></returns>
        public static Exception Call(this Delegate action, params object[] args)
        {
            try
            {
                action.DynamicInvoke(args);
                return null;
            }
            catch (Exception ex) { return ex; }
        }
        #endregion 委托内容 Delegate
        #region // 任务内容 Task
        /// <summary>
        /// 演示调用
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="action"></param>
        public static void DelayCall(TimeSpan timeSpan, Action action)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Thread.Sleep(timeSpan);
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }
        /// <summary>
        /// 启动新任务
        /// </summary>
        /// <param name="method"></param>
        public static void TaskStartNew(this Action method)
        {
            Task.Factory.StartNew(method);
        }
        /// <summary>
        /// 启动新任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="cancellationToken"></param>
        public static void TaskStartNew(this Action method, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(method);
        }
        #region // Task兼容内容
        /// <summary>
        /// 兼容性的 Task.FromResult(0)之类的内容
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TResult> TaskFromResult<TResult>(TResult result) =>
#if NET40
            new Task<TResult>(() => result);
#else
            Task.FromResult(result);
#endif
        /// <summary>
        /// 兼容性的 Task.Delay 之类的内容
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskDelay(int milliseconds) =>
#if NET40
            Task.Factory.StartNew(() => Thread.Sleep(milliseconds));
#else
            Task.Delay(milliseconds);
#endif
        /// <summary>
        /// 兼容性的 Task.Delay 之类的内容
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskDelay(int milliseconds, CancellationToken cancellationToken) =>
#if NET40
            Task.Factory.StartNew(() => Thread.Sleep(milliseconds), cancellationToken);
#else
            Task.Delay(milliseconds, cancellationToken);
#endif
        /// <summary>
        /// 兼容性的 Task.Delay 之类的内容
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskDelay(TimeSpan milliseconds, CancellationToken cancellationToken) =>
#if NET40
            Task.Factory.StartNew(() => Thread.Sleep(milliseconds), cancellationToken);
#else
            Task.Delay(milliseconds, cancellationToken);
#endif
        /// <summary>
        /// 兼容性的 Task.Delay 之类的内容
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskDelay(TimeSpan milliseconds)
        {
#if NET40
            return Task.Factory.StartNew(() => Thread.Sleep(milliseconds));
#else
            return Task.Delay(milliseconds);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.WhenAny 之类的内容
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskWhenAny(params Task[] tasks)
        {
#if NET40
            return Task.Factory.ContinueWhenAny(tasks, TaskContinueDoNothing);
#else
            return Task.WhenAny(tasks);
#endif
        }
        /// <summary>
        /// 继续但什么都不做
        /// </summary>
        /// <param name="_"></param>
        public static void TaskContinueDoNothing(Task _) { }
        /// <summary>
        /// 继续但什么都不做
        /// </summary>
        /// <param name="_"></param>
        public static void TaskContinueDoNothing(Task[] _) { }
        /// <summary>
        /// 兼容性的 Task.WhenAll 之类的内容
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskWhenAll(params Task[] tasks)
        {
#if NET40
            return Task.Factory.ContinueWhenAll(tasks, TaskContinueDoNothing);
#else
            return Task.WhenAll(tasks);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.WhenAll 之类的内容
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskWhenAll(IEnumerable<Task> tasks)
        {
#if NET40
            return Task.Factory.ContinueWhenAll(tasks is Task[] taskArr ? taskArr : tasks.ToArray(), TaskContinueDoNothing);
#else
            return Task.WhenAll(tasks);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskRun(Action action)
        {
#if NET40
            return Task.Factory.StartNew(action);
#else
            return Task.Run(action);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskRun(Action action, CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(action, cancellationToken);
#else
            return Task.Run(action, cancellationToken);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TResult> TaskRun<TResult>(Func<TResult> function)
        {
#if NET40
            return Task.Factory.StartNew(function);
#else
            return Task.Run(function);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TResult> TaskRun<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(function, cancellationToken);
#else
            return Task.Run(function, cancellationToken);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskRun(Func<Task> function)
        {
            return TaskRun(function, default(CancellationToken));
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task TaskRun(Func<Task> function, CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(function, cancellationToken);
#else
            return Task.Run(function, cancellationToken);
#endif
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TResult> TaskRun<TResult>(Func<Task<TResult>> function)
        {
            return TaskRun(function, default(CancellationToken));
        }
        /// <summary>
        /// 兼容性的 Task.Run 之类的内容
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TResult> TaskRun<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(() => function.Invoke().Result, cancellationToken);
#else
            return Task.Run(function, cancellationToken);
#endif
        }
#if NET40
        /// <summary>
        /// 轻量级的信号量异步等待
        /// </summary>
        /// <param name="slim"></param>
        /// <returns></returns>
        public static async Task WaitAsync(this SemaphoreSlim slim)
        {
            await Task.Factory.StartNew(() => slim.Wait());
        }
        /// <summary>
        /// 轻量级的信号量异步等待
        /// </summary>
        /// <param name="slim"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task WaitAsync(this SemaphoreSlim slim, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => slim.Wait(), cancellationToken);
        }
#endif
        #endregion
        #endregion 任务内容 Task
        #region // 调试内容 Debug
        /// <summary>
        /// 调试输出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T DebugConsole<T>(this T model)
        {
            if (IsDebugMode) { Console.WriteLine(model); }
            return model;
        }
        private static readonly Lazy<bool> _isDebugMode = new Lazy<bool>(IsProcessDebug, true);
        /// <summary>
        /// 当前进程是调试状态
        /// </summary>
        /// <returns></returns>
        public static bool IsProcessDebug()
        {
            bool debug = false;
            foreach (var attribute in Assembly.GetExecutingAssembly().GetCustomAttributes(false))
            {
                if (attribute is DebuggableAttribute debuggable)
                {
                    if (debuggable.IsJITTrackingEnabled)
                    {
                        debug = true;
                        break;
                    }
                }
            }
            return debug;
        }
        /// <summary>
        /// 当前是调试模式
        /// </summary>
        public static bool IsDebugMode { get => _isDebugMode.Value; }
        #endregion 调试内容 Debug
    }
}

