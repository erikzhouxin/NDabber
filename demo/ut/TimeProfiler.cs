using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeProfiler
    {
        private Action action = null;
        private string name = null;

        public TimeProfiler(Action action, string name)
        {
            this.action = action;
            this.name = name;
        }

        public TimeSpan Run(int times)
        {
            var watch = Stopwatch.StartNew();
            while (times-- > 0)
            {
                action();
            }
            watch.Stop();

            Trace.WriteLine(String.Format("{0}运行用时：{1}ms", name, watch.Elapsed.TotalMilliseconds));

            return watch.Elapsed;
        }
    }
}
