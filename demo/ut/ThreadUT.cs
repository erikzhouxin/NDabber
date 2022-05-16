using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
