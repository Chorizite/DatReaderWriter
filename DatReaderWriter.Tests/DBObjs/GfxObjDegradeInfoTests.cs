using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class GfxObjDegradeInfoTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new GfxObjDegradeInfo() {
                Id = 0x11000000u,
                Degrades = [
                    new GfxObjInfo() { Id = 0x01, DegradeMode = 1, IdealDist = 2, MaxDist = 3, MinDist = 1 },
                    new GfxObjInfo() { Id = 0x02, DegradeMode = 1, IdealDist = 2, MaxDist = 3, MinDist = 1 }
                ]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<GfxObjDegradeInfo>(0x11000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x11000000u, readObj.Id);

            Assert.AreEqual(2, readObj.Degrades.Count);
            Assert.AreEqual(0x01u, readObj.Degrades[0].Id);
            Assert.AreEqual(1u, readObj.Degrades[0].DegradeMode);
            Assert.AreEqual(2f, readObj.Degrades[0].IdealDist);
            Assert.AreEqual(3f, readObj.Degrades[0].MaxDist);
            Assert.AreEqual(1f, readObj.Degrades[0].MinDist);
            Assert.AreEqual(0x02u, readObj.Degrades[1].Id);
            Assert.AreEqual(1u, readObj.Degrades[1].DegradeMode);
            Assert.AreEqual(2f, readObj.Degrades[1].IdealDist);
            Assert.AreEqual(3f, readObj.Degrades[1].MaxDist);
            Assert.AreEqual(1f, readObj.Degrades[1].MinDist);

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

            var res = dat.TryReadFile<GfxObjDegradeInfo>(0x11000000u, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);
            Assert.AreEqual(0x11000000u, rt1.Id);

            Assert.AreEqual(4, rt1.Degrades.Count);
            Assert.AreEqual(0x01003769u, rt1.Degrades[0].Id);
            Assert.AreEqual(1u, rt1.Degrades[0].DegradeMode);
            Assert.AreEqual(25f, rt1.Degrades[0].IdealDist);
            Assert.AreEqual(50f, rt1.Degrades[0].MaxDist);
            Assert.AreEqual(10f, rt1.Degrades[0].MinDist);
            Assert.AreEqual(0x0100376Eu, rt1.Degrades[1].Id);
            Assert.AreEqual(1u, rt1.Degrades[1].DegradeMode);
            Assert.AreEqual(50f, rt1.Degrades[1].IdealDist);
            Assert.AreEqual(100f, rt1.Degrades[1].MaxDist);
            Assert.AreEqual(25f, rt1.Degrades[1].MinDist);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<GfxObjDegradeInfo>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x11000000u);
            TestHelpers.CanReadAndWriteIdentical<GfxObjDegradeInfo>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x11000001u);
        }
    }
}
