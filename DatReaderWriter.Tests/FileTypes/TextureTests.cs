using ACClientLib.DatReaderWriter.IO.BlockAllocators;
using ACClientLib.DatReaderWriter.IO.DatBTree;
using ACClientLib.DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACClientLib.DatReaderWriter.Options;
using ACDatReader.FileTypes;
using ACClientLib.DatReaderWriter.Enums;

namespace DatReaderWriter.Tests.FileTypes {
    [TestClass]
    public class TextureTests {
        [TestMethod]
        public void CanInsertAndReadTexture() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatDatabaseType.Portal, 0);

            var texture = new Texture() {
                Id = 0x06000001,
                Width = 100,
                Height = 100,
                Format = SurfacePixelFormat.PFID_UNKNOWN,
                SourceData = new byte[10000],
                Length = 10000
            };

            var res = dat.TryWriteFile(texture);
            Assert.IsTrue(res);

            res = dat.TryReadFile<Texture>(texture.Id, out var readTexture);
            Assert.IsTrue(res);

            Assert.AreEqual(texture.Id, readTexture.Id);
            Assert.AreEqual(texture.Width, 100);
            Assert.AreEqual(texture.Height, 100);
            Assert.AreEqual(texture.Format, SurfacePixelFormat.PFID_UNKNOWN);
            Assert.AreEqual(texture.Length, 10000);

            dat.Dispose();

            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORTexture() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });


            var res = dat.TryReadFile<Texture>(0x06000164, out var texture);
            Assert.IsTrue(res);
            Assert.IsNotNull(texture);

            Assert.AreEqual(6, texture.Unknown);
            Assert.AreEqual(20, texture.Width);
            Assert.AreEqual(20, texture.Height);
            Assert.AreEqual(SurfacePixelFormat.PFID_R8G8B8, texture.Format);
            Assert.AreEqual(1200, texture.Length);

            dat.Dispose();
        }
    }
}
