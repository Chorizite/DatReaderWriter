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
    public class ClothingTests {
        [TestMethod]
        public void CanInsertAndReadClothing() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new Clothing() {
                Id = 0x10000001,
                ClothingBaseEffects = new Dictionary<uint, ClothingBaseEffect>() {
                    { 1, new ClothingBaseEffect()}
                },
                ClothingSubPalEffects = new Dictionary<uint, CloSubPalEffect>() {
                    { 2, new CloSubPalEffect() }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<Clothing>(0x10000001, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x10000001u, readObj.Id);

            Assert.AreEqual(writeObj.ClothingBaseEffects.Count, readObj.ClothingBaseEffects.Count);
            Assert.AreEqual(writeObj.ClothingBaseEffects.First().Key, readObj.ClothingBaseEffects.First().Key);

            Assert.AreEqual(writeObj.ClothingSubPalEffects.Count, readObj.ClothingSubPalEffects.Count);
            Assert.AreEqual(writeObj.ClothingSubPalEffects.First().Key, readObj.ClothingSubPalEffects.First().Key);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORClothing() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<Clothing>(0x10000064u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x10000064u, readObj.Id);

            Assert.AreEqual(1, readObj.ClothingBaseEffects.Count);
            Assert.AreEqual(0x02000039u, readObj.ClothingBaseEffects.First().Key);

            Assert.AreEqual(0u, readObj.ClothingSubPalEffects[62].Icon);
            Assert.AreEqual(2048u, readObj.ClothingSubPalEffects[62].CloSubPalettes.First().Ranges.First().NumColors);
            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORClothingAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<Clothing>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x10000863);
            TestHelpers.CanReadAndWriteIdentical<Clothing>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x10000001);
        }
    }
}
