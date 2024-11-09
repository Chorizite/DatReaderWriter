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
    public class CloSubPalEffectTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CloSubPalEffect() {
                Icon = 0x12345678,
                CloSubPalettes = new List<CloSubPalette>() {
                    new CloSubPalette(),
                    new CloSubPalette(),
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CloSubPalEffect();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Icon, readObj.Icon);
            Assert.AreEqual(writeObj.CloSubPalettes.Count, readObj.CloSubPalettes.Count);
        }
    }
}
