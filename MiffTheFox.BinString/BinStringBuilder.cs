using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    /// A buffer that can be used to build up a binary string
    /// </summary>
    public class BinStringBuilder : IDisposable, IFormattable
    {
        protected MemoryStream _MemStream;
        public int Length { get => Convert.ToInt32(_MemStream.Position); }

        /// <summary>
        /// Creates a new BinStringBuilder with an expandable capacity initialized to zero.
        /// </summary>
        public BinStringBuilder()
        {
            _MemStream = new MemoryStream();
        }

        /// <summary>
        /// Creates a new BinStringBuilder with an expandable capacity initialized as specified.
        /// </summary>
        public BinStringBuilder(int capacity)
        {
            _MemStream = new MemoryStream(capacity);
        }

        /// <summary>
        /// Writes the given data to the end of the buffer.
        /// </summary>
        public void Append(byte[] data)
        {
            _MemStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the given data to the end of the buffer.
        /// </summary>
        public void Append(BinString data)
        {
            Append(data.ToArray());
        }

        /// <summary>
        /// Writes the given byte to the end of the buffer.
        /// </summary>
        public void Append(byte data)
        {
            Append(new byte[] { data });
        }

        /// <summary>
        /// Writes the given data from the BinStringBuilder to the end of this buffer.
        /// </summary>
        /// <param name="data"></param>
        public void Append(BinStringBuilder data)
        {
            Append(data._MemStream.ToArray());
        }

        /// <summary>
        /// Returns the buffer contents.
        /// </summary>
        public BinString ToBinString()
        {
            return new BinString(_MemStream.ToArray());
        }

        public void Dispose()
        {
            ((IDisposable)_MemStream).Dispose();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToBinString().ToString(format, formatProvider);
        }
        public string ToString(string format)
        {
            return ToBinString().ToString(format, null);
        }
        public override string ToString()
        {
            return ToBinString().ToString();
        }
    }
}
