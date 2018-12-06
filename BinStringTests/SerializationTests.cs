using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BinStringTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void TestBinarySerialization()
        {
            var testData = BinString.FromEscapedString(@"\xc0\xff\xee Hello, world!");

            var binaryFormatter = new BinaryFormatter();
            var writeStream = new MemoryStream();
            binaryFormatter.Serialize(writeStream, testData);

            var readStream = new MemoryStream(writeStream.ToArray());
            var testData2 = (BinString)binaryFormatter.Deserialize(readStream);

            Assert.AreEqual(testData, testData2);
        }

        [TestMethod]
        public void TestBinaryWriterSupport()
        {
            var testData = BinString.FromEscapedString(@"\xc0\xff\xee Hello, world!"); ;

            var writeStream1 = new MemoryStream();
            using (var writer = new BinaryWriter(writeStream1))
            {
                writer.WriteBinStringIndirect(testData);
            }

            Assert.AreEqual(testData.Length.ToBinString() + testData, writeStream1.ToBinString());

            var writeStream2 = new MemoryStream();
            using (var writer = new BinaryWriter(writeStream2))
            {
                writer.WriteBinStringDirect(testData);
            }

            Assert.AreEqual(testData, writeStream2.ToBinString());
        }

        [TestMethod]
        public void TestBinaryReaderSupport()
        {
            var testData = BinString.FromEscapedString(@"Testing binary reader support!\x09\xf9\x12\x34");

            using (var reader = new BinaryReader(new MemoryStream(testData)))
            {
                Assert.AreEqual(testData, reader.ReadBinStringDirect(testData.Length));
            }

            using (var reader = new BinaryReader(new MemoryStream(testData)))
            {
                Assert.AreEqual(testData.Remove(16), reader.ReadBinStringDirect(16));
                Assert.AreEqual(testData.Substring(16, 4), reader.ReadBinStringDirect(4));
            }

            var indirectStream = new MemoryStream();
            using (var writer = new BinaryWriter(indirectStream))
            {
                writer.WriteBinStringIndirect(testData);
            }

            using (var reader = new BinaryReader(new MemoryStream(indirectStream.ToArray())))
            {
                Assert.AreEqual(testData, reader.ReadBinStringIndirect());
            }
        }
    }
}
