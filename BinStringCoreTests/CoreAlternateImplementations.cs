using MiffTheFox;
using System;
using Xunit;

namespace BinStringCoreTests
{
    public class CoreAlternateImplementations
    {
        [Fact]
        public void BinStringBuilderAppendViaSpan()
        {
            var testData = BinString.FromBytes("01020304");
            var builder = new BinStringBuilder();
            builder.Append(testData);
            builder.Append(testData);
            Assert.Equal(testData + testData, builder.ToBinString());
        }

        [Fact]
        public void BinBoyerMooreFindInSpan()
        {
            var needle = BinString.FromBytes("0102");
            var haystack = BinString.FromBytes("FF00 0102 0344 0102");
            var bbm = new BinBoyerMoore(needle);

            Assert.Equal(2, bbm.FindNeedleIn(haystack.AsSpan()));
        }

        [Fact]
        public void SearchTest()
        {
            var someData = BinString.FromBytes(10, 20, 30, 40, 50, 60, 70, 80, 90, 100);
            var repeatingData = BinString.FromBytes(10, 20, 30, 40, 10, 20, 30, 50, 80);

            Assert.Equal(0, someData.IndexOf(10));
            Assert.Equal(2, someData.IndexOf(30));
            Assert.Equal(-1, someData.IndexOf(55));

            Assert.Equal(2, someData.IndexOf(BinString.FromBytes(30, 40)));
            Assert.Equal(5, someData.IndexOf(BinString.FromBytes(60, 70, 80)));
            Assert.Equal(-1, someData.IndexOf(BinString.FromBytes(4, 8, 15, 16, 23, 42)));
            Assert.Equal(-1, someData.IndexOf(BinString.FromBytes(80, 90, 100, 110)));
            Assert.Equal(0, someData.IndexOf(new BinString()));

            Assert.Equal(0, repeatingData.IndexOf(BinString.FromBytes(10, 20, 30)));
            Assert.Equal(4, repeatingData.IndexOf(BinString.FromBytes(10, 20, 30, 50)));

            Assert.Equal(BinString.FromBytes(200, 210, 40, 200, 210, 50, 80), repeatingData.Replace(BinString.FromBytes(10, 20, 30), BinString.FromBytes(200, 210)));

            var sections = new BinString[] {
                BinString.FromBytes(10),
                BinString.FromBytes(40, 10),
                BinString.FromBytes(50, 80)
            };
            var split = repeatingData.Split(BinString.FromBytes(20, 30));
            Assert.Equal(sections, split);

            var notSplit = someData.Split((BinString)0xff);
            
            Assert.Single(notSplit);
            Assert.Equal(someData, notSplit[0]);

            Assert.Throws<ArgumentException>(() => repeatingData.Split(new BinString()));
        }
    }
}
