using DatReaderWriter.Enums;
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
    public class CloSubPaletteRangeTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CloSubPaletteRange() {
                NumColors = 123,
                Offset = 23
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CloSubPaletteRange();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.NumColors, readObj.NumColors);
            Assert.AreEqual(writeObj.Offset, readObj.Offset);
        }
    }
}
