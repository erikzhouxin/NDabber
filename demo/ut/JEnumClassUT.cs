using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Logger;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// Java类型的Enum实现
    /// </summary>
    [TestClass]
    public class JEnumClassUT
    {
        /// <summary>
        /// 构造
        /// </summary>
        public JEnumClassUT()
        {

        }
        /// <summary>
        /// 生成测试
        /// </summary>
        [TestMethod]
        public void Builder()
        {
            var saveFolder = Path.GetFullPath(Directory.GetCurrentDirectory());
            var types = new Type[]
            {
                typeof(StoreType)
            };

            foreach (var item in types)
            {
                if (!item.IsEnum) { continue; }
                var sb = JavaEnum.BuilderContent(item);
                File.WriteAllText(Path.Combine(saveFolder, $"JEnum{item.Name}.cs"), sb.ToString());
            }
        }
        /// <summary>
        /// 生成测试
        /// </summary>
        [TestMethod]
        public void BuilderSelf()
        {
            var saveFolder = Path.GetFullPath(Directory.GetCurrentDirectory());
            var types = new Type[]
            {
                typeof(LoggerType)
            };

            foreach (var item in types)
            {
                if (!item.IsEnum) { continue; }
                var sb = JavaEnum.BuildSelfContent(item);
                File.WriteAllText(Path.Combine(saveFolder, $"JEnum{item.Name}.cs"), sb.ToString());
            }
        }
    }
}