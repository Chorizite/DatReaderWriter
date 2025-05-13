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
    public class TabooTableTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new TabooTable() {
                Id = 0x0E00001Eu,
                Entries = new Dictionary<uint, TabooTableEntry>() {
                    { 1, new TabooTableEntry() { BannedPatterns = ["test", "test2"] } }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<TabooTable>(0x0E00001Eu, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x0E00001Eu, readObj.Id);

            Assert.AreEqual(1, readObj.Entries.Count);
            Assert.AreEqual("test", readObj.Entries[1].BannedPatterns[0]);
            Assert.AreEqual("test2", readObj.Entries[1].BannedPatterns[1]);

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

            var res = dat.TryGet<TabooTable>(0x0E00001Eu, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);
            Assert.AreEqual(0x0E00001Eu, rt1.Id);

            Assert.AreEqual(32, rt1.Entries.Count);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<TabooTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E00001Eu);
        }
    }
}
