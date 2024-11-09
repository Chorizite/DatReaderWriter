using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using System.Text;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SceneTests {
        [TestMethod]
        public void CanInsertAndReadPaletteSets() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new Scene() {
                Id = 0x12000001,
                Objects = [
                    new ObjectDesc() { ObjectId = 0x1234, BaseLoc = new Frame() },
                    new ObjectDesc() { ObjectId = 0x5678, BaseLoc = new Frame() },
                ]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<Scene>(0x12000001, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x12000001u, readObj.Id);

            Assert.AreEqual(writeObj.Objects.Count, readObj.Objects.Count);
            Assert.AreEqual(writeObj.Objects.First().ObjectId, readObj.Objects.First().ObjectId);
            Assert.AreEqual(writeObj.Objects.Last().ObjectId, readObj.Objects.Last().ObjectId);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORPaletteSet() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<Scene>(0x12000074, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x12000074u, readObj.Id);

            Assert.AreEqual(3, readObj.Objects.Count);

            Assert.AreEqual(0x02000247u, readObj.Objects[0].ObjectId);
            Assert.AreEqual(0.352f, readObj.Objects[0].Frequency);
            Assert.AreEqual(7.38f, readObj.Objects[0].DisplaceX);
            Assert.AreEqual(7.02f, readObj.Objects[0].DisplaceY);
            Assert.AreEqual(0.7f, readObj.Objects[0].MinScale);
            Assert.AreEqual(1.5f, readObj.Objects[0].MaxScale);
            Assert.AreEqual(360f, readObj.Objects[0].MaxRotation);

            Assert.AreEqual(0u, readObj.Objects[2].ObjectId);
            Assert.AreEqual(1f, readObj.Objects[2].Frequency);
            Assert.AreEqual(-0.66f, readObj.Objects[2].DisplaceX);
            Assert.AreEqual(1.38f, readObj.Objects[2].DisplaceY);
            Assert.AreEqual(1f, readObj.Objects[2].MinScale);
            Assert.AreEqual(1f, readObj.Objects[2].MaxScale);
            Assert.AreEqual(360f, readObj.Objects[2].MaxRotation);
            Assert.AreEqual(1u, readObj.Objects[2].WeenieObj);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORWaveAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<Scene>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x12000074);
            TestHelpers.CanReadAndWriteIdentical<Scene>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x12000080);
        }
    }
}
