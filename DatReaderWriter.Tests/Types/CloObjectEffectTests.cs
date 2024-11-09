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
    public class CloObjectEffectTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CloObjectEffect() {
                Index = 2,
                ModelId = 0x12345678,
                CloTextureEffects = new List<CloTextureEffect>() {
                    new CloTextureEffect(),
                    new CloTextureEffect()
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CloObjectEffect();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Index, readObj.Index);
            Assert.AreEqual(writeObj.ModelId, readObj.ModelId);
            Assert.AreEqual(writeObj.CloTextureEffects.Count, readObj.CloTextureEffects.Count);
        }
    }
}
