using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class RegionTests {
        [TestMethod]
        public void CanInsertAndReadRegions() {
            
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORRegion() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<Region>(0x13000000, out var region);
            Assert.IsTrue(res);
            Assert.IsNotNull(region);
            Assert.AreEqual(0x13000000u, region.Id);
            Assert.AreEqual(1u, region.RegionNumber);
            Assert.AreEqual("Dereth", region.RegionName);

            Assert.AreEqual(1u, region.RegionMisc.Version);
            Assert.AreEqual(0x0600127Du, region.RegionMisc.GameMapID);
            Assert.AreEqual(0x06000261u, region.RegionMisc.AutotestMapId);
            Assert.AreEqual(4u, region.RegionMisc.AutotestMapSize);
            Assert.AreEqual(0x01000FDEu, region.RegionMisc.ClearCellId);
            Assert.AreEqual(0x01001612u, region.RegionMisc.ClearMonsterId);

            dat.Dispose();
        }
    }
}
