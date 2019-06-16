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
    public class BuilderTests
    {
        [TestMethod]
        public void BuilderBasicTest()
        {
            var builder = new BinStringBuilder();
            var expected = BinString.FromBytes("0102030405060708090A0B0C0D");

            builder.Append(BinString.FromBytes(1, 2, 3));
            builder.Append(BinString.FromBytes(4, 5, 6));
            builder.Append(7);
            builder.Append(new byte[] { 8, 9, 10 });

            var builder2 = new BinStringBuilder();
            builder2.Append(BinString.FromBytes("0B0C"));
            builder2.Append(0x0d);
            builder.Append(builder2);

            Assert.AreEqual(13, builder.Length);
            Assert.AreEqual(expected, builder.ToBinString());
            Assert.AreEqual(expected.ToString("s"), builder.ToString("s"));
        }

        [TestMethod]
        public void BuilderNullTest()
        {

            var builder = new BinStringBuilder();
            var expected = BinString.FromBytes("0102030405060708090A");

            builder.Append(BinString.FromBytes(1, 2, 3));
            builder.Append((BinString)null);
            builder.Append(BinString.FromBytes(4, 5, 6));
            builder.Append(7);
            builder.Append((byte[])null);
            builder.Append(new byte[] { 8, 9, 10 });
            builder.Append((BinStringBuilder)null);

            Assert.AreEqual(10, builder.Length);
            Assert.AreEqual(expected, builder.ToBinString());
        }

        [TestMethod]
        public void BuilderDisposalTest()
        {
            var builder = new BinStringBuilder();
            builder.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => builder.Append(BinString.FromBytes(1, 2, 3)));
            Assert.ThrowsException<ObjectDisposedException>(() => builder.Append(new byte[] { 8, 9, 10 }));
            Assert.ThrowsException<ObjectDisposedException>(() => builder.Append(42));
            Assert.ThrowsException<ObjectDisposedException>(() => builder.Length);
            Assert.ThrowsException<ObjectDisposedException>(() => builder.ToBinString());

            // subsequent calls to dispose should have no effect
            builder.Dispose();
        }
    }
}
