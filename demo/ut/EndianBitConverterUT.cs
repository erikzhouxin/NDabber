using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Extter;
using System.Linq;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class EndianBitConverterUT
    {
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
            expectedOutput = expectedOutput.Reverse().ToArray();
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
            testValue = testValue.Reverse().ToArray();
            testStartIndex = testValue.Length - testStartIndex - outputSize;
            expectedOutput = bitConverterMethod(testValue, testStartIndex);
            Assert.AreEqual(expectedOutput, otherEndianBitConverterOutput);
        }
        #endregion
    }
}
