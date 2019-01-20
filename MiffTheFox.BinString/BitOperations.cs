using System;
using System.IO;

namespace MiffTheFox.BitOperations
{
    /// <summary>
    /// Provides a method of creating binary data bit-per-bit
    /// </summary>
    public class BitWriter
    {
        private readonly BinStringBuilder _Builder = new BinStringBuilder();

        private int _Index = 0;
        private int _Value = 0;

        /// <summary>
        /// The number of bits written to the writer so far
        /// </summary>
        public int BitsWritten => _Builder.Length * 8 + _Index;

        /// <summary>
        /// Appends the given bit
        /// </summary>
        /// <param name="bit">The bit to append, true for 1 and false for 0</param>
        public void Write(bool bit)
        {
            if (bit) _Value |= 1 << 7 - _Index;
            _Index++;

            if (_Index >= 8)
            {
                _Builder.Append((byte)_Value);
                _Value = 0;
                _Index = 0;
            }
        }

        /// <summary>
        /// Appends the `count` least significant bits of `bitSource`
        /// </summary>
        /// <param name="bitSource">The source of bits to write</param>
        /// <param name="count">The number of bits (starting from the least significant bits) to append.</param>
        public void WriteBits(int bitSource, int count)
        {
            if (count < 0 || count > 32) throw new ArgumentException("Invalid number of bits.", nameof(count));

            // This could probably be optimized to write chunks of bits at one time.
            for (int i = count - 1; i >= 0; i--)
            {
                int mask = 1 << i;
                Write((bitSource & mask) != 0);
            }
        }

        /// <summary>
        /// Converts the bitwise data to a binary string
        /// </summary>
        /// <param name="unevenMode">The behavior to use when an uneven (non multiple of 8) bits have been written.</param>
        /// <returns></returns>
        public BinString ToBinString(BitWriterUnevenMode unevenMode)
        {
            if (_Index == 0)
            {
                return _Builder.ToBinString();
            }
            else
            {
                switch (unevenMode)
                {
                    case BitWriterUnevenMode.Disallow:
                        throw new InvalidOperationException("A noneven number of bits have been written to the BitWriter.");

                    case BitWriterUnevenMode.Pad:
                        return _Builder.ToBinString() + Convert.ToByte(_Value);

                    case BitWriterUnevenMode.Truncate:
                        return _Builder.ToBinString();

                    default:
                        throw new ArgumentException("Invalid uneven mode.", nameof(unevenMode));
                }
            }
        }

        /// <summary>
        /// Converts the bitwise data to a binary string, disallowing uneven lengths
        /// </summary>
        public BinString ToBinString() => ToBinString(BitWriterUnevenMode.Disallow);
    }

    /// <summary>
    /// Specifis behavior when attempting to retireve a BinString from a BitWriter has an uneven number of bits written to it.
    /// </summary>
    public enum BitWriterUnevenMode
    {
        /// <summary>
        /// A InvalidOperationException will be thrown.
        /// </summary>
        Disallow = 0,

        /// <summary>
        /// The data will have a number of zero bits appended to bring it to an even length.
        /// </summary>
        Pad = 1,

        /// <summary>
        /// The data will be truncated to the last even multiple of eight bits.
        /// </summary>
        Truncate = 2
    }

    /// <summary>
    /// Provides a method of consuming binary data bit-by-bit
    /// </summary>
    public class BitReader
    {
        private readonly Stream _DataSource;

        // default values will be initialized at the first read
        // but cancel out in computing Position
        // (-1 * 8) + 8 == 0
        private int _ByteIndex = -1;
        private int _BitIndex = 8;
        private int _Current = -1;

        public int Position => _ByteIndex * 8 + _BitIndex;
        public int Length => Convert.ToInt32(_DataSource.Length) * 8;

        public BitReader(Stream source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            _DataSource = source;
        }

        public BitReader(BinString source) : this(source.ToStream())
        {
        }

        /// <summary>
        /// Attempts to read a single bit
        /// </summary>
        /// <param name="value">The bit value read</param>
        /// <returns>True if sucessfully read, false if the end of stream was reached.</returns>
        public bool TryReadBit(out bool value)
        {
            if (_BitIndex == 8)
            {
                // reached the end of the byte we have, read another one
                _Current = _DataSource.ReadByte();
                if (_Current == -1)
                {
                    // reached the end of the data source
                    value = false;
                    return false;
                }

                _BitIndex = 0;
                _ByteIndex++;
            }

            value = (_Current & (1 << (7 - _BitIndex))) != 0;
            _BitIndex++;
            return true;
        }

        /// <summary>
        /// Attempts to read up to 32 bits.
        /// </summary>
        /// <param name="count">The maximum number of bits to read.</param>
        /// <param name="values">The bits read, aligned to the LSB padded on the LSB side to reach count length.</param>
        /// <returns>The numer of bits actually read.</returns>
        public int ReadBits(int count, out int values)
        {
            if (count < 0 || count > 32) throw new ArgumentException("Invalid number of bits.", nameof(count));

            values = 0;
            int readIndex = 0;

            while (readIndex < count)
            {
                if (TryReadBit(out bool readValue))
                {
                    readIndex++;
                    if (readValue) values |= 1 << (count - readIndex);
                }
                else
                {
                    break;
                }
            }

            return readIndex;
        }
    }
}
