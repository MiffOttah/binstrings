using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    /// Repersents a buffer of bytes that can be appended to or pulled into a BinString.
    /// </summary>
    public class BinStringBuilder : IDisposable, IFormattable
    {
        /// <summary>
        /// The underlying MemoryStream being used to collect the binary data.
        /// </summary>
        protected MemoryStream _MemStream;

        /// <summary>
        /// The number of bytes written to the BinStringBuilder.
        /// </summary>
        public int Length => Convert.ToInt32(_MemStream.Position);

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
#if CORE
            // use a span to avoid making an un-necessary clone
            // of the binary string data
            _MemStream.Write(data.AsSpan());
#else
            Append(data.ToArray());
#endif
        }

        /// <summary>
        /// Writes the given byte to the end of the buffer.
        /// </summary>
        public void Append(byte data)
        {
            _MemStream.WriteByte(data);
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
            return new BinString(_MemStream.ToArray(), false);
        }

        /// <summary>
        /// Disposes the underlying MemoryStream.
        /// </summary>
        public void Dispose()
        {
            if (_MemStream != null)
            {
                ((IDisposable)_MemStream).Dispose();
                _MemStream = null;
            }
        }

        /// <summary>
        /// Converts the binary data to its string repersentation. A format string identical to the one used by MiffTheFox.BinString controls formatting.
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider) => ToBinString().ToString(format, formatProvider);

        /// <summary>
        /// Converts the binary data to its string repersentation. A format string identical to the one used by MiffTheFox.BinString controls formatting.
        /// </summary>
        public string ToString(string format) => ToBinString().ToString(format, null);

        /// <summary>
        /// Converts the binary data to its string repersentation.
        /// </summary>
        public override string ToString() => ToBinString().ToString();
    }
}
