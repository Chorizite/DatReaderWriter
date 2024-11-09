using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class ExperienceTableTests {
        [TestMethod]
        public void CanInsertAndReadXPTable() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var xpTable = new ExperienceTable() {
                Id = 0xE000018,
                Attributes = [1, 2, 3, 4],
                Vitals = [100, 200, 300, 400],
                TrainedSkills = [1000, 2000, 3000, 4000],
                SpecializedSkills = [10000, 20000, 30000, 40000],
                Levels = [5,6,7,8],
                SkillCredits = [9, 10, 11, 12]
            };

            var res = dat.TryWriteFile(xpTable);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<ExperienceTable>(0xE000018, out var readXPTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readXPTable);
            Assert.AreEqual(0xE000018u, readXPTable.Id);


            CollectionAssert.AreEqual(new uint[4] { 1, 2, 3, 4 }, readXPTable.Attributes);
            CollectionAssert.AreEqual(new uint[4] { 100, 200, 300, 400 }, readXPTable.Vitals);
            CollectionAssert.AreEqual(new uint[4] { 1000, 2000, 3000, 4000 }, readXPTable.TrainedSkills);
            CollectionAssert.AreEqual(new uint[4] { 10000, 20000, 30000, 40000 }, readXPTable.SpecializedSkills);
            CollectionAssert.AreEqual(new ulong[4] { 5, 6, 7, 8 }, readXPTable.Levels);
            CollectionAssert.AreEqual(new uint[4] { 9, 10, 11, 12 }, readXPTable.SkillCredits);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORXPTables() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });

            var res = dat.TryReadFile<ExperienceTable>(0xE000018, out var xpTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(xpTable);

            Assert.AreEqual(0u, xpTable.DataCategory);

            Assert.AreEqual(191, xpTable.Attributes.Length);
            Assert.AreEqual(0u, xpTable.Attributes.First());
            Assert.AreEqual(4019438644u, xpTable.Attributes.Last());

            Assert.AreEqual(197, xpTable.Vitals.Length);
            Assert.AreEqual(0u, xpTable.Vitals.First());
            Assert.AreEqual(4285430197u, xpTable.Vitals.Last());

            Assert.AreEqual(209, xpTable.TrainedSkills.Length);
            Assert.AreEqual(0u, xpTable.TrainedSkills.First());
            Assert.AreEqual(4203819496u, xpTable.TrainedSkills.Last());

            Assert.AreEqual(227, xpTable.SpecializedSkills.Length);
            Assert.AreEqual(0u, xpTable.SpecializedSkills.First());
            Assert.AreEqual(4100490438u, xpTable.SpecializedSkills.Last());

            Assert.AreEqual(276, xpTable.Levels.Length);
            Assert.AreEqual(0u, xpTable.Levels.First());
            Assert.AreEqual(0u, xpTable.Levels.Skip(1).First());
            Assert.AreEqual(1000u, xpTable.Levels.Skip(2).First());
            Assert.AreEqual(191226310247u, xpTable.Levels.Last());

            Assert.AreEqual(276, xpTable.SkillCredits.Length);
            Assert.AreEqual(0u, xpTable.SkillCredits.First());
            Assert.AreEqual(0u, xpTable.SkillCredits.Skip(1).First());
            Assert.AreEqual(1u, xpTable.SkillCredits.Last());

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ExperienceTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE000018);
        }
    }
}
