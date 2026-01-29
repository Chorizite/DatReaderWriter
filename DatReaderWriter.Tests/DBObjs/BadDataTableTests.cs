using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class BadDataTableTests {
        [TestMethod]
        public void CanInsertAndReadBadDataTable() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new BadDataTable() {
                Id = 0xE00001A,
                BadIds = new () {
                    { 1, 1 },
                    { 2, 2 }
                }
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<BadDataTable>(0xE00001A, out var readAnim);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readAnim);

            Assert.AreEqual(0xE00001Au, readAnim.Id);

            Assert.AreEqual(2, readAnim.BadIds.Count);
            Assert.AreEqual(1u, readAnim.BadIds[1]);
            Assert.AreEqual(2u, readAnim.BadIds[2]);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAnimations() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);


            var res = dat.TryGet<BadDataTable>(0xE00001A, out var badData);
            Assert.IsTrue(res);
            Assert.IsNotNull(badData);
            Assert.AreEqual(0xE00001Au, badData.Id);

            Assert.AreEqual(1660, badData.BadIds.Count);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<BadDataTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE00001A);
        }
    }
}
