using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class SerialCallerUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            new Dictionary<int, int[]> { { 209, new int[] { 209, 290 } } }.GetJsonString().DebugConsole();
            int times = 10000;
            var now = DateTime.Now;
            TestJsonSerial(times, now);
            TestJsonSerial(times, now);
            TestBinSerial(times, now);
            TestBinSerial(times, now);
        }

        private static void TestJsonSerial(int times, DateTime now)
        {
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                var model = new TestC
                {
                    ID = i,
                    Name = $"名称{i}",
                };
                var json = model.GetJsonString();
                var newModel = json.GetJsonObject<TestC>();
                Assert.AreEqual(model.ID, newModel.ID);
            }
            Console.WriteLine("Json序列化:{0}", DateTime.Now - now);
        }

        private static void TestBinSerial(int times, DateTime now)
        {
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                var model = new TestC
                {
                    ID = i,
                    Name = $"名称{i}",
                };
                var bytes = model.GetBinBytes();
                var newModel = bytes.GetBinModel<TestC>();
                Assert.AreEqual(model.ID, newModel.ID);
            }
            Console.WriteLine("BIN 序列化:{0}", DateTime.Now - now);
        }

        [Serializable]
        private class TestC
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}
