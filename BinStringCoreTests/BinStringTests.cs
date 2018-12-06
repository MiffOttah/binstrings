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
    }
}
