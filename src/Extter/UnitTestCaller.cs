﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Extter
{
    /// <summary>
    /// 单元测试调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestAction(string consoleFmt, Action action, int times = 10000)
        {
            action.Invoke();
            var now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                action.Invoke();
            }
            Console.WriteLine(consoleFmt, DateTime.Now - now, times);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestParallelAction(string consoleFmt, Action action, int times = 10000, int degree = 5)
        {
            action.Invoke();
            var now = DateTime.Now;
            Parallel.For(0, times, new ParallelOptions { MaxDegreeOfParallelism = degree }, (i) => action.Invoke());
            Console.WriteLine(consoleFmt, DateTime.Now - now, times, degree);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestFunc<T>(string consoleFmt, Func<T> action, int times = 10000)
        {
            action.Invoke();
            var now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                action.Invoke();
            }
            Console.WriteLine(consoleFmt, DateTime.Now - now);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static async Task TestActionAsync(string consoleFmt, Action action, int times = 10000)
        {
            await Task.Factory.StartNew(() => TestAction(consoleFmt, action, times));
        }
    }
}
