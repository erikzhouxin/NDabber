using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Runtime.InteropServices;
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
        [TestMethod]
        public void StructToBytes()
        {
            People p1 = new People(10, 180, "李白");
            byte[] buff = Struct2Bytes(p1);
            foreach (var i in buff)
            {
                Console.Write(i + "\t");
            }
            People p2 = ByteToStruct<People>(buff);
            Console.WriteLine();
            Console.WriteLine(p2.Age + "\t" + p2.Name + "\t" + p2.Height);//输出10 李白 180
            Console.ReadKey();
        }

        //结构体转换为byte数组
        static byte[] Struct2Bytes(object o)
        {
            // create a new byte buffer the size of your struct
            byte[] buffer = new byte[Marshal.SizeOf(o)];
            // pin the buffer so we can copy data into it w/o GC collecting it
            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);//为指定的对象分配句柄，不通过GC回收,而是通过手动释放
                                                                                // copy the struct data into the buffer 
                                                                                //StructureToPtr参数1：托管对象，包含要封送的数据。该对象必须是格式化类的实例。
                                                                                //StructureToPtr参数2：指向非托管内存块的指针，必须在调用此方法之前分配该指针。 
                                                                                //StructureToPtr参数3：设置为 true 可在执行Marshal.DestroyStructure方法前对 ptr 参数调用此方法。请注意，传递 false 可导致内存泄漏。
                                                                                // bufferHandle.AddrOfPinnedObject():检索对象的地址并返回
            Marshal.StructureToPtr(o, bufferHandle.AddrOfPinnedObject(), false);//将数据从托管对象封送到非托管内存块。
                                                                                // free the GC handle
            bufferHandle.Free();
            return buffer;
        }

        //将byte数组转换为结构体
        static T ByteToStruct<T>(byte[] by) where T : struct
        {
            int objectSize = Marshal.SizeOf(typeof(T));
            if (objectSize > by.Length) return default(T);
            // 分配内存
            IntPtr buffer = Marshal.AllocHGlobal(objectSize);
            // 将数据复制到内存中
            Marshal.Copy(by, 0, buffer, objectSize);
            // Push the memory into a new struct of type (T).将数据封送到结构体T中
            T returnStruct = (T)Marshal.PtrToStructure(buffer, typeof(T));
            // Free the unmanaged memory block.释放内存
            Marshal.FreeHGlobal(buffer);
            return returnStruct;
        }
        struct People
        {
            public uint Age;
            public ushort Height;
            public string Name;
            public People(uint age, ushort height, string name)
            {
                this.Age = age;
                this.Height = height;
                this.Name = name;
            }
        }
    }
}
