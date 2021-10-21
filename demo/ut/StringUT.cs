using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
    }
}
