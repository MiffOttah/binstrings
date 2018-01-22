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
    }
}
