using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class DataIdMapperTests {
        [TestMethod]
        public void CanInsertAndReadDataIdMappers() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new DataIdMapper() {
                Id = 0x25000014,
                ClientIDNumberingType = NumberingType.Sequential,
                ClientNameNumberingType = NumberingType.Sequential,
                ServerIDNumberingType = NumberingType.Sequential,
                ServerNameNumberingType = NumberingType.Sequential,
                ClientEnumToID = new Dictionary<uint, uint>() { { 0, 0 }, { 1, 0x13000000 } },
                ClientEnumToName = new Dictionary<uint, string>() { { 0, "test" }, { 1, "test2" } },
                ServerEnumToID = new Dictionary<uint, uint>() { },
                ServerEnumToName = new Dictionary<uint, string>() {},
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<DataIdMapper>(0x25000014, out var readDidMap);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readDidMap);

            Assert.AreEqual(0x25000014u, readDidMap.Id);
            Assert.AreEqual(NumberingType.Sequential, readDidMap.ClientIDNumberingType);
            Assert.AreEqual(NumberingType.Sequential, readDidMap.ClientNameNumberingType);
            Assert.AreEqual(NumberingType.Sequential, readDidMap.ServerIDNumberingType);
            Assert.AreEqual(NumberingType.Sequential, readDidMap.ServerNameNumberingType);

            Assert.AreEqual(2, readDidMap.ClientEnumToID.Count);
            Assert.AreEqual(2, readDidMap.ClientEnumToName.Count);
            Assert.AreEqual(0, readDidMap.ServerEnumToID.Count);
            Assert.AreEqual(0, readDidMap.ServerEnumToName.Count);

            Assert.AreEqual(0u, readDidMap.ClientEnumToID.First().Key);
            Assert.AreEqual(0u, readDidMap.ClientEnumToID.First().Value);
            Assert.AreEqual(1u, readDidMap.ClientEnumToID.Last().Key);
            Assert.AreEqual(0x13000000u, readDidMap.ClientEnumToID.Last().Value);

            Assert.AreEqual(0u, readDidMap.ClientEnumToName.First().Key);
            Assert.AreEqual("test", readDidMap.ClientEnumToName.First().Value);
            Assert.AreEqual(1u, readDidMap.ClientEnumToName.Last().Key);
            Assert.AreEqual("test2", readDidMap.ClientEnumToName.Last().Value);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORDataIdMapper() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<DataIdMapper>(0x25000014, out var didMap);
            Assert.IsTrue(res);
            Assert.IsNotNull(didMap);
            Assert.AreEqual(0x25000014u, didMap.Id);

            Assert.AreEqual(NumberingType.Sequential, didMap.ClientIDNumberingType);
            Assert.AreEqual(NumberingType.Sequential, didMap.ClientNameNumberingType);
            Assert.AreEqual(NumberingType.Sequential, didMap.ServerIDNumberingType);
            Assert.AreEqual(NumberingType.Sequential, didMap.ServerNameNumberingType);

            Assert.AreEqual(2, didMap.ClientEnumToID.Count);
            Assert.AreEqual(2, didMap.ClientEnumToName.Count);
            Assert.AreEqual(0, didMap.ServerEnumToID.Count);
            Assert.AreEqual(0, didMap.ServerEnumToName.Count);

            Assert.AreEqual(0u, didMap.ClientEnumToID.First().Key);
            Assert.AreEqual(0u, didMap.ClientEnumToID.First().Value);
            Assert.AreEqual(1u, didMap.ClientEnumToID.Last().Key);
            Assert.AreEqual(0x13000000u, didMap.ClientEnumToID.Last().Value);

            Assert.AreEqual(0u, didMap.ClientEnumToName.First().Key);
            Assert.AreEqual("Undef", didMap.ClientEnumToName.First().Value);
            Assert.AreEqual(1u, didMap.ClientEnumToName.Last().Key);
            Assert.AreEqual("Dereth", didMap.ClientEnumToName.Last().Value);

            dat.Dispose();
        }
    }
}
