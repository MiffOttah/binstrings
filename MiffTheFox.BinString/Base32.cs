using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    /// Provides a Base32 encoder/decoder
    /// </summary>
    public class Base32
    {
        /// <summary>
        /// The RFC 4648 Base32 character set.
        /// </summary>
        public const string CHARSET_RFC4648 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// The zBase32 character set.
        /// </summary>
        public const string CHARSET_ZBASE32 = "ybndrfg8ejkmcpqxot1uwisza345h769";

        /// <summary>
        /// The Douglas Crockford Base32 character set.
        /// </summary>
        public const string CHARSET_CROCKFORD = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

        /// <summary>
        /// The triacontakaidecimal/base32hex character set.
        /// </summary>
        public const string CHARSET_BASE32HEX = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

        private string _CharacterSet;

        /// <summary>
        /// The character set used by the base32 encoder/decoder.
        /// </summary>
        public string CharacterSet
        {
            get => _CharacterSet;
            set
            {
                if (value.Length != 32)
                {
                    throw new InvalidOperationException("Character set must be exactly 32 characters.");
                }
                else
                {
                    _CharacterSet = value;
                }
            }
        }

        /// <summary>
        /// The padding character appended to the end if UsePadding is true.
        /// </summary>
        public char Padding { get; set; } = '=';

        /// <summary>
        /// Whether or not to append a padding character to the end of the data to bring it to an even multiple of eight bytes.
        /// </summary>
        public bool UsePadding { get; set; } = true;

        /// <summary>
        /// If true, case-insensitive comparison is used when decoding.
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        /// <summary>
        /// The culture used for case-insensitive comparison.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Creates a Base32 encoder/decoder with the specified character set.
        /// </summary>
        /// <param name="characterSet">The character set used by the base32 encoder/decoder.</param>
        public Base32(string characterSet = CHARSET_RFC4648)
        {
            CharacterSet = characterSet;
            Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return _CharacterSet;
        }

        /// <summary>
        /// Converts a binary string to its base32 repersentation.
        /// </summary>
        /// <param name="data">The binary string to convert.</param>
        public string GetString(BinString data)
        {
            if (BinString.IsNullOrEmpty(data)) return string.Empty;

            int building = 0;
            int buildLocation = 4;
            var result = new StringBuilder();
            
            foreach (byte b in data)
            {
                for (int i = 7; i >= 0; i--)
                {
                    building |= ((b >> i) & 1) << buildLocation;
                    buildLocation--;

                    if (buildLocation < 0)
                    {
                        result.Append(_CharacterSet[building]);
                        building = 0;
                        buildLocation = 4;
                    }
                }
            }
            
            if (buildLocation < 4)
            {
                result.Append(_CharacterSet[building]);
            }

            if (UsePadding)
            {
                while (result.Length % 8 != 0)
                {
                    result.Append(Padding);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts a base32 encoded string to its original binary form.
        /// </summary>
        /// <param name="encoded">The base32 string to convert.</param>
        public BinString GetBinString(string encoded)
        {
            return GetBinString(encoded, Culture);
        }

        /// <summary>
        /// Converts a base32 encoded string to its original binary form.
        /// </summary>
        /// <param name="encoded">The base32 string to convert.</param>
        /// <param name="culture">The culture used for case-insensitive comparison.</param>
        public BinString GetBinString(string encoded, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(encoded)) return BinString.Empty;
            string chars = _CharacterSet;

            if (IgnoreCase)
            {
                encoded = encoded.ToUpper(culture).TrimEnd(char.ToUpper(Padding, culture));
                chars = chars.ToUpper(culture);
            }
            else
            {
                encoded = encoded.TrimEnd(Padding);
            }

            var intData = encoded.Where(c => !char.IsWhiteSpace(c)).Select(c => chars.IndexOf(c)).ToArray();
            if (intData.Any(c => c == -1)) throw new FormatException("This string contains one or more characters not allowed in the character set.");
            int expectedLength = (intData.Length * 5) / 8;

            int building = 0;
            int buildLocation = 7;
            using (var result = new BinStringBuilder())
            {
                foreach (int encodedInt in intData)
                {
                    for (int i = 4; i >= 0; i--)
                    {
                        building |= ((encodedInt >> i) & 1) << buildLocation;
                        buildLocation--;

                        if (buildLocation < 0)
                        {
                            result.Append(Convert.ToByte(building));
                            building = 0;
                            buildLocation = 7;
                        }
                    }
                }

                if (buildLocation < 7)
                {
                    result.Append(Convert.ToByte(building));
                }

                var decoded = result.ToBinString();
                if (decoded.Length > expectedLength)
                {
                    return decoded.Substring(0, expectedLength);
                } else
                {
                    return decoded;
                }
            }
        }
    }
}
