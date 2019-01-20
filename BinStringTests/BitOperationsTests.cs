using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using MiffTheFox.BitOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStringTests
{
    [TestClass]
    public class BitOperationsTests
    {
        [TestMethod]
        public void TestBitWriterSingleBitWritingLength8()
        {
            var w = new BitWriter();

            w.Write(false);
            w.Write(true);
            w.Write(false);
            w.Write(false);

            Assert.AreEqual(4, w.BitsWritten);

            w.Write(true);
            w.Write(false);
            w.Write(true);
            w.Write(false);

            Assert.AreEqual(8, w.BitsWritten);
            Assert.AreEqual(BinString.FromBytes(0b0100_1010), w.ToBinString());
        }

        [TestMethod]
        public void TestBitWriterSingleBitWritingLength16()
        {
            var w = new BitWriter();

            w.Write(false);
            w.Write(true);
            w.Write(false);
            w.Write(false);
            Assert.AreEqual(4, w.BitsWritten);

            w.Write(true);
            w.Write(false);
            w.Write(true);
            w.Write(false);
            Assert.AreEqual(8, w.BitsWritten);

            w.Write(false);
            w.Write(false);
            w.Write(false);
            w.Write(true);
            Assert.AreEqual(12, w.BitsWritten);

            w.Write(true);
            w.Write(true);
            w.Write(false);
            w.Write(false);
            Assert.AreEqual(16, w.BitsWritten);

            Assert.AreEqual(
                BinString.FromBytes(0b0100_1010, 0b0001_1100),
                w.ToBinString()
            );
        }

        [TestMethod]
        public void TestBitWriterUnevenLength()
        {
            var w = new BitWriter();

            w.Write(false);
            w.Write(true);
            w.Write(false);
            w.Write(false);
            Assert.AreEqual(4, w.BitsWritten);

            Assert.AreEqual(BinString.Empty, w.ToBinString(BitWriterUnevenMode.Pad));
            Assert.AreEqual(BinString.FromBytes(0b0100_0000), w.ToBinString(BitWriterUnevenMode.Pad));
            Assert.ThrowsException<InvalidOperationException>(() => w.ToBinString(BitWriterUnevenMode.Disallow));
            Assert.ThrowsException<ArgumentException>(() => w.ToBinString((BitWriterUnevenMode)100));

            // The zero-parameter overload of ToBinString should always throw the InvalidOperationException for uneven lengths.
            Assert.ThrowsException<InvalidOperationException>(() => w.ToBinString());
        }

        [TestMethod]
        public void TestBitWriterMultiWrite()
        {
            const int bitSource = 0b0000_1111_0001_1100;

            var w = new BitWriter();
            w.WriteBits(bitSource, 12);
            w.WriteBits(bitSource, 4);

            Assert.AreEqual(
                BinString.FromBytes(0b1111_0001, 0b1100_1100),
                w.ToBinString()
            );
        }

        [TestMethod]
        public void TestBitWriterMultiWriteErrors()
        {
            var w = new BitWriter();
            Assert.ThrowsException<ArgumentException>(() => w.WriteBits(0, -1));
            Assert.ThrowsException<ArgumentException>(() => w.WriteBits(0, 128));
        }

        [TestMethod]
        public void TestBitReaderSingleBitRead()
        {
            var r = new BitReader(BinString.FromBytes(0b1010_0101, 0b1100_0011));
            bool value;

            Assert.AreEqual(16, r.Length);
            Assert.AreEqual(0, r.Position);

            Assert.IsTrue(r.TryReadBit(out value));
            Assert.IsTrue(value);
            Assert.IsTrue(r.TryReadBit(out value));
            Assert.IsFalse(value);
            Assert.IsTrue(r.TryReadBit(out value));
            Assert.IsTrue(value);
            Assert.IsTrue(r.TryReadBit(out value));
            Assert.IsFalse(value);

            Assert.AreEqual(4, r.Position);
        }

        [TestMethod]
        public void TestBitReaderSingleBitReadOffEnd()
        {
            var r = new BitReader(BinString.FromBytes(0b1010_0101, 0b1100_0011));
            
            for (int i = 0; i < 16; i++)
            {
                Assert.IsTrue(r.TryReadBit(out _));
            }

            Assert.IsFalse(r.TryReadBit(out _));
            Assert.IsFalse(r.TryReadBit(out _));
            Assert.IsFalse(r.TryReadBit(out _));
            Assert.IsFalse(r.TryReadBit(out _));
        }

        [TestMethod]
        public void TestBitReaderMultiRead()
        {
            var r = new BitReader(BinString.FromBytes(0b1010_0111, 0b1100_0011));

            Assert.AreEqual(8, r.ReadBits(8, out int values));
            Assert.AreEqual(0b1010_0111, values);

            Assert.AreEqual(5, r.ReadBits(5, out values));
            Assert.AreEqual(0b11000, values);

            Assert.AreEqual(3, r.ReadBits(16, out values));
            Assert.AreEqual(0b0110_0000_0000_0000, values);

            Assert.AreEqual(0, r.ReadBits(16, out values));
        }
    }
}
