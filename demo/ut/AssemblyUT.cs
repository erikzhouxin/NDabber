using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Extter;
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
    }
}
