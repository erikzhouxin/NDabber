using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 媒介调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytes(this MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin); //一定不要忘记将流的初始位置重置
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length); //如果上面流没有seek 则这里读取的数据全会为0
            ms.Dispose();
            return bytes;
        }
    }
}
