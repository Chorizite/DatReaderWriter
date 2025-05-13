using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class MasterPropertyTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new MasterProperty() {
                Id = 0x39000001u,
                EnumMapper = new EnumMapperData() {
                    BaseEnumMap = 0x12345678,
                    IdToStringMap = new Dictionary<uint, string>() {
                        { 1, "test" },
                    },
                    Unknown = 0x12345678,
                },
                Properties = new Dictionary<uint, BasePropertyDesc>() {
                    { 1, new BasePropertyDesc() {
                        Data = 0x12345678,
                        Type = BasePropertyType.Integer
                    } }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<MasterProperty>(0x39000001u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x39000001u, readObj.Id);

            Assert.AreEqual(1, readObj.EnumMapper.IdToStringMap.Count);
            Assert.AreEqual("test", readObj.EnumMapper.IdToStringMap[1]);

            Assert.AreEqual(1, readObj.Properties.Count);
            Assert.AreEqual(0x12345678u, readObj.Properties[1].Data);

            Assert.AreEqual(BasePropertyType.Integer, readObj.Properties[1].Type);

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

            var res = dat.TryReadFile<MasterProperty>(0x39000001u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x39000001u, readObj.Id);

            Assert.AreEqual(384, readObj.EnumMapper.IdToStringMap.Count);
            Assert.AreEqual("Invalid", readObj.EnumMapper.IdToStringMap[0]);
            Assert.AreEqual("UICore_Element_container", readObj.EnumMapper.IdToStringMap[0x36]);
            Assert.AreEqual("UI_Radar_XCoordField", readObj.EnumMapper.IdToStringMap[0x10000037]);

            Assert.AreEqual(383, readObj.Properties.Count);
            Assert.AreEqual(1u, readObj.Properties[1].Name);
            Assert.AreEqual(0x2200001Bu, readObj.Properties[1].Data);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            var datCollection = new DatCollection(EORCommonData.DatDirectory, DatAccessType.Read);
            TestHelpers.CanReadAndWriteIdentical<MasterProperty>("", 0x39000001u, datCollection.Portal);
        }
    }
}
