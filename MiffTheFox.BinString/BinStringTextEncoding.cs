using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    partial class BinString
    {
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
    }
}
