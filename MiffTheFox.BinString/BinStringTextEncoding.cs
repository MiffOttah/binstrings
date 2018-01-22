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
            else if (format == "q" || format == "Q")
            {
                return ToQuotedPrintableString(
                    format == "q" ?
                        QuotedPrintableFormattingOptions.LowerCaseHex | QuotedPrintableFormattingOptions.DontEnforceLineLengthLimit :
                        QuotedPrintableFormattingOptions.DontEnforceLineLengthLimit,
                    formatProvider
                );
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

        public string ToQuotedPrintableString() => ToQuotedPrintableString(0, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns the BinString as an ASCII text string with non-ASCII bytes repersented by MIME quoted-printable encoding.
        /// </summary>
        /// <param name="formattingOptions">A set of QuotedPrintableFormattingOptions to format with.</param>
        /// <param name="formatProvider">The format provider used to format the bytes for hexadecimal encoding.</param>
        public string ToQuotedPrintableString(QuotedPrintableFormattingOptions formattingOptions, IFormatProvider formatProvider)
        {
            var result = new StringBuilder();
            bool first = true;
            int lineLength = 0;
            const int maxLineLength = 75;
            bool usedNewlines = false;

            string hexFormat = formattingOptions.HasFlag(QuotedPrintableFormattingOptions.LowerCaseHex) ? "x2" : "X2";
            BinString[] sourceParts = new BinString[] { this };

            if (formattingOptions.HasFlag(QuotedPrintableFormattingOptions.KeepNewlines))
            {
                sourceParts = Split(FromTextString(Environment.NewLine, Encoding.ASCII));
                usedNewlines = true;
            }

            var extendLineCounter = formattingOptions.HasFlag(QuotedPrintableFormattingOptions.DontEnforceLineLengthLimit) ? null : new Action<int>(toAdd =>
            {
                if (lineLength + toAdd > maxLineLength)
                {
                    result.Append('=');
                    result.AppendLine();
                    usedNewlines = true;
                    lineLength = toAdd;
                }
                else
                {
                    lineLength += toAdd;
                }
            });

            foreach (var part in sourceParts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.AppendLine();
                    lineLength = 0;
                }

                foreach (byte b in part)
                {
                    if ((b >= 0x20 && b <= 0x7e) && b != 0x3d)
                    {
                        extendLineCounter?.Invoke(1);
                        result.Append((char)b);
                    }
                    else
                    {
                        extendLineCounter?.Invoke(3);
                        result.Append('=');
                        result.Append(b.ToString(hexFormat, formatProvider));
                    }
                }
            }

            string resultString = result.ToString();
            if (usedNewlines)
            {
                resultString = resultString.Replace(" " + Environment.NewLine, "=20" + Environment.NewLine);
            }
            if (resultString.EndsWith(" "))
            {
                resultString = resultString.Remove(resultString.Length - 1) + "=20";
            }
            return resultString;
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
        /// Creates a BinString from a base64-encoded string
        /// </summary>
        public static BinString FromBase64String(string data)
        {
            return new BinString(Convert.FromBase64String(data));
        }

        /// <summary>
        /// Encodes the BinString using the uuencode method.
        /// </summary>
        public string Uuencode(string filename, string unixPermissions = "644")
        {
            var paddedData = this + BinString.FromBytes(32, 32, 32);
            var encodedRaw = new StringBuilder();
            for (int i = 0; i < _Data.Length; i += 3)
            {
                int sectionData = paddedData[i] << 16 | paddedData[i + 1] << 8 | paddedData[i + 2];
                for (int j = 3; j >= 0; j--)
                {
                    int section = (sectionData >> (j * 6)) & 0b111111;
                    encodedRaw.Append((char)(section + 32));
                }
            }
            string encodedRawStr = encodedRaw.ToString();

            var uu = new StringBuilder();
            uu.Append("begin ");
            uu.Append(unixPermissions);
            uu.Append(' ');
            uu.Append(filename);
            uu.Append(Environment.NewLine);

            int finalLineLength = _Data.Length % 45;

            for (int i = 0; i < encodedRawStr.Length; i += 60)
            {
                if (i + 60 <= encodedRawStr.Length)
                {
                    uu.Append('M');
                    uu.Append(encodedRawStr.Substring(i, 60));
                }
                else if (finalLineLength > 0)
                {
                    uu.Append((char)(finalLineLength + 32));
                    uu.Append(encodedRawStr.Substring(i));
                }
                uu.Append(Environment.NewLine);
            }

            uu.Append('`');
            uu.Append(Environment.NewLine);
            uu.Append("end");

            return uu.ToString();
        }

        public static BinString Uudecode(string uuencoded)
        {
            var lines = uuencoded.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 4 || !lines[0].StartsWith("begin ")) throw new ArgumentException("This doesn't appear to be uuencoded data.");

            var result = new BinStringBuilder();
            var lineData = new BinStringBuilder(45);

            int valueAt(string line, int i, int lsh)
            {
                if (i < line.Length)
                {
                    int n = (line[i] - 32);
                    if (n >= 64) return 0;
                    return n << lsh;
                }
                else
                {
                    return 0;
                }
            }

            try
            {
                foreach (string line in lines.Skip(1))
                {
                    if (line == "`") break;

                    int bytes = line[0] - 32;
                    double expectedLength = Convert.ToDouble(bytes) * 4.0 / 3.0 + 1.0;
                    if (line.Length < expectedLength) throw new FormatException("Invalid uuencoded data.");

                    for (int i = 1; i < line.Length; i += 4)
                    {
                        int section = valueAt(line, i, 18) | valueAt(line, i + 1, 12) | valueAt(line, i + 2, 6) | valueAt(line, i + 3, 0);

                        lineData.Append(Convert.ToByte((section >> 16) & 0xff));
                        lineData.Append(Convert.ToByte((section >> 8) & 0xff));
                        lineData.Append(Convert.ToByte(section & 0xff));
                    }

                    if (lineData.Length < bytes) throw new FormatException("Invalid uuencoded data.");
                    else if (lineData.Length == bytes) result.Append(lineData);
                    else result.Append(lineData.ToBinString().Substring(0, bytes));

                    lineData.Dispose();
                    lineData = new BinStringBuilder(45);
                }

                return result.ToBinString();
            }
            finally
            {
                result.Dispose();
                lineData.Dispose();
            }
        }
    }
}
