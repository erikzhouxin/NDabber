using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace System
{
    public static partial class TestTry
    {
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        [Obsolete("替代方案:Try")]
        public static void TryCatch(this Delegate tryer)
        {
            try { tryer.DynamicInvoke(); }
            catch { tryer.DynamicInvoke(); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        [Obsolete("替代方案:Try")]
        public static void TryCatch(this Delegate tryer, params object[] args)
        {
            try { tryer.DynamicInvoke(args); }
            catch { tryer.DynamicInvoke(args); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        [Obsolete("替代方案:TryCatch")]
        public static void TryCatch(this Delegate tryer, Delegate catcher)
        {
            try { tryer.DynamicInvoke(); }
            catch { catcher.DynamicInvoke(); }
        }
        /// <summary>
        /// 简写try{action}catch{}
        /// </summary>
        [Obsolete("替代方案:TryNext")]
        public static void TryTryCatch(this Delegate tryer)
        {
            try { tryer.DynamicInvoke(); }
            catch { try { tryer.DynamicInvoke(); } catch { } }
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [Obsolete("替代方案:Try")]
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
        [Obsolete("替代方案:Try")]
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
        [Obsolete("替代方案:Try")]
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
        [Obsolete("替代方案:Try")]
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
        [Obsolete("替代方案:Try")]
        public static Exception Call(this Delegate action, params object[] args)
        {
            try
            {
                action.DynamicInvoke(args);
                return null;
            }
            catch (Exception ex) { return ex; }
        }
        /// <summary>
        /// 演示调用(内有Try)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="action"></param>
        [Obsolete("替代方案:DelayTryStartAsync")]
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
        [Obsolete("替代方案:TryStartAsync")]
        public static Task TaskStartNew(this Delegate method)
        {
            return Task.Factory.StartNew(() => method?.DynamicInvoke());
        }
        /// <summary>
        /// 启动新任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        [Obsolete("替代方案:TryStartAsync")]
        public static Task TaskStartNew(this Delegate method, params object[] args)
        {
            return Task.Factory.StartNew(() => method?.DynamicInvoke(args));
        }
        /// <summary>
        /// 启动新任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="cancellationToken"></param>
        [Obsolete("替代方案:TryStartAsync")]
        public static Task TaskStartNew(this Delegate method, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => method?.DynamicInvoke(), cancellationToken);
        }
        /// <summary>
        /// 有TryCatch的启动新任务
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [Obsolete("替代方案:TryStartAsync")]
        public static Task TryTaskStartNew(this Delegate method)
        {
            return Task.Factory.StartNew(() => { try { method.DynamicInvoke(); } catch { } });
        }
        /// <summary>
        /// 有TryCatch的启动新任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("替代方案:TryStartAsync")]
        public static Task TryTaskStartNew(this Delegate method, params object[] args)
        {
            return Task.Factory.StartNew(() => { try { method.DynamicInvoke(args); } catch { } });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="time"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("替代方案:DelayTryTaskStartNew")]
        public static Task TryDelayTaskStartNew(this Delegate method, TimeSpan time, params object[] args)
        {
            return Task.Factory.StartNew(() => { Thread.Sleep(time); try { method.DynamicInvoke(args); } catch { } });
        }
        /// <summary>
        /// 有TryCatch的启动延时新任务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="milliseconds"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("替代方案:DelayTryTaskStartNew")]
        public static Task TryDelayTaskStartNew(this Delegate method, int milliseconds, params object[] args)
        {
            return Task.Factory.StartNew(() => { Thread.Sleep(milliseconds); try { method.DynamicInvoke(args); } catch { } });
        }
    }
}
