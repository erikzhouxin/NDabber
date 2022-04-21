using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq;
using System.Net;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 扩展测试
    /// </summary>
    [TestClass]
    public class ExtterUT
    {
        #region // StringTest
        [TestMethod]
        public void TestCaseConverter()
        {
            Console.WriteLine("ATest".ToSnakeCase());
            Console.WriteLine("aTest".ToSnakeCase());
            Console.WriteLine("a_test".SnakeToCamelCase());
            Console.WriteLine("a_test".SnakeToPascalCase());
        }
        [TestMethod]
        public void TestStringSplit()
        {
            var strChar = "你,是，谁|哈 屁";

            IEnumerable<Tuble2String> list = new List<Tuble2String>()
            {
                new Tuble2String("你是谁哈皮","11"),
                new Tuble2String("你是谁哈屁","11"),
                new Tuble2String("你是谁哈哈","11"),
                new Tuble2String("你是谁哈狗屁","11"),
            };
            var result = list.SearchModels(strChar, new String[] { "Item1" });
            result.ForEach((s) => Console.WriteLine(s.Item1));
        }
        #endregion
        #region // IPAddressCaller
        [TestMethod]
        public void IPAddressUT()
        {
            Console.WriteLine(CobberCaller.GetIPv4Value("192.168.0.178"));
            Console.WriteLine(CobberCaller.GetIPv4Value("255.255.255.255"));
            Console.WriteLine(CobberCaller.GetIPv4Address(2130706433));
            Console.WriteLine(CobberCaller.GetIPv4Address(-1));
            Console.WriteLine((2130706433L).GetIPAddress());
            Console.WriteLine(IPAddress.Parse("192.168.0.178").GetValue());
        }
        #endregion
        #region // MemberCaller
        [TestMethod]
        public void TestCallMethodKey()
        {
            ExtterCaller.TestAction("{0}", () => Console.WriteLine(ExtterCaller.TraceMathodDayKey), 10000);
        }
        #endregion
        #region // NumberCaller
        [TestMethod]
        public void TestNumberUT()
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
        public void TestNumberUT2()
        {
            var intBytes = Int32.MaxValue.GetBytes();
            Console.WriteLine(intBytes.GetInt32());
            var uintBytes = uint.MaxValue.GetBytes();
            Console.WriteLine(uintBytes.GetUInt32());
            var shortBytes = short.MaxValue.GetBytes();
            Console.WriteLine(shortBytes.GetInt16());
            var ushortBytes = ushort.MaxValue.GetBytes();
            Console.WriteLine(ushortBytes.GetUInt16());
            var longBytes = long.MaxValue.GetBytes();
            Console.WriteLine(longBytes.GetInt64());
            var ulongBytes = ulong.MaxValue.GetBytes();
            Console.WriteLine(ulongBytes.GetUInt64());

            Console.WriteLine("-----------------------------");

            var intsBytes = Int32.MaxValue.GetSBytes();
            Console.WriteLine(intsBytes.GetInt32());
            var uintsBytes = uint.MaxValue.GetSBytes();
            Console.WriteLine(uintsBytes.GetUInt32());
            var shortsBytes = short.MaxValue.GetSBytes();
            Console.WriteLine(shortsBytes.GetInt16());
            var ushortsBytes = ushort.MaxValue.GetSBytes();
            Console.WriteLine(ushortsBytes.GetUInt16());
            var longsBytes = long.MaxValue.GetSBytes();
            Console.WriteLine(longsBytes.GetInt64());
            var ulongsBytes = ulong.MaxValue.GetSBytes();
            Console.WriteLine(ulongsBytes.GetUInt64());
        }
        [TestMethod]
        public void TestNumberLogicMath()
        {
            var intMax = Int64.MaxValue;
            var sb = (sbyte)intMax;
            Console.WriteLine(sb);
            Console.WriteLine(((byte)sb));
        }

        [TestMethod]
        public void TestNumberTimes()
        {
            var times = 100000;
            UnitTestCaller.TestFunc("地址反转=>{0}", () =>
            {
                var intBytes = BitConverter.GetBytes(Int32.MaxValue);
                Array.Reverse(intBytes);
                return intBytes;
            }, times);
            UnitTestCaller.TestFunc("地址反转=>{0}", () =>
            {
                var intBytes = BitConverter.GetBytes(Int32.MaxValue);
                Array.Reverse(intBytes);
                return intBytes;
            }, times);
            UnitTestCaller.TestFunc("逻辑强转=>{0}", () => NumberCaller.GetBytes(int.MaxValue), times);
            UnitTestCaller.TestFunc("逻辑强转=>{0}", () => NumberCaller.GetBytes(int.MaxValue), times);
        }

        [TestMethod]
        public void TestNumberRound()
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
        #endregion
        #region // AssemblyTypeCaller
        [TestMethod]
        public void TestAssemblyType()
        {
            foreach (var item in typeof(AssemblyTypeCaller).GetTypes())
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine("-------------------------------------");
            foreach (var item in typeof(AssemblyTypeCaller).GetTypes(typeof(AssemblyTypeCaller).Namespace))
            {
                Console.WriteLine(item.Name);
            }
        }
        #endregion
        #region // EndianBitConverter

        [TestMethod]
        public void IsLittleEndian()
        {
            //big-endian
            Assert.IsFalse(EndianBitConverter.BigEndian.IsLittleEndian);
            Assert.IsFalse(EndianBitConverter.BigEndian.IsMid);

            //mid big-endian
            Assert.IsFalse(EndianBitConverter.MidBigEndian.IsLittleEndian);
            Assert.IsTrue(EndianBitConverter.MidBigEndian.IsMid);

            //little-endian
            Assert.IsTrue(EndianBitConverter.LittleEndian.IsLittleEndian);
            Assert.IsFalse(EndianBitConverter.LittleEndian.IsMid);

            //mid little-endian
            Assert.IsTrue(EndianBitConverter.MidLittleEndian.IsLittleEndian);
            Assert.IsTrue(EndianBitConverter.MidLittleEndian.IsMid);
        }

        #region // GetBytes
        private static EndianBitConverter machineEndianBitConverter;
        private static EndianBitConverter otherEndianBitConverter;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            if (BitConverter.IsLittleEndian)
            {
                machineEndianBitConverter = EndianBitConverter.LittleEndian;
                otherEndianBitConverter = EndianBitConverter.BigEndian;
            }
            else
            {
                machineEndianBitConverter = EndianBitConverter.BigEndian;
                otherEndianBitConverter = EndianBitConverter.LittleEndian;
            }
        }

        [TestMethod]
        public void GetBytesFromBool()
        {
            // don't use System.BitConverter as an oracle for boolean true, as it can theoretically map to any non-zero byte
            AssertArraysEqual(new byte[] { 0x01 }, EndianBitConverter.BigEndian.GetBytes(true));
            AssertArraysEqual(new byte[] { 0x01 }, EndianBitConverter.LittleEndian.GetBytes(true));

            // compare to System.BitConverter
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, false);
        }

        [TestMethod]
        public void GetBytesFromChar()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, '\0');
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 'a');
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, '❤');
        }

        [TestMethod]
        public void GetBytesFromDouble()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0D);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0.123456789e100);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, double.Epsilon);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, double.MaxValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, double.MinValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, double.NaN);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, double.NegativeInfinity);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, double.PositiveInfinity);
        }

        [TestMethod]
        public void GetBytesFromShort()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, (short)0);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, (short)0x0123);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, short.MaxValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, short.MinValue);
        }

        [TestMethod]
        public void GetBytesFromInt()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0x01234567);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, int.MaxValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, int.MinValue);
        }

        [TestMethod]
        public void GetBytesFromLong()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0L);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0x0123456789ABCDEF);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, long.MaxValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, long.MinValue);
        }

        [TestMethod]
        public void GetBytesFromFloat()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0F);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0.123456e10F);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, float.Epsilon);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, float.MaxValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, float.MinValue);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, float.NaN);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, float.NegativeInfinity);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, float.PositiveInfinity);
        }

        [TestMethod]
        public void GetBytesFromUShort()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, (ushort)0);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, (ushort)0x0123);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, ushort.MaxValue);
        }

        [TestMethod]
        public void GetBytesFromUInt()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0U);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0x01234567U);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, uint.MaxValue);
        }

        [TestMethod]
        public void GetBytesFromULong()
        {
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0uL);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, 0x0123456789ABCDEFuL);
            AssertGetBytesResult(BitConverter.GetBytes, machineEndianBitConverter.GetBytes, otherEndianBitConverter.GetBytes, ulong.MaxValue);
        }

        private void AssertGetBytesResult<TInput>(
            Func<TInput, byte[]> bitConverterMethod,
            Func<TInput, byte[]> machineEndianBitConverterMethod,
            Func<TInput, byte[]> otherEndianBitConverterMethod,
            TInput testValue)
        {
            // compare endianness that matches the machine architecture
            byte[] expectedOutput = bitConverterMethod(testValue);
            byte[] machineEndianBitConverterOutput = machineEndianBitConverterMethod(testValue);
            AssertArraysEqual(expectedOutput, machineEndianBitConverterOutput);

            // compare other endianness by reversing the expected output of System.BitConverter
            expectedOutput = expectedOutput.Reverse();
            byte[] otherEndianBitConverterOutput = otherEndianBitConverterMethod(testValue);
            AssertArraysEqual(expectedOutput, otherEndianBitConverterOutput);
        }

        private void AssertArraysEqual(byte[] expected, byte[] actual)
        {
            Assert.IsNotNull(expected, "Expected value is null.");
            Assert.IsNotNull(actual, "Actual value is null.");
            Assert.AreEqual(expected.Length, actual.Length, "Length of array is incorrect.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], $"Incorrect value at index {i}.");
            }
        }
        #endregion 
        #region // GetValue
        [TestMethod]
        public void OneByte()
        {
            ValidateArgumentsChecks(machineEndianBitConverter.ToBoolean, otherEndianBitConverter.ToBoolean, sizeof(bool));

            AssertValueResult(BitConverter.ToBoolean, machineEndianBitConverter.ToBoolean, otherEndianBitConverter.ToBoolean,
                sizeof(bool), new byte[] { 0x00, 0x01, 0x00 }, 0);
            AssertValueResult(BitConverter.ToBoolean, machineEndianBitConverter.ToBoolean, otherEndianBitConverter.ToBoolean,
                sizeof(bool), new byte[] { 0x00, 0x01, 0x00 }, 1);
            AssertValueResult(BitConverter.ToBoolean, machineEndianBitConverter.ToBoolean, otherEndianBitConverter.ToBoolean,
                sizeof(bool), new byte[] { 0x00, 0x00, 0xFE }, 2);
        }

        [TestMethod]
        public void TwoBytes()
        {
            AssertTwoByteValueResult(BitConverter.ToChar, machineEndianBitConverter.ToChar, otherEndianBitConverter.ToChar);
            AssertTwoByteValueResult(BitConverter.ToInt16, machineEndianBitConverter.ToInt16, otherEndianBitConverter.ToInt16);
            AssertTwoByteValueResult(BitConverter.ToUInt16, machineEndianBitConverter.ToUInt16, otherEndianBitConverter.ToUInt16);
        }

        [TestMethod]
        public void FourBytes()
        {
            AssertFourByteValueResult(BitConverter.ToInt32, machineEndianBitConverter.ToInt32, otherEndianBitConverter.ToInt32);
            AssertFourByteValueResult(BitConverter.ToUInt32, machineEndianBitConverter.ToUInt32, otherEndianBitConverter.ToUInt32);
            AssertFourByteValueResult(BitConverter.ToSingle, machineEndianBitConverter.ToSingle, otherEndianBitConverter.ToSingle);
        }

        [TestMethod]
        public void EightBytes()
        {
            AssertEightByteValueResult(BitConverter.ToInt64, machineEndianBitConverter.ToInt64, otherEndianBitConverter.ToInt64);
            AssertEightByteValueResult(BitConverter.ToUInt64, machineEndianBitConverter.ToUInt64, otherEndianBitConverter.ToUInt64);
            AssertEightByteValueResult(BitConverter.ToDouble, machineEndianBitConverter.ToDouble, otherEndianBitConverter.ToDouble);
        }

        private void ValidateArgumentsChecks<TOutput>(
            Func<byte[], int, TOutput> machineEndianBitConverterMethod,
            Func<byte[], int, TOutput> otherEndianBitConverterMethod,
            int outputSize)
        {
            // null check
            Assert.ThrowsException<ArgumentNullException>(() => machineEndianBitConverterMethod(null, 0));
            Assert.ThrowsException<ArgumentNullException>(() => otherEndianBitConverterMethod(null, 0));

            // negative index
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => machineEndianBitConverterMethod(new byte[8], -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => otherEndianBitConverterMethod(new byte[8], -1));

            // index + outputSize longer than byte array
            const int arrayLength = 16;
            int badStartIndex = arrayLength - outputSize + 1;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => machineEndianBitConverterMethod(new byte[arrayLength], badStartIndex));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => otherEndianBitConverterMethod(new byte[arrayLength], badStartIndex));
        }

        private void AssertTwoByteValueResult<TOutput>(
            Func<byte[], int, TOutput> bitConverterMethod,
            Func<byte[], int, TOutput> machineEndianBitConverterMethod,
            Func<byte[], int, TOutput> otherEndianBitConverterMethod)
        {
            ValidateArgumentsChecks(machineEndianBitConverterMethod, otherEndianBitConverterMethod, 2);

            AssertValueResult(bitConverterMethod, machineEndianBitConverterMethod, otherEndianBitConverterMethod,
                2, new byte[] { 0x00, 0x01 }, 0);
            AssertValueResult(bitConverterMethod, machineEndianBitConverterMethod, otherEndianBitConverterMethod,
                2, new byte[] { 0x00, 0xAB, 0x00 }, 1);
        }

        private void AssertFourByteValueResult<TOutput>(
            Func<byte[], int, TOutput> bitConverterMethod,
            Func<byte[], int, TOutput> machineEndianBitConverterMethod,
            Func<byte[], int, TOutput> otherEndianBitConverterMethod)
        {
            ValidateArgumentsChecks(machineEndianBitConverterMethod, otherEndianBitConverterMethod, 4);

            AssertValueResult(bitConverterMethod, machineEndianBitConverterMethod, otherEndianBitConverterMethod,
                4, new byte[] { 0x00, 0x01, 0x02, 0x03 }, 0);
            AssertValueResult(bitConverterMethod, machineEndianBitConverterMethod, otherEndianBitConverterMethod,
                4, new byte[] { 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 1);
        }

        private void AssertEightByteValueResult<TOutput>(
            Func<byte[], int, TOutput> bitConverterMethod,
            Func<byte[], int, TOutput> machineEndianBitConverterMethod,
            Func<byte[], int, TOutput> otherEndianBitConverterMethod)
        {
            ValidateArgumentsChecks(machineEndianBitConverterMethod, otherEndianBitConverterMethod, 8);

            AssertValueResult(bitConverterMethod, machineEndianBitConverterMethod, otherEndianBitConverterMethod,
                8, new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }, 0);
            AssertValueResult(bitConverterMethod, machineEndianBitConverterMethod, otherEndianBitConverterMethod,
                8, new byte[] { 0x00, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 1);
        }

        private void AssertValueResult<TOutput>(
            Func<byte[], int, TOutput> bitConverterMethod,
            Func<byte[], int, TOutput> machineEndianBitConverterMethod,
            Func<byte[], int, TOutput> otherEndianBitConverterMethod,
            int outputSize,
            byte[] testValue,
            int testStartIndex)
        {
            // compare endianness that matches the machine architecture
            TOutput expectedOutput = bitConverterMethod(testValue, testStartIndex);
            TOutput machineEndianBitConverterOutput = machineEndianBitConverterMethod(testValue, testStartIndex);
            Assert.AreEqual(expectedOutput, machineEndianBitConverterOutput);

            // compare other endianness by reversing the input to System.BitConverter
            TOutput otherEndianBitConverterOutput = otherEndianBitConverterMethod(testValue, testStartIndex);
            testValue = testValue.Reverse();
            testStartIndex = testValue.Length - testStartIndex - outputSize;
            expectedOutput = bitConverterMethod(testValue, testStartIndex);
            Assert.AreEqual(expectedOutput, otherEndianBitConverterOutput);
        }
        #endregion
        #endregion
    }
}
