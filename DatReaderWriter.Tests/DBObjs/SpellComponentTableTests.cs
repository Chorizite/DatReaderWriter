using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using System.Text;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SpellComponentTableTests {
        [TestMethod]
        public void CanInsertAndReadSpellComponentTables() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeSpellTable = new SpellComponentTable() {
                Id = 0x0E00000F,
                Components = new Dictionary<uint, SpellComponentBase>() {
                    { 1, new SpellComponentBase() {
                        Name = "Test Spell",
                        Category = 1,
                        Text = "Test Spell Description",
                    } }
                }
            };

            var res = dat.TryWriteFile(writeSpellTable);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<SpellComponentTable>(0x0E00000F, out var readSpellTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readSpellTable);

            Assert.AreEqual(0x0E00000Fu, readSpellTable.Id);

            Assert.AreEqual(1, readSpellTable.Components.Count);
            Assert.AreEqual("Test Spell", readSpellTable.Components[1].Name);
            Assert.AreEqual("Test Spell Description", readSpellTable.Components[1].Text);
            Assert.AreEqual(1u, readSpellTable.Components[1].Category);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSpellComponentTable() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryGet<SpellComponentTable>(0x0E00000F, out var spellTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(spellTable);
            Assert.AreEqual(0x0E00000Fu, spellTable.Id);

            Assert.AreEqual(163, spellTable.Components.Count);

            Assert.AreEqual("Lead Scarab", spellTable.Components[1].Name);
            Assert.AreEqual(ComponentType.Scarab, spellTable.Components[1].Type);
            Assert.AreEqual(0x060013E7u, spellTable.Components[1].Icon);

            Assert.AreEqual("Essence of Kemeroi", spellTable.Components[198].Name);
            Assert.AreEqual(ComponentType.Potion, spellTable.Components[198].Type);
            Assert.AreEqual(0x06006E33u, spellTable.Components[198].Icon);


            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<SpellComponentTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E00000Fu);
        }
    }
}
