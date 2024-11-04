using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class SpellSetTiersTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SpellSetTiers() {
                Spells = new List<uint>() {
                    0x12345678,
                    0x23456789,
                    0x34567890
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SpellSetTiers();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            CollectionAssert.AreEqual(writeObj.Spells, readObj.Spells);
        }
    }
}
