using MiffTheFox;
using System;
using Xunit;

namespace BinStringCoreTests
{
    public class BinStringTests
    {
        [Fact]
        public void FullSpanAccess()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            var testSpan = testData.AsSpan();

            Assert.Equal(testData.Length, testSpan.Length);
            for (int i = 0; i < testData.Length; i++)
            {
                Assert.Equal(testData[i], testSpan[i]);
            }
        }

        [Fact]
        public void CreationFromSpan()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            var testSource = testData.ToArray().AsSpan();
            var reconstructed = new BinString(testSource);
            Assert.Equal(testData, reconstructed);
        }

        [Fact]
        public void CreationFromReadOnlySpan()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            var testSource = testData.AsSpan();
            var reconstructed = new BinString(testSource);
            Assert.Equal(testData, reconstructed);
        }

        [Fact]
        public void PartialSpanAccessWithoutLength()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            var testSlice = testData.Slice(2);
            Assert.Equal(testData.Substring(2), new BinString(testSlice));
        }

        [Fact]
        public void PartialSpanAccessWithLength()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            var testSlice = testData.Slice(2, 4);
            Assert.Equal(testData.Substring(2, 4), new BinString(testSlice));
        }

        [Fact]
        public void IndexingWithInt()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            Assert.Equal((byte)2, testData[2]);
            Assert.Equal((byte)0x80, testData[5]);
            Assert.Throws<IndexOutOfRangeException>(() => testData[-1]);
            Assert.Throws<IndexOutOfRangeException>(() => testData[30]);
        }

        [Fact]
        public void IndexingWithIndex()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            Assert.Equal((byte)2, testData[new Index(2)]);
            Assert.Equal((byte)0x80, testData[new Index(5)]);
            Assert.Equal((byte)0xa0, testData[^1]);
            Assert.Equal((byte)0x70, testData[^4]);
            Assert.Throws<IndexOutOfRangeException>(() => testData[new Index(30)]);
            Assert.Throws<IndexOutOfRangeException>(() => testData[new Index(30, true)]);
            Assert.Throws<IndexOutOfRangeException>(() => testData[new Index(0, true)]);
        }

        [Fact]
        public void IndexingWithRange()
        {
            var testData = BinString.FromBytes("00010203708090a0");
            Assert.Equal(testData.Substring(2, 2), testData[2..4]);
            Assert.Equal(testData.Substring(5), testData[^3..]);
            Assert.Equal(testData, testData[0..^0]);
        }
    }
}
