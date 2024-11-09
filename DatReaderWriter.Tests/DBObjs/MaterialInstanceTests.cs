using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class MaterialInstanceTests {
        [TestMethod]
        public void CanInsertAndReadMaterialInstances() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new MaterialInstance() {
                Id = 0x18000000u,
                MaterialId = 0x1000001,
                MaterialType = 2,
                WantDiscardGeometry = true,
                AllowStencilShadows = true,
                ModifierRefs = [1, 2, 3, 4, 5, 6]
            };

            var res = dat.TryWriteFile(writeObj);

            var res2 = dat.TryReadFile<MaterialInstance>(0x18000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x18000000u, readObj.Id);
            Assert.AreEqual(0x1000001u, readObj.MaterialId);
            Assert.AreEqual(2u, readObj.MaterialType);
            Assert.IsTrue(readObj.WantDiscardGeometry);
            Assert.IsTrue(readObj.AllowStencilShadows);
            CollectionAssert.AreEqual(writeObj.ModifierRefs, readObj.ModifierRefs);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORMaterialInstances() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryReadFile<MaterialInstance>(0x18000000, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x18000000u, readObj.Id);

            Assert.AreEqual(0x16000000u, readObj.MaterialId);
            Assert.AreEqual(2u, readObj.MaterialType);
            CollectionAssert.AreEqual(new uint[] { 0x17000000 }, readObj.ModifierRefs);
            Assert.AreEqual(true, readObj.AllowStencilShadows);
            Assert.AreEqual(false, readObj.WantDiscardGeometry);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<MaterialInstance>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x18000000);
        }
    }
}
