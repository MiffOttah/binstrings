using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStringTests
{
    [TestClass]
    public class BinStringTypeConvterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var conv = TypeDescriptor.GetConverter(typeof(BinString));

            var string1 = new BinString("Test\\x00\\\"thisis athing\\x1f\\xa7", BinaryTextEncoding.BackslashEscape);
            Assert.AreEqual(string1.ToBase64String(), conv.ConvertToString(string1));
            Assert.AreEqual(string1, conv.ConvertFromString(string1.ToBase64String()));
        }
    }
}
