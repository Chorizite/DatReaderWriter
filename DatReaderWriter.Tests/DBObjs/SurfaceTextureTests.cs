using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SurfaceTextureTests {
        [TestMethod]
        public void CanInsertAndReadTextures() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var texture1 = new SurfaceTexture() {
                Id = 0x05000001,
                Type = TextureType.Texture2D,
                Textures = [ 0x06000002, 0x06000003 ],
            };

            var res = dat.TryWriteFile(texture1);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<SurfaceTexture>(0x05000001, out var readTexture1);
            Assert.IsTrue(res2);
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
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryGet<SurfaceTexture>(0x0500330D, out var texture1);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture1);

            Assert.AreEqual(0u, texture1.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, texture1.Type);
            Assert.AreEqual(1, texture1.Textures.Count);
            Assert.AreEqual(0x060074CDu, texture1.Textures[0]);

            var textureObj = texture1.Textures.First().Get(dat);
            Assert.IsNotNull(textureObj);
            Assert.AreEqual(0x060074CDu, textureObj.Id);
            Assert.AreEqual(256, textureObj.Width);
            Assert.AreEqual(256, textureObj.Height);
            Assert.AreEqual(PixelFormat.PFID_R8G8B8, textureObj.Format);

            res = dat.TryGet<SurfaceTexture>(0x050023BF, out var texture2);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture2);

            Assert.AreEqual(0u, texture2.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, texture2.Type);
            Assert.AreEqual(2, texture2.Textures.Count);
            Assert.AreEqual(0x06005731u, texture2.Textures[0]);
            Assert.AreEqual(0x06005731u, texture2.Textures[1]);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<SurfaceTexture>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0500330D);
        }
    }
}
