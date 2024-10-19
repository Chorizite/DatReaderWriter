using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SurfaceTextureTests {
        [TestMethod]
        public void CanInsertAndReadTextures() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatDatabaseType.Portal, 0);

            var texture1 = new SurfaceTexture() {
                Id = 0x06000001,
                Width = 100,
                Height = 100,
                Format = PixelFormat.PFID_UNKNOWN,
                SourceData = new byte[1000],
                DefaultPaletteId = 1111, // this shouldn't get serialized for PFID_UNKNOWN
            };
            var texture2 = new SurfaceTexture() {
                Id = 0x06000002,
                Width = 100,
                Height = 100,
                Format = PixelFormat.PFID_INDEX16,
                SourceData = new byte[1000],
                DefaultPaletteId = 1111
            };

            var res = dat.TryWriteFile(texture1);
            Assert.IsTrue(res);

            res = dat.TryWriteFile(texture2);
            Assert.IsTrue(res);

            res = dat.TryReadFile<SurfaceTexture>(0x06000001, out var readTexture1);
            Assert.IsTrue(res);
            Assert.IsNotNull(readTexture1);

            Assert.AreEqual(0x06000001u, readTexture1.Id);
            Assert.AreEqual(100, readTexture1.Width);
            Assert.AreEqual(100, readTexture1.Height);
            Assert.AreEqual(PixelFormat.PFID_UNKNOWN, readTexture1.Format);
            Assert.AreEqual(1000, readTexture1.SourceData.Length);
            Assert.AreEqual(0u, readTexture1.DefaultPaletteId); // should be empty even though it's set above

            res = dat.TryReadFile<SurfaceTexture>(0x06000002, out var readTexture2);
            Assert.IsTrue(res);
            Assert.IsNotNull(readTexture2);

            Assert.AreEqual(0x06000002u, readTexture2.Id);
            Assert.AreEqual(100, readTexture2.Width);
            Assert.AreEqual(100, readTexture2.Height);
            Assert.AreEqual(PixelFormat.PFID_INDEX16, readTexture2.Format);
            Assert.AreEqual(1000, readTexture2.SourceData.Length);
            Assert.AreEqual(1111u, readTexture2.DefaultPaletteId);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORTextures() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });


            var res = dat.TryReadFile<SurfaceTexture>(0x06000164, out var texture1);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture1);

            Assert.AreEqual(6u, texture1.DataCategory);
            Assert.AreEqual(20, texture1.Width);
            Assert.AreEqual(20, texture1.Height);
            Assert.AreEqual(PixelFormat.PFID_R8G8B8, texture1.Format);
            Assert.AreEqual(1200, texture1.SourceData.Length);
            Assert.AreEqual(0u, texture1.DefaultPaletteId);

            res = dat.TryReadFile<SurfaceTexture>(0x06007364, out var texture2);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture2);

            Assert.AreEqual(4u, texture2.DataCategory);
            Assert.AreEqual(512, texture2.Width);
            Assert.AreEqual(512, texture2.Height);
            Assert.AreEqual(PixelFormat.PFID_INDEX16, texture2.Format);
            Assert.AreEqual(524288, texture2.SourceData.Length);
            Assert.AreEqual(0x0400007Eu, texture2.DefaultPaletteId);

            dat.Dispose();
        }
    }
}
