using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BinStringTests
{
    [TestClass]
    public class ExtensionMethodTests
    {
        [TestMethod]
        public void ConverterTest()
        {
            bool nBool = true;
            char nChar = 'Z';
            sbyte nSbyte = -42;
            byte nByte = 200;
            short nShort = 1024;
            ushort nUshort = 2048;
            int nInt = -48151623;
            uint nUint = 1234512345;
            long nLong = -8589934592;
            ulong nUlong = 67553994410557440;
            float nFloat = 3.141f;
            double nDouble = 6.282;

            CollectionAssert.AreEqual(nBool.ToBinString().ToArray(), BitConverter.GetBytes(nBool));
            CollectionAssert.AreEqual(nChar.ToBinString().ToArray(), BitConverter.GetBytes(nChar));
            CollectionAssert.AreEqual(nByte.ToBinString().ToArray(), BitConverter.GetBytes(nByte));
            CollectionAssert.AreEqual(nSbyte.ToBinString().ToArray(), BitConverter.GetBytes(nSbyte));
            CollectionAssert.AreEqual(nShort.ToBinString().ToArray(), BitConverter.GetBytes(nShort));
            CollectionAssert.AreEqual(nUshort.ToBinString().ToArray(), BitConverter.GetBytes(nUshort));
            CollectionAssert.AreEqual(nInt.ToBinString().ToArray(), BitConverter.GetBytes(nInt));
            CollectionAssert.AreEqual(nUint.ToBinString().ToArray(), BitConverter.GetBytes(nUint));
            CollectionAssert.AreEqual(nLong.ToBinString().ToArray(), BitConverter.GetBytes(nLong));
            CollectionAssert.AreEqual(nUlong.ToBinString().ToArray(), BitConverter.GetBytes(nUlong));
            CollectionAssert.AreEqual(nFloat.ToBinString().ToArray(), BitConverter.GetBytes(nFloat));
            CollectionAssert.AreEqual(nDouble.ToBinString().ToArray(), BitConverter.GetBytes(nDouble));
        }

        [TestMethod]
        public void OtherToBinStringTest()
        {
            string myString = "mañana";
            Assert.AreEqual(BinString.FromTextString(myString, Encoding.UTF8), myString.ToBinString(Encoding.UTF8));

            var myArr = new byte[] { 10, 20, 150, 40, 160, 50, 120, 180, 200 };
            Assert.AreEqual(new BinString(myArr), myArr.ToBinString());

            var myMemoryStream = new MemoryStream();
            myMemoryStream.Write(myArr, 0, myArr.Length);
            Assert.AreEqual(new BinString(myArr), myArr.ToBinString());

        }

        [TestMethod]
        public void StreamWriteTest()
        {
            var myData = BinString.FromBytes("C0FFEE123456");
            using (var ms = new MemoryStream())
            {
                ms.Write(myData);
                Assert.AreEqual(myData, ms.ToArray().ToBinString());
            }
        }

        [TestMethod]
        public void StreamReadTest()
        {
            var source = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse varius magna dictum leo vehicula maximus.");
            var sourceBinString = source.ToBinString();

            using (var ms1 = new MemoryStream(source))
            {
                Assert.AreEqual(sourceBinString, ms1.ReadBinString(2048));
            }

            using (var ms2 = new MemoryStream(source))
            {
                Assert.AreEqual(sourceBinString.Remove(32), ms2.ReadBinString(32));
            }
        }

        [TestMethod]
        public void RandomTest()
        {
            var nonSecure = new Random();
            var secure = System.Security.Cryptography.RandomNumberGenerator.Create();

            Assert.AreEqual(32, nonSecure.NextBinString(32).Length);
            Assert.AreEqual(48, nonSecure.NextBinString(48).Length);
        }
    }
}
