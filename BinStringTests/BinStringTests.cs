using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Linq;
using System.Text;

namespace BinStringTests
{
    [TestClass]
    public class BinStringTests
    {
        [TestMethod]
        public void TestBasicDeclaration()
        {
            var hello = Encoding.UTF8.GetBytes("Hello, world!");
            var helloBin = new BinString(hello);

            CollectionAssert.AreEqual(hello, helloBin.ToArray());
        }

        [TestMethod]
        public void IterationTest()
        {
            int i;
            var hello = Encoding.UTF8.GetBytes("This is some test data.");
            var helloBin = new BinString(hello);

            Assert.AreEqual(hello.Length, helloBin.Length);
            CollectionAssert.AreEqual(hello, helloBin.ToList());

            for (i = 0; i < helloBin.Length; i++)
            {
                Assert.AreEqual(hello[i], helloBin[i]);
            }

            i = 0;
            foreach (var b in helloBin)
            {
                Assert.AreEqual(hello[i], b);
                i++;
            }
        }

        [TestMethod]
        public void CastTest()
        {
            string input = "Ut et elementum diam.";
            var byteArray = Encoding.UTF8.GetBytes(input);
            var binString = (BinString)byteArray;
            var array2 = (byte[])binString;

            Assert.AreEqual(byteArray, array2);
            Assert.AreEqual(input, Encoding.UTF8.GetString(binString));
        }

        [TestMethod]
        public void ComparisonTest()
        {
            var stringA = BinString.FromBytes(1, 2, 3, 4, 5);
            var stringB = BinString.FromBytes(1, 2, 3, 4, 5);
            var stringC = BinString.FromBytes(2, 1, 4, 5, 3);

            Assert.IsTrue(stringA.Equals(stringB));
            Assert.IsFalse(stringA.Equals(stringC));

            Assert.IsTrue(stringA == stringB);
            Assert.IsTrue(stringB == stringA);
            Assert.IsTrue(stringB != stringC);
            Assert.IsTrue(stringC != stringB);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var input = BinString.FromTextString("Input", Encoding.ASCII);

            Assert.AreEqual("496e707574", input.ToString());
            Assert.AreEqual("496e707574", input.ToString("x"));
            Assert.AreEqual("496E707574", input.ToString("X"));
            Assert.AreEqual("49 6e 70 75 74", input.ToString("s"));
            Assert.AreEqual("49 6E 70 75 74", input.ToString("S"));
            Assert.AreEqual("SW5wdXQ=", input.ToString("64"));
            Assert.AreEqual("(49.6e.70.75.74)", $"({input:x.})");
        }

        [TestMethod]
        public void CatenationTest()
        {
            var foo = BinString.FromBytes(1, 2, 3);
            var bar = BinString.FromBytes(4, 5, 6);

            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 4, 5, 6), foo + bar);
            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 255), foo + 255);
            Assert.AreEqual(BinString.FromBytes(128, 1, 2, 3), 128 + foo);
        }

        [TestMethod]
        public void RepeatTest()
        {
            var shortStr = BinString.FromTextString("Test", Encoding.ASCII);
            var longStr = BinString.FromTextString("TestTestTestTest", Encoding.ASCII);

            var repeated = shortStr * 4;

            Assert.AreEqual(longStr, repeated);
            Assert.AreEqual("TestTestTestTest", repeated.ToString(Encoding.ASCII));

            Assert.AreEqual("ZZZZZZZZZZ", BinString.FromBytes(0x5a).Repeat(10).ToString(Encoding.ASCII));

            Assert.AreEqual(new BinString(), shortStr * 0);
            Assert.ThrowsException<ArgumentException>(() => shortStr.Repeat(-1));
        }

        [TestMethod]
        public void ModificationTest()
        {
            var input = BinString.FromTextString("ABCD", Encoding.ASCII);
            var anotherString = BinString.FromTextString("xyz", Encoding.ASCII);

            Assert.AreEqual("AB~CD", input.Insert(2, 0x7e).ToString(Encoding.ASCII));
            Assert.AreEqual("ABxyzCD", input.Insert(2, anotherString).ToString(Encoding.ASCII));
            Assert.AreEqual("~ABCD", input.Insert(0, 0x7e).ToString(Encoding.ASCII));
            Assert.AreEqual("ABCD~", input.Insert(4, 0x7e).ToString(Encoding.ASCII));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Insert(7, anotherString));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Insert(-4, anotherString));

            Assert.AreEqual(10, input.PadRight(10).Length);
            Assert.AreEqual(10, input.PadLeft(10).Length);
            Assert.AreEqual("ABCD++++", input.PadRight(8, 0x2b).ToString(Encoding.ASCII));
            Assert.AreEqual("%%%%%%ABCD", input.PadLeft(10, 0x25).ToString(Encoding.ASCII));
            Assert.AreEqual((input + 0) + 0, input.PadRight(6));
            Assert.AreEqual(input, input.PadLeft(3));
            Assert.ThrowsException<ArgumentException>(() => input.PadLeft(0));
            Assert.ThrowsException<ArgumentException>(() => input.PadRight(-10));
        }

        [TestMethod]
        public void SpliceTest()
        {
            var input = BinString.FromTextString("ABCD", Encoding.ASCII);

            Assert.AreEqual("ABD", input.Remove(2, 1).ToString(Encoding.ASCII));
            Assert.AreEqual("AB", input.Remove(2).ToString(Encoding.ASCII));
            Assert.AreEqual("AB", input.Remove(2, 5).ToString(Encoding.ASCII));
            Assert.AreEqual("AD", input.Remove(1, 2).ToString(Encoding.ASCII));
            Assert.AreEqual("CD", input.Remove(0, 2).ToString(Encoding.ASCII));
            Assert.AreEqual(input, input.Remove(2, 0));
            Assert.AreEqual(input, input.Remove(4));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Remove(6));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Remove(-1));

            Assert.AreEqual("BC", input.Substring(1, 2).ToString(Encoding.ASCII));
            Assert.AreEqual("BCD", input.Substring(1).ToString(Encoding.ASCII));
            Assert.AreEqual("BCD", input.Substring(1, 8).ToString(Encoding.ASCII));
            Assert.AreEqual(0, input.Substring(2, 0).Length);
            Assert.AreEqual(0, input.Substring(4).Length);
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Substring(6));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Substring(-1));
        }

        [TestMethod]
        public void TrimTest()
        {
            var center = BinString.FromBytes(1, 2, 0, 3, 4);
            var zeroes = BinString.FromBytes(0) * 5;
            var ffs = BinString.FromBytes(0xff) * 5;

            var zeroPadded = zeroes + center + zeroes;
            var ffPadded = ffs + center + ffs;

            Assert.AreEqual(center, zeroPadded.Trim());
            Assert.AreEqual(ffPadded, ffPadded.Trim());


            Assert.AreEqual(center, ffPadded.Trim(0xff));
            Assert.AreEqual(zeroPadded, zeroPadded.Trim(0xff));
        }
    }
}
