using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.Sqller;
using System.Text;

namespace System.Data.DabberUT.SqllerUT
{
    /// <summary>
    /// Sqlite脚本测试
    /// </summary>
    [TestClass]
    public class SqlScriptUT
    {
        [TestMethod]
        [DataRow(StoreType.SQLite)]
        [DataRow(StoreType.MySQL)]
        [DataRow(StoreType.SqlServer)]
        public void InsertTest(StoreType storeType)
        {
            var insertSqls = GetInsertSqls(storeType);
            Console.WriteLine(insertSqls);

            var updateSqls = GetUpdateSqls(storeType);
            Console.Write(updateSqls);
        }
        public StringBuilder GetInsertSqls(StoreType storeType)
        {
            return new StringBuilder()
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).From<TSysParams>().SqlScript)
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).From(typeof(TSysSettings)).SqlScript)
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).UseSelect<TSysParams>().Select(SqlScriptBuilder.CreateSimpleSelect(storeType).FromWhere<TSysParams>()).SqlScript)
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).UseSelect(typeof(TSysSettings)).Select(SqlScriptBuilder.CreateSimpleSelect(storeType).FromWhere(typeof(TSysSettings))).SqlScript);
        }
        public StringBuilder GetUpdateSqls(StoreType storeType)
        {
            return new StringBuilder()
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).From<TSysParams>().SetAParam(nameof(TSysParams.Key)).SetBParam(nameof(TSysParams.Value), "Value").Where().AndEqualAParam("ID", 123).SqlScript)
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).From(typeof(TSysParams)).SetClause("Value = IFNULL(Value,'',Value)").Where().SqlScript)
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).FromWhere<TSysParams>().SqlScript)
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).FromWhere(typeof(TSysSettings)).SqlScript);
        }
    }
}
