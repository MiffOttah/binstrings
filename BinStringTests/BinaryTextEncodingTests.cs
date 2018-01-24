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
    public class BinaryTextEncodingTests
    {
        [TestMethod]
        public void UuencodeTest()
        {
            string expectedCat = "begin 644 cat.txt\n#0V%T\n`\nend".Replace("\n", Environment.NewLine);
            var catData = BinString.FromTextString("Cat", Encoding.ASCII);
            Assert.AreEqual(expectedCat, catData.Uuencode("cat.txt"));

            string wikipediaUrl = "begin 644 wikipedia-url.txt\n::'1T<#HO+W=W=RYW:6MI<&5D:6$N;W)G#0H`\n`\nend";
            Assert.AreEqual("http://www.wikipedia.org\r\n", BinString.Uudecode(wikipediaUrl).ToString(Encoding.ASCII));

            var knownHard = BinString.FromBytes("000306090c0f1215181b1e2124272a2d303336393c3f4245484b4e5154575a5d606366696c6f7275787b7e8184");
            Assert.AreEqual(knownHard, BinString.Uudecode(knownHard.Uuencode("test")));
        }

        [TestMethod]
        public void QuotedPrintableTest()
        {
            var shortText = BinString.FromTextString("Café", Encoding.UTF8);
            Assert.AreEqual("Caf=C3=A9", shortText.ToString("Q"));
            Assert.AreEqual("Caf=c3=a9", shortText.ToString("q"));

            var longText = BinString.FromTextString(@"J'interdis aux marchands de vanter trop leur marchandises. Car ils se font vite pédagogues et t'enseignent comme but ce qui n'est par essence qu'un moyen, et te trompant ainsi sur la route à suivre les voilà bientôt qui te dégradent, car si leur musique est vulgaire ils te fabriquent pour te la vendre une âme vulgaire.", Encoding.UTF8);
            Assert.AreEqual(@"J'interdis aux marchands de vanter trop leur marchandises. Car ils se font =
vite p=C3=A9dagogues et t'enseignent comme but ce qui n'est par essence qu'=
un moyen, et te trompant ainsi sur la route =C3=A0 suivre les voil=C3=A0 bi=
ent=C3=B4t qui te d=C3=A9gradent, car si leur musique est vulgaire ils te f=
abriquent pour te la vendre une =C3=A2me vulgaire.", longText.ToQuotedPrintableString(QuotedPrintableFormattingOptions.Default, CultureInfo.InvariantCulture));

            var endsInSpace = BinString.FromTextString("Ends in space ", Encoding.ASCII);
            Assert.AreEqual("Ends in space=20", endsInSpace.ToQuotedPrintableString());

            var withNewlines = BinString.FromTextString("This string $has newlines $for this system. ".Replace("$", Environment.NewLine), Encoding.ASCII);
            string withNewLinesQP = withNewlines.ToQuotedPrintableString(QuotedPrintableFormattingOptions.KeepNewlines, CultureInfo.InvariantCulture);
            Assert.AreEqual("This string=20$has newlines=20$for this system.=20".Replace("$", Environment.NewLine), withNewLinesQP);

            var withEquals = BinString.FromTextString("2 × 5 = 10", Encoding.UTF8);
            Assert.AreEqual("2 =C3=97 5 =3D 10", withEquals.ToString("Q"));
        }

        [TestMethod]
        public void Ascii85Test()
        {
            const string leviathan = @"Man is distinguished, not only by his reason, but by this singular passion from other animals, which is a lust of the mind, that by a perseverance of delight in the continued and indefatigable generation of knowledge, exceeds the short vehemence of any carnal pleasure.";
            const string expected = "<~9jqo^BlbD-BleB1DJ+*+F(f,q/0JhKF<GL>Cj@.4Gp$d7F!,L7@<6@)/0JDEF<G%<+EV:2F!,O<DJ+*.@<*K0@<6L(Df-\\0Ec5e;DffZ(EZee.Bl.9pF\"AGXBPCsi+DGm>@3BB/F*&OCAfu2/AKYi(DIb:@FD,*)+C]U=@3BN#EcYf8ATD3s@q?d$AftVqCh[NqF<G:8+EV:.+Cf>-FD5W8ARlolDIal(DId<j@<?3r@:F%a+D58'ATD4$Bl@l3De:,-DJs`8ARoFb/0JMK@qB4^F!,R<AKZ&-DfTqBG%G>uD.RTpAKYo'+CT/5+Cei#DII?(E,9)oF*2M7/c~>";

            var leviathanBin = BinString.FromTextString(leviathan, Encoding.ASCII);
            Assert.AreEqual(expected, leviathanBin.ToString("85"));
            Assert.AreEqual(leviathanBin, BinString.FromAscii85String(expected));
            Assert.AreEqual(leviathanBin, BinString.FromAscii85String(expected.Substring(2, expected.Length - 4), false));
        }

        [TestMethod]
        public void RoundTripTest()
        {
            var builder = new BinStringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                builder.Append(Convert.ToByte((i * 3) & 0xff));
                var b = builder.ToBinString();

                //Assert.AreEqual(b, BinString.FromAscii85String(b.ToAscii85String()));
                Assert.AreEqual(b, BinString.Uudecode(b.Uuencode("test")));
            }
        }
    }
}
