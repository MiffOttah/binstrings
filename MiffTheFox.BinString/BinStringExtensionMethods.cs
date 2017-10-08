using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    public static class BinStringExtensionMethods
    {
        #region BitConverter methods

        public static BinString ToBinString(this bool n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this char n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this sbyte n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this byte n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this short n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this ushort n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this int n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this uint n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this long n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this ulong n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this float n) => (BinString)BitConverter.GetBytes(n);
        public static BinString ToBinString(this double n) => (BinString)BitConverter.GetBytes(n);

        #endregion

        #region Other objects that can become binstrings

        public static BinString ToBinString(this string str, Encoding encoding)
        {
            return BinString.FromTextString(str, encoding);
        }

        public static BinString ToBinString(this byte[] array)
        {
            return new BinString(array);
        }

        #endregion

        #region Stream methods

        public static void Write(this Stream stream, BinString data)
        {
            stream.Write(data.ToArray(), 0, data.Length);
        }

        public static BinString ReadBinString(this Stream stream, int bufferSize, int offset = 0)
        {
            var buffer = new byte[bufferSize];
            int read = stream.Read(buffer, offset, bufferSize);

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
