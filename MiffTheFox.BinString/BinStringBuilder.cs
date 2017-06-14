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

        public BinStringBuilder()
        {
            _MemStream = new MemoryStream();
        }
        public BinStringBuilder(int capacity)
        {
            _MemStream = new MemoryStream(capacity);
        }

        public void Append(byte[] data)
        {
            _MemStream.Write(data, 0, data.Length);
        }

        public void Append(BinString data)
        {
            Append(data.ToArray());
        }

        public void Append(byte data)
        {
            Append(new byte[] { data });
        }

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
