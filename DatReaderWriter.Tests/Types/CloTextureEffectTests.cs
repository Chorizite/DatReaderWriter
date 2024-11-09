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
    public class CloTextureEffectTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CloTextureEffect() {
                OldTexture = 0x12345678,
                NewTexture = 0x87654321,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CloTextureEffect();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.OldTexture, readObj.OldTexture);
            Assert.AreEqual(writeObj.NewTexture, readObj.NewTexture);
        }
    }
}
