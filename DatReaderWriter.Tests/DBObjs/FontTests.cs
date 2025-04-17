using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class FontTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new Font() {
                Id = 0x40000000u,
                BackgroundSurfaceDataId = 0x12345678u,
                BaselineOffset =2,
                ForegroundSurfaceDataId = 0x23456789u,
                MaxCharHeight = 1,
                MaxCharWidth = 2,
                NumHorizontalBorderPixels = 4,
                NumVerticalBorderPixels = 5,
                CharDescs = [
                    new FontCharDesc() {
                        HorizontalOffsetAfter = 1,
                        Height = 2,
                        HorizontalOffsetBefore = 3,
                        OffsetX = 4,
                        OffsetY = 5,
                        Unicode = 'A',
                        VerticalOffsetBefore = 6,
                        Width = 7
                    },
                ]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<Font>(0x40000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x40000000u, readObj.Id);

            Assert.AreEqual(0x12345678u, readObj.BackgroundSurfaceDataId);
            Assert.AreEqual(2u, readObj.BaselineOffset);
            Assert.AreEqual(0x23456789u, readObj.ForegroundSurfaceDataId);
            Assert.AreEqual(1u, readObj.MaxCharHeight);
            Assert.AreEqual(2u, readObj.MaxCharWidth);
            Assert.AreEqual(4u, readObj.NumHorizontalBorderPixels);
            Assert.AreEqual(5u, readObj.NumVerticalBorderPixels);

            Assert.AreEqual(1, readObj.CharDescs.Count);
            Assert.AreEqual(1u, readObj.CharDescs[0].HorizontalOffsetAfter);
            Assert.AreEqual(2u, readObj.CharDescs[0].Height);
            Assert.AreEqual(3u, readObj.CharDescs[0].HorizontalOffsetBefore);
            Assert.AreEqual(4u, readObj.CharDescs[0].OffsetX);
            Assert.AreEqual(5u, readObj.CharDescs[0].OffsetY);
            Assert.AreEqual('A', readObj.CharDescs[0].Unicode);
            Assert.AreEqual(6u, readObj.CharDescs[0].VerticalOffsetBefore);
            Assert.AreEqual(7u, readObj.CharDescs[0].Width);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryReadFile<Font>(0x40000000u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x40000000u, readObj.Id);

            Assert.AreEqual(0x6005EE6u, readObj.BackgroundSurfaceDataId);
            Assert.AreEqual(12u, readObj.BaselineOffset);
            Assert.AreEqual(0x6005EE5u, readObj.ForegroundSurfaceDataId);
            Assert.AreEqual(16u, readObj.MaxCharHeight);
            Assert.AreEqual(16u, readObj.MaxCharWidth);
            Assert.AreEqual(4u, readObj.NumHorizontalBorderPixels);
            Assert.AreEqual(4u, readObj.NumVerticalBorderPixels);

            Assert.AreEqual(1050, readObj.CharDescs.Count);

            Assert.AreEqual(1u, readObj.CharDescs[0].HorizontalOffsetAfter);
            Assert.AreEqual(16u, readObj.CharDescs[0].Height);
            Assert.AreEqual(0u, readObj.CharDescs[0].HorizontalOffsetBefore);
            Assert.AreEqual(4u, readObj.CharDescs[0].OffsetX);
            Assert.AreEqual(4u, readObj.CharDescs[0].OffsetY);
            Assert.AreEqual(' ', readObj.CharDescs[0].Unicode);
            Assert.AreEqual(0u, readObj.CharDescs[0].VerticalOffsetBefore);
            Assert.AreEqual(3u, readObj.CharDescs[0].Width);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<Font>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x40000000u);
            TestHelpers.CanReadAndWriteIdentical<Font>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x40000001u);
            TestHelpers.CanReadAndWriteIdentical<Font>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x40000002u);
        }
    }
}
