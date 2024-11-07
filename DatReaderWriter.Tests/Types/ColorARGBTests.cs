using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class ColorARGBTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new ColorARGB() {
                Alpha = 100,
                Red = 200,
                Green = 150,
                Blue = 0
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new ColorARGB();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Alpha, readObj.Alpha);
            Assert.AreEqual(writeObj.Red, readObj.Red);
            Assert.AreEqual(writeObj.Green, readObj.Green);
            Assert.AreEqual(writeObj.Blue, readObj.Blue);
        }
    }
}
