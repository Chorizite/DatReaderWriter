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
    public class RenderTextureTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new RenderTexture() {
                Id = 0x15000000u,
                DataCategory = 1,
                TextureType = TextureType.Texture2D,
                SourceLevels = [
                    0x00000001u,
                    0x00000002u
                ]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<RenderTexture>(0x15000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x15000000u, readObj.Id);

            Assert.AreEqual(1u, readObj.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, readObj.TextureType);
            Assert.AreEqual(2, readObj.SourceLevels.Count);
            Assert.AreEqual(0x00000001u, readObj.SourceLevels[0]);
            Assert.AreEqual(0x00000002u, readObj.SourceLevels[1]);

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

            var res = dat.TryReadFile<RenderTexture>(0x15000000u, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);
            Assert.AreEqual(0x15000000u, rt1.Id);

            Assert.AreEqual(0u, rt1.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, rt1.TextureType);
            Assert.AreEqual(1, rt1.SourceLevels.Count);
            Assert.AreEqual(0x06004B91u, rt1.SourceLevels[0]);

            var res2 = dat.TryReadFile<RenderTexture>(0x15000001u, out var rt2);
            Assert.IsTrue(res2);
            Assert.IsNotNull(rt2);
            Assert.AreEqual(0x15000001u, rt2.Id);

            Assert.AreEqual(0u, rt2.DataCategory);
            Assert.AreEqual(TextureType.Texture2D, rt2.TextureType);
            Assert.AreEqual(1, rt2.SourceLevels.Count);
            Assert.AreEqual(0x06004B92u, rt2.SourceLevels[0]);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<RenderTexture>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x15000000u);
            TestHelpers.CanReadAndWriteIdentical<RenderTexture>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x15000001u);
        }
    }
}
