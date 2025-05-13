using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class PhysicsScriptTableTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new PhysicsScriptTable() {
                Id = 0x34000000u,
                ScriptTable = {
                    { PlayScript.AetheriaLevelUp, new PhysicsScriptTableData() { Scripts = [
                        new ScriptAndModData() { Mod = 1.1f, ScriptId = 0x12345678u},
                    ] } },
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<PhysicsScriptTable>(0x34000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x34000000u, readObj.Id);

            Assert.AreEqual(1, readObj.ScriptTable.Count);
            Assert.AreEqual(0x12345678u, readObj.ScriptTable[PlayScript.AetheriaLevelUp].Scripts[0].ScriptId);
            Assert.AreEqual(1.1f, readObj.ScriptTable[PlayScript.AetheriaLevelUp].Scripts[0].Mod);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryGet<PhysicsScriptTable>(0x34000004u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x34000004u, readObj.Id);

            Assert.AreEqual(139, readObj.ScriptTable.Count);
            Assert.AreEqual(0x3300125Eu, readObj.ScriptTable[PlayScript.AetheriaSurgeAffliction].Scripts[0].ScriptId);
            Assert.AreEqual(1f, readObj.ScriptTable[PlayScript.AetheriaLevelUp].Scripts[0].Mod);

            Assert.AreEqual(3, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts.Count);
            Assert.AreEqual(0x3300018Fu, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts[0].ScriptId);
            Assert.AreEqual(0f, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts[0].Mod);

            Assert.AreEqual(0x33000190u, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts[1].ScriptId);
            Assert.AreEqual(0.5f, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts[1].Mod);

            Assert.AreEqual(0x33000191u, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts[2].ScriptId);
            Assert.AreEqual(1f, readObj.ScriptTable[PlayScript.SwapHealth_Yellow_To_Red].Scripts[2].Mod);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<PhysicsScriptTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x34000004u);
            TestHelpers.CanReadAndWriteIdentical<PhysicsScriptTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x34000005u);
            TestHelpers.CanReadAndWriteIdentical<PhysicsScriptTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x34000006u);
        }
    }
}
