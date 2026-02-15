using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SurfaceTests {
        [TestMethod]
        public void CanInsertAndReadSurfaces() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var surface1 = new Surface() {
                Id = 0x05000001, // Ids of surfaces dont get written!
                Type = SurfaceType.Base1Image,
                OrigTextureId = 1,
                OrigPaletteId = 2,
                Diffuse = 0.2f,
                Luminosity = 0.3f,
                Translucency = 0.4f,
            };

            var surface2 = new Surface() {
                Id = 0x05000002, // Ids of surfaces dont get written!
                Type = SurfaceType.Base1Solid | SurfaceType.Alpha,
                ColorValue = new ColorARGB() {
                    Alpha = 0x80,
                    Red = 0x81,
                    Green = 0x82,
                    Blue = 0x83
                },
                Diffuse = 0.2f,
                Luminosity = 0.3f,
                Translucency = 0.4f,
            };

            var res = dat.TryWriteFile(surface1);
            Assert.IsTrue(res);

            res = dat.TryWriteFile(surface2);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<Surface>(0x05000001, out var readSurface1);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readSurface1);

            Assert.AreEqual(0x05000001u, readSurface1.Id); // Ids of surfaces dont get written!
            Assert.AreEqual(SurfaceType.Base1Image, readSurface1.Type);
            Assert.AreEqual(1u, readSurface1.OrigTextureId);
            Assert.AreEqual(2u, readSurface1.OrigPaletteId);
            Assert.AreEqual(0.2f, readSurface1.Diffuse);
            Assert.AreEqual(0.3f, readSurface1.Luminosity);
            Assert.AreEqual(0.4f, readSurface1.Translucency);
            Assert.IsNull(readSurface1.ColorValue);

            res2 = dat.TryGet<Surface>(0x05000002, out var readSurface2);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readSurface2);

            Assert.AreEqual(0x05000002u, readSurface2.Id); // Ids of surfaces dont get written!
            Assert.AreEqual(SurfaceType.Base1Solid | SurfaceType.Alpha, readSurface2.Type);
            Assert.AreEqual(0u, readSurface2.OrigTextureId);
            Assert.AreEqual(0u, readSurface2.OrigPaletteId);
            Assert.AreEqual(0.2f, readSurface2.Diffuse);
            Assert.AreEqual(0.3f, readSurface2.Luminosity);
            Assert.AreEqual(0.4f, readSurface2.Translucency);
            Assert.IsNotNull(readSurface2.ColorValue);

            Assert.AreEqual(0x80u, readSurface2.ColorValue.Alpha);
            Assert.AreEqual(0x81u, readSurface2.ColorValue.Red);
            Assert.AreEqual(0x82u, readSurface2.ColorValue.Green);
            Assert.AreEqual(0x83u, readSurface2.ColorValue.Blue);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORTextures() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });

            var res = dat.TryGet<Surface>(0x08000DCB, out var surface1);
            Assert.IsTrue(res);
            Assert.IsNotNull(surface1);

            Assert.AreEqual(0u, surface1.DataCategory);
            Assert.AreEqual(SurfaceType.Base1Image, surface1.Type);
            Assert.AreEqual(0x05001A9Au, surface1.OrigTextureId);
            Assert.AreEqual(0u, surface1.OrigPaletteId);
            Assert.AreEqual(0f, surface1.Luminosity);
            Assert.AreEqual(0f, surface1.Translucency);
            Assert.AreEqual(0.800781f, surface1.Diffuse);
            Assert.IsNull(surface1.ColorValue);

            res = dat.TryGet<Surface>(0x08000219, out var surface2);
            Assert.IsTrue(res);
            Assert.IsNotNull(surface2);

            Assert.AreEqual(0u, surface2.DataCategory);
            Assert.AreEqual(SurfaceType.Base1Solid, surface2.Type);
            Assert.AreEqual(0u, surface2.OrigTextureId);
            Assert.AreEqual(0u, surface2.OrigPaletteId);
            Assert.AreEqual(0f, surface2.Luminosity);
            Assert.AreEqual(0f, surface2.Translucency);
            Assert.AreEqual(1f, surface2.Diffuse);
            Assert.IsNotNull(surface2.ColorValue);

            Assert.AreEqual(255, surface2.ColorValue.Alpha);
            Assert.AreEqual(111, surface2.ColorValue.Red);
            Assert.AreEqual(57, surface2.ColorValue.Green);
            Assert.AreEqual(22, surface2.ColorValue.Blue);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<Surface>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x08000219);
        }
    }
}
