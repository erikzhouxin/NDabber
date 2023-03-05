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
        public TimeProfiler(string name, Action action) : this(action, name) { }

        public TimeSpan Run(int times)
        {
            var watch = Stopwatch.StartNew();
            while (times-- > 0) { action(); }
            watch.Stop();
            Trace.WriteLine($"{name}运行用时：{watch.Elapsed:d.hh:mm:ss.fffffff}");
            return watch.Elapsed;
        }
    }
}
