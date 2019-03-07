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
    }
}
