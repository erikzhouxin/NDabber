using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 打开调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFile(string fileName) => StartFile(new FileInfo(fileName));
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFile(this FileInfo fileName)
        {
            Process.Start(new ProcessStartInfo(fileName.FullName) { UseShellExecute = true });
        }
    }
}
