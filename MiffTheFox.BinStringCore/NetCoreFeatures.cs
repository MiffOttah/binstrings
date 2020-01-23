using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox
{
    partial class BinString
    {
        /// <summary>
        /// Creates a BinString from the provided byte span. (Only avaiable in .NET core version.)
        /// </summary>
        /// <param name="data">The data to create the BinString from.</param>
        public BinString(Span<byte> data)
        {
            _Data = new byte[data.Length];
            var dspan = _Data.AsSpan();
            data.CopyTo(dspan);
        }

        /// <summary>
        /// Creates a BinString from the provided byte span. (Only avaiable in .NET core version.)
        /// </summary>
        /// <param name="data">The data to create the BinString from.</param>
        public BinString(ReadOnlySpan<byte> data)
        {
            _Data = new byte[data.Length];
            var dspan = _Data.AsSpan();
            data.CopyTo(dspan);
        }

        /// <summary>
        /// Returns a ReadOnlySpan&lt;byte&gt; containing the binary data. (Only avaiable in .NET core version.)
        /// </summary>
        /// <returns>A ReadOnlySpan&lt;byte&gt; containing the binary data.</returns>
        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(_Data);

        /// <summary>
        /// Returns a ReadOnlySpan&lt;byte&gt; containing a portion of the binary data. (Only avaiable in .NET core version.)
        /// </summary>
        /// <returns>A ReadOnlySpan&lt;byte&gt; containing a portion of the binary data.</returns>
        public ReadOnlySpan<byte> Slice(int start) => new ReadOnlySpan<byte>(_Data, start, _Data.Length - start);

        /// <summary>
        /// Returns a ReadOnlySpan&lt;byte&gt; containing a portion of the binary data. (Only avaiable in .NET core version.)
        /// </summary>
        /// /// <returns>A ReadOnlySpan&lt;byte&gt; containing a portion of the binary data.</returns>
        public ReadOnlySpan<byte> Slice(int start, int length) => new ReadOnlySpan<byte>(_Data, start, length);

        /// <summary>
        /// Retreives the byte at the specified index.
        /// </summary>
        public byte this[Index index]
        {
            get
            {
                if (index.Value >= Length) throw new IndexOutOfRangeException();
                return _Data[index.IsFromEnd ? _Data.Length - index.Value : index.Value];
            }
        }
         
        /// <summary>
        /// Retrieves a substring feating the specified range.
        /// </summary>
        public BinString this[Range range]
        {
            get
            {
                int start = range.Start.IsFromEnd ? _Data.Length - (range.Start.Value) : range.Start.Value;
                int end = range.End.IsFromEnd ? _Data.Length - (range.End.Value) : range.End.Value;
                return Range(start, end);
            }
        }
    }
}
