using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
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
    }
}
