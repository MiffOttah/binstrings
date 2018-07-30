using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BinStringTests
{
    [TestClass]
    public class BinStringTests
    {
        [TestMethod]
        public void DeclarationTest()
        {
            var hello = Encoding.UTF8.GetBytes("Hello, world!");
            var helloBin = new BinString(hello);

            CollectionAssert.AreEqual(hello, helloBin.ToArray());
            Assert.AreEqual(13, helloBin.Length);
            Assert.AreEqual(0, BinString.Empty.Length);
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
        public void CreationFromBytesTest()
        {
            Assert.AreEqual("C0FFEEFF", BinString.FromBytes(0xc0, 0xff, 0xee, 0xff).ToString("X"));
            Assert.AreEqual("DEADBEEF", BinString.FromBytes("DeadBeef").ToString("X"));
            Assert.AreEqual("pharetra", BinString.FromBase64String("cGhhcmV0cmE=").ToString(Encoding.ASCII));
        }

        [TestMethod]
        public void CreationFromStringTest()
        {
            Assert.AreEqual(BinString.FromTextString("¡Hola, señor!", Encoding.UTF8), BinString.FromUrlEncoding("%C2%A1Hola%2C+se%C3%B1or!"));

            var string1 = BinString.FromBytes("DEADBEEF00C0FFEE");
            Assert.AreEqual(string1, BinString.FromUrlEncoding(string1.ToString("u")));
            Assert.AreEqual(string1, BinString.FromUrlEncoding(string1.ToString("U")));
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

            Assert.IsTrue(stringA.Equals((object)stringB));
            Assert.IsFalse(stringA.Equals((object)null));
        }

        [TestMethod]
        public void ComparisonUnequalLengthTest()
        {
            var string2 = BinString.FromBytes(1, 2);
            var string3 = BinString.FromBytes(1, 2, 3);
            var string4 = BinString.FromBytes(1, 2, 3, 4);

            Assert.IsFalse(string4.Equals(string3));
            Assert.IsFalse(string2.Equals(string4));
            Assert.IsFalse(string3.Equals(null));

            Assert.IsTrue(string2 < string3);
            Assert.IsTrue(string2 <= string3);
            Assert.IsFalse(string2 == string3);
            Assert.IsTrue(string4 > string3);
            Assert.IsTrue(string4 >= string3);
            Assert.IsFalse(string4 == string3);

            Assert.IsFalse(string2 == null);
            Assert.IsFalse(null == string3);
            Assert.IsTrue(string4 != null);
            Assert.IsTrue(BinString.Empty != null);
        }

        [TestMethod]
        public void ComparableTest()
        {
            var stringA = BinString.FromBytes(5, 10, 15, 20);
            var stringB = BinString.FromBytes(5, 15, 25, 35);

            Assert.IsTrue(stringA.CompareTo(stringA) == 0);
            Assert.IsTrue(stringA.CompareTo(stringB) < 0);
            Assert.IsTrue(stringA.CompareTo((object)stringB) < 0);
            Assert.IsTrue(stringB.CompareTo(stringA) > 0);
            Assert.IsTrue(stringA.CompareTo(BinString.Empty) > 0);

            Assert.IsTrue(stringA > BinString.Empty);
            Assert.IsTrue(stringA > null);
            Assert.IsTrue(BinString.Empty > null);
            Assert.IsTrue(BinString.Empty != null);
            Assert.IsFalse(BinString.Empty < null);

            Assert.ThrowsException<ArgumentException>(() => stringA.CompareTo("TEST"));
            Assert.ThrowsException<ArgumentException>(() => stringA.CompareTo(ConsoleColor.Red));
        }

        [TestMethod]
        public void HashTest()
        {
            var stringA = BinString.FromTextString("ABCDE", Encoding.ASCII);
            var stringB = BinString.FromBytes(65, 66, 67, 68, 69);

            Assert.IsTrue(stringA == stringB);
            Assert.AreEqual(stringA.GetHashCode(), stringB.GetHashCode());

            var stringC = BinString.FromTextString("EDCBA", Encoding.ASCII);
            Assert.IsTrue(stringA != stringC);
            Assert.AreNotEqual(stringA.GetHashCode(), stringC.GetHashCode());
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

            Assert.AreEqual("Ping%C3%BCino", BinString.FromTextString("Pingüino", Encoding.UTF8).ToString("U"));
            Assert.AreEqual("Caf%c3%a9", BinString.FromTextString("Café", Encoding.UTF8).ToString("u"));
            Assert.AreEqual("\\\"Pi\\xC3\\xB1ata\\\"", BinString.FromTextString("\"Piñata\"", Encoding.UTF8).ToString("E"));
        }

        [TestMethod]
        public void CatenationTest()
        {
            var foo = BinString.FromBytes(1, 2, 3);
            var bar = BinString.FromBytes(4, 5, 6);

            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 4, 5, 6), foo + bar);
            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 255), foo + 255);
            Assert.AreEqual(BinString.FromBytes(128, 1, 2, 3), 128 + foo);

            var baz1 = BinString.Join((new string[] { "Lorem", "Ipsum", "Dolor", "Sit", "Amet" }).Select(_ => BinString.FromTextString(_, Encoding.ASCII)), (BinString)0x20);
            Assert.AreEqual("Lorem Ipsum Dolor Sit Amet", baz1.ToString(Encoding.ASCII));

            var baz2 = BinString.Join((new string[] { "Aenean", "Porttitor", "Dictumst" }).Select(_ => BinString.FromTextString(_, Encoding.ASCII)));
            Assert.AreEqual("AeneanPorttitorDictumst", baz2.ToString(Encoding.ASCII));
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

            Assert.AreEqual(center + zeroes, zeroPadded.TrimLeft());
            Assert.AreEqual(ffs + center, ffPadded.TrimRight(0xff));
        }

        [TestMethod]
        public void SearchTest()
        {
            var someData = BinString.FromBytes(10, 20, 30, 40, 50, 60, 70, 80, 90, 100);
            var repeatingData = BinString.FromBytes(10, 20, 30, 40, 10, 20, 30, 50, 80);

            Assert.AreEqual(0, someData.IndexOf(10));
            Assert.AreEqual(2, someData.IndexOf(30));
            Assert.AreEqual(-1, someData.IndexOf(55));

            Assert.AreEqual(2, someData.IndexOf(BinString.FromBytes(30, 40)));
            Assert.AreEqual(5, someData.IndexOf(BinString.FromBytes(60, 70, 80)));
            Assert.AreEqual(-1, someData.IndexOf(BinString.FromBytes(4, 8, 15, 16, 23, 42)));
            Assert.AreEqual(-1, someData.IndexOf(BinString.FromBytes(80, 90, 100, 110)));
            Assert.AreEqual(0, someData.IndexOf(new BinString()));

            Assert.AreEqual(0, repeatingData.IndexOf(BinString.FromBytes(10, 20, 30)));
            Assert.AreEqual(4, repeatingData.IndexOf(BinString.FromBytes(10, 20, 30, 50)));

            Assert.AreEqual(BinString.FromBytes(200, 210, 40, 200, 210, 50, 80), repeatingData.Replace(BinString.FromBytes(10, 20, 30), BinString.FromBytes(200, 210)));

            var sections = new BinString[] {
                BinString.FromBytes(10),
                BinString.FromBytes(40, 10),
                BinString.FromBytes(50, 80)
            };
            var split = repeatingData.Split(BinString.FromBytes(20, 30));
            CollectionAssert.AreEqual(sections, split);

            var notSplit = someData.Split((BinString)0xff);
            Assert.AreEqual(1, notSplit.Length);
            Assert.AreEqual(someData, notSplit[0]);

            Assert.ThrowsException<InvalidOperationException>(() => repeatingData.Split(new BinString()));
        }

        [TestMethod]
        public void IsNullOrEmptyTest()
        {
            BinString bsNull = null;
            BinString bsEmpty = new BinString();
            BinString bsNotEmpty = BinString.FromTextString("Hello", Encoding.UTF8);

            Assert.IsTrue(BinString.IsNullOrEmpty(bsNull));
            Assert.IsTrue(BinString.IsNullOrEmpty(bsEmpty));
            Assert.IsFalse(BinString.IsNullOrEmpty(bsNotEmpty));
        }

        [TestMethod]
        public void BuilderTest()
        {
            var builder = new BinStringBuilder();
            builder.Append(BinString.FromTextString("Hello", Encoding.ASCII));
            builder.Append(0x2c);
            builder.Append(0x20);
            builder.Append(Encoding.ASCII.GetBytes("world!"));

            Assert.AreEqual("Hello, world!", builder.ToBinString().ToString(Encoding.ASCII));
            Assert.AreEqual("48 65 6c 6c 6f 2c 20 77 6f 72 6c 64 21", builder.ToString("s"));
        }

        [TestMethod]
        public void SortTest()
        {
            var binStrings = new List<BinString>() {
                BinString.FromBytes(1, 2, 3),
                BinString.FromBytes(4, 5, 6),
                BinString.FromBytes(1, 2, 3),
                BinString.FromBytes(1, 2, 3, 20),
                BinString.FromBytes(1, 2, 2),
                BinString.FromBytes(10, 20, 30),
                BinString.FromBytes(1, 0, 0)
            };

            binStrings.Sort();

            Assert.AreEqual(BinString.FromBytes(1, 0, 0, 255, 1, 2, 2, 255, 1, 2, 3, 255, 1, 2, 3, 255, 1, 2, 3, 20, 255, 4, 5, 6, 255, 10, 20, 30), BinString.Join(binStrings, (BinString)255));
        }

        [TestMethod]
        public void ConversionTest()
        {
            const ushort MY_USHORT = 1024;
            var foo = new BinString(BitConverter.GetBytes(MY_USHORT));

            Assert.AreEqual(2, foo.Length);
            Assert.AreEqual(MY_USHORT, foo.ToUInt16(CultureInfo.InvariantCulture));
            Assert.AreEqual(1024, foo.ToInt32(CultureInfo.InvariantCulture));

            Assert.AreEqual('\0', BinString.FromBytes(0).ToChar(CultureInfo.InvariantCulture));
            Assert.AreEqual('\0', BinString.FromBytes().ToChar(CultureInfo.InvariantCulture));
            Assert.AreEqual('$', BinString.FromBytes(0x24).ToChar(CultureInfo.InvariantCulture));
            Assert.ThrowsException<OverflowException>(() => BinString.FromBytes(0xff).ToChar(CultureInfo.InvariantCulture));
            Assert.ThrowsException<OverflowException>(() => BinString.FromBytes(0x31, 0x32).ToChar(CultureInfo.InvariantCulture));

            Assert.AreEqual((sbyte)-1, BinString.FromBytes(0xff).ToSByte(CultureInfo.InvariantCulture));
            Assert.AreEqual((byte)0xff, BinString.FromBytes(0xff).ToByte(CultureInfo.InvariantCulture));
            Assert.ThrowsException<InvalidCastException>(() => foo.ToDecimal(CultureInfo.InvariantCulture));
            Assert.ThrowsException<InvalidCastException>(() => foo.ToDateTime(CultureInfo.InvariantCulture));

            Assert.AreEqual(foo.ToString(), Convert.ToString(foo));

            const float MY_SINGLE = 3.14f;
            const double MY_DOUBLE = 6.28;

            var mySingleBytes = new BinString(BitConverter.GetBytes(MY_SINGLE));
            var myDoubleBytes = new BinString(BitConverter.GetBytes(MY_DOUBLE));

            Assert.AreEqual(4, mySingleBytes.Length);
            Assert.AreEqual(8, myDoubleBytes.Length);
            Assert.AreEqual(MY_SINGLE, Convert.ToSingle(mySingleBytes));
            Assert.AreEqual(MY_DOUBLE, Convert.ToDouble(myDoubleBytes));
            Assert.ThrowsException<OverflowException>(() => Convert.ToDouble(mySingleBytes));
            Assert.ThrowsException<OverflowException>(() => Convert.ToSingle(myDoubleBytes));
        }
    }
}
