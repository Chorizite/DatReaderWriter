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
    public class SpellSetTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SpellSet() {
                SpellSetTiers = new() {
                    { 0, new SpellSetTiers() {
                        Spells = [1, 2, 3]
                    } },
                    { 1, new SpellSetTiers() {
                        Spells = [4, 5, 6]
                    } },
                    { 2, new SpellSetTiers() {
                        Spells = [7, 8, 9]
                    } }
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SpellSet();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            CollectionAssert.AreEqual(writeObj.SpellSetTiers[0].Spells, readObj.SpellSetTiers[0].Spells);
            CollectionAssert.AreEqual(writeObj.SpellSetTiers[1].Spells, readObj.SpellSetTiers[1].Spells);
            CollectionAssert.AreEqual(writeObj.SpellSetTiers[2].Spells, readObj.SpellSetTiers[2].Spells);
        }
    }
}
