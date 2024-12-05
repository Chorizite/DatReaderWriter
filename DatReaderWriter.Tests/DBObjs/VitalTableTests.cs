using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class VitalTableTests {
        [TestMethod]
        public void CanInsertAndReadAnimations() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new VitalTable() {
                Id = 0xE000003,
                Health = new SkillFormula() {
                    Attribute1 = AttributeId.Quickness
                },
                Mana = new SkillFormula() {
                    Attribute1 = AttributeId.Strength
                },
                Stamina = new SkillFormula() {
                    Attribute1 = AttributeId.Focus
                }
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<VitalTable>(0xE000003, out var readVitalTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readVitalTable);

            Assert.AreEqual(0xE000003u, readVitalTable.Id);

            Assert.AreEqual(writeAnim.Health.Attribute1, readVitalTable.Health.Attribute1);
            Assert.AreEqual(writeAnim.Mana.Attribute1, readVitalTable.Mana.Attribute1);
            Assert.AreEqual(writeAnim.Stamina.Attribute1, readVitalTable.Stamina.Attribute1);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORVitalTables() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<VitalTable>(0xE000003, out var vitalTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(vitalTable);
            Assert.AreEqual(0xE000003u, vitalTable.Id);

            Assert.AreEqual(AttributeId.Endurance, vitalTable.Health.Attribute1);
            Assert.AreEqual((AttributeId)0, vitalTable.Health.Attribute2);
            Assert.AreEqual(2, vitalTable.Health.Divisor);

            Assert.AreEqual(AttributeId.Self, vitalTable.Mana.Attribute1);
            Assert.AreEqual((AttributeId)0, vitalTable.Mana.Attribute2);
            Assert.AreEqual(1, vitalTable.Mana.Divisor);

            Assert.AreEqual(AttributeId.Endurance, vitalTable.Stamina.Attribute1);
            Assert.AreEqual((AttributeId)0, vitalTable.Stamina.Attribute2);
            Assert.AreEqual(1, vitalTable.Stamina.Divisor);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<VitalTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE000003u);
        }
    }
}
