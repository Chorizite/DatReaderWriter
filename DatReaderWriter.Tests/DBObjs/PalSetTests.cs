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
    public class PalSetTests {
        [TestMethod]
        public void CanInsertAndReadPaletteSets() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new PalSet() {
                Id = 0xF000001,
                Palettes = [1, 2, 3, 4, 5, 6, 7]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<PalSet>(0xF000001, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0xF000001u, readObj.Id);

            CollectionAssert.AreEqual(writeObj.Palettes, readObj.Palettes);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORPaletteSet() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);


            var res = dat.TryGet<PalSet>(0xF000001, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0xF000001u, readObj.Id);
            Assert.AreEqual(4, readObj.Palettes.Count);

            CollectionAssert.AreEqual(new uint[] { 0x040005F3, 0x040005F4, 0x040005F5, 0x040005F2 }, readObj.Palettes.Select(p => p.DataId).ToArray());

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORWaveAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<PalSet>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xF000001);
        }
    }
}
