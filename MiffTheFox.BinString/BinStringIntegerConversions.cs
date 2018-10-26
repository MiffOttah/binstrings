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

        public BinString ConvertEndianess(IntegerEndianess from, IntegerEndianess to)
        {
            if (from == IntegerEndianess.Native) from = BitConverter.IsLittleEndian ? IntegerEndianess.LittleEndian : IntegerEndianess.BigEndian;
            if (to == IntegerEndianess.Native) to = BitConverter.IsLittleEndian ? IntegerEndianess.LittleEndian : IntegerEndianess.BigEndian;

            byte[] d = new byte[_Data.Length];
            _Data.CopyTo(d, 0);
            if (from != to) Array.Reverse(d);
            return new BinString(d);
        }

        public short ToInt16(IFormatProvider provider) => _BitConvert(2, BitConverter.ToInt16);
        public ushort ToUInt16(IFormatProvider provider) => _BitConvert(2, BitConverter.ToUInt16);
        public int ToInt32(IFormatProvider provider) => _BitConvert(4, BitConverter.ToInt32);
        public uint ToUInt32(IFormatProvider provider) => _BitConvert(4, BitConverter.ToUInt32);
        public long ToInt64(IFormatProvider provider) => _BitConvert(8, BitConverter.ToInt64);
        public ulong ToUInt64(IFormatProvider provider) => _BitConvert(8, BitConverter.ToUInt64);

        public short ToInt16(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(2, BitConverter.ToInt16, endianess);
        public ushort ToUInt16(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(2, BitConverter.ToUInt16, endianess);
        public int ToInt32(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(4, BitConverter.ToInt32, endianess);
        public uint ToUInt32(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(4, BitConverter.ToUInt32, endianess);
        public long ToInt64(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(8, BitConverter.ToInt64, endianess);
        public ulong ToUInt64(IntegerEndianess endianess = IntegerEndianess.Native) => _BitConvert(8, BitConverter.ToUInt64, endianess);

        public static BinString FromInt16(short value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
        public static BinString FromUInt16(ushort value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
        public static BinString FromInt32(int value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
        public static BinString FromUInt32(uint value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
        public static BinString FromInt64(long value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
        public static BinString FromUInt64(ulong value, IntegerEndianess endiness = IntegerEndianess.Native) => new BinString(BitConverter.GetBytes(value)).ConvertEndianess(IntegerEndianess.Native, endiness);
    }

    /// <summary>
    /// The desired endiness of an integer conversion
    /// </summary>
    public enum IntegerEndianess
    {
        Native = 0,
        BigEndian = 1,
        LittleEndian = 2
    }
}
