using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    public class Base32
    {
        public const string CHARSET_RFC4648 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        public const string CHARSET_ZBASE32 = "ybndrfg8ejkmcpqxot1uwisza345h769";
        public const string CHARSET_CROCKFORD = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        public const string CHARSET_BASE32HEX = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

        private string _CharacterSet;
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

        public char Padding { get; set; } = '=';
        public bool UsePadding { get; set; } = true;
        public bool IgnoreCase { get; set; } = true;
        public CultureInfo Culture { get; set; }
        
        public Base32(string characterSet = CHARSET_RFC4648)
        {
            CharacterSet = characterSet;
            Culture = CultureInfo.CurrentCulture;
        }

        public override string ToString()
        {
            return _CharacterSet;
        }

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

        public BinString GetBinString(string encoded)
        {
            return GetBinString(encoded, Culture);
        }

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
