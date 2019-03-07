using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    partial class BinString
    {
        private T _BitConvert<T>(int length, Func<byte[], int, T> convertFunction, IntegerEndianess endianess = IntegerEndianess.Native)
        {
            var source = ConvertEndianess(endianess, IntegerEndianess.Native);

            if (source.Length == 0)
            {
                return default(T);
            }
            else if (source.Length == length)
            {
                return convertFunction(source._Data, 0);
            }
            else if (source.Length < length)
            {
                if (BitConverter.IsLittleEndian)
                {
                    source = source.PadRight(length);
                }
                else
                {
                    source = source.PadLeft(length);
                }
                return convertFunction(source._Data, 0);
            }
            else
            {
                throw new OverflowException();
            }
        }

        /// <summary>
        /// Changes the endianess of this BinString from the source to the destination.
        /// </summary>
        /// <param name="from">The source endianess.</param>
        /// <param name="to">The target endianess.</param>
        public BinString ConvertEndianess(IntegerEndianess from, IntegerEndianess to)
        {
            if (from == IntegerEndianess.Native) from = BitConverter.IsLittleEndian ? IntegerEndianess.LittleEndian : IntegerEndianess.BigEndian;
            if (to == IntegerEndianess.Native) to = BitConverter.IsLittleEndian ? IntegerEndianess.LittleEndian : IntegerEndianess.BigEndian;

            byte[] d = new byte[_Data.Length];
            _Data.CopyTo(d, 0);
            if (from != to) Array.Reverse(d);
            return new BinString(d);
        }

        short IConvertible.ToInt16(IFormatProvider provider) => _BitConvert(2, BitConverter.ToInt16);
        ushort IConvertible.ToUInt16(IFormatProvider provider) => _BitConvert(2, BitConverter.ToUInt16);
        int IConvertible.ToInt32(IFormatProvider provider) => _BitConvert(4, BitConverter.ToInt32);
        uint IConvertible.ToUInt32(IFormatProvider provider) => _BitConvert(4, BitConverter.ToUInt32);
        long IConvertible.ToInt64(IFormatProvider provider) => _BitConvert(8, BitConverter.ToInt64);
        ulong IConvertible.ToUInt64(IFormatProvider provider) => _BitConvert(8, BitConverter.ToUInt64);

        /// <summary>
        /// Interprets the BinString as a Int16 with the specified endianess.
        /// </summary>
        public short ToInt16(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(2, BitConverter.ToInt16, endianess);

        /// <summary>
        /// Interprets the BinString as a UInt16 with the specified endianess.
        /// </summary>
        public ushort ToUInt16(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(2, BitConverter.ToUInt16, endianess);

        /// <summary>
        /// Interprets the BinString as a Int32 with the specified endianess.
        /// </summary>
        public int ToInt32(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(4, BitConverter.ToInt32, endianess);

        /// <summary>
        /// Interprets the BinString as a UInt32 with the specified endianess.
        /// </summary>
        public uint ToUInt32(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(4, BitConverter.ToUInt32, endianess);

        /// <summary>
        /// Interprets the BinString as a Int64 with the specified endianess.
        /// </summary>
        public long ToInt64(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(8, BitConverter.ToInt64, endianess);

        /// <summary>
        /// Interprets the BinString as a UInt64 with the specified endianess.
        /// </summary>
        public ulong ToUInt64(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(8, BitConverter.ToUInt64, endianess);

        /// <summary>
        /// Interperts the integer as a BinString with the specified endianess.
        /// </summary>
        public static BinString FromInt16(short value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);

        /// <summary>
        /// Interperts the integer as a BinString with the specified endianess.
        /// </summary>
        public static BinString FromUInt16(ushort value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);

        /// <summary>
        /// Interperts the integer as a BinString with the specified endianess.
        /// </summary>
        public static BinString FromInt32(int value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);

        /// <summary>
        /// Interperts the integer as a BinString with the specified endianess.
        /// </summary>
        public static BinString FromUInt32(uint value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);

        /// <summary>
        /// Interperts the integer as a BinString with the specified endianess.
        /// </summary>
        public static BinString FromInt64(long value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);

        /// <summary>
        /// Interperts the integer as a BinString with the specified endianess.
        /// </summary>
        public static BinString FromUInt64(ulong value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
    }

    /// <summary>
    /// The desired endiness of an integer conversion
    /// </summary>
    public enum IntegerEndianess
    {
        /// <summary>
        /// The endianess of the native system, based on BitConverter.IsLittleEndian.
        /// </summary>
        Native = 0,

        /// <summary>
        /// Big-endian format, with the least significant byte last.
        /// </summary>
        BigEndian = 1,

        /// <summary>
        /// Little-endian format, with the least significant byte first.
        /// </summary>
        LittleEndian = 2
    }
}
