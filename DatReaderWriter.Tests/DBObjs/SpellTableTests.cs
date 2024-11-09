using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using System.Text;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SpellTableTests {
        [TestMethod]
        public void CanInsertAndReadSpellTables() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeSpellTable = new SpellTable() {
                Id = 0x0E00000E,
                Spells = new Dictionary<uint, SpellBase>() {
                    { 1, new SpellBase() {
                        Name = "Test Spell",
                        Description = "Test Spell Description",
                        Components = [1, 2, 3, 4, 5, 6, 7, 8]
                    } },
                    { 2, new SpellBase() {
                        Name = "Test Spell 2",
                        Description = "Test Spell 2 Description",
                        Components = [8, 7, 6, 5, 4, 3, 2, 1]
                    } }
                }
            };

            var res = dat.TryWriteFile(writeSpellTable);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<SpellTable>(0x0E00000E, out var readSpellTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readSpellTable);

            Assert.AreEqual(0x0E00000Eu, readSpellTable.Id);

            Assert.AreEqual(2, readSpellTable.Spells.Count);
            Assert.AreEqual("Test Spell", readSpellTable.Spells[1].Name);
            Assert.AreEqual("Test Spell 2", readSpellTable.Spells[2].Name);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSpellTable() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<SpellTable>(0x0E00000E, out var spellTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(spellTable);
            Assert.AreEqual(0x0E00000Eu, spellTable.Id);

            Assert.AreEqual(6266, spellTable.Spells.Count);

            Assert.AreEqual("Strength Other I", spellTable.Spells[1].Name);
            Assert.AreEqual("Increases the target's Strength by 10 points.", spellTable.Spells[1].Description);
            Assert.AreEqual(10412u, spellTable.Spells[1].DisplayOrder);

            Assert.AreEqual("Gauntlet Vitality III", spellTable.Spells[6340].Name);
            Assert.AreEqual("Your vitality is increased by 3.", spellTable.Spells[6340].Description);
            Assert.AreEqual(3850u, spellTable.Spells[6340].DisplayOrder);
            Assert.AreEqual(6340u, spellTable.Spells[6340].MetaSpellId);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSpellTableAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<SpellTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E00000E);
        }
    }
}
