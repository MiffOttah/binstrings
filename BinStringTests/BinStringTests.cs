using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

            CollectionAssert.AreEqual(byteArray, array2);
            Assert.AreEqual(input, Encoding.UTF8.GetString(binString));
        }

        [TestMethod]
        public void CreationFromByteArrayTest()
        {
            byte[] sourceArray = new byte[] { 1, 2, 3 };
            var str = new BinString(sourceArray);
            CollectionAssert.AreEqual(sourceArray, str.ToArray());

            // binstrings should be immutable
            sourceArray[1] = 200;
            CollectionAssert.AreNotEqual(sourceArray, str.ToArray());

            // creation from null array
            Assert.AreEqual(BinString.Empty, new BinString((byte[])null));
        }

        [TestMethod]
        public void CreationFromEnumerableTest()
        {
            var str = new BinString(Enumerable.Range(5, 20).Select(x => (byte)x));
            Assert.AreEqual(20, str.Length);
            Assert.AreEqual(5, str[0]);
            Assert.AreEqual(9, str[4]);
        }

        [TestMethod]
        public void CreationFromPartOfByteArray()
        {
            byte[] sourceArray = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var str = new BinString(sourceArray, 2, 4);

            Assert.AreEqual(4, str.Length);
            Assert.AreEqual(BinString.FromBytes(3, 4, 5, 6), str);

            Assert.AreEqual(BinString.Empty, new BinString(sourceArray, 2, 0));

            Assert.ThrowsException<ArgumentNullException>(() => new BinString(null, 2, 4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BinString(sourceArray, -2, 4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BinString(sourceArray, 100, 4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BinString(sourceArray, 2, -12));
            Assert.ThrowsException<ArgumentException>(() => new BinString(sourceArray, 2, 100));
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
            Assert.AreEqual(new BinString("¡Hola, señor!", Encoding.UTF8), new BinString("%C2%A1Hola%2C+se%C3%B1or!", BinaryTextEncoding.Url));
            Assert.AreEqual(new BinString("¡Hola, señor!", Encoding.UTF8), new BinString(@"\xC2\xA1Hola\x2C se\xC3\xB1or!", BinaryTextEncoding.BackslashEscape));

            var string1 = BinString.FromBytes("DEADBEEF00C0FFEE255C");
            Assert.AreEqual(string1, new BinString(string1.ToString("u"), BinaryTextEncoding.Url));
            Assert.AreEqual(string1, new BinString(string1.ToString("U"), BinaryTextEncoding.Url));
            Assert.AreEqual(string1, new BinString(string1.ToString("e"), BinaryTextEncoding.BackslashEscape));
            Assert.AreEqual(string1, new BinString(string1.ToString("E"), BinaryTextEncoding.BackslashEscape));

            Assert.ThrowsException<ArgumentNullException>(() => BinaryTextEncoding.Url.GetBinString(null));
            Assert.AreEqual(BinString.Empty, new BinString(null, BinaryTextEncoding.Url));

            Assert.ThrowsException<ArgumentException>(() => new BinString("99% undone", BinaryTextEncoding.Url));
            Assert.ThrowsException<ArgumentException>(() => new BinString("giving 110%", BinaryTextEncoding.Url));
            Assert.ThrowsException<ArgumentException>(() => new BinString("C:\\", BinaryTextEncoding.BackslashEscape));
            Assert.ThrowsException<ArgumentException>(() => new BinString("C:\\WINDOWS", BinaryTextEncoding.BackslashEscape));
            Assert.ThrowsException<ArgumentException>(() => new BinString("C:\\XZZZZZZ", BinaryTextEncoding.BackslashEscape));
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

            Assert.AreEqual(0, BinString.Compare(stringA, stringA));
            Assert.AreEqual(0, BinString.Compare(null, null));
            Assert.IsTrue(BinString.Compare(stringA, stringB) < 0);
            Assert.IsTrue(BinString.Compare(stringB, null) > 0);
            Assert.IsTrue(BinString.Compare(stringB, stringA) > 0);
            Assert.IsTrue(BinString.Compare(null, stringA) < 0);
        }

        [TestMethod]
        public void HashTest()
        {
            var stringA = new BinString("ABCDE", Encoding.ASCII);
            var stringB = BinString.FromBytes(65, 66, 67, 68, 69);

            Assert.IsTrue(stringA == stringB);
            Assert.AreEqual(stringA.GetHashCode(), stringB.GetHashCode());

            var stringC = new BinString("EDCBA", Encoding.ASCII);
            Assert.IsTrue(stringA != stringC);
            Assert.AreNotEqual(stringA.GetHashCode(), stringC.GetHashCode());
        }

        [TestMethod]
        public void ToStringTest()
        {
            var input = new BinString("Input", Encoding.ASCII);

            Assert.AreEqual("496e707574", input.ToString());
            Assert.AreEqual("496e707574", input.ToString("x"));
            Assert.AreEqual("496E707574", input.ToString("X"));
            Assert.AreEqual("49 6e 70 75 74", input.ToString("s"));
            Assert.AreEqual("49 6E 70 75 74", input.ToString("S"));
            Assert.AreEqual("SW5wdXQ=", input.ToString("64"));
            Assert.AreEqual("(49.6e.70.75.74)", $"({input:x.})");

            Assert.AreEqual("Ping%C3%BCino", new BinString("Pingüino", Encoding.UTF8).ToString("U"));
            Assert.AreEqual("Caf%c3%a9", new BinString("Café", Encoding.UTF8).ToString("u"));
            Assert.AreEqual("20%25%20cooler", new BinString("20% cooler", Encoding.UTF8).ToString("U"));
            Assert.AreEqual("\\\"Pi\\xC3\\xB1ata\\\"", new BinString("\"Piñata\"", Encoding.UTF8).ToString("E"));
            Assert.AreEqual("Backslash\\\\Apos\\'", new BinString("Backslash\\Apos'", Encoding.UTF8).ToString("E"));
        }

        [TestMethod]
        public void CatenationTest()
        {
            var foo = BinString.FromBytes(1, 2, 3);
            var bar = BinString.FromBytes(4, 5, 6);

            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 4, 5, 6), BinString.Concat(foo, bar));
            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 4, 5, 6), foo + bar);
            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 255), foo + 255);
            Assert.AreEqual(BinString.FromBytes(128, 1, 2, 3), 128 + foo);
            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 4, 5, 6, 1, 2, 3), BinString.Concat(foo, bar, foo));
            Assert.AreEqual(BinString.FromBytes(1, 2, 3, 4, 5, 6, 1, 2, 3), BinString.Concat(foo, BinString.Empty, bar, null, foo));

            var baz1 = BinString.Join((new string[] { "Lorem", "Ipsum", "Dolor", "Sit", "Amet" }).Select(_ => new BinString(_, Encoding.ASCII)), (BinString)0x20);
            Assert.AreEqual("Lorem Ipsum Dolor Sit Amet", baz1.ToString(Encoding.ASCII));

            var baz2 = BinString.Join((new string[] { "Aenean", "Porttitor", "Dictumst" }).Select(_ => new BinString(_, Encoding.ASCII)));
            Assert.AreEqual("AeneanPorttitorDictumst", baz2.ToString(Encoding.ASCII));

            Assert.AreEqual(foo, foo + (BinString)null);
            Assert.AreEqual(foo, (BinString)null + foo);

            Assert.AreEqual(foo, foo + (byte[])null);
            Assert.AreEqual(foo, (byte[])null + foo);
        }

        [TestMethod]
        public void RepeatTest()
        {
            var shortStr = new BinString("Test", Encoding.ASCII);
            var longStr = new BinString("TestTestTestTest", Encoding.ASCII);

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
            var input = new BinString("ABCD", Encoding.ASCII);
            var anotherString = new BinString("xyz", Encoding.ASCII);

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
            var input = new BinString("ABCD", Encoding.ASCII);

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

            Assert.AreEqual(input, input.Range(0, 4));
            Assert.AreEqual("AB", input.Range(0, 2).ToString(Encoding.ASCII));
            Assert.AreEqual("B", input.Range(1, 2).ToString(Encoding.ASCII));
            Assert.AreEqual("ABC", input.Range(0, 3).ToString(Encoding.ASCII));
            Assert.AreEqual("BCD", input.Range(1, 4).ToString(Encoding.ASCII));
            Assert.AreEqual("", input.Range(1, 1).ToString(Encoding.ASCII));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Range(0, 6));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Range(7, 10));
            Assert.ThrowsException<IndexOutOfRangeException>(() => input.Range(-4, 2));
            Assert.ThrowsException<ArgumentException>(() => input.Range(3, 0));
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

            Assert.ThrowsException<ArgumentException>(() => repeatingData.Split(new BinString()));
        }

        [TestMethod]
        [DataRow("00123400123400", "0001000100", "01", "1234")]
        [DataRow("5544", "557777", "7777", "44")]
        [DataRow("1100", "000000", "0000", "11")]
        [DataRow("AB00", "ABCDEF00", "CDEF", "")]
        [DataRow("AB00", "ABCDEF00", "CDEF", null)]
        [DataRow("", "", "CDEF", "ABCD")]
        [DataRow("1234AB", "1234AB", "", "FFFF")]
        [DataRow("1234AB", "1234AB", null, "FFFF")]
        [DataRow("1234AB", "1234AB", null, null)]
        [DataRow("0000", "AA0000", "AA", null)]
        [DataRow("0000CCDD", "0000AA", "AA", "CCDD")]
        public void ReplaceTest(string expected, string haystack, string needle, string replacement)
        {
            BinString bHaystack = BinString.FromBytes(haystack);
            BinString bNeedle = needle == null ? null : BinString.FromBytes(needle);
            BinString bReplacement = replacement == null ? null : BinString.FromBytes(replacement);

            Assert.AreEqual(BinString.FromBytes(expected), bHaystack.Replace(bNeedle, bReplacement));
        }

        [TestMethod]
        public void IsNullOrEmptyTest()
        {
            BinString bsNull = null;
            BinString bsEmpty = new BinString();
            BinString bsNotEmpty = new BinString("Hello", Encoding.UTF8);

            Assert.IsTrue(BinString.IsNullOrEmpty(bsNull));
            Assert.IsTrue(BinString.IsNullOrEmpty(bsEmpty));
            Assert.IsFalse(BinString.IsNullOrEmpty(bsNotEmpty));
        }

        [TestMethod]
        public void BuilderTest()
        {
            var builder = new BinStringBuilder();
            builder.Append(new BinString("Hello", Encoding.ASCII));
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
            var fooConvertible = (IConvertible)foo;

            Assert.AreEqual(2, foo.Length);
            Assert.AreEqual(MY_USHORT, fooConvertible.ToUInt16(CultureInfo.InvariantCulture));
            Assert.AreEqual(1024, fooConvertible.ToInt32(CultureInfo.InvariantCulture));

            Assert.AreEqual('\0', ((IConvertible)BinString.FromBytes(0)).ToChar(CultureInfo.InvariantCulture));
            Assert.AreEqual('\0', ((IConvertible)BinString.FromBytes()).ToChar(CultureInfo.InvariantCulture));
            Assert.AreEqual('\t', ((IConvertible)BinString.FromBytes(0x9)).ToChar(CultureInfo.InvariantCulture));
            Assert.AreEqual('$', ((IConvertible)BinString.FromBytes(0x24)).ToChar(CultureInfo.InvariantCulture));
            Assert.ThrowsException<OverflowException>(() => ((IConvertible)BinString.FromBytes(0xff)).ToChar(CultureInfo.InvariantCulture));
            Assert.ThrowsException<OverflowException>(() => ((IConvertible)BinString.FromBytes(0x31, 0x32)).ToChar(CultureInfo.InvariantCulture));

            Assert.AreEqual((sbyte)-1, ((IConvertible)BinString.FromBytes(0xff)).ToSByte(CultureInfo.InvariantCulture));
            Assert.AreEqual((byte)0xff, ((IConvertible)BinString.FromBytes(0xff)).ToByte(CultureInfo.InvariantCulture));
            Assert.ThrowsException<InvalidCastException>(() => fooConvertible.ToDecimal(CultureInfo.InvariantCulture));
            Assert.ThrowsException<InvalidCastException>(() => fooConvertible.ToDateTime(CultureInfo.InvariantCulture));

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

        [TestMethod]
        public void EndinessConversionTests()
        {
            var b = new BinString("Hello World", Encoding.ASCII);
            Assert.AreEqual(b, b.ConvertEndianess(IntegerEndianess.BigEndian, IntegerEndianess.BigEndian));
            Assert.AreEqual(b, b.ConvertEndianess(IntegerEndianess.Native, IntegerEndianess.Native));
            CollectionAssert.AreEqual(b.Reverse().ToArray(), b.ConvertEndianess(IntegerEndianess.LittleEndian, IntegerEndianess.BigEndian).ToArray());

            Assert.AreEqual(BinString.FromBytes("0000abcd"), BinString.FromInt32(0xabcd, IntegerEndianess.BigEndian));
            Assert.AreEqual(BinString.FromBytes("cdab0000"), BinString.FromInt32(0xabcd, IntegerEndianess.LittleEndian));
        }

        [TestMethod]
        public void StreamConversionTest()
        {
            var b = new BinString("Hello, world.", Encoding.ASCII);

            // create from memory stream
            using (var ms = new MemoryStream(b))
            {
                Assert.AreEqual(b, BinString.FromStream(ms));
            }

            // create from unknown stream
            using (var ms = new Mocks.MockStream(b))
            {
                Assert.AreEqual(b, BinString.FromStream(ms));
            }

            // create stream from binstring
            using (var bs = b.ToStream())
            {
                byte[] buffer = new byte[64];
                int read = bs.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(b, new BinString(buffer, 0, read));
            }
        }
    }
}
