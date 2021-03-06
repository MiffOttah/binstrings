﻿using System;
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
        /// <exception cref="ObjectDisposedException">The BinStringBuilder is disposed.</exception>
        public int Length => _MemStream is null
            ? throw new ObjectDisposedException(null)
            : Convert.ToInt32(_MemStream.Position);

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
        /// <param name="capacity">The initial size of the internal array in bytes.</param>
        public BinStringBuilder(int capacity)
        {
            _MemStream = new MemoryStream(capacity);
        }

        /// <summary>
        /// Writes the given data to the end of the buffer.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <exception cref="ObjectDisposedException">The BinStringBuilder is disposed.</exception>
        public void Append(byte[] data)
        {
            if (_MemStream is null) throw new ObjectDisposedException(null);

            if (data is null) return;
            _MemStream?.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the given data to the end of the buffer.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <exception cref="ObjectDisposedException">The BinStringBuilder is disposed.</exception>
        public void Append(BinString data)
        {
            if (data is null) return;

#if CORE
            // use a span to avoid making an un-necessary clone
            // of the binary string data
            if (_MemStream is null) throw new ObjectDisposedException(null);
            _MemStream?.Write(data.AsSpan());
#else
            Append(data.ToArray());
#endif
        }

        /// <summary>
        /// Writes the given byte to the end of the buffer.
        /// </summary>
        /// <param name="data">The byte to write.</param>
        /// <exception cref="ObjectDisposedException">The BinStringBuilder is disposed.</exception>
        public void Append(byte data)
        {
            if (_MemStream is null) throw new ObjectDisposedException(null);
            _MemStream?.WriteByte(data);
        }

        /// <summary>
        /// Writes the given data from the BinStringBuilder to the end of this buffer.
        /// </summary>
        /// <param name="data">The BinStringBuilder to copy the data from.</param>
        /// <exception cref="ObjectDisposedException">The BinStringBuilder is disposed.</exception>
        public void Append(BinStringBuilder data)
        {
            if (data is null) return;
            Append(data._MemStream?.ToArray());
        }

        /// <summary>
        /// Returns the buffer contents.
        /// </summary>
        /// <returns>A BinString containing all data that has been written to this builder.</returns>
        /// <exception cref="ObjectDisposedException">The BinStringBuilder is disposed.</exception>
        public BinString ToBinString()
        {
            if (_MemStream is null) throw new ObjectDisposedException(null);
            return new BinString(_MemStream?.ToArray(), false);
        }

        /// <summary>
        /// Disposes the underlying MemoryStream.
        /// </summary>
        public void Dispose()
        {
            (_MemStream as IDisposable)?.Dispose();
            _MemStream = null;
        }

        /// <summary>
        /// Converts the binary data to its string repersentation. A format string identical to the one used by <see cref="BinString"/> controls formatting.
        /// </summary>
        /// <seealso cref="BinString.ToString(string, IFormatProvider)"/>
        public string ToString(string format, IFormatProvider formatProvider) => ToBinString().ToString(format, formatProvider);

        /// <summary>
        /// Converts the binary data to its string repersentation. A format string identical to the one used by <see cref="BinString"/> controls formatting.
        /// </summary>
        /// /// <seealso cref="BinString.ToString(string)"/>
        public string ToString(string format) => ToBinString().ToString(format, null);

        /// <summary>
        /// Converts the binary data to a string repersentation.
        /// </summary>
        public override string ToString() => ToBinString().ToString();
    }
}
