﻿using System;
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

        #endregion

        #region Other objects that can become binstrings

        /// <summary>
        /// Converts the given text string to a binstring.
        /// </summary>
        public static BinString ToBinString(this string str, Encoding encoding)
        {
            return BinString.FromTextString(str, encoding);
        }

        /// <summary>
        /// Converts the given array of bytes to a binstring.
        /// </summary>
        public static BinString ToBinString(this byte[] array)
        {
            return new BinString(array);
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
        /// Reads binary data from the stream and returns it as a binstring.
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
    }
}
