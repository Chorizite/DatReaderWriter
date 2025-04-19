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
    public class ContractTableTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new ContractTable() {
                Id = 0x0E00001Du,
                Contracts = {
                    { 1, new Contract() {
                        ContractId = 0x1u,
                        LocationQuestArea = new Position() { CellId = 0x00010100, Frame = new Frame() { Orientation = Quaternion.Identity, Origin = Vector3.UnitZ } },
                        ContractName = "Contract 1",
                        Description = "Description 1",
                        DescriptionProgress = "Description Progress 1",
                        LocationNPCEnd = new Position() { CellId = 0x00010100, Frame = new Frame() { Orientation = Quaternion.Identity, Origin = Vector3.UnitZ } },
                        LocationNPCStart = new Position() { CellId = 0x00010100, Frame = new Frame() { Orientation = Quaternion.Identity, Origin = Vector3.UnitZ } },
                        NameNPCEnd = "NPC End 1",
                        NameNPCStart = "NPC Start 1",
                        QuestflagFinished = "Finished 1",
                        QuestflagProgress = "Progress 1",
                        QuestflagRepeatTime = "Repeat Time 1",
                        QuestflagStamped = "Stamped 1",
                        QuestflagStarted = "Started 1",
                        QuestflagTimer = "Timer 1",
                        Version = 1
                    } }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<ContractTable>(0x0E00001Du, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x0E00001Du, readObj.Id);

            Assert.AreEqual(1, readObj.Contracts.Count);
            Assert.AreEqual(0x1u, readObj.Contracts[1].ContractId);
            Assert.AreEqual("Contract 1", readObj.Contracts[1].ContractName);
            Assert.AreEqual("Description 1", readObj.Contracts[1].Description);
            Assert.AreEqual("Description Progress 1", readObj.Contracts[1].DescriptionProgress);
            Assert.AreEqual("NPC End 1", readObj.Contracts[1].NameNPCEnd);
            Assert.AreEqual("NPC Start 1", readObj.Contracts[1].NameNPCStart);
            Assert.AreEqual("Finished 1", readObj.Contracts[1].QuestflagFinished);
            Assert.AreEqual("Progress 1", readObj.Contracts[1].QuestflagProgress);
            Assert.AreEqual("Repeat Time 1", readObj.Contracts[1].QuestflagRepeatTime);
            Assert.AreEqual("Stamped 1", readObj.Contracts[1].QuestflagStamped);
            Assert.AreEqual("Started 1", readObj.Contracts[1].QuestflagStarted);
            Assert.AreEqual("Timer 1", readObj.Contracts[1].QuestflagTimer);
            Assert.AreEqual(1u, readObj.Contracts[1].Version);

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

            var res = dat.TryReadFile<ContractTable>(0x0E00001Du, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);
            Assert.AreEqual(0x0E00001Du, rt1.Id);

            Assert.AreEqual(322, rt1.Contracts.Count);
            Assert.AreEqual(200u, rt1.Contracts[200].ContractId);
            Assert.AreEqual("Jailbreak: Ardent Leader", rt1.Contracts[200].ContractName);
            Assert.AreEqual("Defeat the Large Ardent Moarsman in the Freebooter Prison.", rt1.Contracts[200].Description);
            Assert.AreEqual("%d/1 Large Ardent Moarsman", rt1.Contracts[200].DescriptionProgress);
            Assert.AreEqual("Avarin", rt1.Contracts[200].NameNPCEnd);
            Assert.AreEqual("Avarin", rt1.Contracts[200].NameNPCStart);
            Assert.AreEqual("", rt1.Contracts[200].QuestflagFinished);
            Assert.AreEqual("FreebooterKillTaskBoss10809", rt1.Contracts[200].QuestflagProgress);
            Assert.AreEqual("FreebooterKillTaskBoss1Wait0809", rt1.Contracts[200].QuestflagRepeatTime);
            Assert.AreEqual("", rt1.Contracts[200].QuestflagStamped);
            Assert.AreEqual("", rt1.Contracts[200].QuestflagStarted);
            Assert.AreEqual("", rt1.Contracts[200].QuestflagTimer);
            Assert.AreEqual(0u, rt1.Contracts[200].Version);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ContractTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E00001Du);
        }
    }
}
