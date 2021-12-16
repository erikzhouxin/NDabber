using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class AttributeUT
    {
        public AttributeUT()
        {

        }
        [TestMethod]
        public void MyTestMethod()
        {
            var parent = AutoSQLiteBuilder.Builder<TestAttrParent>();
            var child = AutoSQLiteBuilder.Builder<TestAttrChild>();
            Console.WriteLine(parent.Select);
            Console.WriteLine(child.Select);

            /* SELECT[id] AS[Id],[name] AS[name],[age] AS[age] FROM[TestAttrParent]
     SELECT[Id] AS[Id],[Tester] AS[Tester],[age] AS[age] FROM[TestAttrChild]
    SELECT [id] AS [Id],[name] AS [name],[age] AS [age] FROM [TestAttrParent]
    SELECT [Id] AS [Id],[Tester] AS [Tester],[age] AS [age] FROM [TestAttrChild]

            */
        }
    }
    [DbCol("Test", Name = "TestAttrParent")]
    internal class TestAttrParent
    {
        [DbCol("标识", Name = "id")]
        public virtual string Id { get; set; }

        [DbCol("标识", Name = "name")]
        public virtual string name { get; set; }
        [DbCol("标识", Name = "age")]
        public virtual int age { get; set; }
        public virtual DateTime Birthday { get; set; }
    }
    [DbCol("Test", Name = "TestAttrChild")]
    internal class TestAttrChild : TestAttrParent
    {
        [DbCol("id2", Name = "id2")]
        public override string Id { get; set; }
        public override string name { get; set; }
        [DbCol("tester")]
        public virtual string Tester { get; set; }
    }
}
