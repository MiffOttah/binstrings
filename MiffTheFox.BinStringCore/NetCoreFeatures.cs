using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox
{
    partial class BinString
    {
        public BinString(Span<byte> data)
        {
            _Data = new byte[data.Length];
            var dspan = _Data.AsSpan();
            data.CopyTo(dspan);
        }

        public BinString(ReadOnlySpan<byte> data)
        {
            _Data = new byte[data.Length];
            var dspan = _Data.AsSpan();
            data.CopyTo(dspan);
        }

        /// <summary>
        /// Returns a Span&lt;byte&gt; containing the binary data.
        /// </summary>
        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(_Data);

        /// <summary>
        /// Returns a Span&lt;byte&gt; containing a portion of the binary data.
        /// </summary>
        public ReadOnlySpan<byte> Slice(int start) => new ReadOnlySpan<byte>(_Data, start, _Data.Length - start);

        /// <summary>
        /// Returns a Span&lt;byte&gt; containing a portion of the binary data.
        /// </summary>
        public ReadOnlySpan<byte> Slice(int start, int length) => new ReadOnlySpan<byte>(_Data, start, length);
    }
}
