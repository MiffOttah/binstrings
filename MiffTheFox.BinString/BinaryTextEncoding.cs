using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MiffTheFox
{
    /// <summary>
    /// Repersents a method for converting binary data to and from text.
    /// </summary>
    public abstract class BinaryTextEncoding
    {
        /// <summary>
        /// When overridden in a derived class, converts the binary data to a textual repersentation.
        /// </summary>
        public abstract string GetString(BinString data);

        /// <summary>
        /// When overridden in a derived class, converts the textual repersentation to binary data.
        /// </summary>
        public abstract BinString GetBinString(string encoded);

        /// <summary>
        /// The culture to use for the conversion.
        /// </summary>
        public virtual IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Base-64 encoding.
        /// </summary>
        public static BinaryTextEncoding Base64 { get; } = new BinaryTextEncodings.Base64();

        /// <summary>
        /// URL/URI encoding.
        /// </summary>
        public static BinaryTextEncoding Url { get; } = new BinaryTextEncodings.UrlEncoding();

        /// <summary>
        /// Backslash-escaped encoding.
        /// </summary>
        public static BinaryTextEncoding BackslashEscape { get; } = new BinaryTextEncodings.BackslashEscapeEncoding();
    }

    namespace BinaryTextEncodings
    {
        /// <summary>
        /// Implements URL/URI encoding
        /// </summary>
        public class UrlEncoding : BinaryTextEncoding
        {
            /// <summary>
            /// True for hexadecimal bytes in lower case, false for upper case.
            /// </summary>
            public bool LowerCaseHex { get; set; } = true;

            /// <summary>
            /// Decodes a URL encoded binstring
            /// </summary>
            public override BinString GetBinString(string encoded)
            {
                if (encoded is null) throw new ArgumentNullException("Provided data is null.", nameof(encoded));
                var result = new BinStringBuilder();

                for (int i = 0; i < encoded.Length; i++)
                {
                    switch (encoded[i])
                    {
                        case '+':
                            result.Append(0x20);
                            break;

                        case '%':
                            if ((i + 2) < encoded.Length && int.TryParse(encoded.Substring(i + 1, 2), NumberStyles.AllowHexSpecifier, FormatProvider, out int hexValue))
                            {
                                i += 2;
                                result.Append(Convert.ToByte(hexValue));
                            }
                            else
                            {
                                throw new ArgumentException("URL encoded data contains an invalid escape sequence.", nameof(encoded));
                            }
                            break;

                        case char c when c <= '~':
                            result.Append((byte)c);
                            break;

                        default:
                            throw new ArgumentException("URL encoded data contains a non-ASCII character.", nameof(encoded));
                    }
                }

                return result.ToBinString();
            }


            /// <summary>
            /// Encodes a binstring with URL encoding
            /// </summary>
            public override string GetString(BinString data)
            {
                var result = new StringBuilder();
                foreach (byte b in data)
                {
                    if (b > 0x20 && b < 0x7f && b != '+' && b != '%')
                    {
                        result.Append((char)b);
                    }
                    else
                    {
                        result.Append('%');
                        result.Append(b.ToString(LowerCaseHex ? "x2" : "X2", FormatProvider));
                    }
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// Implements Base64.
        /// </summary>
        public class Base64 : BinaryTextEncoding
        {
            /// <summary>
            /// Converts the binary data to a textual repersentation.
            /// </summary>
            public override string GetString(BinString data) => Convert.ToBase64String(data.ToArray());

            /// <summary>
            /// Converts the textual repersentation to binary data.
            /// </summary>
            public override BinString GetBinString(string data) => new BinString(Convert.FromBase64String(data));
        }

        /// <summary>
        /// Implmentes Ascii85.
        /// </summary>
        public class Ascii85 : BinaryTextEncoding
        {
            /// <summary>
            /// The maximum length for each line of encoded text. If zero or negative,
            /// will not insert any line breaks.
            /// </summary>
            public int LineLength { get; set; } = 75;

            /// <summary>
            /// The newline character to use when encoding.
            /// </summary>
            public string Newline { get; set; } = Environment.NewLine;

            /// <summary>
            /// If true, requires a string to be decoded to have Ascii85 delimiters.
            /// </summary>
            public bool RequireDelimiters { get; set; } = true;

            /// <summary>
            /// Converts the textual repersentation to binary data.
            /// </summary>
            public override BinString GetBinString(string encoded)
            {
                if (string.IsNullOrEmpty(encoded)) return BinString.Empty;

                if (RequireDelimiters)
                {
                    if (encoded.Length < 4 || !encoded.StartsWith("<~") || !encoded.EndsWith("~>")) throw new FormatException("Invalid Ascii85 string.");
                    encoded = encoded.Substring(2, encoded.Length - 4);
                }

                var trueData = new List<int>(encoded.Length);
                foreach (int n in encoded.Select(c => (int)c))
                {
                    if (n > 117)
                    {
                        throw new FormatException("Invalid Ascii85 string.");
                    }
                    else if (n > 32)
                    {
                        trueData.Add(n - 33);
                    }
                }

                using (var result = new BinStringBuilder())
                {
                    for (int i = 0; i < trueData.Count; i += 5)
                    {
                        long groupNum = 0;
                        int padding = 0;

                        for (int j = 0; j < 5; j++)
                        {
                            if ((i + j) < trueData.Count)
                            {
                                groupNum = (groupNum * 85) + trueData[i + j];
                            }
                            else
                            {
                                groupNum = (groupNum * 85) + 84;
                                padding++;
                            }
                        }

                        result.Append(Convert.ToByte((groupNum >> 24) & 0xff));
                        if (padding < 3) result.Append(Convert.ToByte((groupNum >> 16) & 0xff));
                        if (padding < 2) result.Append(Convert.ToByte((groupNum >> 8) & 0xff));
                        if (padding < 1) result.Append(Convert.ToByte(groupNum & 0xff));
                    }

                    return result.ToBinString();
                }
            }

            /// <summary>
            /// Converts the binary data to a textual repersentation.
            /// </summary>
            public override string GetString(BinString data)
            {
                var result = new StringBuilder();
                result.Append("<~");

                var groupResult = new int[5];
                bool lineBreaks = LineLength > 0;
                int linePos = 0;

                for (int i = 0; i < data.Length; i += 4)
                {
                    var group = data.Substring(i, 4);
                    int padding = 0;

                    if (group.Length < 4)
                    {
                        padding = 4 - group.Length;
                        group = group.PadRight(4);
                    }

                    uint groupNum =
                        Convert.ToUInt32(group[0]) << 24 |
                        Convert.ToUInt32(group[1]) << 16 |
                        Convert.ToUInt32(group[2]) << 8 |
                        Convert.ToUInt32(group[3]);

                    for (int j = 0; j < 5; j++)
                    {
                        groupResult[j] = 33 + Convert.ToInt32(groupNum % 85);
                        groupNum /= 85;
                    }

                    for (int j = 4; j >= padding; j--)
                    {
                        if (lineBreaks && linePos >= LineLength)
                        {
                            result.AppendLine();
                            linePos = 0;
                        }

                        result.Append((char)groupResult[j]);
                        linePos++;
                    }
                }

                result.Append("~>");
                return result.ToString();
            }
        }

        /// <summary>
        /// An encoding method that treats the string as as ASCII string, and each non-ASII character
        /// is escaped with a backslash (or other character).
        /// </summary>
        public class BackslashEscapeEncoding : BinaryTextEncoding
        {
            /// <summary>
            /// The escape character to use.
            /// </summary>
            public char EscapeCharacter { get; set; } = '\\';

            /// <summary>
            /// The ASCII characters that will have a prefix from the escape character.
            /// </summary>
            public HashSet<char> SelfRepersentingChars { get; set; } =
                new HashSet<char>() { '\\', '"', '\'' };

            /// <summary>
            /// True for hexadecimal bytes in lower case, false for upper case.
            /// </summary>
            public bool LowerCaseHex { get; set; } = true;


            /// <summary>
            /// Converts the textual repersentation to binary data.
            /// </summary>
            public override BinString GetBinString(string encoded)
            {
                if (encoded == null)
                {
                    throw new ArgumentNullException(nameof(encoded));
                }

                var result = new BinStringBuilder();
                int i = 0;

                while (i < encoded.Length)
                {
                    if (encoded[i] == EscapeCharacter)
                    {
                        i++;
                        if (i < encoded.Length)
                        {
                            if (SelfRepersentingChars != null && _IsPrintableAscii(encoded[i]) && SelfRepersentingChars.Contains(encoded[i]))
                            {
                                result.Append((byte)encoded[i++]);
                                continue;
                            }

                            if (ReadEscapedByte(encoded, result, ref i))
                            {
                                continue;
                            }
                        }
                        throw new ArgumentException("Invalid escape sequence in data.", nameof(encoded));
                    }
                    else if (_IsPrintableAscii(encoded[i]))
                    {
                        result.Append((byte)encoded[i++]);
                    }
                    else
                    {
                        throw new ArgumentException("Escaped data contains a non-ASCII character.", nameof(encoded));
                    }
                }

                return result.ToBinString();
            }

            /// <summary>
            /// Reads an escaped byte and writes it to the BinStringBuilder. Can be overridden in a derived class to change the default behavior.
            /// </summary>
            /// <param name="encoded">The encoded data</param>
            /// <param name="unescapedDataBuilder">The BinStringBuilder building up the unescaped data.</param>
            /// <param name="i">The position in the encoded data of the decoder. This must be incremented past the encoded byte.</param>
            /// <returns>True if sucessful, false for a failed read (such as an invalid escape sequence or trying to read off the end of the string).</returns>
            protected virtual bool ReadEscapedByte(string encoded, BinStringBuilder unescapedDataBuilder, ref int i)
            {
                if ((i + 3 <= encoded.Length) && (encoded[i] == 'x'))
                {
                    string hex = encoded.Substring(i + 1, 2);
                    if (byte.TryParse(hex, NumberStyles.HexNumber, FormatProvider, out byte b))
                    {
                        unescapedDataBuilder.Append(b);
                        i += 3;
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Converts the textual repersentation to binary data.
            /// </summary>
            public override string GetString(BinString data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                var result = new StringBuilder();
                foreach (byte b in data)
                {
                    if (_IsPrintableAscii(b))
                    {
                        char bc = (char)b;
                        if (SelfRepersentingChars != null && SelfRepersentingChars.Contains(bc))
                        {
                            result.Append(EscapeCharacter);
                            result.Append(bc);
                        }
                        else if (bc == EscapeCharacter)
                        {
                            WriteEscapedCharacter(result, b);
                        }
                        else
                        {
                            result.Append((char)b);
                        }
                    }
                    else
                    {
                        WriteEscapedCharacter(result, b);
                    }
                }
                return result.ToString();
            }

            private static bool _IsPrintableAscii(int b) => b >= 0x20 && b < 0x7f;

            /// <summary>
            /// Writes an escaped byte to the StringBuilder. Can be overridden in a derived class to change the default behavior.
            /// </summary>
            /// <param name="escapedTextBuilder">The StringBuilder building up the escaped text</param>
            /// <param name="b">The byte to write</param>
            protected virtual void WriteEscapedCharacter(StringBuilder escapedTextBuilder, byte b)
            {
                escapedTextBuilder.Append(EscapeCharacter);
                escapedTextBuilder.Append('x');
                escapedTextBuilder.Append(b.ToString(LowerCaseHex ? "x2" : "X2", FormatProvider));
            }
        }

        /// <summary>
        /// Implements UUEncode.
        /// </summary>
        public class UUEncode : BinaryTextEncoding
        {
            /// <summary>
            /// The filename to include with the uuencoded data.
            /// </summary>
            public string FileName { get; set; } = "data";

            /// <summary>
            /// The UNIX permissions to include with the uuencoded data.
            /// </summary>
            public string UnixPermissions { get; set; } = "644";

            /// <inheritdoc />
            public override BinString GetBinString(string encoded)
            {
                var lines = encoded.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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

            /// <inheritdoc />
            public override string GetString(BinString data)
            {
                int paddingRequired = data.Length % 3;
                var paddedData = data + BinString.FromBytes(32, 32, 32);

                var encodedRaw = new StringBuilder();
                for (int i = 0; i < data.Length; i += 3)
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
                uu.Append(UnixPermissions);
                uu.Append(' ');
                uu.Append(FileName);
                uu.Append(Environment.NewLine);

                int finalLineLength = (data.Length % 45);

                for (int i = 0; i < encodedRawStr.Length; i += 60)
                {
                    if (i + 60 < encodedRawStr.Length)
                    {
                        uu.Append('M');
                        uu.Append(encodedRawStr.Substring(i, 60));
                    }
                    else if (i + 60 == encodedRawStr.Length && finalLineLength == 0)
                    {
                        uu.Append((char)(77 - paddingRequired));
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
        }

        /// <summary>
        /// Implements Quoted-Printable encoding. Decoding is not supported.
        /// </summary>
        public class QuotedPrintable : BinaryTextEncoding
        {
            /// <summary>
            /// If true, hexadecimal digits are lower case, instead of upper case.
            /// </summary>
            public bool LowerCaseHex { get; set; } = false;

            /// <summary>
            /// If true, newlines in the input text are preserved, rater than encoded
            /// </summary>
            public bool KeepNewlines { get; set; } = false;

            /// <summary>
            /// If true, newlines are added automatically to keep output lines under 75 characters in length (as specified by QP)
            /// </summary>
            public bool EnforceLineLengthLimit { get; set; } = true;

            /// <summary>
            /// The newline character to use.
            /// </summary>
            public string Newline { get; set; } = Environment.NewLine;
            
            /// <summary>
            /// This is not supported and will throw an exception.
            /// </summary>
            public override BinString GetBinString(string encoded)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Encodes the data as a Quoted-Printable string.
            /// </summary>
            public override string GetString(BinString data)
            {
                var result = new StringBuilder();
                bool first = true;
                int lineLength = 0;
                const int maxLineLength = 75;
                bool usedNewlines = false;

                string hexFormat = LowerCaseHex ? "x2" : "X2";
                BinString[] sourceParts;

                if (KeepNewlines)
                {
                    sourceParts = data.Split(new BinString(Newline, Encoding.ASCII));
                    usedNewlines = true;
                }
                else
                {
                    sourceParts = new BinString[] { data };
                }

                var extendLineCounter = EnforceLineLengthLimit ? new Action<int>(toAdd =>
                {
                    if (lineLength + toAdd > maxLineLength)
                    {
                        result.Append('=');
                        result.Append(Newline);
                        usedNewlines = true;
                        lineLength = toAdd;
                    }
                    else
                    {
                        lineLength += toAdd;
                    }
                }) : null;

                foreach (var part in sourceParts)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        result.Append(Newline);
                        lineLength = 0;
                    }

                    foreach (byte b in part)
                    {
                        if ((b >= 0x20 && b <= 0x7e) && b != '=')
                        {
                            extendLineCounter?.Invoke(1);
                            result.Append((char)b);
                        }
                        else
                        {
                            extendLineCounter?.Invoke(3);
                            result.Append('=');
                            result.Append(b.ToString(hexFormat, FormatProvider));
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
        }
    }
}