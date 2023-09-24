using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Chineser;
using System.Data.Cobber;
using System.Data.Extter;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.DabberUT
{
    [TestClass]
    public class CobberUT
    {
        #region // PropertyAccess
        [TestMethod]
        public void TestAccess()
        {
            var testObject = new TestMan("ErikZhouXin") { Name = "ErikZhouXin" };
            ExtterCaller.GetPropertyAccess(typeof(TestMan)).FuncSetValue(null, nameof(TestMan.Value), "姓名");
            Console.WriteLine(ExtterCaller.GetPropertyAccess(typeof(TestMan)).FuncGetValue(null, nameof(TestMan.Value)));
            ExtterCaller.SetPropertyValue(typeof(TestMan), nameof(TestMan.Value), "名字");
            Console.WriteLine(ExtterCaller.GetPropertyValue(typeof(TestMan), nameof(TestMan.Value)));
            ExtterCaller.SetPropertyValue<TestMan>(nameof(TestMan.Value), "名儿");
            Console.WriteLine(ExtterCaller.GetPropertyValue<TestMan>(nameof(TestMan.Value)));

            ExtterCaller.GetPropertyAccess(typeof(TestMan)).FuncSetValue(testObject, nameof(TestMan.Name), "姓名");
            Console.WriteLine(ExtterCaller.GetPropertyAccess(typeof(TestMan)).FuncGetValue(testObject, nameof(TestMan.Name)));
            ExtterCaller.SetPropertyValue<TestMan>(testObject, nameof(TestMan.Name), "名儿");
            Console.WriteLine(ExtterCaller.GetPropertyValue<TestMan>(testObject, nameof(TestMan.Value)));

            foreach (var item in PropertyAccess<TestMan>.InternalSetDic)
            {
                item.Value.Invoke(testObject, item.Key);
            }

            foreach (var item in PropertyAccess<TestMan>.InternalGetDic)
            {
                Console.WriteLine(item.Value.Invoke(testObject));
            }
            PropertyAccess.SetInstance(testObject);
            Assert.IsTrue(PropertyAccess<TestMan>.Instance == testObject);
            PropertyAccess.SetSingleton(testObject);
            Assert.IsTrue(PropertyAccess<TestMan>.Instance == testObject);
        }

        [TestMethod]
        public void TestColors()
        {

        }

        [TestMethod]
        public void TestEqual()
        {
            var testObject = new TestMan100("ErikZhouXin") { Name90 = "ErikZhouXin" };
            var ra = new MemberReflectionAccessor();
            var ea = new MemberDelegatedExpressionAccessor();
            var da = new MemberDelegatedReflectionAccessor();
            var ma = new MemberExpressionAccessor();
            var testMemberName = nameof(TestMan100.Name90);
            //预热
            var val = ea.GetValue(testObject, testMemberName);
            ea.SetValue(testObject, testMemberName, val);
            Assert.IsTrue(val.Equals(testObject.Name90));

            val = ma.GetValue(testObject, testMemberName);
            ma.SetValue(testObject, testMemberName, val);
            Assert.IsTrue(val.Equals(testObject.Name90));

            val = ra.GetValue(testObject, testMemberName);
            ra.SetValue(testObject, testMemberName, val);
            Assert.IsTrue(val.Equals(testObject.Name90));

            val = da.GetValue(testObject, testMemberName);
            da.SetValue(testObject, testMemberName, val);
            Assert.IsTrue(val.Equals(testObject.Name90));
        }
        [TestMethod]
        public void PerformanceTest()
        {
            var runTime = 10000000;
            var testObject = new TestMan100("ErikZhouXin") { Name90 = "ErikZhouXin" };

            var ra = new MemberReflectionAccessor();
            var ea = new MemberDelegatedExpressionAccessor();
            var da = new MemberDelegatedReflectionAccessor();
            var ma = new MemberExpressionAccessor();
            var testMemberName = nameof(TestMan100.Name90);
            var pa = ExtterCaller.GetPropertyAccess(testObject.GetType());
            var ga = new PropertyAccess<TestMan100>();

            new TimeProfiler(() => testObject.Name90 = testObject.Name90, "直接调用").Run(runTime);
            TestClassMember(runTime, testObject, ra, ea, da, ma, pa, ga, testMemberName);
        }

        private static void TestClassMember<T>(int runTime, T testObject, MemberReflectionAccessor ra, MemberDelegatedExpressionAccessor ea, MemberDelegatedReflectionAccessor da, MemberExpressionAccessor ma, IPropertyAccess pa, PropertyAccess<T> ga, string testMemberName)
        {
            new TimeProfiler(() => ra.SetValue(testObject, testMemberName, ra.GetValue(testObject, testMemberName)), "反射调用").Run(runTime);
            new TimeProfiler(() => ea.SetValue(testObject, testMemberName, ea.GetValue(testObject, testMemberName)), "Expression委托调用").Run(runTime);
            new TimeProfiler(() => da.SetValue(testObject, testMemberName, da.GetValue(testObject, testMemberName)), "CreateDelegate委托调用").Run(runTime);
            new TimeProfiler(() => ma.SetValue(testObject, testMemberName, ma.GetValue(testObject, testMemberName)), "动态生成函数调用").Run(runTime);
            new TimeProfiler(() => pa.FuncSetValue(testObject, testMemberName, pa.FuncGetValue(testObject, testMemberName)), "动态生成函数优化").Run(runTime);
            new TimeProfiler(() => ga.SetValue(testObject, testMemberName, ga.GetValue(testObject, testMemberName)), "动态生成函数泛型优化").Run(runTime);
        }
        [TestMethod]
        public void TestDateTime()
        {
            var DateTime1970 = new DateTime(1970, 1, 1);
            Console.WriteLine((DateTime1970 - DateTime.MinValue).TotalSeconds);
            Console.WriteLine((DateTime1970 - new DateTime()).Ticks);
        }
        [TestMethod]
        public void TestCreateSameHashCode()
        {
            var lookup = new Dictionary<int, string>();

            while (true)
            {
                var s = RandomString();
                var h = s.GetHashCode();
                string s2;
                if (lookup.TryGetValue(h, out s2) && s2 != s)
                {
                    Console.WriteLine("MATCH FOUND!! {0} and {1} hash code {2}",
                        lookup[h],
                        s,
                        h);
                    break;
                }
                lookup[h] = s;

                if (lookup.Count % 1000 == 0)
                {
                    //Console.WriteLine(lookup.Count);
                }
            }
        }

        static Random r = new Random();

        // Define other methods and classes here
        static string RandomString()
        {
            var s = ((char)r.Next((int)'A', ((int)'Z') + 1)).ToString() +
                    ((char)r.Next((int)'A', ((int)'Z') + 1)).ToString() +
                    ((char)r.Next((int)'A', ((int)'Z') + 1)).ToString() +
                    ((char)r.Next((int)'A', ((int)'Z') + 1)).ToString() +
                    ((char)r.Next((int)'A', ((int)'Z') + 1)).ToString() +
                    ((char)r.Next((int)'A', ((int)'Z') + 1)).ToString();

            return s;
        }
        #region // 测试类
        /// <summary>
        /// 测试类
        /// </summary>
        private class TestMan
        {
            /// <summary>
            /// 静态值
            /// </summary>
            public static String Value { get; set; }
            public TestMan(string name)
            {
                Name = name;
            }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            public string Name00 { get; set; }
            public string Name01 { get; set; }
            public string Name02 { get; set; }
            public string Name03 { get; set; }
            public string Name04 { get; set; }
            public string Name05 { get; set; }
            public string Name06 { get; set; }
            public string Name07 { get; set; }
            public string Name08 { get; set; }
            public string Name09 { get; set; }
            public string Name10 { get; set; }
        }
        /// <summary>
        /// 测试类
        /// </summary>
        private class TestMan50 : TestMan
        {
            public TestMan50(string name) : base(name) { }
            public string Name11 { get; set; }
            public string Name12 { get; set; }
            public string Name13 { get; set; }
            public string Name14 { get; set; }
            public string Name15 { get; set; }
            public string Name16 { get; set; }
            public string Name17 { get; set; }
            public string Name18 { get; set; }
            public string Name19 { get; set; }
            public string Name20 { get; set; }
            public string Name21 { get; set; }
            public string Name22 { get; set; }
            public string Name23 { get; set; }
            public string Name24 { get; set; }
            public string Name25 { get; set; }
            public string Name26 { get; set; }
            public string Name27 { get; set; }
            public string Name28 { get; set; }
            public string Name29 { get; set; }
            public string Name30 { get; set; }
            public string Name31 { get; set; }
            public string Name32 { get; set; }
            public string Name33 { get; set; }
            public string Name34 { get; set; }
            public string Name35 { get; set; }
            public string Name36 { get; set; }
            public string Name37 { get; set; }
            public string Name38 { get; set; }
            public string Name39 { get; set; }
            public string Name40 { get; set; }
            public string Name41 { get; set; }
            public string Name42 { get; set; }
            public string Name43 { get; set; }
            public string Name44 { get; set; }
            public string Name45 { get; set; }
            public string Name46 { get; set; }
            public string Name47 { get; set; }
            public string Name48 { get; set; }
            public string Name49 { get; set; }
        }
        /// <summary>
        /// 测试类
        /// </summary>
        private class TestMan100 : TestMan50
        {
            public TestMan100(string name) : base(name) { }
            public string Aa { get; set; }
            public string BB { get; set; }
            public string Ba { get; set; }
            public string CB { get; set; }
            public string Ca { get; set; }
            public string DB { get; set; }
            public string xqzrbn { get; set; }
            public string krumld { get; set; }
            public string bvyfbj { get; set; }
            public string vzgbyh { get; set; }
            public string Name50 { get; set; }
            public string Name51 { get; set; }
            public string Name52 { get; set; }
            public string Name53 { get; set; }
            public string Name54 { get; set; }
            public string Name55 { get; set; }
            public string Name56 { get; set; }
            public string Name57 { get; set; }
            public string Name58 { get; set; }
            public string Name59 { get; set; }
            public string Name60 { get; set; }
            public string Name61 { get; set; }
            public string Name62 { get; set; }
            public string Name63 { get; set; }
            public string Name64 { get; set; }
            public string Name65 { get; set; }
            public string Name66 { get; set; }
            public string Name67 { get; set; }
            public string Name68 { get; set; }
            public string Name69 { get; set; }
            public string Name70 { get; set; }
            public string Name71 { get; set; }
            public string Name72 { get; set; }
            public string Name73 { get; set; }
            public string Name74 { get; set; }
            public string Name75 { get; set; }
            public string Name76 { get; set; }
            public string Name77 { get; set; }
            public string Name78 { get; set; }
            public string Name79 { get; set; }
            public string Name80 { get; set; }
            public string Name81 { get; set; }
            public string Name82 { get; set; }
            public string Name83 { get; set; }
            public string Name84 { get; set; }
            public string Name85 { get; set; }
            public string Name86 { get; set; }
            public string Name87 { get; set; }
            public string Name88 { get; set; }
            public string Name89 { get; set; }
            public string Name90 { get; set; }
            public string Name91 { get; set; }
            public string Name92 { get; set; }
            public string Name93 { get; set; }
            public string Name94 { get; set; }
            public string Name95 { get; set; }
            public string Name96 { get; set; }
            public string Name97 { get; set; }
            public string Name98 { get; set; }
            public string Name99 { get; set; }
        }
        #endregion
        #endregion // PropertyAccess
        #region // Chinese PinYin
        [TestMethod]
        public void MyTestMethod()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // 注册编码格式
            Console.WriteLine(Environment.Is64BitProcess);
            var pinyin = new ChineseChar('得');
            Console.WriteLine(pinyin.Pinyins.GetJsonString());
            Assert.IsTrue(ChineseChar.IsValidPinyin(pinyin.Pinyins[0]));
            Assert.IsTrue(ChineseChar.IsValidChar('啊'));
        }
        [TestMethod]
        public void TestCulture()
        {
            var cn = new CultureInfo("zh-CN");
            var cn1 = new CultureInfo("zh-cn");
            var cn2 = new CultureInfo("ZH-CN");
            Assert.AreEqual(cn.Name, cn1.Name);
            Assert.AreEqual(cn.Name, cn2.Name);
            Console.WriteLine($"{cn.Name} {cn1.Name} {cn2.Name}");
        }
        [TestMethod]
        public void TestCarNo()
        {
            var pattern = @"^([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[a-zA-Z](([DF]((?![IO])[a-zA-Z0-9](?![IO]))[0-9]{4})|([0-9]{5}[DF]))|[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1})$";
            var regex = new Regex(pattern);
            var carnos = new List<string>
            {
                "宁A123456",
                "宁ADDDF6",
                "宁A12345",
                "宁112345",
                "宁A1234",
                "宁AD12345",
                "宁A1234挂",
                "宁A123挂",
                "宁61234挂",
                "宁A12345D"
            };
            foreach (var item in carnos)
            {
                Console.WriteLine($"{item.PadRight(10, ' ')} => {regex.IsMatch(item)}");
            }
        }
        #endregion Chinese PinYin
    }
}
