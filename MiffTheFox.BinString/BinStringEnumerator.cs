using System;
using System.Collections;
using System.Collections.Generic;

namespace MiffTheFox
{
    /// <summary>
    /// Enumerates over the bytes of a binary string.
    /// </summary>
    public class BinStringEnumerator : IEnumerator<byte>
    {
        /// <summary>
        /// The BinString being read from.
        /// </summary>
        protected readonly BinString _BinStringSource;

        /// <summary>
        /// The current position of the enumerator inside the BinString.
        /// </summary>
        protected int _Index = -1;

        /// <summary>
        /// Creates a new BinStringEnumerator for enumerating over the given binary string.
        /// </summary>
        /// <param name="source">The BinString to enumerate over.</param>
        public BinStringEnumerator(BinString source)
        {
            _BinStringSource = source;

        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public byte Current =>
            _Index > -1 && _Index < _BinStringSource.Length ?
            _BinStringSource[_Index] :
            throw new InvalidOperationException();
        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        public bool MoveNext() => ++_Index < _BinStringSource.Length;

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset() => _Index = -1;

        void IDisposable.Dispose()
        {
            // Enumerators are required to be disposable
            // even though there is nothing to dispose.
        }
    }
}
