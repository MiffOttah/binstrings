using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox
{
    partial class BinString
    {
        /// <summary>
        /// Creates a BinString from the provided byte span.
        /// </summary>
        /// <param name="data">The data to create the BinString from.</param>
        public BinString(Span<byte> data)
        {
            _Data = new byte[data.Length];
            var dspan = _Data.AsSpan();
            data.CopyTo(dspan);
        }

        /// <summary>
        /// Creates a BinString from the provided byte span.
        /// </summary>
        /// <param name="data">The data to create the BinString from.</param>
        public BinString(ReadOnlySpan<byte> data)
        {
            _Data = new byte[data.Length];
            var dspan = _Data.AsSpan();
            data.CopyTo(dspan);
        }

        /// <summary>
        /// Returns a ReadOnlySpan&lt;byte&gt; containing the binary data.
        /// </summary>
        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(_Data);

        /// <summary>
        /// Returns a ReadOnlySpan&lt;byte&gt; containing a portion of the binary data.
        /// </summary>
        public ReadOnlySpan<byte> Slice(int start) => new ReadOnlySpan<byte>(_Data, start, _Data.Length - start);

        /// <summary>
        /// Returns a ReadOnlySpan&lt;byte&gt; containing a portion of the binary data.
        /// </summary>
        public ReadOnlySpan<byte> Slice(int start, int length) => new ReadOnlySpan<byte>(_Data, start, length);
    }
}
