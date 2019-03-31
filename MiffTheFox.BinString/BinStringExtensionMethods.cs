using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    /// Provides extension methods for integrating BinStrings into other codebases.
    /// </summary>
    public static class BinStringExtensionMethods
    {
        #region BitConverter methods

        /// <summary>
        /// Converts the given boolean value to a binstring.
        /// </summary>
        public static BinString ToBinString(this bool n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given character value to a binstring.
        /// </summary>
        public static BinString ToBinString(this char n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this sbyte n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this byte n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this short n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this ushort n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this int n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this uint n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this long n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring.
        /// </summary>
        public static BinString ToBinString(this ulong n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given floating-point value to a binstring.
        /// </summary>
        public static BinString ToBinString(this float n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given floating-point value to a binstring.
        /// </summary>
        public static BinString ToBinString(this double n) => (BinString)BitConverter.GetBytes(n);

        /// <summary>
        /// Converts the given numeric value to a binstring with the specified endianess.
        /// </summary>
        public static BinString ToBinString(this short n, IntegerEndianess endianess) => new BinString(BitConverter.GetBytes(n), false).ConvertEndianess(IntegerEndianess.Native, endianess);

        /// <summary>
        /// Converts the given numeric value to a binstring with the specified endianess.
        /// </summary>
        public static BinString ToBinString(this ushort n, IntegerEndianess endianess) => new BinString(BitConverter.GetBytes(n), false).ConvertEndianess(IntegerEndianess.Native, endianess);

        /// <summary>
        /// Converts the given numeric value to a binstring with the specified endianess.
        /// </summary>
        public static BinString ToBinString(this int n, IntegerEndianess endianess) => new BinString(BitConverter.GetBytes(n), false).ConvertEndianess(IntegerEndianess.Native, endianess);

        /// <summary>
        /// Converts the given numeric value to a binstring with the specified endianess.
        /// </summary>
        public static BinString ToBinString(this uint n, IntegerEndianess endianess) => new BinString(BitConverter.GetBytes(n), false).ConvertEndianess(IntegerEndianess.Native, endianess);

        /// <summary>
        /// Converts the given numeric value to a binstring with the specified endianess.
        /// </summary>
        public static BinString ToBinString(this long n, IntegerEndianess endianess) => new BinString(BitConverter.GetBytes(n), false).ConvertEndianess(IntegerEndianess.Native, endianess);

        /// <summary>
        /// Converts the given numeric value to a binstring with the specified endianess.
        /// </summary>
        public static BinString ToBinString(this ulong n, IntegerEndianess endianess) => new BinString(BitConverter.GetBytes(n), false).ConvertEndianess(IntegerEndianess.Native, endianess);

        #endregion

        #region Other objects that can become binstrings

        /// <summary>
        /// Converts the given text string to a binstring.
        /// </summary>
        public static BinString ToBinString(this string str, Encoding encoding)
        {
            return new BinString(str, encoding);
        }

        /// <summary>
        /// Converts the given array of bytes to a binstring.
        /// </summary>
        public static BinString ToBinString(this byte[] array)
        {
            return new BinString(array);
        }

        /// <summary>
        /// Converts the given memory stream to a binstring.
        /// </summary>
        public static BinString ToBinString(this MemoryStream stream)
        {
            return new BinString(stream.ToArray());
        }

        #endregion

        #region Stream methods

        /// <summary>
        /// Writes an entire binary string to the stream at the current position.
        /// </summary>
        /// <param name="stream">The stream to be written to.</param>
        /// <param name="data">The data to write to be written.</param>
        public static void Write(this Stream stream, BinString data)
        {
            stream.Write(data.ToArray(), 0, data.Length);
        }

        /// <summary>
        /// Reads an amount of binary data from the stream and returns it as a BinString. To read the entire stream, use BinString.FromStream
        /// </summary>
        /// <param name="stream">The stream to be read from.</param>
        /// <param name="bufferSize">The maximum amount of data to read from the stream.</param>
        public static BinString ReadBinString(this Stream stream, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            int read = stream.Read(buffer, 0, bufferSize);

            if (read == 0)
            {
                return BinString.Empty;
            }
            else if (read == bufferSize)
            {
                return new BinString(buffer);
            }
            else
            {
                var result = new byte[read];
                Array.Copy(buffer, result, read);
                return new BinString(result);
            }
        }

        #endregion

        #region Random methods

        /// <summary>
        /// Returns a binary string of the specified length with random contents.
        /// </summary>
        public static BinString NextBinString(this Random rng, int length)
        {
            var buffer = new byte[length];
            rng.NextBytes(buffer);
            return new BinString(buffer, false);
        }

        /// <summary>
        /// Returns a binary string of the specified length with random contents.
        /// </summary>
        public static BinString GetBinString(this System.Security.Cryptography.RandomNumberGenerator rng, int length)
        {
            var buffer = new byte[length];
            rng.GetBytes(buffer);
            return new BinString(buffer, false);
        }

        #endregion

        #region Serialization to BinaryWriter methods

        /// <summary>
        /// Writes the given binstring to the binary writer, including its length.
        /// </summary>
        public static void WriteBinStringIndirect(this BinaryWriter writer, BinString data)
        {
            writer.Write(data.Length);
            writer.Write((byte[])data);
        }

        /// <summary>
        /// Writes the given binstring directly to the binary writer (not including its length).
        /// </summary>
        public static void WriteBinStringDirect(this BinaryWriter writer, BinString data)
        {
            writer.Write((byte[])data);
        }


        /// <summary>
        /// Reads a binstring (that includes the length) from the binary reader.
        /// </summary>
        public static BinString ReadBinStringIndirect(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length < 0) throw new FormatException("Invalid indirect BinString.");

            return (BinString)reader.ReadBytes(length);
        }

        /// <summary>
        /// Reads a binstring of the specified length directly from the binary writer.
        /// </summary>
        public static BinString ReadBinStringDirect(this BinaryReader reader, int length)
        {
            return (BinString)reader.ReadBytes(length);
        }
        #endregion
    }
}
