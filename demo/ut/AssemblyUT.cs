using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.SolutionCore;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 程序集测试
    /// </summary>
    [TestClass]
    public class AssemblyUT
    {
        delegate int MsgBox(int hWnd, string msg, string caption, int type);
        [TestMethod]
        public void MyTestMethod()
        {
            var assDll = ExtterBuilder.CreateCDllModel("user32.dll").GetDelegate<MsgBox>("MessageBoxA");
            assDll.Invoke(0, "这是个动态弹框", "动态挑战", 0x30);
            assDll = ExtterBuilder.CreateCDllModel("user32.dll").GetDelegate<MsgBox>("MessageBoxA");
            assDll.Invoke(0, "这是个动态弹框", "动态挑战", 0x30);
        }

        [TestMethod]
        public void GetAssemblyExportTypes()
        {
            var jsonString = File.ReadAllText("CodeAssemblyFiles.Json");
            var jsonValues = jsonString.GetJsonObject<List<String>>();
            foreach (string item in jsonValues)
            {
                ConsoleExportedTypes(item);
            }
        }

        private static void ConsoleExportedTypes(string libPath)
        {
            var assembly = Assembly.LoadFile(libPath);
            foreach (var item in assembly.GetExportedTypes())
            {
                Console.WriteLine(item.FullName);
            }
        }
        [TestMethod]
        public void TestAssembly()
        {
            var infoModel = new AssemblyInfoModel(typeof(AssemblyUT));
            Console.WriteLine(infoModel.Assembly.FullName);
            Console.WriteLine(Assembly.GetExecutingAssembly().FullName);
            Console.WriteLine(Assembly.GetEntryAssembly().FullName);
            Console.WriteLine(Assembly.GetCallingAssembly().FullName);
            Console.WriteLine("================================================");
            Console.WriteLine(infoModel.ExecutingAssembly.FullName);
            Console.WriteLine(infoModel.EntryAssembly.FullName);
            Console.WriteLine(infoModel.CallingAssembly.FullName);
            Console.WriteLine("================================================");
            var infoModel2 = new AssemblyInfoModel(typeof(TestTry));
            Console.WriteLine("================================================");
            Console.WriteLine(infoModel2.Assembly.FullName);
            Console.WriteLine(infoModel2.ExecutingAssembly.FullName);
            Console.WriteLine(infoModel2.EntryAssembly.FullName);
            Console.WriteLine(infoModel2.CallingAssembly.FullName);
        }
        [TestMethod]
        public void TestDebugable()
        {
            Console.WriteLine(TestTry.IsDebugMode);
        }
    }
}
