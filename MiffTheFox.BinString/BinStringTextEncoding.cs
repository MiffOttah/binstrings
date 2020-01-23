using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// <param name="format">The format string for the BinString. See the documentation for examples.</param>
        /// <param name="formatProvider">The format provider used to format the bytes.</param>
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
            else if (format == "85")
            {
                var ascii85 = new BinaryTextEncodings.Ascii85
                {
                    LineLength = -1,
                    FormatProvider = formatProvider
                };
                return ascii85.GetString(this);
            }
            else if (format == "u" || format == "U")
            {
                var urlEncoding = new BinaryTextEncodings.UrlEncoding
                {
                    LowerCaseHex = format == "u",
                    FormatProvider = formatProvider
                };
                return urlEncoding.GetString(this);
            }
            else if (format == "e" || format == "E")
            {
                var backslashEncoding = new BinaryTextEncodings.BackslashEscapeEncoding
                {
                    LowerCaseHex = format == "e",
                    FormatProvider = formatProvider
                };
                return backslashEncoding.GetString(this);
            }
            else if (format == "q" || format == "Q")
            {
                var qp = new BinaryTextEncodings.QuotedPrintable
                {
                    LowerCaseHex = format == "q",
                    EnforceLineLengthLimit = false,
                    FormatProvider = formatProvider
                };
                return qp.GetString(this);
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
        /// Converts the binary string to a text string with the provided binary-to-text encoding.
        /// </summary>
        public string ToString(BinaryTextEncoding encoding) =>
            encoding is null ?
                throw new ArgumentNullException(nameof(encoding)) :
                encoding.GetString(this);

        /// <summary>
        /// Interperts the binary string as a text string with the provided text encoding.
        /// </summary>
        public string ToString(Encoding encoding) =>
            encoding is null ?
                throw new ArgumentNullException(nameof(encoding)) :
                encoding.GetString(_Data);

        /// <summary>
        /// Formats the BinString as a text string for display.
        /// </summary>
        /// <param name="format">The format string for the BinString. See the documentation for examples.</param>
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
        public string ToBase64String()
        {
            return Convert.ToBase64String(_Data);
        }

        /// <summary>
        /// Creates a BinString from a base64-encoded string
        /// </summary>
        public static BinString FromBase64String(string data)
        {
            return new BinString(Convert.FromBase64String(data));
        }
    }
}
