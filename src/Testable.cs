global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 尝试测试
    /// </summary>
    public static partial class TestTry
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
        /// 延迟等待
        /// </summary>
        /// <param name="milli"></param>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Delay(int milli)
        {
#if NET40
            Thread.Sleep(milli);
#else
            Task.Delay(milli).Wait();
#endif
        }
        /// <summary>
        /// 延迟等待
        /// </summary>
        /// <param name="milli"></param>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Delay(TimeSpan milli)
        {
#if NET40
            Thread.Sleep(milli);
#else
            Task.Delay(milli).Wait();
#endif
        }
        /// <summary>
        /// 延迟等待
        /// </summary>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Delay(int milli, CancellationToken cancellation)
        {
#if NET40
            new Task(() => Thread.Sleep(milli), cancellation).StartAsync().Wait(cancellation);
#else
            Task.Delay(milli, cancellation).Wait(cancellation);
#endif
        }
        /// <summary>
        /// 延迟等待
        /// </summary>
#if !NET40 // 内联函数减少性能损耗
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Delay(TimeSpan milli, CancellationToken cancellation)
        {
#if NET40
            new Task(() => Thread.Sleep(milli), cancellation).StartAsync().Wait(cancellation);
#else
            Task.Delay(milli, cancellation).Wait(cancellation);
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
        #region // 无所事事 DONOTHING
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing() { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1>(T1 t1) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2>(T1 t1, T2 t2) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3>(T1 t1, T2 t2, T3 t3) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te) { }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static void DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf) { }

        /// <summary>
        /// 无所作为
        /// </summary>
        public static T1 DoNothing<T1>() { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T2 DoNothing<T1, T2>(T1 t1) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T3 DoNothing<T1, T2, T3>(T1 t1, T2 t2) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T4 DoNothing<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T5 DoNothing<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T6 DoNothing<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T7 DoNothing<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T8 DoNothing<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static T9 DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TA DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TB DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TC DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TD DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TE DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TF DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td) { return default; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static TG DoNothing<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te) { return default; }

        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg DoNothingAlert() { return AlertMsg.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T1> DoNothingAlert<T1>() { return AlertMsg<T1>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T2> DoNothingAlert<T1, T2>(T1 t1) { return AlertMsg<T2>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T3> DoNothingAlert<T1, T2, T3>(T1 t1, T2 t2) { return AlertMsg<T3>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T4> DoNothingAlert<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3) { return AlertMsg<T4>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T5> DoNothingAlert<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4) { return AlertMsg<T5>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T6> DoNothingAlert<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) { return AlertMsg<T6>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T7> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) { return AlertMsg<T7>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T8> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) { return AlertMsg<T8>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<T9> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) { return AlertMsg<T9>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TA> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) { return AlertMsg<TA>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TB> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta) { return AlertMsg<TB>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TC> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb) { return AlertMsg<TC>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TD> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc) { return AlertMsg<TD>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TE> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td) { return AlertMsg<TE>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TF> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td) { return AlertMsg<TF>.Nothing; }
        /// <summary>
        /// 无所作为
        /// </summary>
        public static IAlertMsg<TG> DoNothingAlert<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te) { return AlertMsg<TG>.Nothing; }
        #endregion 无所事事
        #region // 委托执行 Delegate
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
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static void Try(this Delegate action, object[] args)
        {
            try { action.DynamicInvoke(args); }
            catch { }
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static object Try(this Delegate action, object[] args, object defVal)
        {
            try { return action.DynamicInvoke(args); }
            catch { return defVal; }
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static void Try(this Delegate action, object[] args, Action<Exception> excep)
        {
            try { action.DynamicInvoke(args); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static object Try(this Delegate action, object[] args, object defVal, Action<Exception> excep)
        {
            try { return action.DynamicInvoke(args); }
            catch (Exception ex) { excep?.Invoke(ex); return defVal; }
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static object Try(this Delegate action, object[] args, Func<Exception, object> excep)
        {
            try { return action.DynamicInvoke(args); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task TryAsync(this Delegate action, object[] args)
        {
            return new Task(() =>
            {
                try { action.DynamicInvoke(args); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task TryAsync(this Delegate action, object[] args, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.DynamicInvoke(args); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task<object> TryAsync(this Delegate action, object[] args, object defVal)
        {
            return new Task<object>(() =>
            {
                try { return action.DynamicInvoke(args); }
                catch { return defVal; }
            }).StartAsync();
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task<object> TryAsync(this Delegate action, object[] args, object defVal, Action<Exception> excep)
        {
            return new Task<object>(() =>
            {
                try { return action.DynamicInvoke(args); }
                catch (Exception ex) { excep?.Invoke(ex); return defVal; }
            }).StartAsync();
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task<object> TryAsync(this Delegate action, object[] args, Func<Exception, object> excep)
        {
            return new Task<object>(() =>
            {
                try { return action.DynamicInvoke(args); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 启动新任务
        /// </summary>
        public static Task TryAsync(this Delegate method, object[] args, CancellationToken cancellationToken)
        {
            return new Task(() =>
            {
                try { method?.DynamicInvoke(args); }
                catch { }
            }, cancellationToken).StartAsync();
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task TryStartAsync(this Delegate action, object[] args)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.DynamicInvoke(args); }
                catch { }
            });
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task TryStartAsync(this Delegate action, object[] args, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.DynamicInvoke(args); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task<object> TryStartAsync(this Delegate action, object[] args, object defVal)
        {
            return Task.Factory.StartNew(() =>
            {
                try { return action.DynamicInvoke(args); }
                catch { return defVal; }
            });
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task<object> TryStartAsync(this Delegate action, object[] args, object defVal, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { return action.DynamicInvoke(args); }
                catch (Exception ex) { excep?.Invoke(ex); return defVal; }
            });
        }
        /// <summary>
        /// 针对超过Action长度的委托简写try{action}catch{}
        /// </summary>
        public static Task<object> TryStartAsync(this Delegate action, object[] args, Func<Exception, object> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { return action.DynamicInvoke(args); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 启动新任务
        /// </summary>
        public static Task TryStartAsync(this Delegate method, object[] args, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try { method?.DynamicInvoke(args); }
                catch { }
            }, cancellationToken);
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task DelayTryStartAsync(this Delegate method, TimeSpan time, object[] args)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(time);
                try { method.DynamicInvoke(args); }
                catch { }
            });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task DelayTryStartAsync(this Delegate method, int milliseconds, object[] args)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(milliseconds);
                try { method.DynamicInvoke(args); }
                catch { }
            });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task<object> DelayTryStartAsync(this Delegate method, TimeSpan time, object[] args, object defVal)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(time);
                try { return method.DynamicInvoke(args); }
                catch { return defVal; }
            });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task<object> DelayTryStartAsync(this Delegate method, int milliseconds, object[] args, object defVal)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(milliseconds);
                try { return method.DynamicInvoke(args); }
                catch { return defVal; }
            });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task DelayTryStartAsync(this Delegate method, TimeSpan time, object[] args, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(time);
                try { method.DynamicInvoke(args); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task<object> DelayTryStartAsync(this Delegate method, TimeSpan time, object[] args, object defVal, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(time);
                try { return method.DynamicInvoke(args); }
                catch (Exception ex) { excep?.Invoke(ex); return defVal; }
            });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <returns></returns>
        public static Task<object> DelayTryStartAsync(this Delegate method, TimeSpan time, object[] args, Func<Exception, object> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(time);
                try { return method.DynamicInvoke(args); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        #endregion 委托内容 Delegate
        #region // 尝试异常执行 Action/Func
        /// <summary>
        /// 尝试执行
        /// </summary>
        /// <param name="action"></param>
        /// <param name="excep"></param>
        public static void TryCatch(this Action action, Action excep)
        {
            try { action?.Invoke(); }
            catch { excep?.Invoke(); }
        }
        /// <summary>
        /// 尝试执行次数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="times"></param>
        public static void TryNext(this Action action, int times)
        {
            if (times < 0) { return; }
            try { action?.Invoke(); return; }
            catch { TryNext(action, --times); }
        }
        /// <summary>
        /// 尝试执行次数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="times"></param>
        public static T TryNext<T>(this Func<T> action, int times)
        {
            if (times < 0) { return default; }
            try { return action.Invoke(); }
            catch { return TryNext(action, --times); }
        }
        #endregion 尝试异常执行 Action/Func
        #region // 尝试执行 Action/Func
        /// <summary>
        /// 默认异常捕获后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static T Catcher<T>(Exception ex)
        {
            Console.WriteLine(ex);
            return default;
        }
        /// <summary>
        /// 默认异常捕获后返回
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static void Catcher(Exception ex) => Console.WriteLine(ex);
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try(this Action action)
        {
            try { action.Invoke(); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1>(this Action<T1> action, T1 t1)
        {
            try { action.Invoke(t1); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
        {
            try { action.Invoke(t1, t2); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            try { action.Invoke(t1, t2, t3); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            try { action.Invoke(t1, t2, t3, t4); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            try { action.Invoke(t1, t2, t3, t4, t5); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
            catch { }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T1 Try<T1>(this Func<T1> action, T1 t1 = default)
        {
            try { return action.Invoke(); }
            catch { return t1; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T2 Try<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2 = default)
        {
            try { return action.Invoke(t1); }
            catch { return t2; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T3 Try<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3 = default)
        {
            try { return action.Invoke(t1, t2); }
            catch { return t3; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T4 Try<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            try { return action.Invoke(t1, t2, t3); }
            catch { return t4; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T5 Try<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            try { return action.Invoke(t1, t2, t3, t4); }
            catch { return t5; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T6 Try<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5); }
            catch { return t6; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T7 Try<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
            catch { return t7; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T8 Try<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
            catch { return t8; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T9 Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
            catch { return t9; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TA Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
            catch { return ta; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TB Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
            catch { return tb; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TC Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
            catch { return tc; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TD Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
            catch { return td; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TE Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
            catch { return te; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TF Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
            catch { return tf; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TG Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
            catch { return tg; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TH Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
            catch { return th; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try(this Action action, Action<Exception> excep)
        {
            try { action.Invoke(); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1>(this Action<T1> action, T1 t1, Action<Exception> excep)
        {
            try { action.Invoke(t1); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
            catch (Exception ex) { excep?.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T1 Try<T1>(this Func<T1> action, T1 t1, Action<Exception> excep)
        {
            try { return action.Invoke(); }
            catch (Exception ex) { excep?.Invoke(ex); return t1; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T2 Try<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep)
        {
            try { return action.Invoke(t1); }
            catch (Exception ex) { excep?.Invoke(ex); return t2; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T3 Try<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2); }
            catch (Exception ex) { excep?.Invoke(ex); return t3; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T4 Try<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3); }
            catch (Exception ex) { excep?.Invoke(ex); return t4; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T5 Try<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4); }
            catch (Exception ex) { excep?.Invoke(ex); return t5; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T6 Try<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5); }
            catch (Exception ex) { excep?.Invoke(ex); return t6; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T7 Try<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
            catch (Exception ex) { excep?.Invoke(ex); return t7; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T8 Try<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
            catch (Exception ex) { excep?.Invoke(ex); return t8; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T9 Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
            catch (Exception ex) { excep?.Invoke(ex); return t9; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TA Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
            catch (Exception ex) { excep?.Invoke(ex); return ta; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TB Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
            catch (Exception ex) { excep?.Invoke(ex); return tb; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TC Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
            catch (Exception ex) { excep?.Invoke(ex); return tc; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TD Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
            catch (Exception ex) { excep?.Invoke(ex); return td; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TE Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
            catch (Exception ex) { excep?.Invoke(ex); return te; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TF Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
            catch (Exception ex) { excep?.Invoke(ex); return tf; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TG Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
            catch (Exception ex) { excep?.Invoke(ex); return tg; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TH Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
            catch (Exception ex) { excep?.Invoke(ex); return th; }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T1 Try<T1>(this Func<T1> action, Func<Exception, T1> excep)
        {
            try { return action.Invoke(); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T2 Try<T1, T2>(this Func<T1, T2> action, T1 t1, Func<Exception, T2> excep)
        {
            try { return action.Invoke(t1); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T3 Try<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            try { return action.Invoke(t1, t2); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T4 Try<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            try { return action.Invoke(t1, t2, t3); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T5 Try<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T6 Try<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T7 Try<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T8 Try<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T9 Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TA Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TB Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TC Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TD Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TE Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TF Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TG Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TH Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
            catch (Exception ex) { return excep.Invoke(ex); }
        }
        #endregion 尝试执行 Action/Func
        #region // 尝试锁定执行 Action/Func
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter(this Action action, TaskTryEnterModel task)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T1 TryEnter<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(); }
                catch { return t1; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T2 TryEnter<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1); }
                catch { return t2; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T3 TryEnter<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T4 TryEnter<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T5 TryEnter<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T6 TryEnter<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T7 TryEnter<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T8 TryEnter<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T9 TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TA TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TB TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TC TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TD TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TE TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TF TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TG TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TH TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter(this Action action, TaskTryEnterModel task, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static void TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
                finally { task.Exit(); }
            }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T1 TryEnter<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T2 TryEnter<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T3 TryEnter<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T4 TryEnter<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T5 TryEnter<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T6 TryEnter<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T7 TryEnter<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T8 TryEnter<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T9 TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TA TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TB TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TC TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TD TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TE TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TF TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TG TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TH TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T1 TryEnter<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, Func<Exception, T1> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T2 TryEnter<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, Func<Exception, T2> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T3 TryEnter<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T4 TryEnter<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T5 TryEnter<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T6 TryEnter<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T7 TryEnter<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T8 TryEnter<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static T9 TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TA TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TB TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TC TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TD TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TE TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TF TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TG TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        public static TH TryEnter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            if (task.TryEnter())
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
                finally { task.Exit(); }
            }
            return task.UnenterResult;
        }
        #endregion 尝试锁定执行 Action/Func
        #region // 异步尝试执行 Action/Func
        /// <summary>
        /// 启动一个初始化好的Task并返回
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task StartAsync(this Task task)
        { task.Start(); return task; }
        /// <summary>
        /// 启动一个初始化好的Task并返回
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task<T> StartAsync<T>(this Task<T> task)
        { task.Start(); return task; }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync(this Action action)
        {
            return new Task(() =>
            {
                try { action.Invoke(); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1>(this Action<T1> action, T1 t1)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryAsync<T1>(this Func<T1> action, T1 t1 = default)
        {
            return new Task<T1>(() =>
            {
                try { return action.Invoke(); }
                catch { return t1; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2 = default)
        {
            return new Task<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch { return t2; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3 = default)
        {
            return new Task<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            return new Task<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            return new Task<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            return new Task<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            return new Task<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            return new Task<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            return new Task<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            return new Task<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            return new Task<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            return new Task<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            return new Task<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            return new Task<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            return new Task<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            return new Task<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            return new Task<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync(this Action action, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1>(this Action<T1> action, T1 t1, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryAsync<T1>(this Func<T1> action, T1 t1, Action<Exception> excep)
        {
            return new Task<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep)
        {
            return new Task<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return new Task<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return new Task<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return new Task<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return new Task<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return new Task<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return new Task<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return new Task<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return new Task<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return new Task<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return new Task<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return new Task<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return new Task<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return new Task<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return new Task<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            return new Task<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryAsync<T1>(this Func<T1> action, Func<Exception, T1> excep)
        {
            return new Task<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryAsync<T1, T2>(this Func<T1, T2> action, T1 t1, Func<Exception, T2> excep)
        {
            return new Task<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            return new Task<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            return new Task<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            return new Task<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            return new Task<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            return new Task<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            return new Task<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            return new Task<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            return new Task<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            return new Task<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            return new Task<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            return new Task<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            return new Task<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            return new Task<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            return new Task<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            return new Task<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync(this Action action, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1>(this Action<T1> action, T1 t1, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryAsync<T1>(this Func<T1> action, T1 t1, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                try { return action.Invoke(); }
                catch { return t1; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch { return t2; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync(this Action action, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1>(this Action<T1> action, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryAsync<T1>(this Func<T1> action, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryAsync<T1>(this Func<T1> action, Func<Exception, T1> excep, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryAsync<T1, T2>(this Func<T1, T2> action, T1 t1, Func<Exception, T2> excep, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, Func<Exception, T3> excep, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Run(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync(this Action action)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1>(this Action<T1> action, T1 t1)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryStartAsync<T1>(this Func<T1> action, T1 t1 = default)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                try { return action.Invoke(); }
                catch { return t1; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryStartAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2 = default)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch { return t2; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3 = default)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync(this Action action, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1>(this Action<T1> action, T1 t1, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryStartAsync<T1>(this Func<T1> action, T1 t1, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryStartAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryStartAsync<T1>(this Func<T1> action, Func<Exception, T1> excep)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryStartAsync<T1, T2>(this Func<T1, T2> action, T1 t1, Func<Exception, T2> excep)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync(this Action action, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1>(this Action<T1> action, T1 t1, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryStartAsync<T1>(this Func<T1> action, T1 t1, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                try { return action.Invoke(); }
                catch { return t1; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryStartAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch { return t2; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync(this Action action, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1>(this Action<T1> action, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryStartAsync<T1>(this Func<T1> action, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryStartAsync<T1, T2>(this Func<T1, T2> action, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryStartAsync<T1>(this Func<T1> action, Func<Exception, T1> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryStartAsync<T1, T2>(this Func<T1, T2> action, T1 t1, Func<Exception, T2> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, T1 t1, T2 t2, Func<Exception, T3> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        #endregion 异步尝试执行 Action/Func
        #region // 异步延迟尝试执行 Action/Func
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync(this Action action, TimeSpan time)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1 = default)
        {
            return new Task<T1>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(); }
                catch { return t1; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2 = default)
        {
            return new Task<T2>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1); }
                catch { return t2; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3 = default)
        {
            return new Task<T3>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            return new Task<T4>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            return new Task<T5>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            return new Task<T6>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            return new Task<T7>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            return new Task<T8>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            return new Task<T9>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            return new Task<TA>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            return new Task<TB>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            return new Task<TC>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            return new Task<TD>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            return new Task<TE>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            return new Task<TF>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            return new Task<TG>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            return new Task<TH>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync(this Action action, TimeSpan time, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return new Task(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1, Action<Exception> excep)
        {
            return new Task<T1>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep)
        {
            return new Task<T2>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return new Task<T3>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return new Task<T4>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return new Task<T5>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return new Task<T6>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return new Task<T7>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return new Task<T8>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return new Task<T9>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return new Task<TA>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return new Task<TB>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return new Task<TC>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return new Task<TD>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return new Task<TE>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return new Task<TF>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return new Task<TG>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            return new Task<TH>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryAsync<T1>(this Func<T1> action, TimeSpan time, Func<Exception, T1> excep)
        {
            return new Task<T1>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, Func<Exception, T2> excep)
        {
            return new Task<T2>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            return new Task<T3>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            return new Task<T4>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            return new Task<T5>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            return new Task<T6>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            return new Task<T7>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            return new Task<T8>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            return new Task<T9>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            return new Task<TA>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            return new Task<TB>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            return new Task<TC>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            return new Task<TD>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            return new Task<TE>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            return new Task<TF>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            return new Task<TG>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            return new Task<TH>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync(this Action action, TimeSpan time, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(); }
                catch { return t1; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1); }
                catch { return t2; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync(this Action action, TimeSpan time, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryAsync<T1>(this Func<T1> action, TimeSpan time, Func<Exception, T1> excep, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, Func<Exception, T2> excep, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, Func<Exception, T3> excep, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync(this Action action, TimeSpan time)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryStartAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1 = default)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(); }
                catch { return t1; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryStartAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2 = default)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1); }
                catch { return t2; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3 = default)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync(this Action action, TimeSpan time, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryStartAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryStartAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryStartAsync<T1>(this Func<T1> action, TimeSpan time, Func<Exception, T1> excep)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryStartAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, Func<Exception, T2> excep)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                TestTry.Delay(time);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync(this Action action, TimeSpan time, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryStartAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(); }
                catch { return t1; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryStartAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1); }
                catch { return t2; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2); }
                catch { return t3; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3); }
                catch { return t4; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch { return t5; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch { return t6; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch { return t7; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch { return t8; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch { return t9; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch { return ta; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch { return tb; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch { return tc; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch { return td; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch { return te; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch { return tf; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch { return tg; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch { return th; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync(this Action action, TimeSpan time, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1>(this Action<T1> action, TimeSpan time, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2>(this Action<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                TestTry.Delay(time, cancellation);
                try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryStartAsync<T1>(this Func<T1> action, TimeSpan time, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(); }
                catch (Exception ex) { excep?.Invoke(ex); return t1; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryStartAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1); }
                catch (Exception ex) { excep?.Invoke(ex); return t2; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { excep?.Invoke(ex); return t3; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { excep?.Invoke(ex); return t4; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { excep?.Invoke(ex); return t5; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { excep?.Invoke(ex); return t6; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { excep?.Invoke(ex); return t7; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { excep?.Invoke(ex); return t8; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { excep?.Invoke(ex); return t9; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { excep?.Invoke(ex); return ta; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { excep?.Invoke(ex); return tb; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { excep?.Invoke(ex); return tc; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { excep?.Invoke(ex); return td; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { excep?.Invoke(ex); return te; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { excep?.Invoke(ex); return tf; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { excep?.Invoke(ex); return tg; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { excep?.Invoke(ex); return th; }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> DelayTryStartAsync<T1>(this Func<T1> action, TimeSpan time, Func<Exception, T1> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> DelayTryStartAsync<T1, T2>(this Func<T1, T2> action, TimeSpan time, T1 t1, Func<Exception, T2> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> DelayTryStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TimeSpan time, T1 t1, T2 t2, Func<Exception, T3> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> DelayTryStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TimeSpan time, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> DelayTryStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> DelayTryStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> DelayTryStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TimeSpan time, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                TestTry.Delay(time, cancellation);
                try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                catch (Exception ex) { return excep.Invoke(ex); }
            }, cancellation);
        }
        #endregion 尝试执行 Action/Func
        #region // 异步锁定尝试执行 Action/Func
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync(this Action action, TaskTryEnterModel task)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1 = default)
        {
            return new Task<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch { return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2 = default)
        {
            return new Task<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch { return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3 = default)
        {
            return new Task<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch { return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            return new Task<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch { return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            return new Task<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch { return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            return new Task<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch { return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            return new Task<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            return new Task<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            return new Task<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            return new Task<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            return new Task<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            return new Task<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            return new Task<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            return new Task<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            return new Task<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            return new Task<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            return new Task<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync(this Action action, TaskTryEnterModel task, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, Action<Exception> excep)
        {
            return new Task<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, Action<Exception> excep)
        {
            return new Task<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return new Task<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return new Task<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return new Task<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return new Task<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return new Task<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return new Task<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return new Task<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return new Task<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return new Task<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return new Task<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return new Task<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return new Task<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return new Task<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return new Task<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            return new Task<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, Func<Exception, T1> excep)
        {
            return new Task<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, Func<Exception, T2> excep)
        {
            return new Task<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            return new Task<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            return new Task<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            return new Task<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            return new Task<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            return new Task<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            return new Task<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            return new Task<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            return new Task<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            return new Task<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            return new Task<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            return new Task<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            return new Task<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            return new Task<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            return new Task<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            return new Task<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync(this Action action, TaskTryEnterModel task, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch { return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch { return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch { return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch { return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch { return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch { return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync(this Action action, TaskTryEnterModel task, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, Func<Exception, T1> excep, CancellationToken cancellation)
        {
            return new Task<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, Func<Exception, T2> excep, CancellationToken cancellation)
        {
            return new Task<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, Func<Exception, T3> excep, CancellationToken cancellation)
        {
            return new Task<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep, CancellationToken cancellation)
        {
            return new Task<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep, CancellationToken cancellation)
        {
            return new Task<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep, CancellationToken cancellation)
        {
            return new Task<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep, CancellationToken cancellation)
        {
            return new Task<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep, CancellationToken cancellation)
        {
            return new Task<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep, CancellationToken cancellation)
        {
            return new Task<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep, CancellationToken cancellation)
        {
            return new Task<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep, CancellationToken cancellation)
        {
            return new Task<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep, CancellationToken cancellation)
        {
            return new Task<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep, CancellationToken cancellation)
        {
            return new Task<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep, CancellationToken cancellation)
        {
            return new Task<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep, CancellationToken cancellation)
        {
            return new Task<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep, CancellationToken cancellation)
        {
            return new Task<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写new Task(try...catch...)不附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep, CancellationToken cancellation)
        {
            return new Task<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation).StartAsync();
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync(this Action action, TaskTryEnterModel task)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterStartAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1 = default)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch { return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterStartAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2 = default)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch { return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3 = default)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch { return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4 = default)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch { return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 = default)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch { return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 = default)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch { return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 = default)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8 = default)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9 = default)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta = default)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb = default)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc = default)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td = default)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te = default)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf = default)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg = default)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th = default)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync(this Action action, TaskTryEnterModel task, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterStartAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterStartAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterStartAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, Func<Exception, T1> excep)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterStartAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, Func<Exception, T2> excep)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, Func<Exception, T3> excep)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            });
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync(this Action action, TaskTryEnterModel task, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterStartAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch { return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterStartAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch { return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch { return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch { return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch { return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch { return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch { return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch { return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch { return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch { return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch { return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch { return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch { return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch { return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch { return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch { return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch { return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync(this Action action, TaskTryEnterModel task, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1>(this Action<T1> action, TaskTryEnterModel task, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2>(this Action<T1, T2> action, TaskTryEnterModel task, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3>(this Action<T1, T2, T3> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (task.TryEnter())
                {
                    try { action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); }
                    finally { task.Exit(); }
                }
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterStartAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, T1 t1, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { excep?.Invoke(ex); return t1; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterStartAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, T2 t2, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { excep?.Invoke(ex); return t2; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, T3 t3, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { excep?.Invoke(ex); return t3; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, T4 t4, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { excep?.Invoke(ex); return t4; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { excep?.Invoke(ex); return t5; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { excep?.Invoke(ex); return t6; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { excep?.Invoke(ex); return t7; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { excep?.Invoke(ex); return t8; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { excep?.Invoke(ex); return t9; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { excep?.Invoke(ex); return ta; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { excep?.Invoke(ex); return tb; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { excep?.Invoke(ex); return tc; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { excep?.Invoke(ex); return td; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { excep?.Invoke(ex); return te; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { excep?.Invoke(ex); return tf; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { excep?.Invoke(ex); return tg; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, TH th, Action<Exception> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { excep?.Invoke(ex); return th; }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T1> TryEnterStartAsync<T1>(this Func<T1> action, TaskTryEnterModel<T1> task, Func<Exception, T1> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T1>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T2> TryEnterStartAsync<T1, T2>(this Func<T1, T2> action, TaskTryEnterModel<T2> task, T1 t1, Func<Exception, T2> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T2>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T3> TryEnterStartAsync<T1, T2, T3>(this Func<T1, T2, T3> action, TaskTryEnterModel<T3> task, T1 t1, T2 t2, Func<Exception, T3> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T3>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T4> TryEnterStartAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> action, TaskTryEnterModel<T4> task, T1 t1, T2 t2, T3 t3, Func<Exception, T4> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T4>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T5> TryEnterStartAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> action, TaskTryEnterModel<T5> task, T1 t1, T2 t2, T3 t3, T4 t4, Func<Exception, T5> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T5>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T6> TryEnterStartAsync<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> action, TaskTryEnterModel<T6> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<Exception, T6> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T6>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T7> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> action, TaskTryEnterModel<T7> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<Exception, T7> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T7>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T8> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> action, TaskTryEnterModel<T8> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<Exception, T8> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T8>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<T9> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, TaskTryEnterModel<T9> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<Exception, T9> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<T9>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TA> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> action, TaskTryEnterModel<TA> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<Exception, TA> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TA>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TB> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> action, TaskTryEnterModel<TB> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, Func<Exception, TB> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TB>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TC> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> action, TaskTryEnterModel<TC> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, Func<Exception, TC> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TC>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TD> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> action, TaskTryEnterModel<TD> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, Func<Exception, TD> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TD>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TE> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> action, TaskTryEnterModel<TE> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, Func<Exception, TE> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TE>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TF> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> action, TaskTryEnterModel<TF> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, Func<Exception, TF> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TF>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TG> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG> action, TaskTryEnterModel<TG> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, Func<Exception, TG> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TG>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        /// <summary>
        /// 简写Task.Factory.StartNew(try...catch...)附加到当前线程
        /// </summary>
        public static Task<TH> TryEnterStartAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, TG, TH> action, TaskTryEnterModel<TH> task, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, TA ta, TB tb, TC tc, TD td, TE te, TF tf, TG tg, Func<Exception, TH> excep, CancellationToken cancellation)
        {
            return Task.Factory.StartNew<TH>(() =>
            {
                if (task.TryEnter())
                {
                    try { return action.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, ta, tb, tc, td, te, tf, tg); }
                    catch (Exception ex) { return excep.Invoke(ex); }
                    finally { task.Exit(); }
                }
                return task.UnenterResult;
            }, cancellation);
        }
        #endregion 异步尝试锁定执行 Action/Func
        #region // 尝试执行数据库 TryDb
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="conn"></param>
        /// <param name="excep"></param>
        /// <returns></returns>
        public static T TryDb<T>(Func<DbConnection, DbTransaction, T> action, DbConnection conn, Func<Exception, T> excep)
        {
            try
            {
                using (conn)
                {
                    var trans = conn.BeginTransaction();
                    try { return action.Invoke(conn, trans); }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return excep.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return excep.Invoke(ex);
            }
        }
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="conn"></param>
        /// <param name="excep"></param>
        /// <returns></returns>
        public static IAlertMsg<T> TryDb<T>(Func<DbConnection, DbTransaction, IAlertMsg<T>> action, DbConnection conn, Func<Exception, IAlertMsg<T>> excep)
        {
            try
            {
                using (conn)
                {
                    var trans = conn.BeginTransaction();
                    try { return action.Invoke(conn, trans); }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return excep.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return excep.Invoke(ex);
            }
        }
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="conn"></param>
        /// <param name="excep"></param>
        /// <returns></returns>
        public static IAlertMsgs<T> TryDb<T>(Func<DbConnection, DbTransaction, IAlertMsgs<T>> action, DbConnection conn, Func<Exception, IAlertMsgs<T>> excep)
        {
            try
            {
                using (conn)
                {
                    var trans = conn.BeginTransaction();
                    try { return action.Invoke(conn, trans); }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return excep.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return excep.Invoke(ex);
            }
        }
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="conn"></param>
        /// <param name="excep"></param>
        /// <returns></returns>
        public static T TryDb<T>(Func<DbConnection, T> action, DbConnection conn, Func<Exception, T> excep)
        {
            try
            {
                using (conn)
                {
                    try { return action.Invoke(conn); }
                    catch (Exception ex)
                    {
                        return excep.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return excep.Invoke(ex);
            }
        }
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="conn"></param>
        /// <param name="excep"></param>
        /// <returns></returns>
        public static IAlertMsg<T> TryDb<T>(Func<DbConnection, IAlertMsg<T>> action, DbConnection conn, Func<Exception, IAlertMsg<T>> excep)
        {
            try
            {
                using (conn)
                {
                    try { return action.Invoke(conn); }
                    catch (Exception ex)
                    {
                        return excep.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return excep.Invoke(ex);
            }
        }
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="conn"></param>
        /// <param name="excep"></param>
        /// <returns></returns>
        public static IAlertMsgs<T> TryDb<T>(Func<DbConnection, IAlertMsgs<T>> action, DbConnection conn, Func<Exception, IAlertMsgs<T>> excep)
        {
            try
            {
                using (conn)
                {
                    try { return action.Invoke(conn); }
                    catch (Exception ex)
                    {
                        return excep.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return excep.Invoke(ex);
            }
        }
        #endregion 尝试执行数据库 TryDb
    }
}

