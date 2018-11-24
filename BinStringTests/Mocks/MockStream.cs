using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStringTests.Mocks
{
    // Since BinString.FromStream has an optimization for converting from
    // a MemoryStream, we use this type to provide it a stream that
    // isn't technically a MemoryStream.

    public class MockStream : Stream
    {
        private readonly MemoryStream _Underlying;

        public MockStream(byte[] data)
        {
            _Underlying = new MemoryStream(data);
        }

        public override bool CanRead => _Underlying.CanRead;

        public override bool CanSeek => _Underlying.CanSeek;

        public override bool CanWrite => _Underlying.CanWrite;

        public override long Length => _Underlying.Length;

        public override long Position { get => _Underlying.Position; set => _Underlying.Position = value; }

        public override void Flush()
        {
            _Underlying.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => _Underlying.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _Underlying.Seek(offset, origin);
        public override void SetLength(long value) => _Underlying.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _Underlying.Write(buffer, offset, count);
    }
}
