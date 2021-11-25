using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Extter;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class NumberUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var intBytes = Int32.MaxValue.ReadBytes();
            Console.WriteLine(intBytes.ReadInt32());
            var uintBytes = uint.MaxValue.ReadBytes();
            Console.WriteLine(uintBytes.ReadUInt32());
            var shortBytes = short.MaxValue.ReadBytes();
            Console.WriteLine(shortBytes.ReadInt16());
            var ushortBytes = ushort.MaxValue.ReadBytes();
            Console.WriteLine(ushortBytes.ReadUInt16());
            var longBytes = long.MaxValue.ReadBytes();
            Console.WriteLine(longBytes.ReadInt64());
            var ulongBytes = ulong.MaxValue.ReadBytes();
            Console.WriteLine(ulongBytes.ReadUInt64());

            Console.WriteLine("-----------------------------");

            var intsBytes = Int32.MaxValue.ReadSBytes();
            Console.WriteLine(intsBytes.ReadInt32());
            var uintsBytes = uint.MaxValue.ReadSBytes();
            Console.WriteLine(uintsBytes.ReadUInt32());
            var shortsBytes = short.MaxValue.ReadSBytes();
            Console.WriteLine(shortsBytes.ReadInt16());
            var ushortsBytes = ushort.MaxValue.ReadSBytes();
            Console.WriteLine(ushortsBytes.ReadUInt16());
            var longsBytes = long.MaxValue.ReadSBytes();
            Console.WriteLine(longsBytes.ReadInt64());
            var ulongsBytes = ulong.MaxValue.ReadSBytes();
            Console.WriteLine(ulongsBytes.ReadUInt64());
        }
        [TestMethod]
        public void TestLogicMath()
        {
            var intMax = Int64.MaxValue;
            var sb = (sbyte)intMax;
            Console.WriteLine(sb);
            Console.WriteLine(((byte)sb));
        }

        [TestMethod]
        public void TestRound()
        {
            Console.WriteLine($"【11.314999】四舍五入    =>{Math.Round(11.31499999999, 2, MidpointRounding.AwayFromZero)}");
            Console.WriteLine($"【11.314999】银行家舍入法=>{Math.Round(11.31499999999, 2, MidpointRounding.ToEven)}");
            Console.WriteLine($"【11.315】四舍五入    =>{Math.Round(11.315, 2, MidpointRounding.AwayFromZero)}");
            Console.WriteLine($"【11.315】银行家舍入法=>{Math.Round(11.315, 2, MidpointRounding.ToEven)}");
            Console.WriteLine($"【11.305】四舍五入    =>{Math.Round(11.305, 2, MidpointRounding.AwayFromZero)}");
            Console.WriteLine($"【11.305】银行家舍入法=>{Math.Round(11.305, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.314999】四舍五入    =>{11.31499999999:0.00}");
            Console.WriteLine($"【11.314999】银行家舍入法=>{11.31499999999:#.##}");
            Console.WriteLine($"【11.314999】银行家舍入法=>{11.31499999999:f2}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.305】四舍五入    =>{11.305:0.00}");
            Console.WriteLine($"【11.305】银行家#.##  =>{11.305:#.##}");
            Console.WriteLine($"【11.305】银行家F2    =>{11.305:f2}");
            Console.WriteLine($"【11.305】银行家N2    =>{11.305:N2}");
            Console.WriteLine($"【11.305】银行家ToEven=>{Math.Round(11.305, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.315】四舍五入    =>{11.315:0.00}");
            Console.WriteLine($"【11.315】银行家#.##  =>{11.315:#.##}");
            Console.WriteLine($"【11.315】银行家F2    =>{11.315:f2}");
            Console.WriteLine($"【11.315】银行家N2    =>{11.315:N2}");
            Console.WriteLine($"【11.315】银行家ToEven=>{Math.Round(11.315, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.325】四舍五入    =>{11.325:0.00}");
            Console.WriteLine($"【11.325】银行家#.##  =>{11.325:#.##}");
            Console.WriteLine($"【11.325】银行家F2    =>{11.325:f2}");
            Console.WriteLine($"【11.325】银行家N2    =>{11.325:N2}");
            Console.WriteLine($"【11.325】银行家ToEven=>{Math.Round(11.325, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.335】四舍五入    =>{11.335:0.00}");
            Console.WriteLine($"【11.335】银行家#.##  =>{11.335:#.##}");
            Console.WriteLine($"【11.335】银行家F2    =>{11.335:f2}");
            Console.WriteLine($"【11.335】银行家N2    =>{11.335:N2}");
            Console.WriteLine($"【11.335】银行家ToEven=>{Math.Round(11.335, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.345】四舍五入    =>{11.345:0.00}");
            Console.WriteLine($"【11.345】银行家#.##  =>{11.345:#.##}");
            Console.WriteLine($"【11.345】银行家F2    =>{11.345:f2}");
            Console.WriteLine($"【11.345】银行家ToEven=>{Math.Round(11.345, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.355】四舍五入    =>{11.355:0.00}");
            Console.WriteLine($"【11.355】银行家#.##  =>{11.355:#.##}");
            Console.WriteLine($"【11.355】银行家F2    =>{11.355:f2}");
            Console.WriteLine($"【11.355】银行家N2    =>{11.355:N2}");
            Console.WriteLine($"【11.355】银行家ToEven=>{Math.Round(11.355, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.365】四舍五入    =>{11.365:0.00}");
            Console.WriteLine($"【11.365】银行家#.##  =>{11.365:#.##}");
            Console.WriteLine($"【11.365】银行家F2    =>{11.365:f2}");
            Console.WriteLine($"【11.365】银行家ToEven=>{Math.Round(11.365, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.375】四舍五入    =>{11.375:0.00}");
            Console.WriteLine($"【11.375】银行家#.##  =>{11.375:#.##}");
            Console.WriteLine($"【11.375】银行家F2    =>{11.375:f2}");
            Console.WriteLine($"【11.375】银行家ToEven=>{Math.Round(11.375, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.385】四舍五入    =>{11.385:0.00}");
            Console.WriteLine($"【11.385】银行家#.##  =>{11.385:#.##}");
            Console.WriteLine($"【11.385】银行家F2    =>{11.385:f2}");
            Console.WriteLine($"【11.385】银行家ToEven=>{Math.Round(11.385, 2, MidpointRounding.ToEven)}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"【11.395】四舍五入    =>{11.395:0.00}");
            Console.WriteLine($"【11.395】银行家#.##  =>{11.395:#.##}");
            Console.WriteLine($"【11.395】银行家F2    =>{11.395:f2}");
            Console.WriteLine($"【11.395】银行家ToEven=>{Math.Round(11.395, 2, MidpointRounding.ToEven)}");

        }
    }
}
