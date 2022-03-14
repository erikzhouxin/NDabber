using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Data.Extter
{
    /// <summary>
    /// A BitConverter with a specific endianness that converts base data types to an array of bytes, and an array of bytes to base data types, regardless of
    /// machine architecture. Access the little-endian and big-endian converters with their respective properties.
    /// </summary>
    /// <remarks>
    /// The EndianBitConverter implementations provide the same interface as <see cref="System.BitConverter"/>, but exclude those methods which perform the
    /// same on both big-endian and little-endian machines (such as <see cref="BitConverter.ToString(byte[])"/>). However, <see cref="GetBytes(bool)"/> is
    /// included for consistency.
    /// </remarks>
    public abstract class EndianBitConverter
    {
        /// <summary>
        /// Get an instance of a <see cref="LittleEndianBitConverter"/>, a BitConverter which performs all conversions in little-endian format regardless of
        /// machine architecture.
        /// </summary>
        public static EndianBitConverter LittleEndian { get; } = new LittleEndianBitConverter();

        /// <summary>
        /// Get an instance of a <see cref="MidLittleEndianBitConverter"/>, a BitConverter which performs all conversions in mid little-endian format regardless of
        /// machine architecture.
        /// </summary>
        public static EndianBitConverter MidLittleEndian { get; } = new MidLittleEndianBitConverter();

        /// <summary>
        /// Get an instance of a <see cref="BigEndianBitConverter"/>, a BitConverter which performs all conversions in big-endian format regardless of
        /// machine architecture.
        /// </summary>
        public static EndianBitConverter BigEndian { get; } = new BigEndianBitConverter();

        /// <summary>
        /// Get an instance of a <see cref="MidBigEndianBitConverter"/>, a BitConverter which performs all conversions in mid big-endian format regardless of
        /// machine architecture.
        /// </summary>
        public static EndianBitConverter MidBigEndian { get; } = new MidBigEndianBitConverter();

        /// <summary>
        /// Indicates the byte order ("endianness") in which data should be converted.
        /// </summary>
        public abstract bool IsLittleEndian { get; }

        /// <summary>
        /// Indicates if the bytes should be converted from mid.
        /// </summary>
        public abstract bool IsMid { get; }


        /// <summary>
        /// Returns the specified Boolean value as a byte array.
        /// </summary>
        /// <param name="value">A Boolean value.</param>
        /// <returns>A byte array with length 1.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Boolean"/> value by calling the <see cref="ToBoolean(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(bool value)
        {
            return new byte[] { value ? (byte)1 : (byte)0 };
        }

        /// <summary>
        /// Returns the specified Unicode character value as an array of bytes.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Char"/> value by calling the <see cref="ToChar(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(char value)
        {
            return this.GetBytes((short)value);
        }

        /// <summary>
        /// Returns the specified double-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Double"/> value by calling the <see cref="ToDouble(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(double value)
        {
            long val = BitConverter.DoubleToInt64Bits(value);
            return this.GetBytes(val);
        }

        /// <summary>
        /// Returns the specified 16-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Int16"/> value by calling the <see cref="ToInt16(byte[], int)"/> method.</remarks>
        public abstract byte[] GetBytes(short value);

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Int32"/> value by calling the <see cref="ToInt32(byte[], int)"/> method.</remarks>
        public abstract byte[] GetBytes(int value);

        /// <summary>
        /// Returns the specified 64-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Int64"/> value by calling the <see cref="ToInt64(byte[], int)"/> method.</remarks>
        public abstract byte[] GetBytes(long value);

        /// <summary>
        /// Returns the specified single-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="Single"/> value by calling the <see cref="ToSingle(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(float value)
        {
            int val = new SingleConverter(value).GetIntValue();
            return this.GetBytes(val);
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>An array of bytes with length 2.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="UInt16"/> value by calling the <see cref="ToUInt16(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(ushort value)
        {
            return this.GetBytes((short)value);
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="UInt32"/> value by calling the <see cref="ToUInt32(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(uint value)
        {
            return this.GetBytes((int)value);
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        /// <remarks>You can convert a byte array back to a <see cref="UInt64"/> value by calling the <see cref="ToUInt64(byte[], int)"/> method.</remarks>
        public byte[] GetBytes(ulong value)
        {
            return this.GetBytes((long)value);
        }

        /// <summary>
        /// Returns a Boolean value converted from the byte at a specified position in a byte array.
        /// </summary>
        /// <param name="value">A byte array.</param>
        /// <param name="startIndex">The index of the byte within <paramref name="value"/>.</param>
        /// <returns>
        /// true if the byte at <paramref name="startIndex"/> in <paramref name="value"/> is nonzero; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 1.
        /// </exception>
        public bool ToBoolean(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, 1);

            return value[startIndex] != 0;
        }

        /// <summary>
        /// Returns a Unicode character converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A character formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToChar method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 1 to a <see cref="Char"/> value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 2.
        /// </exception>
        public char ToChar(byte[] value, int startIndex)
        {
            return (char)this.ToInt16(value, startIndex);
        }

        /// <summary>
        /// Returns a double-precision floating point number converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A double precision floating point number formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToDouble method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 7 to a <see cref="Double"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 8.
        /// </exception>
        public double ToDouble(byte[] value, int startIndex)
        {
            long val = this.ToInt64(value, startIndex);
            return BitConverter.Int64BitsToDouble(val);
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToInt16 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 1 to an <see cref="Int16"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 2.
        /// </exception>
        public abstract short ToInt16(byte[] value, int startIndex);

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes. </param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToInt32 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 3 to an <see cref="Int32"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 4.
        /// </exception>
        public abstract int ToInt32(byte[] value, int startIndex);

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToInt64 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 7 to an <see cref="Int64"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 8.
        /// </exception>
        public abstract long ToInt64(byte[] value, int startIndex);

        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A single-precision floating point number formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToSingle method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 3 to a <see cref="Single"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 4.
        /// </exception>
        public float ToSingle(byte[] value, int startIndex)
        {
            int val = this.ToInt32(value, startIndex);
            return new SingleConverter(val).GetFloatValue();
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToUInt16 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 1 to a <see cref="UInt16"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 2.
        /// </exception>
        public ushort ToUInt16(byte[] value, int startIndex)
        {
            return (ushort)this.ToInt16(value, startIndex);
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes. </param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToUInt32 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 3 to a <see cref="UInt32"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 4.
        /// </exception>
        public uint ToUInt32(byte[] value, int startIndex)
        {
            return (uint)this.ToInt32(value, startIndex);
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes. </param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 64-bit unsigned integer formed by the eight bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <remarks>
        /// The ToUInt64 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 7 to a <see cref="UInt64"/>
        /// value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 8.
        /// </exception>
        public ulong ToUInt64(byte[] value, int startIndex)
        {
            return (ulong)this.ToInt64(value, startIndex);
        }

        // Testing showed that this method wasn't automatically being inlined, and doing so offers a significant performance improvement.
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal void CheckArguments(byte[] value, int startIndex, int byteLength)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // confirms startIndex is not negative or too far along the byte array
            if ((uint)startIndex > value.Length - byteLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
    /// <summary>
    /// A big-endian BitConverter that converts base data types to an array of bytes, and an array of bytes to base data types. All conversions are in
    /// big-endian format regardless of machine architecture.
    /// </summary>
    internal class BigEndianBitConverter : EndianBitConverter
    {
        // Instance available from EndianBitConverter.BigEndian
        internal BigEndianBitConverter() { }

        public override bool IsLittleEndian { get; } = false;
        public override bool IsMid { get; } = false;
        public override byte[] GetBytes(short value)
        {
            return new byte[] { (byte)(value >> 8), (byte)value };
        }

        public override byte[] GetBytes(int value)
        {
            return new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
        }

        public override byte[] GetBytes(long value)
        {
            return new byte[] {
                (byte)(value >> 56), (byte)(value >> 48), (byte)(value >> 40), (byte)(value >> 32),
                (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value
            };
        }

        public override short ToInt16(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(short));

            return (short)((value[startIndex] << 8) | (value[startIndex + 1]));
        }

        public override int ToInt32(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(int));

            return (value[startIndex] << 24) | (value[startIndex + 1] << 16) | (value[startIndex + 2] << 8) | (value[startIndex + 3]);
        }

        public override long ToInt64(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(long));

            int highBytes = (value[startIndex] << 24) | (value[startIndex + 1] << 16) | (value[startIndex + 2] << 8) | (value[startIndex + 3]);
            int lowBytes = (value[startIndex + 4] << 24) | (value[startIndex + 5] << 16) | (value[startIndex + 6] << 8) | (value[startIndex + 7]);
            return ((uint)lowBytes | ((long)highBytes << 32));
        }
    }
    /// <summary>
    /// A little-endian BitConverter that converts base data types to an array of bytes, and an array of bytes to base data types. All conversions are in
    /// little-endian format regardless of machine architecture.
    /// </summary>
    internal class LittleEndianBitConverter : EndianBitConverter
    {
        // Instance available from EndianBitConverter.LittleEndian
        internal LittleEndianBitConverter() { }

        public override bool IsLittleEndian { get; } = true;
        public override bool IsMid { get; } = false;
        public override byte[] GetBytes(short value)
        {
            return new byte[] { (byte)value, (byte)(value >> 8) };
        }

        public override byte[] GetBytes(int value)
        {
            return new byte[] { (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24) };
        }

        public override byte[] GetBytes(long value)
        {
            return new byte[] {
                (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
                (byte)(value >> 32), (byte)(value >> 40), (byte)(value >> 48), (byte)(value >> 56)
            };
        }

        public override short ToInt16(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(short));

            return (short)((value[startIndex]) | (value[startIndex + 1] << 8));
        }

        public override int ToInt32(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(int));

            return (value[startIndex]) | (value[startIndex + 1] << 8) | (value[startIndex + 2] << 16) | (value[startIndex + 3] << 24);
        }

        public override long ToInt64(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(long));

            int lowBytes = (value[startIndex]) | (value[startIndex + 1] << 8) | (value[startIndex + 2] << 16) | (value[startIndex + 3] << 24);
            int highBytes = (value[startIndex + 4]) | (value[startIndex + 5] << 8) | (value[startIndex + 6] << 16) | (value[startIndex + 7] << 24);
            return ((uint)lowBytes | ((long)highBytes << 32));
        }
    }
    /// <summary>
    /// A mid big-endian BitConverter that converts base data types to an array of bytes, and an array of bytes to base data types. 
    /// All conversions are in mid big-endian format regardless of machine architecture.
    /// </summary>
    internal class MidBigEndianBitConverter : EndianBitConverter
    {
        // Instance available from EndianBitConverter.BigEndian
        internal MidBigEndianBitConverter() { }

        public override bool IsLittleEndian { get; } = false;
        public override bool IsMid { get; } = true;


        public override byte[] GetBytes(short value)
        {
            return new byte[] { (byte)(value >> 8), (byte)value };
        }

        public override byte[] GetBytes(int value)
        {
            return new byte[] { (byte)(value >> 16), (byte)(value >> 24), (byte)value, (byte)(value >> 8) };
        }

        public override byte[] GetBytes(long value)
        {
            return new byte[] {
                (byte)(value >> 48), (byte)(value >> 56), (byte)(value >> 32), (byte)(value >> 40),
                (byte)(value >> 16), (byte)(value >> 24), (byte)value, (byte)(value >> 8)
            };
        }

        public override short ToInt16(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(short));

            return (short)((value[startIndex] << 8) | (value[startIndex + 1]));
        }

        public override int ToInt32(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(int));

            return (value[startIndex] << 16) | (value[startIndex + 1] << 24) | (value[startIndex + 2]) | (value[startIndex + 3] << 8);
        }

        public override long ToInt64(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(long));

            int highBytes = (value[startIndex] << 16) | (value[startIndex + 1] << 24) | (value[startIndex + 2]) | (value[startIndex + 3] << 8);
            int lowBytes = (value[startIndex + 4] << 16) | (value[startIndex + 5] << 24) | (value[startIndex + 6]) | (value[startIndex + 7] << 8);
            return ((uint)lowBytes | ((long)highBytes << 32));
        }
    }
    /// <summary>
    /// A mid little-endian BitConverter that converts base data types to an array of bytes, and an array of bytes to base data types. 
    /// All conversions are in mid little-endian format regardless of machine architecture.
    /// </summary>
    internal class MidLittleEndianBitConverter : EndianBitConverter
    {
        // Instance available from EndianBitConverter.MidLittleEndian
        internal MidLittleEndianBitConverter() { }

        public override bool IsLittleEndian { get; } = true;
        public override bool IsMid { get; } = true;


        public override byte[] GetBytes(short value)
        {
            return new byte[] { (byte)value, (byte)(value >> 8) };
        }

        public override byte[] GetBytes(int value)
        {
            return new byte[] { (byte)(value >> 8), (byte)value, (byte)(value >> 24), (byte)(value >> 16) };
        }

        public override byte[] GetBytes(long value)
        {
            return new byte[] {
                (byte)(value >> 8), (byte)value, (byte)(value >> 24), (byte)(value >> 16),
                (byte)(value >> 40), (byte)(value >> 32), (byte)(value >> 56), (byte)(value >> 48)
            };
        }

        public override short ToInt16(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(short));

            return (short)((value[startIndex]) | (value[startIndex + 1] << 8));
        }

        public override int ToInt32(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(int));

            return (value[startIndex] << 8) | (value[startIndex + 1]) | (value[startIndex + 2] << 24) | (value[startIndex + 3] << 16);
        }

        public override long ToInt64(byte[] value, int startIndex)
        {
            this.CheckArguments(value, startIndex, sizeof(long));

            int lowBytes = (value[startIndex] << 8) | (value[startIndex + 1]) | (value[startIndex + 2] << 24) | (value[startIndex + 3] << 16);
            int highBytes = (value[startIndex + 4] << 8) | (value[startIndex + 5]) | (value[startIndex + 6] << 24) | (value[startIndex + 7] << 16);
            return ((uint)lowBytes | ((long)highBytes << 32));
        }
    }
    // Converts between Single (float) and Int32 (int), as System.BitConverter does not have a method to do this in all .NET versions.
    // A union is used instead of an unsafe pointer cast so we don't have to worry about the trusted environment implications.
    [StructLayout(LayoutKind.Explicit)]
    internal struct SingleConverter
    {
        // map int value to offset zero
        [FieldOffset(0)]
        private int intValue;

        // map float value to offset zero - intValue and floatValue now take the same position in memory
        [FieldOffset(0)]
        private float floatValue;

        internal SingleConverter(int intValue)
        {
            this.floatValue = 0;
            this.intValue = intValue;
        }

        internal SingleConverter(float floatValue)
        {
            this.intValue = 0;
            this.floatValue = floatValue;
        }

        internal int GetIntValue() => this.intValue;

        internal float GetFloatValue() => this.floatValue;
    }
}
