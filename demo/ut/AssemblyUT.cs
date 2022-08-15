using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Extter;
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
            var libPath = Path.GetFullPath(@"F:\works\res\lngms\zmpc\Hardware\康美室外音柱\IPGBSDK_MUser\V2.4\C#\SdkServerV2.4\x64\debug\IPGBNET.dll");
            ConsoleExportedTypes(libPath);
            libPath = Path.GetFullPath(@"F:\works\res\lngms\zmpc\Hardware\康美室外音柱\IPGBSDK_MUser\V2.4\C#\SdkPushStreamClientV2.2\x64\debug\IPGBNETPush.dll");
            ConsoleExportedTypes(libPath);
        }

        private static void ConsoleExportedTypes(string libPath)
        {
            var assembly = Assembly.LoadFile(libPath);
            foreach (var item in assembly.GetExportedTypes())
            {
                Console.WriteLine(item.FullName);
            }
        }
    }
}
