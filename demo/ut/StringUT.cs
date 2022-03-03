using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class StringUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            Console.WriteLine("ATest".ToSnakeCase());
            Console.WriteLine("aTest".ToSnakeCase());
            Console.WriteLine("a_test".SnakeToCamelCase());
            Console.WriteLine("a_test".SnakeToPascalCase());
        }
        [TestMethod]
        public void TestSplit()
        {
            var strChar = "你,是，谁|哈 屁";

            IEnumerable<Tuble2String> list = new List<Tuble2String>()
            {
                new Tuble2String("你是谁哈皮","11"),
                new Tuble2String("你是谁哈屁","11"),
                new Tuble2String("你是谁哈哈","11"),
                new Tuble2String("你是谁哈狗屁","11"),
            };
            var result = list.SearchModels(strChar, new String[] { "Item1" });
            result.ForEach((s) => Console.WriteLine(s.Item1));
        }
    }
}
