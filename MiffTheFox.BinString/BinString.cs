using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MiffTheFox
{
    /// <summary>
    ///  Repersents binary data as a series of System.Byte objects that can be manipulated like a string.
    /// </summary>
    [Serializable, System.ComponentModel.TypeConverter(typeof(BinStringTypeConverter))]
    public partial class BinString : IReadOnlyList<byte>, IFormattable, ICloneable, IEquatable<BinString>, IComparable, IComparable<BinString>, IConvertible, ISerializable
    {
        /// <summary>
        /// The byte[] data referenced by this BinString. Mutating this is not a good idea.
        /// </summary>
        protected readonly byte[] _Data;

        /// <summary>
        /// Returns the number of bytes in the BinString.
        /// </summary>
        public int Length => _Data.Length;
        
        int IReadOnlyCollection<byte>.Count => _Data.Length;

        /// <summary>
        /// Retreives the byte at the specified index.
        /// </summary>
        public byte this[int index] => _Data[index];

#region Array methods

        /// <summary>
        /// Returns the BinString as an array of System.Byte.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var clone = new byte[Length];
            CopyTo(clone, 0);
            return clone;
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
        /// Returns an empty BinString.
        /// </summary>
        public static BinString Empty { get; } = new BinString();
        // since BinStrings are (or at least should be) immutable,
        // we can assign a singleton empty object

        /// <summary>
        /// Creates an empty BinString.
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
            if (data == null)
            {
                _Data = new byte[0];
            }
            else
            {
                _Data = new byte[data.Length];
                Array.Copy(data, _Data, data.Length);
            }
        }

        /// <summary>
        /// Creates a BinString with binary data from a specific portion of the byte array.
        /// </summary>
        /// <param name="data">The byte array containing the data.</param>
        /// <param name="offset">The index into the byte array to start taking data from.</param>
        /// <param name="length">The length of data to take.</param>
        public BinString(byte[] data, int offset, int length)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset >= data.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (offset + length > data.Length) throw new ArgumentException("Cannot read past the end of the byte array.");

            _Data = new byte[length];
            Array.Copy(data, offset, _Data, 0, length);
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
        ///  Creates a BinString with the specified data encoded in Base64.
        /// </summary>
        public BinString(string base64)
        {
            _Data = string.IsNullOrEmpty(base64) ? new byte[0] : Convert.FromBase64String(base64);
        }

        /// <summary>
        /// Creates a BinString with the specified text encoded in the specified encoding.
        /// </summary>
        public BinString(string text, Encoding encoding)
        {
            if (encoding is null) throw new ArgumentNullException(nameof(encoding));
            _Data = string.IsNullOrEmpty(text) ? new byte[0] : encoding.GetBytes(text);
        }

        /// <summary>
        /// Creates a BinString with the specifed data decoded with the specified encoding
        /// </summary>
        /// <param name="encoded">The encoded text</param>
        /// <param name="encoding">The Binary-to-Text encoding used.</param>
        public BinString(string encoded, BinaryTextEncoding encoding)
        {
            if (encoding is null) throw new ArgumentNullException(nameof(encoding));
            if (string.IsNullOrEmpty(encoded))
            {
                _Data = new byte[0];
            }
            else
            {
                _Data = encoding.GetBinString(encoded)._Data;
            }
        }

        /// <summary>
        /// Creates a BinString from an ASCII text string with non-ASCII bytes repersented by backslash-encoding.
        /// </summary>
        /// <param name="escapedData">The escaped string, such as produced by BinString.ToEscapedString</param>
        public static BinString FromEscapedString(string escapedData)
        {
            if (escapedData is null) throw new ArgumentNullException("Provided data is null.", nameof(escapedData));
            var result = new BinStringBuilder();

            for (int i = 0; i < escapedData.Length; i++)
            {
                if (escapedData[i] == '\\')
                {
                    i++;
                    if (i < escapedData.Length)
                    {
                        switch (escapedData[i])
                        {
                            case 'x':
                                if ((i + 2) < escapedData.Length && int.TryParse(escapedData.Substring(i + 1, 2), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out int hexValue))
                                {
                                    i += 2;
                                    result.Append(Convert.ToByte(hexValue));
                                    continue;
                                }
                                break;

                            case '\\':
                            case '\'':
                            case '"':
                                result.Append((byte)escapedData[i]);
                                continue;
                        }
                    }
                    throw new ArgumentException("Invalid escape sequence in data.", nameof(escapedData));
                }
                else if (escapedData[i] <= '~')
                {
                    result.Append((byte)escapedData[i]);
                }
                else
                {
                    throw new ArgumentException("Escaped data contains a non-ASCII character.", nameof(escapedData));
                }
            }

            return result.ToBinString();
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<byte> GetEnumerator() => new BinStringEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new BinStringEnumerator(this);

        /// <summary>
        /// Converts the byte array to a BinString.
        /// </summary>
        public static explicit operator BinString(byte[] source) => new BinString(source);

        /// <summary>
        /// Converts the BinString to a byte array.
        /// </summary>
        public static implicit operator byte[] (BinString source) => source.ToArray();

        /// <summary>
        /// Converts the byte to a BinString.
        /// </summary>
        public static explicit operator BinString(byte source) => new BinString(new byte[] { source });

        /// <summary>
        /// Returns a hash of the contents of the BinString.
        /// </summary>
        public override int GetHashCode()
        {
            // FNV-1a hash https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
            ulong hash = 14695981039346656037;
            unchecked
            {
                foreach (byte b in _Data)
                {
                    hash ^= b;
                    hash *= 1099511628211;
                }

                return (int)hash;
            }
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object. If null or a non-BinString object, always returns false.</param>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is BinString && Equals((BinString)obj);
        }

        /// <summary>
        /// Determines whether the two binary strings are equal.
        /// Two binary strings are equal if and only if they have the same number of bytes and each byte in each position is identical.
        /// </summary>
        /// <param name="other">The binary string to compare against.</param>
        public bool Equals(BinString other)
        {
            if (other is null) return false;
            if (this.Length != other.Length) return false;
            for (int i = 0; i < this._Data.Length; i++)
            {
                if (other._Data[i] != this._Data[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the two binary strings are equal.
        /// </summary>
        public static bool operator ==(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;

            return x.Equals(y);
        }

        /// <summary>
        /// Determines whether the two binary strings are not equal.
        /// </summary>
        public static bool operator !=(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return false;
            if (x is null) return true;
            if (y is null) return true;

            return !x.Equals(y);
        }

        /// <summary>
        /// Compares this BinString to another object.
        /// </summary>
        /// <param name="obj">The other object to compare against.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">A BinString can only be compared to another BinString or null.</exception>
        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                return 1;
            }
            else if (obj is BinString objString)
            {
                return CompareTo(objString);
            }
            else
            {
                throw new ArgumentException($"Cannot compare BinString to {obj.GetType().Name}");
            }
        }

        /// <summary>
        /// Compares this BinString to another BinString, based on sort order.
        /// </summary>
        /// <param name="other">The other BinString to compare against.</param>
        /// <returns>A positive int if this BinString should be sorted after the other, a negative int if this BinString should be sorted before the other, and 0 if the BinStrings are identical.</returns>
        public int CompareTo(BinString other)
        {
            if (other is null) return 1;
            int l = Math.Min(this.Length, other.Length);

            for (int i = 0; i < l; i++)
            {
                int c = _Data[i].CompareTo(other._Data[i]);
                if (c != 0) return c;
            }

            if (this.Length < other.Length) return -1;
            else if (this.Length > other.Length) return 1;
            else return 0;
        }

        /// <summary>
        /// Compares two BinStrings, based on sort order.
        /// </summary>
        /// <param name="x">The first BinString to compare.</param>
        /// <param name="y">The second BinString to compare.</param>
        /// <returns>A positive int if y comes before x, a negative int if x comes before y, 0 if x and y are identical.</returns>
        public static int Compare(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return x.CompareTo(y);
        }

        /// <summary>
        /// Compares two BinStrings, based on sort order.
        /// </summary>
        /// <param name="x">The first BinString to compare.</param>
        /// <param name="y">The second BinString to compare.</param>
        /// <returns>True if x comes after y, false otherwise.</returns>
        public static bool operator >(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return false;
            if (x is null) return false;
            return x.CompareTo(y) > 0;
        }

        /// <summary>
        /// Compares two BinStrings, based on sort order.
        /// </summary>
        /// <param name="x">The first BinString to compare.</param>
        /// <param name="y">The second BinString to compare.</param>
        /// <returns>True if x comes before y, false otherwise.</returns>
        public static bool operator <(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return false;
            if (x is null) return true;
            return x.CompareTo(y) < 0;
        }

        /// <summary>
        /// Compares two BinStrings, based on sort order.
        /// </summary>
        /// <param name="x">The first BinString to compare.</param>
        /// <param name="y">The second BinString to compare.</param>
        /// <returns>True if x comes after or is equal to y, false otherwise.</returns>
        public static bool operator >=(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            return x.CompareTo(y) >= 0;
        }

        /// <summary>
        /// Compares two BinStrings, based on sort order.
        /// </summary>
        /// <param name="x">The first BinString to compare.</param>
        /// <param name="y">The second BinString to compare.</param>
        /// <returns>True if x comes before or is equal to y, false otherwise.</returns>
        public static bool operator <=(BinString x, BinString y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return true;
            return x.CompareTo(y) <= 0;
        }

        /// <summary>
        /// Creates a new BinString object with the same contents.
        /// </summary>
        public object Clone() => new BinString(ToArray());

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
        /// Concatnates the BinString and byte array.
        /// </summary>
        public static BinString operator +(BinString x, byte[] y)
        {
            byte[] result = new byte[x.Length + y.Length];
            x.CopyTo(result, 0);
            y.CopyTo(result, x.Length);
            return new BinString(result);
        }

        /// <summary>
        /// Concatnates the BinString and byte array.
        /// </summary>
        public static BinString operator +(byte[] x, BinString y)
        {
            byte[] result = new byte[x.Length + y.Length];
            x.CopyTo(result, 0);
            y.CopyTo(result, x.Length);
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
            if (count < 0) throw new ArgumentException("count must be 0 or greater.", nameof(count));

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
        public static BinString operator *(BinString x, int count) => x.Repeat(count);

        /// <summary>
        /// Repeats the BinString a number of times.
        /// </summary>
        public static BinString operator *(int count, BinString x) => x.Repeat(count);

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
            if (length <= 0) throw new ArgumentException("Length must be positive.", nameof(length));
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
            if (length <= 0) throw new ArgumentException("Length must be positive.", nameof(length));
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
            if (BinString.IsNullOrEmpty(needle)) throw new ArgumentException("Cannot split with an empty needle.", nameof(needle));

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

        /// <summary>
        /// Joins the specified BinStrings into one.
        /// </summary>
        /// <param name="binStrings">The BinStrings to join into one.</param>
        /// <param name="glue">The BinString glue to insert between each string, or null for none.</param>
        /// <returns></returns>
        public static BinString Join(IEnumerable<BinString> binStrings, BinString glue = null)
        {
            bool useGlue = !IsNullOrEmpty(glue);
            bool first = true;
            var builder = new BinStringBuilder();

            foreach (var part in binStrings)
            {
                if (useGlue)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.Append(glue);
                    }
                }

                builder.Append(part);
            }

            return builder.ToBinString();
        }

        #endregion

        #region Check methods

        /// <summary>
        /// Checks if the specified index is valid for this BinString and throws an IndexOutOfRangeException if it isn't.
        /// A valid index is greater than or equal to zero and less than the length of the BinString.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">The given index is not valid for this BinString.</exception>
        protected void _CheckIndex(int index)
        {
            if (index < 0 || index > _Data.Length) throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Checks if the value given is null or empty, returning True if so and False otherwise.
        /// </summary>
        public static bool IsNullOrEmpty(BinString value)
        {
            if (value is null) return true;
            if (value._Data.Length == 0) return true;
            return false;
        }

#endregion

#region IConvertable methods

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return _Data.Length > 0;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            if (_Data.Length == 0)
            {
                return '\0';
            }
            else if (_Data.Length == 1)
            {
                byte n = _Data[0];
                if (n == 0 || (n >= 0x20 && n <= 0x7e))
                {
                    return (char)n;
                }
            }

            throw new OverflowException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            if (_Data.Length == 0)
            {
                return 0;
            }
            else if (_Data.Length == 1)
            {
                return (sbyte)_Data[0];
            }
            else
            {
                throw new OverflowException();
            }
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            if (_Data.Length == 0)
            {
                return 0;
            }
            else if (_Data.Length == 1)
            {
                return _Data[0];
            }
            else
            {
                throw new OverflowException();
            }
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            if (_Data.Length == 0)
            {
                return 0;
            }
            else if (_Data.Length == 4)
            {
                return BitConverter.ToSingle(_Data, 0);
            }
            else
            {
                throw new OverflowException();
            }
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            if (_Data.Length == 0)
            {
                return 0;
            }
            else if (_Data.Length == 8)
            {
                return BitConverter.ToDouble(_Data, 0);
            }
            else
            {
                throw new OverflowException();
            }
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider) => throw new InvalidCastException();
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => throw new InvalidCastException();
        string IConvertible.ToString(IFormatProvider provider) => ToString("g", provider);

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(byte[]))
            {
                return _Data;
            }
            else if (conversionType == typeof(BinString))
            {
                return this;
            }
            else
            {
                var code = Type.GetTypeCode(conversionType);
                switch (code)
                {
                    case TypeCode.Boolean: return this._Data.Length > 0;
                    case TypeCode.String: return this.ToString("g", provider);
                    case TypeCode.Byte: return ((IConvertible)this).ToByte(provider);
                    case TypeCode.SByte: return ((IConvertible)this).ToSByte(provider);
                    case TypeCode.Int16: return ((IConvertible)this).ToInt16(provider);
                    case TypeCode.Int32: return ((IConvertible)this).ToInt32(provider);
                    case TypeCode.Int64: return ((IConvertible)this).ToInt64(provider);
                    case TypeCode.UInt16: return ((IConvertible)this).ToUInt16(provider);
                    case TypeCode.UInt32: return ((IConvertible)this).ToUInt32(provider);
                    case TypeCode.UInt64: return ((IConvertible)this).ToUInt64(provider);
                    case TypeCode.Single: return ((IConvertible)this).ToSingle(provider);
                    case TypeCode.Double: return ((IConvertible)this).ToDouble(provider);
                    case TypeCode.Char: return ((IConvertible)this).ToChar(provider);
                    default: throw new InvalidCastException();
                }
            }
        }

#endregion

#region Serialization
        /// <summary>
        /// Reconstructs a seralized BinString.
        /// </summary>
        public BinString(SerializationInfo info, StreamingContext context)
        {
            if (info is null) throw new ArgumentNullException(nameof(info));

            string dataStr = (string)info.GetValue("BinStringData", typeof(string));
            _Data = Convert.FromBase64String(dataStr);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null) throw new ArgumentNullException(nameof(info));

            info.AddValue("BinStringData", ToBase64String());
        }
#endregion

#region Stream conversion

        /// <summary>
        /// Converts the BinString to a Stream.
        /// </summary>
        /// <returns>A Stream from which this BinString's data can be read.</returns>
        public Stream ToStream()
        {
            return new MemoryStream(_Data, 0, _Data.Length, false, false);
        }

        /// <summary>
        /// Creates a BinString from a Stream.
        /// </summary>
        public static BinString FromStream(Stream str)
        {
            if (str is MemoryStream strMs)
            {
                return new BinString(strMs.ToArray());
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    str.CopyTo(ms);
                    return new BinString(ms.ToArray());
                }
            }
        }

#endregion
    }
}
