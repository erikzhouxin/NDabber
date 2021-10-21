using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Net;
using System.Text;

namespace NRfidUT.CodeGener
{
    [TestClass]
    public class IPAddressUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            Console.WriteLine(CobberCaller.GetIPv4Value("192.168.0.178"));
            Console.WriteLine(CobberCaller.GetIPv4Value("255.255.255.255"));
            Console.WriteLine(CobberCaller.GetIPv4Address(2130706433));
            Console.WriteLine(CobberCaller.GetIPv4Address(-1));
            Console.WriteLine((2130706433L).GetIPAddress());
            Console.WriteLine(IPAddress.Parse("192.168.0.178").GetValue());
        }
    }
}
