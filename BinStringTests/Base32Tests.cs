using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStringTests
{
    [TestClass]
    public class Base32Tests
    {
        [TestMethod]
        public void Base32EncodingTest()
        {
            var base32 = new Base32();
            var hello = new BinString("Hello, world!", Encoding.ASCII);

            // null/empty
            Assert.AreEqual(string.Empty, base32.GetString(BinString.Empty));
            Assert.AreEqual(string.Empty, base32.GetString(null));

            // basic
            Assert.AreEqual("JBSWY3DPFQQHO33SNRSCC===", base32.GetString(hello));
            Assert.AreEqual("KRUGS4ZANFZSAYJAORSXG5BO", base32.GetString(new BinString("This is a test.", Encoding.UTF8)));
            
            // custom/no padding
            base32.Padding = '?';
            Assert.AreEqual("ME??????", base32.GetString((BinString)97));
            base32.UsePadding = false;
            Assert.AreEqual("JBSWY3DPFQQHO33SNRSCC", base32.GetString(hello));

            // alternate character set
            var crockfordBase32 = new Base32(Base32.CHARSET_CROCKFORD.ToLowerInvariant()) { UsePadding = false };
            Assert.AreEqual("91jprv3f5gg7evvjdhj22", crockfordBase32.GetString(hello));
        }

        [TestMethod]
        public void Base32DecodingTest()
        {
            var base32 = new Base32() { Culture = CultureInfo.InvariantCulture };

            // null/empty
            Assert.AreEqual(BinString.Empty, base32.GetBinString(null));
            Assert.AreEqual(BinString.Empty, base32.GetBinString(string.Empty));
            Assert.AreEqual(BinString.Empty, base32.GetBinString("========"));

            // basic
            Assert.AreEqual("vwxyz", base32.GetBinString("OZ3XQ6L2").ToString(Encoding.ASCII));
            Assert.AreEqual("Decode", base32.GetBinString("IRSWG33EMU======").ToString(Encoding.ASCII));
            Assert.AreEqual("==TEST==", base32.GetBinString("HU6VIRKTKQ6T2===").ToString(Encoding.ASCII));

            // without padding
            Assert.AreEqual("~test~", base32.GetBinString("PZ2GK43UPY").ToString(Encoding.ASCII));
            Assert.AreEqual("?", base32.GetBinString("H4").ToString(Encoding.ASCII));

            // white space, lower case
            Assert.AreEqual("WhiteSpace", base32.GetBinString("\r\nK5UG S5DF KNYG\tCY\t3F\n").ToString(Encoding.ASCII));
            Assert.AreEqual("~?~", base32.GetBinString("py7x4===").ToString(Encoding.ASCII));

            // invalid characters
            Assert.ThrowsException<FormatException>(() => base32.GetBinString("ERROR!"));
            base32.IgnoreCase = false;
            Assert.ThrowsException<FormatException>(() => base32.GetBinString("py7x4==="));
            base32.IgnoreWhiteSpace = false;
            Assert.ThrowsException<FormatException>(() => base32.GetBinString("\r\nK5UG S5DF KNYG\tCY\t3F\n"));
            base32.IgnoreCase = true;
            Assert.ThrowsException<FormatException>(() => base32.GetBinString("\r\nK5UG S5DF KNYG\tCY\t3F\n"));

            // alternate character set
            var crockfordBase32 = new Base32(Base32.CHARSET_CROCKFORD.ToLowerInvariant());
            Assert.AreEqual("12345", crockfordBase32.GetBinString("64s36d1n").ToString(Encoding.ASCII));
        }

        [TestMethod]
        [DataRow("", Base32.CHARSET_CROCKFORD)]
        [DataRow("AFWPjq109Z2TGHvuZMCvVg==", Base32.CHARSET_CROCKFORD)]
        [DataRow("HbPLdjim33EFgXOwi5qRtA==", Base32.CHARSET_ZBASE32)]
        [DataRow("hUhErbXES0j4Sycji9VI9A2jHFjHkXShcBcH+sWauVU=", Base32.CHARSET_RFC4648)]
        [DataRow("yYwf0SSVdEhEhgoa9N9TIsUDIHo=", "1234567890!@#$%^&*()_-+H`~:;abcZ")]
        public void RoundTripTest(string bytes, string charset)
        {
            var data = BinString.FromBase64String(bytes);
            var base32 = new Base32(charset) { Padding = '=' };

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(data, base32.GetBinString(base32.GetString(data)));
                data += Convert.ToByte((i * 3) % 256);
            }
        }
    }
}
