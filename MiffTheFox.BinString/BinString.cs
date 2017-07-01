using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    ///  Repersents binary data as a series of System.Byte objects that can be manipulated like a string.
    /// </summary>
    public class BinString : IReadOnlyList<byte>, IFormattable, ICloneable
    {
        protected byte[] _Data;

        /// <summary>
        /// Returns the number of bytes in the BinString.
        /// </summary>
        public int Length => _Data.Length;

        /// <summary>
        /// Returns the number of bytes in the BinString.
        /// </summary>
        public int Count => _Data.Length;

        public byte this[int index] => _Data[index];

        #region Array methods

        /// <summary>
        /// Returns the BinString as an array of System.Byte.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return _Data;
        }

        /// <summary>
        /// Copys the data from the BinString to a byte[] buffer.
        /// </summary>
        /// <param name="buffer">The buffer to copy to.</param>
        /// <param name="bufferIndex">The index within the buffer to begin the copy at.</param>
        public void CopyTo(byte[] buffer, int bufferIndex)
        {
            Array.Copy(_Data, 0, buffer, bufferIndex, _Data.Length);
        }

        /// <summary>
        /// Copys the data from the BinString to a byte[] buffer.
        /// </summary>
        /// <param name="sourceIndex">The index within this BinString to begin the copy at.</param>
        /// <param name="buffer">The buffer to copy to.</param>
        /// <param name="bufferIndex">The index within the buffer to begin the copy at.</param>
        /// <param name="length">The number of bytes to copy from the BinString to the buffer.</param>
        public void CopyTo(int sourceIndex, byte[] buffer, int bufferIndex, int length)
        {
            Array.Copy(_Data, sourceIndex, buffer, bufferIndex, length);
        }

        #endregion

        #region Creation methods

        /// <summary>
        /// Creates an empty BinString
        /// </summary>
        public BinString()
        {
            _Data = new byte[0];
        }

        /// <summary>
        /// Creates a BinString with the specified binary data.
        /// </summary>
        public BinString(byte[] data)
        {
            _Data = data;
        }

        /// <summary>
        /// Creates a BinString with the specified length where all bytes are 0.
        /// </summary>
        public BinString(int length) : this(length, 0)
        {
        }

        /// <summary>
        /// Creates a BinString with the specified length where all bytes are the given byte.
        /// </summary>
        public BinString(int length, byte given)
        {
            _Data = new byte[length];
            for (int i = 0; i < length; i++) _Data[i] = given;
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

        /// <summary>
        /// Creates a BinString from a series of bytes repersented as hexadecimal
        /// </summary>
        /// <param name="hex">The bytes to create the BinString with, repersented as hexedecimal 0-9 A-F/a-f. All other characters are ignored.</param>
        public static BinString FromBytes(string hex)
        {
            const string HEX_CHARS = "0123456789ABCDEFabcdef";

            var parsedHex = hex
                .Select(_ => HEX_CHARS.IndexOf(_))
                .Where(_ => _ != -1)
                .Select(_ => _ > 0xf ? _ - 6 : _)
                .ToArray();

            if ((parsedHex.Length & 1) == 1) throw new InvalidOperationException("Input string must contain an even number of hexadecimal digits.");

            byte[] result = new byte[parsedHex.Length >> 1];
            for (int i = 0; i < parsedHex.Length; i += 2)
            {
                result[i >> 1] = Convert.ToByte(parsedHex[i] << 4 | parsedHex[i + 1]);
            }

            return new BinString(result);
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
        /// <param name="format">One of: x/X, s/S, 64, u/U, e/E. Passing null or g/G defaults to x. An x/X can optionally be followed with a separator string to indicate the sepeartor between bytes, such as "x-".</param>
        /// <param name="formatProvider">The format provider used to format the bytes for hexadecimal encoding.</param>
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
            else if (format == "u" || format == "U")
            {
                return ToUrlString(format == "u", formatProvider);
            }
            else if (format == "e" || format == "E")
            {
                return ToEscapedString(format == "e", formatProvider);
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

        /// <summary>
        /// Formats the BinString as a text string for display.
        /// </summary>
        /// <param name="format">One of: x/X, s/S, 64, u/U, e/E. Passing null or g/G defaults to x. An x/X can optionally be followed with a separator string to indicate the sepeartor between bytes, such as "x-".</param>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Formats the BinString as a text string for display. The format is a string of hexadecimal characters.
        /// </summary>
        public override string ToString()
        {
            return ToString("x", null);
        }

        /// <summary>
        /// Returns the BinString as a base-64 encoded text string.
        /// </summary>
        /// <returns></returns>
        public string ToBase64String()
        {
            return Convert.ToBase64String(_Data);
        }

        /// <summary>
        /// Returns the BinString as an ASCII text string with non-ASCII bytes (and %) repersented by %-encoding.
        /// </summary>
        /// <param name="lowerCase">True for hexadecimal bytes in lower case, false for upper case.</param>
        /// <param name="formatProvider">The format provider used to format the bytes for hexadecimal encoding.</param>
        /// <returns></returns>
        public string ToUrlString(bool lowerCase, IFormatProvider formatProvider)
        {
            var result = new StringBuilder();
            foreach (byte b in _Data)
            {
                if (b > 0x20 && b < 0x7f && b != 0x2b)
                {
                    result.Append((char)b);
                }
                else
                {
                    result.Append('%');
                    result.Append(b.ToString(lowerCase ? "x2" : "X2", formatProvider));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns the BinString as an ASCII text string with non-ASCII bytes repersented by backslash-encoding, as well as escaping backslashes and quotes.
        /// </summary>
        /// <param name="lowerCase">True for hexadecimal bytes in lower case, false for upper case.</param>
        /// <param name="formatProvider">The format provider used to format the bytes for hexadecimal encoding.</param>
        /// <returns></returns>
        public string ToEscapedString(bool lowerCase, IFormatProvider formatProvider)
        {
            var result = new StringBuilder();
            foreach (byte b in _Data)
            {
                if (b == 0x5c)
                {
                    result.Append("\\\\");
                }
                else if (b == 0x22 || b == 0x27)
                {
                    result.Append('\\');
                    result.Append((char)b);
                }
                else if (b >= 0x20 && b < 0x7f)
                {
                    result.Append((char)b);
                }
                else
                {
                    result.Append("\\x");
                    result.Append(b.ToString(lowerCase ? "x2" : "X2", formatProvider));
                }
            }
            return result.ToString();
        }

        #endregion

        #region String operations

        /// <summary>
        /// Concatenates two BinStrings
        /// </summary>
        public static BinString operator +(BinString x, BinString y)
        {
            byte[] result = new byte[x.Length + y.Length];
            x.CopyTo(result, 0);
            y.CopyTo(result, x.Length);
            return new BinString(result);
        }

        /// <summary>
        /// Appends a byte to a BinString
        /// </summary>
        public static BinString operator +(BinString x, byte append)
        {
            byte[] result = new byte[x.Length + 1];
            x.CopyTo(result, 0);
            result[result.Length - 1] = append;
            return new BinString(result);
        }

        /// <summary>
        /// Prepends a byte to a BinString
        /// </summary>
        public static BinString operator +(byte prepend, BinString x)
        {
            byte[] result = new byte[x.Length + 1];
            x.CopyTo(result, 1);
            result[0] = prepend;
            return new BinString(result);
        }

        /// <summary>
        /// Repeats the BinString a number of times.
        /// </summary>
        /// <param name="count">The number of times to repeat the BinString.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Repeats the BinString a number of times.
        /// </summary>
        public static BinString operator *(BinString x, int count)
        {
            return x.Repeat(count);
        }

        /// <summary>
        /// Repeats the BinString a number of times.
        /// </summary>
        public static BinString operator *(int count, BinString x)
        {
            return x.Repeat(count);
        }

        /// <summary>
        /// Inserts the contents of another BinString into this BinString at a specified index.
        /// </summary>
        public BinString Insert(int index, BinString toInsert)
        {
            _CheckIndex(index);
            byte[] result = new byte[_Data.Length + toInsert.Length];
            CopyTo(0, result, 0, index);
            toInsert.CopyTo(0, result, index, toInsert.Length);
            CopyTo(index, result, index + toInsert.Length, _Data.Length - index);
            return new BinString(result);
        }

        /// <summary>
        /// Inserts a single byte into this BinString at a specified index.
        /// </summary>
        public BinString Insert(int index, byte toInsert)
        {
            _CheckIndex(index);
            byte[] result = new byte[_Data.Length + 1];
            CopyTo(0, result, 0, index);
            result[index] = toInsert;
            CopyTo(index, result, index + 1, _Data.Length - index);
            return new BinString(result);
        }

        /// <summary>
        /// Prepends bytes to the beginning of the BinString until the length of the BinString is equal to or greater than length.
        /// </summary>
        /// <param name="length">The minimum final length of the string.</param>
        /// <param name="padding">The byte with which to pad the string to length bytes.</param>
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

        /// <summary>
        /// Appends bytes to the end of the BinString until the length of the BinString is equal to or greater than length.
        /// </summary>
        /// <param name="length">The minimum final length of the string.</param>
        /// <param name="padding">The byte with which to pad the string to length bytes.</param>
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

        /// <summary>
        /// Removes a number of bytes from the BinString starting the specified index.
        /// </summary>
        /// <param name="index">The index to begin removing bytes at.</param>
        /// <param name="length">The number of bytes to remove. If less than zero, removes all bytes from the index to the end of the BinString.</param>
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

        /// <summary>
        /// Extracts a number of bytes from the BinString starting the specified index.
        /// </summary>
        /// <param name="index">The index to begin extracting bytes at.</param>
        /// <param name="length">The number of bytes to extracting. If less than zero, extracts all bytes from the index to the end of the BinString.</param>
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

        /// <summary>
        /// Removes any occurance of the specified byte from the beginning and end of the BinString.
        /// </summary>
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

        /// <summary>
        /// Removes any occurance of the specified byte from the beginning of the BinString.
        /// </summary>
        public BinString TrimLeft(byte trimByte = 0)
        {
            int startIndex;
            for (startIndex = 0; startIndex < _Data.Length && _Data[startIndex] == trimByte; startIndex++) ;
            return Substring(startIndex);
        }

        /// <summary>
        /// Removes any occurance of the specified byte from the end of the BinString.
        /// </summary>
        public BinString TrimRight(byte trimByte = 0)
        {
            int endIndex;
            for (endIndex = _Data.Length - 1; endIndex >= 0 && _Data[endIndex] == trimByte; endIndex--) ;
            return Remove(endIndex + 1);
        }

        /// <summary>
        /// Finds the first occurrence of the specified byte in the BinString.
        /// </summary>
        public int IndexOf(byte needle)
        {
            for (int i = 0; i < _Data.Length; i++)
            {
                if (_Data[i] == needle) return i;
            }
            return -1;
        }

        /// <summary>
        /// Finds the first occurrence of the specified substring in the BinString.
        /// </summary>
        public int IndexOf(BinString needle)
        {
            var bbm = new BinBoyerMoore(needle);
            return bbm.FindNeedleIn(this);
        }

        /// <summary>
        /// Replaces all instances of the needle substring with the replacement substring in this substring.
        /// </summary>
        /// <param name="needle">The substring to search for.</param>
        /// <param name="replacement">The substring to replace the needle with.</param>
        public BinString Replace(BinString needle, BinString replacement)
        {
            return Replace(needle, replacement, this);
        }

        /// <summary>
        /// Replaces all instances of the needle substring with the replacement substring in a specified substring.
        /// </summary>
        /// <param name="needle">The substring to search for.</param>
        /// <param name="replacement">The substring to replace the needle with.</param>
        /// <param name="haystack">The substring to search within.</param>
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

        /// <summary>
        /// Returns an array of BinStrings based on dividing the BinString up along any occurances of the specifeid substring.
        /// </summary>
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

        /// <summary>
        /// Checks if the value given is null or empty, returning True if so and False otherwise.
        /// </summary>
        public static bool IsNullOrEmpty(BinString value)
        {
            if (ReferenceEquals(value, null)) return true;
            if (value._Data.Length == 0) return true;
            return false;
        }

        #endregion
    }
}
