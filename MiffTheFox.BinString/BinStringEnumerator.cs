using System;
using System.Collections;
using System.Collections.Generic;

namespace MiffTheFox
{
    public class BinStringEnumerator : IEnumerator<byte>
    {
        protected readonly BinString _BinStringSource;
        protected int _Index = -1;

        public BinStringEnumerator(BinString source)
        {
            _BinStringSource = source;
        }

        public byte Current =>
            _Index > -1 && _Index < _BinStringSource.Length ?
            _BinStringSource[_Index] :
            throw new InvalidOperationException();
        object IEnumerator.Current => Current;

        public bool MoveNext() => ++_Index < _BinStringSource.Length;
        public void Reset() => _Index = -1;

        void IDisposable.Dispose()
        {
            // Enumerators are required to be disposable
            // even though there is nothing to dispose.
        }
    }
}
