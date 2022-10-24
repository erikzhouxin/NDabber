using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.Impeller;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class FunctionUT
    {
        [TestMethod]
        public void RenameFiles()
        {
            var path = System.IO.Path.GetFullPath(@"F:\works\zex\nsolutionui\src\MaterialDesignWpfToolkit\Themes");
            var dir = new System.IO.DirectoryInfo(path);
            foreach (var item in dir.GetFiles())
            {
                var fileName = item.Name;
                if (fileName.StartsWith("MaterialDesignTheme"))
                {
                    item.MoveTo(System.IO.Path.Combine(path, fileName.Replace("MaterialDesignTheme", "Theme")));
                }
                if (fileName.EndsWith(" - 副本.xaml"))
                {
                    item.MoveTo(System.IO.Path.Combine(path, "NET40." + fileName.Replace(" - 副本", "")));
                }
            }
        }
        [TestMethod]
        public void ToEvenCalc()
        {
            // 单精度浮点数(Float)
            Console.WriteLine(Math.Round(18.254550f, 4, MidpointRounding.ToEven)); // 18.2546
            Console.WriteLine(Math.Round(18.745450f, 4, MidpointRounding.ToEven)); // 18.7454
            // 双精度浮点数(Double)
            Console.WriteLine(Math.Round(18.254550d, 4, MidpointRounding.ToEven)); // 18.2545
            Console.WriteLine(Math.Round(18.745450d, 4, MidpointRounding.ToEven)); // 18.7455
            // 高精度浮点数(Decimal)
            Console.WriteLine(Math.Round(18.254550m, 4, MidpointRounding.ToEven)); // 18.2546
            Console.WriteLine(Math.Round(18.745450m, 4, MidpointRounding.ToEven)); // 18.7454

            Console.WriteLine(string.Format("{0:f4}", 18.254550d));
            Console.WriteLine(string.Format("{0:f4}", 18.745450d));

            Console.WriteLine(string.Format("{0:f4}", 18.254550m));
            Console.WriteLine(string.Format("{0:f4}", 18.745450m));
        }
        [TestMethod]
        public void CreateGuidString()
        {
            Console.WriteLine(Guid.NewGuid());
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(Guid.NewGuid().GetString());
            }
        }
        [TestMethod]
        public void TestRandomNumber()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(new Random().Next(1, 100));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestHashSet()
        {
            var hashSet = new HashSet<Tuble<int, String>>();
            for (int i = 0; i < 100; i++)
            {
                hashSet.Add(new Tuble<int, String>(i, UserPassword.GetMd5Hash(Path.GetRandomFileName())));
                hashSet.Add(new Tuble<int, String>(i, ExtterCaller.GetRandomInt32().ToString("0000") + Path.GetRandomFileName()));
            }
            for (int i = 10; i < 20; i++)
            {
                var item = hashSet.First(s => i == s.Item1);
                hashSet.Add(item);
            }
            foreach (var item in hashSet)
            {
                Console.WriteLine($"{item.Item1:0000} => {item.Item2}");
            }
        }
        /// <summary>
        /// 测试Nano及Guid效率
        /// </summary>
        [TestMethod]
        public void TestGuidNanoID()
        {
            var times = 100;
            var now = DateTime.Now;
            Console.WriteLine(Guid.NewGuid().GetString());
            Console.WriteLine(Nanoid.Generate());
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                _ = Guid.NewGuid().ToString("N");
            }
            Console.WriteLine($"Guid    =>   {DateTime.Now - now}");
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                _ = Nanoid.Generate();
            }
            Console.WriteLine($"Nanoid  =>   {DateTime.Now - now}");
        }
        /// <summary>
        /// 测试时间区间
        /// </summary>
        [TestMethod]
        public void TestDateTimeOffset()
        {
            Console.WriteLine(DateTime.Now.GetDateTimeString());
            Console.WriteLine(DateTime.UtcNow.GetDateTimeString());
            Console.WriteLine(DateTimeOffset.Now.GetDateTimeString());
            Console.WriteLine(DateTimeOffset.UtcNow.GetDateTimeString());
            Console.WriteLine(new DateTimeOffset(DateTime.Now).GetDateTimeString());
            Console.WriteLine(new DateTimeOffset(DateTime.UtcNow).GetDateTimeString());
        }
        /// <summary>
        /// 获取空闲时间
        /// </summary>
        [TestMethod]
        public void TestGetIdleTime()
        {
            var inputInfo = new USER32.PLASTINPUTINFO();
            inputInfo.CbSize = Marshal.SizeOf(inputInfo);
            if (!USER32.GetLastInputInfo(ref inputInfo))
            {
                Console.WriteLine("获取失败");
            }
            var mili = TimeSpan.FromMilliseconds((long)Environment.TickCount - (long)inputInfo.DwTime);
            Console.WriteLine($"距离上次输入已经过去{mili}毫秒");
        }
    }
}
