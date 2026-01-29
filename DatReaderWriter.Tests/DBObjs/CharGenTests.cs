using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using Newtonsoft.Json;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class CharGenTests {
        [TestMethod]
        public void CanInsertAndReadChargenTable() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new CharGen() {
                Id = 0xE000002,
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<CharGen>(0xE000002, out var readAnim);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readAnim);

            Assert.AreEqual(0xE000002u, readAnim.Id);
            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORCharGenTable() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);


            var res = dat.TryGet<CharGen>(0xE000002, out var chargen);
            Assert.IsTrue(res);
            Assert.IsNotNull(chargen);
            Assert.AreEqual(0xE000002u, chargen.Id);
            

            Assert.AreEqual(5, chargen.StartingAreas.Count);
            Assert.AreEqual("Holtburg", chargen.StartingAreas[0].Name);
            Assert.AreEqual("Shoushi", chargen.StartingAreas[1].Name);
            Assert.AreEqual("Yaraq", chargen.StartingAreas[2].Name);
            Assert.AreEqual("Sanamar", chargen.StartingAreas[3].Name);
            Assert.AreEqual("OlthoiLair", chargen.StartingAreas[4].Name);

            Assert.AreEqual(13, chargen.HeritageGroups.Count);

            Assert.AreEqual("Aluvian", chargen.HeritageGroups[1].Name);
            Assert.AreEqual(0x060004C6u, chargen.HeritageGroups[1].IconId);
            Assert.AreEqual(52u, chargen.HeritageGroups[1].SkillCredits);
            Assert.AreEqual(1, chargen.HeritageGroups[1].Skills.Count);
            Assert.AreEqual(SkillId.ArcaneLore, chargen.HeritageGroups[1].Skills.First().Id);
            Assert.AreEqual(0, chargen.HeritageGroups[1].Skills.First().NormalCost);
            Assert.AreEqual(2, chargen.HeritageGroups[1].Skills.First().PrimaryCost);

            Assert.AreEqual(7, chargen.HeritageGroups[1].Templates.Count);
            Assert.AreEqual(100, chargen.HeritageGroups[1].Templates.Last().Strength);
            Assert.AreEqual(100, chargen.HeritageGroups[1].Templates.Last().Coordination);
            Assert.AreEqual(4, chargen.HeritageGroups[1].Templates.Last().PrimarySkills.Count);


            Assert.AreEqual(2, chargen.HeritageGroups[1].Genders.Count);
            Assert.AreEqual("Male", chargen.HeritageGroups[1].Genders[1].Name);
            Assert.AreEqual("Female", chargen.HeritageGroups[1].Genders[2].Name);


            Assert.AreEqual("OlthoiAcid", chargen.HeritageGroups[13].Name);
            Assert.AreEqual(52u, chargen.HeritageGroups[13].SkillCredits);
            Assert.AreEqual(1, chargen.HeritageGroups[13].Skills.Count);
            Assert.AreEqual(SkillId.ArcaneLore, chargen.HeritageGroups[13].Skills.First().Id);
            Assert.AreEqual(0, chargen.HeritageGroups[13].Skills.First().NormalCost);
            Assert.AreEqual(2, chargen.HeritageGroups[13].Skills.First().PrimaryCost);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<CharGen>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE000002);
        }
    }
}
