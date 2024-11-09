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
    public class ClothingBaseEffectTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new ClothingBaseEffect() {
                CloObjectEffects = new List<CloObjectEffect>() {
                    new CloObjectEffect(),
                    new CloObjectEffect()
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new ClothingBaseEffect();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.CloObjectEffects.Count, readObj.CloObjectEffects.Count);
        }
    }
}
