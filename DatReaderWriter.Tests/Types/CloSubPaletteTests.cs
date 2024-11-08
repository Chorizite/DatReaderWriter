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
    public class CloSubPaletteTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CloSubPalette() {
                PaletteSet = 0x12345678,
                Ranges = new List<CloSubPaletteRange>() {
                    new CloSubPaletteRange()
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CloSubPalette();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.PaletteSet, readObj.PaletteSet);
            Assert.AreEqual(writeObj.Ranges.Count, readObj.Ranges.Count);
        }
    }
}
