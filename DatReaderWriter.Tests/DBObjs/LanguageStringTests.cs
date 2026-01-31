using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class LanguageStringTests {
        [TestMethod]
        public void CanInsertAndReadLanguageStrings() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new LanguageString() {
                Id = 0x31000000,
                Value = "test",
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<LanguageString>(0x31000000, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x31000000u, readObj.Id);
            Assert.AreEqual(writeObj.Value, readObj.Value);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORLanguageStrings() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryGet<LanguageString>(0x31000010, out var obj);
            Assert.IsTrue(res);
            Assert.IsNotNull(obj);
            Assert.AreEqual(0x31000010u, obj.Id);

            Assert.AreEqual("Sho men's names have the surname first, and the \"first name\" last. Examples: Ninwa Xaojhen, Fenping Banli-Zan, Shui Chon-Po.", obj.Value);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<LanguageString>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x31000004);
            TestHelpers.CanReadAndWriteIdentical<LanguageString>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x3100000D);
            TestHelpers.CanReadAndWriteIdentical<LanguageString>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x3100000F);
            TestHelpers.CanReadAndWriteIdentical<LanguageString>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x31000013);
        }
    }
}
