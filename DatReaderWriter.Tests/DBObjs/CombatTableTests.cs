using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using DatReaderWriter.Lib.IO.DatBTree;
using DatReaderWriter.Lib.IO.BlockAllocators;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class CombatTableTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new CombatTable() {
                Id = 0x30000000u,
                CombatManeuvers = [
                    new CombatManeuver(){
                        AttackHeight = AttackHeight.Medium,
                        AttackType = AttackType.DoubleStrike,
                        MinSkillLevel = 20,
                        Motion = MotionCommand.AimHigh75,
                        Style = MotionStance.BowCombat
                    }    
                ]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<CombatTable>(0x30000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x30000000u, readObj.Id);

            Assert.AreEqual(1, readObj.CombatManeuvers.Count);
            Assert.AreEqual(AttackHeight.Medium, readObj.CombatManeuvers[0].AttackHeight);
            Assert.AreEqual(AttackType.DoubleStrike, readObj.CombatManeuvers[0].AttackType);
            Assert.AreEqual(20u, readObj.CombatManeuvers[0].MinSkillLevel);
            Assert.AreEqual(MotionCommand.AimHigh75, readObj.CombatManeuvers[0].Motion);
            Assert.AreEqual(MotionStance.BowCombat, readObj.CombatManeuvers[0].Style);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new PortalDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryGet<CombatTable>(0x30000001u, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);
            Assert.AreEqual(0x30000001u, rt1.Id);

            Assert.AreEqual(3, rt1.CombatManeuvers.Count);

            Assert.AreEqual(AttackHeight.High, rt1.CombatManeuvers[0].AttackHeight);
            Assert.AreEqual(AttackType.Punch, rt1.CombatManeuvers[0].AttackType);
            Assert.AreEqual(0u, rt1.CombatManeuvers[0].MinSkillLevel);
            Assert.AreEqual(MotionCommand.AttackHigh1, rt1.CombatManeuvers[0].Motion);
            Assert.AreEqual(MotionStance.HandCombat, rt1.CombatManeuvers[0].Style);

            Assert.AreEqual(AttackHeight.Medium, rt1.CombatManeuvers[1].AttackHeight);
            Assert.AreEqual(AttackType.Punch, rt1.CombatManeuvers[1].AttackType);
            Assert.AreEqual(0u, rt1.CombatManeuvers[1].MinSkillLevel);
            Assert.AreEqual(MotionCommand.AttackMed1, rt1.CombatManeuvers[1].Motion);
            Assert.AreEqual(MotionStance.HandCombat, rt1.CombatManeuvers[1].Style);

            Assert.AreEqual(AttackHeight.Low, rt1.CombatManeuvers[2].AttackHeight);
            Assert.AreEqual(AttackType.Punch, rt1.CombatManeuvers[2].AttackType);
            Assert.AreEqual(0u, rt1.CombatManeuvers[2].MinSkillLevel);
            Assert.AreEqual(MotionCommand.AttackLow1, rt1.CombatManeuvers[2].Motion);
            Assert.AreEqual(MotionStance.HandCombat, rt1.CombatManeuvers[2].Style);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<CombatTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x30000000u);
            TestHelpers.CanReadAndWriteIdentical<CombatTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x30000001u);
            TestHelpers.CanReadAndWriteIdentical<CombatTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x30000008u);
            TestHelpers.CanReadAndWriteIdentical<CombatTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x30000003u);
        }
    }
}
