using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;
using System.Text;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SpellComponentTableTests {
        [TestMethod]
        public void CanInsertAndReadSpellComponentTables() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
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
                        RawComponents = [1, 2, 3, 4, 5, 6, 7, 8]
                    } },
                    { 2, new SpellBase() {
                        Name = "Test Spell 2",
                        Description = "Test Spell 2 Description",
                        RawComponents = [8, 7, 6, 5, 4, 3, 2, 1]
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
    }
}
