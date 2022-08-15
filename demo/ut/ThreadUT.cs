using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 线程测试
    /// </summary>
    [TestClass]
    public class ThreadUT
    {
        [TestMethod]
        public void TestEvent()
        {
            Console.WriteLine(TaskScheduler.Default.MaximumConcurrencyLevel);
            Console.WriteLine(TaskScheduler.Current.MaximumConcurrencyLevel);
            var testEvent = new TestEventClass();
            testEvent.WhenAge += (age) =>
            {
                Thread.Sleep(age * 1000);
                Console.WriteLine(age >= 32 ? $"{age:d2}岁到了,需要结婚了" : $"{age:d2}岁还小,可以继续浪");
            };
            Parallel.For(0, 35, new ParallelOptions() { MaxDegreeOfParallelism = 50 }, (i) =>
            {
                testEvent.OnAgeChanged(i);
            });
        }
        [TestMethod]
        public void TestInputIDLETime()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(GetIdleTime());
            }
        }
        /// <summary>
        /// 获取空闲时间
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetIdleTime()
        {
            var inputInfo = new PLASTINPUTINFO();
            inputInfo.cbSize = Marshal.SizeOf(inputInfo);
            if (!GetLastInputInfo(ref inputInfo)) { return TimeSpan.Zero; }
            return TimeSpan.FromMilliseconds(Environment.TickCount - inputInfo.dwTime);
        }
        [DllImport("user32.dll")]
        internal static extern bool GetLastInputInfo(ref PLASTINPUTINFO plii);
        /// <summary>
        /// 最后一次输入信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct PLASTINPUTINFO
        {
            /// <summary>
            /// 大小
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            /// <summary>
            /// 时间
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        internal class TestEventClass
        {
            public event Action<Int32> WhenAge;
            /// <summary>
            /// 当年龄变化时
            /// </summary>
            /// <param name="i"></param>
            public void OnAgeChanged(int i)
            {
                WhenAge?.Invoke(i);
            }
        }
    }
}
