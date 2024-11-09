using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class EnumMapperTests {
        [TestMethod]
        public void CanInsertAndReadEnumMapper() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new EnumMapper() {
                Id = 0x22000001,
                NumberingType = NumberingType.Sequential,
                BaseEnumMap = 0x12345678,
                IdToStringMap = new Dictionary<uint, string>() {
                    { 1, "test" },
                    { 2, "test 2" }
                }
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<EnumMapper>(0x22000001, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x22000001u, readObj.Id);

            Assert.AreEqual(NumberingType.Sequential, readObj.NumberingType);
            Assert.AreEqual(0x12345678u, readObj.BaseEnumMap);
            Assert.AreEqual(2, readObj.IdToStringMap.Count);
            Assert.AreEqual("test", readObj.IdToStringMap[1]);
            Assert.AreEqual("test 2", readObj.IdToStringMap[2]);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOREnumMappers() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<EnumMapper>(0x2200001B, out var obj);
            Assert.IsTrue(res);
            Assert.IsNotNull(obj);
            Assert.AreEqual(0x2200001Bu, obj.Id);

            Assert.AreEqual((NumberingType)7, obj.NumberingType);
            Assert.AreEqual(0x22000027u, obj.BaseEnumMap);
            Assert.AreEqual(1765, obj.IdToStringMap.Count);


            res = dat.TryReadFile<EnumMapper>(0x22000014, out var obj2);
            Assert.IsTrue(res);
            Assert.IsNotNull(obj2);
            Assert.AreEqual(0x22000014u, obj2.Id);

            Assert.AreEqual(3, obj2.IdToStringMap.Count);
            Assert.AreEqual(NumberingType.Undefined, obj2.NumberingType);
            Assert.AreEqual("Invalid", obj2.IdToStringMap[0]);
            Assert.AreEqual("All", obj2.IdToStringMap[1]);
            Assert.AreEqual("Default", obj2.IdToStringMap[2]);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<EnumMapper>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x22000005);
            TestHelpers.CanReadAndWriteIdentical<EnumMapper>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x2200000E);
            TestHelpers.CanReadAndWriteIdentical<EnumMapper>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x2200001B);
        }
    }
}
