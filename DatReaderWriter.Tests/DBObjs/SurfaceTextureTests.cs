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
                Id = 0x05000001,
                Type = TextureType.Texture2D,
                Textures = [ 0x06000002, 0x06000003 ],
            };

            var res = dat.TryWriteFile(texture1);
            Assert.IsTrue(res);

            res = dat.TryReadFile<SurfaceTexture>(0x05000001, out var readTexture1);
            Assert.IsTrue(res);
            Assert.IsNotNull(readTexture1);

            Assert.AreEqual(0x05000001u, readTexture1.Id);
            Assert.AreEqual(TextureType.Texture2D, readTexture1.Type);
            Assert.AreEqual(2, readTexture1.Textures.Count);
            Assert.AreEqual(0x06000002u, readTexture1.Textures[0]);
            Assert.AreEqual(0x06000003u, readTexture1.Textures[1]);

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


            var res = dat.TryReadFile<SurfaceTexture>(0x0500330D, out var texture1);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture1);

            Assert.AreEqual(0u, texture1.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, texture1.Type);
            CollectionAssert.AreEqual(new uint[] { 0x060074CD }, texture1.Textures);

            res = dat.TryReadFile<SurfaceTexture>(0x050023BF, out var texture2);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture2);

            Assert.AreEqual(0u, texture2.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, texture2.Type);
            CollectionAssert.AreEqual(new uint[] { 0x06005731, 0x06005731 }, texture2.Textures);

            dat.Dispose();
        }
    }
}
