using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Chineser;
using System.Data.Cobber;
using System.Data.Extter;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.DabberUT
{
    public class FunctionUT
    {
        [Test]
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
        [Test]
        public void CreateGuidString()
        {
            Console.WriteLine(Guid.NewGuid());
            Console.WriteLine(Guid.NewGuid().GetString());
        }
        [Test]
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
        [Test]
        public void TestHashSet()
        {
            var times = 1000;
            var hashSet = new HashSet<Tuble<int, String>>();
            var hashSet2 = new HashSet<object>()
            {
                1,2,17,33
            };
            var hashSet3 = new HashSet<int>();
            var hashSet4 = new HashSet<int>();
            HashSet<int> hashSet5 = new HashSet<int>();
            hashSet5.Add(1);
            hashSet5.Add(2);
            hashSet5.Add(3);
            hashSet5.Add(4);
            hashSet5.Add(5);
            for (int i = 0; i < times; i++)
            {
                hashSet.Add(new Tuble<int, String>(i, UserPassword.GetMd5Hash(Path.GetRandomFileName())));
                hashSet.Add(new Tuble<int, String>(i, ExtterCaller.GetRandomInt32().ToString("0000") + Path.GetRandomFileName()));
                hashSet3.Add(ExtterCaller.GetRandomInt32(10000) + i * 1000000);
                hashSet4.Add(ExtterCaller.GetRandomInt32(10000, 90000) * 1000 + i);
            }
            hashSet.Clear();
            //hashSet2.Clear();
            hashSet3.Clear();
            hashSet4.Clear();
            for (int i = times - 1; i >= 0; i--)
            {
                hashSet.Add(new Tuble<int, String>(i, UserPassword.GetMd5Hash(Path.GetRandomFileName())));
                hashSet.Add(new Tuble<int, String>(i, ExtterCaller.GetRandomInt32().ToString("0000") + Path.GetRandomFileName()));
                hashSet3.Add(ExtterCaller.GetRandomInt32(1000) + i * 10000);
                hashSet4.Add(ExtterCaller.GetRandomInt32(1000, 9900) * 1000 + i);
            }
            for (int i = 10; i < 15; i++)
            {
                var item = hashSet.First(s => i == s.Item1);
                hashSet.Add(item);
            }
            foreach (var item in hashSet.ToArray())
            {
                Console.WriteLine($"{item.Item1:0000} => {item.Item2}");
            }
            Console.WriteLine("====================================================================");
            foreach (var item in hashSet2)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("====================================================================");
            foreach (var item in hashSet3)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("====================================================================");
            foreach (var item in hashSet4)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("====================================================================");
            foreach (var item in hashSet5)
            {
                Console.WriteLine(item);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestPinYinCovnerter()
        {
            Assert.IsTrue(ChineseChar.IsValidPinyin("ni"));
            Assert.IsTrue(ChineseChar.IsValidChar('啊'));
        }
    }
}
