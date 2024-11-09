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
    public class WaveTests {
        [TestMethod]
        public void CanInsertAndReaWaves() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeWave = new Wave() {
                Id = 0xA000001,
                Header = [4, 3, 2, 1],
                Data = [1, 2, 3, 4, 5, 6, 7]
            };

            var res = dat.TryWriteFile(writeWave);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<Wave>(0xA000001, out var readWave);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readWave);

            Assert.AreEqual(0xA000001u, readWave.Id);

            Assert.AreEqual(writeWave.Header.Length, readWave.Header.Length);
            Assert.AreEqual(writeWave.Data.Length, readWave.Data.Length);

            CollectionAssert.AreEqual(writeWave.Header, readWave.Header);
            CollectionAssert.AreEqual(writeWave.Data, readWave.Data);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORWave() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<Wave>(0x0A000002, out var wave1);
            Assert.IsTrue(res);
            Assert.IsNotNull(wave1);
            Assert.AreEqual(0x0A000002u, wave1.Id);
            Assert.AreEqual(18, wave1.Header.Length);
            Assert.AreEqual(7046, wave1.Data.Length);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<Wave>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0A000014);
        }
    }
}
