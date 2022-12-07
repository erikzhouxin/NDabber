using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.IO;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 文件测试
    /// </summary>
    [TestClass]
    public class FileUT
    {
        /// <summary>
        /// 移除SQL中UTF8编码影响
        /// </summary>
        [TestMethod]
        public void RemoveSqlUtf8Encode()
        {
            var filePath = Path.GetFullPath(@"F:\wezcs\.pubs\.bak");
            var fileName = Path.Combine(filePath, "20221123.qms.sql");
            var tempFileName = Path.Combine(filePath, @"temp.qms.sql");
            var ireplaceList = new List<Tuble2String>()
            {
                new Tuble2String("CHARACTER SET utf8 COLLATE utf8_bin ",""),
                new Tuble2String("CHARACTER SET utf8 COLLATE utf8_general_ci ",""),
                new Tuble2String("CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ",""),
                new Tuble2String("CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci ",""),

                new Tuble2String("CHARACTER SET = utf8 COLLATE = utf8_bin ",""),
                new Tuble2String("CHARACTER SET = utf8 COLLATE = utf8_general_ci ",""),
                new Tuble2String("CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ",""),
                new Tuble2String("CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ",""),

                //new Tuble2String("DEFAULT CHARSET=utf8mb3 COLLATE=utf8_bin ",""),
                //new Tuble2String("DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci ",""),

                new Tuble2String("ROW_FORMAT = COMPACT","ROW_FORMAT = Dynamic"),

                new Tuble2String("varchar(2550)","TEXT"),
                new Tuble2String("varchar(3000)","TEXT"),
                new Tuble2String("varchar(4000)","TEXT"),
                new Tuble2String("varchar(4096)","TEXT"),
                new Tuble2String("varchar(5000)","TEXT"),
                new Tuble2String("varchar(6000)","TEXT"),
            };
            var ignoretables = new List<String>()
            {
                "t_dictionary_duty_copy1",
                "t_menu_copy1",
                "t_log",
                "t_operation_log",
                "t_product_category1",
                "t_production_coal_washing_copy1",
                "t_storage_product_copy1",
                "t_storage_product1",
                "t_sts_assay_copy1",
                "t_sts_assay_extras_copy1",
                "t_sts_assay_items_copy1",
                "t_sts_assay_items_copy2",
                "t_sts_assay_result_copy1",
                "t_sts_assay_vad_copy1",
                "t_sts_sampling_2022-2-23 11:56:52",
                "t_sts_sampling_copy1",
                "t_sts_sampling_copy2",
                "t_sts_sampling_recorder_copy1",
                "t_sts_sampling_recorder_copy2",
                "t_sts_show_samp_copy1",
                "t_truck_dayreport_ladingbill_copy1",
                "t_truck_ladingbill_copy1",
                "t_truck_ladingbill_copy2",
                "t_truck_ladingbill_copy3",
                "t_truck_ladingbill_sale11",
                "t_truck_plan_copy1",
                "t_user_copy1",
                "t_user_copy2",
                "t_v_plan_volume_copy1",
                "t_v_plan_volume_copy2",
            };
            File.WriteAllText("CopySqlReplaceContent.json", new object[] { new CopySqlReplaceContentModel
            {
                Src = fileName,
                Tag = tempFileName,
                Replaces = ireplaceList.AsList<Tuble<String,String>>(),
                IgnoreTables = ignoretables
            } }.GetJsonFormatString());
            long lineCount = 0;
            long lineReplace = 0;
            long lineIgnore = 0;
            using (var ofs = new StreamReader(fileName))
            {
                using (var nfs = new StreamWriter(new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    int tag = 0;
                    while (true)
                    {
                        var line = ofs.ReadLine();
                        lineCount++;
                        if (line == null) { break; }
                        if (line.StartsWith("INSERT"))
                        {
                            bool isIgnore = false;
                            foreach (var item in ignoretables)
                            {
                                if (line.StartsWith($"INSERT INTO `{item}`"))
                                {
                                    isIgnore = true;
                                    lineIgnore++;
                                    break;
                                }
                            }
                            if (isIgnore) { continue; }
                        }
                        else
                        {
                            foreach (var item in ireplaceList)
                            {
                                if (line.Contains(item.Item1))
                                {
                                    lineReplace++;
                                    line = line.Replace(item.Item1, item.Item2);
                                }
                            }
                        }
                        nfs.WriteLine(line);
                        if (tag++ > 1000)
                        {
                            tag = 0;
                            nfs.Flush();
                        }
                    }
                    nfs.Flush();
                }
            }
            Console.WriteLine($"文件总行数:{lineCount}行,已处理替换:{lineReplace}次,已忽略{lineIgnore}行");
        }
        internal class CopySqlReplaceContentModel
        {
            public string Src { get; set; }
            public string Tag { get; set; }
            public List<Tuble<string, string>> Replaces { get; set; }
            public List<string> IgnoreTables { get; set; }
        }
    }
}
