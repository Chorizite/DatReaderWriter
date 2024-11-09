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
    public class ParticleEmitterTests {
        [TestMethod]
        public void CanInsertAndReadParticleEmitter() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new ParticleEmitter() {
                Id = 0x32000001,
                EmitterType = EmitterType.BirthratePerMeter,
                TotalParticles = 30,
                GfxObjId = 0x12345678,
                IsParentLocal = true
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<ParticleEmitter>(0x32000001, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x32000001u, readObj.Id);

            Assert.AreEqual(writeObj.EmitterType, readObj.EmitterType);
            Assert.AreEqual(writeObj.TotalParticles, readObj.TotalParticles);
            Assert.AreEqual(writeObj.GfxObjId, readObj.GfxObjId);
            Assert.AreEqual(writeObj.IsParentLocal, readObj.IsParentLocal);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORParticleEmitter() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<ParticleEmitter>(0x320009C4u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x320009C4u, readObj.Id);

            Assert.AreEqual(EmitterType.BirthratePerSec, readObj.EmitterType);
            Assert.AreEqual(ParticleType.Explode, readObj.ParticleType);
            Assert.AreEqual(0.2, readObj.Birthrate);
            Assert.AreEqual(1, readObj.FinalTrans);
            Assert.AreEqual(false, readObj.IsParentLocal);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORParticleEmitterAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ParticleEmitter>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x3200000C);
            TestHelpers.CanReadAndWriteIdentical<ParticleEmitter>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x3200002B);
            TestHelpers.CanReadAndWriteIdentical<ParticleEmitter>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x320009D1);
        }
    }
}
