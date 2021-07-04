using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class MemberAccessorUT
    {
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
            var testObject = new TestMan100("ErikZhouXin") { Name90 = "ErikZhouXin"};

            var ra = new MemberReflectionAccessor();
            var ea = new MemberDelegatedExpressionAccessor();
            var da = new MemberDelegatedReflectionAccessor();
            var ma = new MemberExpressionAccessor();
            var testMemberName = nameof(TestMan100.Name90);

            new TimeProfiler(() => testObject.Name90 = testObject.Name90, "直接调用").Run(runTime);
            TestClassMember(runTime, testObject, ra, ea, da, ma, testMemberName);
        }

        private static void TestClassMember<T>(int runTime, T testObject, MemberReflectionAccessor ra, MemberDelegatedExpressionAccessor ea, MemberDelegatedReflectionAccessor da, MemberExpressionAccessor ma, string testMemberName)
        {
            new TimeProfiler(() => ra.SetValue(testObject, testMemberName, ra.GetValue(testObject, testMemberName)), "反射调用").Run(runTime);
            new TimeProfiler(() => ea.SetValue(testObject, testMemberName, ea.GetValue(testObject, testMemberName)), "Expression委托调用").Run(runTime);
            new TimeProfiler(() => da.SetValue(testObject, testMemberName, da.GetValue(testObject, testMemberName)), "CreateDelegate委托调用").Run(runTime);
            new TimeProfiler(() => ma.SetValue(testObject, testMemberName, ma.GetValue(testObject, testMemberName)), "动态生成函数调用").Run(runTime);
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
    }
}
