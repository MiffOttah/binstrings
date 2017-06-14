using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    ///  Repersents a series of System.Byte objects that can be manipulated like a string.
    /// </summary>
    public class BinString : IReadOnlyList<byte>, IFormattable, ICloneable
    {
        protected byte[] _Data;

        public int Length => _Data.Length;
        public int Count => _Data.Length;
        public byte this[int index] => _Data[index];

        #region Array methods

        public byte[] ToArray()
        {
            return _Data;
        }

        public void CopyTo(byte[] buffer, int bufferIndex)
        {
            Array.Copy(_Data, 0, buffer, bufferIndex, _Data.Length);
        }

        public void CopyTo(int sourceIndex, byte[] buffer, int bufferIndex, int length)
        {
            Array.Copy(_Data, sourceIndex, buffer, bufferIndex, length);
        }

        #endregion

        #region Creation methods

        /// <summary>
        /// Creats an empty BinString
        /// </summary>
        public BinString()
        {
            _Data = new byte[0];
        }

        public BinString(byte[] data)
        {
            _Data = data;
        }

        public BinString(int length)
        {
            _Data = new byte[length];
        }

        /// <summary>
        /// Creates a BinString from a text string
        /// </summary>
        /// <param name="text">The text string to encode as binary</param>
        /// <param name="encoding">The System.Text.Encoding to encode the text string with</param>
        /// <returns></returns>
        public static BinString FromTextString(string text, Encoding encoding)
        {
            return new BinString(encoding.GetBytes(text));
        }

        /// <summary>
        /// Creates a BinString from a base64-encoded string
        /// </summary>
        public static BinString FromBase64String(string data)
        {
            return new BinString(Convert.FromBase64String(data));
        }

        /// <summary>
        /// Creates a BinString from a series of bytes
        /// </summary>
        public static BinString FromBytes(params byte[] data)
        {
            return new BinString(data);
        }

        #endregion

        #region Interfaces and Comparison

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)_Data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Data.GetEnumerator();
        }

        public static explicit operator BinString(byte[] source)
        {
            return new BinString(source);
        }

        public static implicit operator byte[] (BinString source)
        {
            return source._Data;
        }

        public static explicit operator BinString(byte source)
        {
            return new BinString(new byte[] { source });
        }

        public override int GetHashCode()
        {
            return _Data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is BinString)
            {
                var that = (BinString)obj;
                if (that.Length != this.Length) return false;
                for (int i = 0; i < this._Data.Length; i++)
                {
                    if (that._Data[i] != this._Data[i]) return false;
                }
                return true;
            }
            return false;
        }

        public static bool operator ==(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Equals(y);
        }

        public static bool operator !=(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return false;
            if (ReferenceEquals(x, null)) return true;
            if (ReferenceEquals(y, null)) return true;

            return !x.Equals(y);
        }

        public object Clone()
        {
            byte[] clone = new byte[_Data.Length];
            CopyTo(clone, 0);
            return new BinString(clone);
        }

        #endregion

        #region ToString

        /// <summary>
        /// Formats the BinString as a text string for display.
        /// </summary>
        /// <param name="format">One of: "x", "X", "s", "S", "64". Passing null or "G" defaults to "x". An x can optionally be followed with a separator string to indicate the sepeartor between bytes, such as "x-".</param>
        /// <param name="formatProvider">The format provider used to format the bytes for hexadecimal encoding.</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = "x";
            else if (format == "G" || format == "g") format = "x";

            if (format == "s") format = "x ";
            if (format == "S") format = "X ";

            if (format == "64")
            {
                return ToBase64String();
            }
            else if (format[0] == 'x' || format[0] == 'X')
            {
                string separator = format.Substring(1);
                string byteFormat = format[0] == 'X' ? "X2" : "x2";

                return string.Join(separator, _Data.Select(b => b.ToString(byteFormat, formatProvider)));
            }
            else
            {
                throw new FormatException($"The format string {format} is not supported.");
            }
        }

        /// <summary>
        /// Converts the binary string to a text string with the provided encoding.
        /// </summary>
        public string ToString(Encoding encoding)
        {
            return encoding.GetString(_Data);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public override string ToString()
        {
            return ToString("x", null);
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(_Data);
        }

        #endregion

        #region String operations

        public static BinString operator +(BinString x, BinString y)
        {
            byte[] result = new byte[x.Length + y.Length];
            x.CopyTo(result, 0);
            y.CopyTo(result, x.Length);
            return new BinString(result);
        }

        public static BinString operator +(BinString x, byte append)
        {
            byte[] result = new byte[x.Length + 1];
            x.CopyTo(result, 0);
            result[result.Length - 1] = append;
            return new BinString(result);
        }

        public static BinString operator +(byte prepend, BinString x)
        {
            byte[] result = new byte[x.Length + 1];
            x.CopyTo(result, 1);
            result[0] = prepend;
            return new BinString(result);
        }

        public BinString Repeat(int count)
        {
            if (count == 0) return new BinString();
            if (count < 0) throw new ArgumentException("count must be 0 or greater.", "count");

            byte[] result = new byte[_Data.Length * count];
            for (int i = 0; i < count; i++)
            {
                CopyTo(result, i * _Data.Length);
            }
            return new BinString(result);
        }

        public static BinString operator *(BinString x, int count)
        {
            return x.Repeat(count);
        }
        public static BinString operator *(int count, BinString x)
        {
            return x.Repeat(count);
        }

        public BinString Insert(int index, BinString toInsert)
        {
            _CheckIndex(index);
            byte[] result = new byte[_Data.Length + toInsert.Length];
            CopyTo(0, result, 0, index);
            toInsert.CopyTo(0, result, index, toInsert.Length);
            CopyTo(index, result, index + toInsert.Length, _Data.Length - index);
            return new BinString(result);
        }

        public BinString Insert(int index, byte toInsert)
        {
            _CheckIndex(index);
            byte[] result = new byte[_Data.Length + 1];
            CopyTo(0, result, 0, index);
            result[index] = toInsert;
            CopyTo(index, result, index + 1, _Data.Length - index);
            return new BinString(result);
        }

        public BinString PadLeft(int length, byte padding = 0)
        {
            if (length <= 0) throw new ArgumentException("Length must be positive.", "length");
            if (_Data.Length >= length) return this;

            byte[] result = new byte[length];
            int index = length - _Data.Length;
            CopyTo(result, index);
            for (int i = 0; i < index; i++)
            {
                result[i] = padding;
            }
            return new BinString(result);
        }

        public BinString PadRight(int length, byte padding = 0)
        {
            if (length <= 0) throw new ArgumentException("Length must be positive.", "length");
            if (_Data.Length >= length) return this;

            byte[] result = new byte[length];
            CopyTo(result, 0);
            for (int i = _Data.Length; i < length; i++)
            {
                result[i] = padding;
            }
            return new BinString(result);
        }

        public BinString Remove(int index, int length = -1)
        {
            _CheckIndex(index);
            if (length < 0) length = _Data.Length - index;

            int finalLength = Math.Max(_Data.Length - length, index);
            if (finalLength == 0) return new BinString();

            byte[] result = new byte[finalLength];
            CopyTo(0, result, 0, index);

            int followLength = Math.Max(_Data.Length - (length + index), 0);
            if (followLength > 0)
            {
                CopyTo(index + length, result, index, followLength);
            }
            return new BinString(result);
        }

        public BinString Substring(int index, int length = -1)
        {
            _CheckIndex(index);
            if (length < 0) length = _Data.Length - index;

            int finalLength = Math.Min(_Data.Length - index, length);
            if (finalLength <= 0) return new BinString();

            byte[] result = new byte[finalLength];
            CopyTo(index, result, 0, finalLength);
            return new BinString(result);
        }

        public BinString Trim(byte trimByte = 0)
        {
            int startIndex, endIndex;

            for (startIndex = 0; startIndex < _Data.Length && _Data[startIndex] == trimByte; startIndex++) ;
            for (endIndex = _Data.Length - 1; endIndex >= 0 && _Data[endIndex] == trimByte; endIndex--) ;
            endIndex++;

            if (endIndex <= startIndex) return new BinString();
            int length = endIndex - startIndex;
            return Substring(startIndex, length);
        }

        public BinString TrimLeft(byte trimByte = 0)
        {
            int startIndex;
            for (startIndex = 0; startIndex < _Data.Length && _Data[startIndex] == trimByte; startIndex++) ;
            return Substring(startIndex);
        }

        public BinString TrimRight(byte trimByte = 0)
        {
            int endIndex;
            for (endIndex = _Data.Length - 1; endIndex >= 0 && _Data[endIndex] == trimByte; endIndex--) ;
            return Remove(endIndex + 1);
        }

        public int IndexOf(byte needle)
        {
            for (int i = 0; i < _Data.Length; i++)
            {
                if (_Data[i] == needle) return i;
            }
            return -1;
        }

        public int IndexOf(BinString needle)
        {
            var bbm = new BinBoyerMoore(needle);
            return bbm.FindNeedleIn(this);
        }

        public BinString Replace(BinString needle, BinString replacement)
        {
            return Replace(needle, replacement, this);
        }

        public static BinString Replace(BinString needle, BinString replacement, BinString haystack)
        {
            if (needle.Length == 0) return haystack;

            var bbm = new BinBoyerMoore(needle);
            int index;
            while ((index = bbm.FindNeedleIn(haystack)) != -1)
            {
                byte[] newHaystack = new byte[(haystack.Length + replacement.Length) - needle.Length];
                haystack.CopyTo(0, newHaystack, 0, index);
                replacement.CopyTo(0, newHaystack, index, replacement.Length);

                int endIndex = index + needle.Length;
                haystack.CopyTo(endIndex, newHaystack, index + replacement.Length, haystack.Length - endIndex);
                haystack = new BinString(newHaystack);
            }

            return haystack;
        }

        public BinString[] Split(BinString needle)
        {
            if (needle.Length == 0) throw new InvalidOperationException("Cannot split on an empty BinString!");
            
            var parts = new List<BinString>();
            var bbm = new BinBoyerMoore(needle);
            var haystack = this;
            int index;

            while ((index = bbm.FindNeedleIn(haystack)) != -1)
            {
                parts.Add(haystack.Remove(index));
                haystack = haystack.Substring(index + needle.Length);
            }

            parts.Add(haystack);
            return parts.ToArray();
        }

        #endregion

        #region Check methods

        protected void _CheckIndex(int index)
        {
            if (index < 0 || index > _Data.Length) throw new IndexOutOfRangeException();
        }

        #endregion
    }
}
