using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStringTests
{
    [TestClass]
    public class BinaryTextEncodingTests
    {

        [TestMethod]
        public void TestUuEncode()
        {
            string expectedCat = "begin 644 cat.txt\n#0V%T\n`\nend".Replace("\n", Environment.NewLine);
            var catData = BinString.FromTextString("Cat", Encoding.ASCII);
            Assert.AreEqual(expectedCat, catData.Uuencode("cat.txt"));

            string wikipediaUrl = "begin 644 wikipedia-url.txt\n::'1T<#HO+W=W=RYW:6MI<&5D:6$N;W)G#0H`\n`\nend";
            Assert.AreEqual("http://www.wikipedia.org\r\n", BinString.Uudecode(wikipediaUrl).ToString(Encoding.ASCII));
        }
    }
}
