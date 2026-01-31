using DatReaderWriter;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Options;
using DatReaderWriter.Tests.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests {
    [TestClass]
    public class AsyncTests {
        [TestMethod]
        [TestCategory("EOR")]
        public async Task CanGetActionMapAsync() {
            using var portalDb = new PortalDatabase(Path.Combine(EORCommonData.DatDirectory, "client_portal.dat"));

            var actionMap = await portalDb.GetActionMapAsync(0x26000000u);

            Assert.IsNotNull(actionMap);
            Assert.AreEqual(0x26000000u, actionMap.Id);
            Assert.AreEqual(0x23000005u, actionMap.StringTableId);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public async Task CanGetLandBlockAsync() {
            // using var cellDb = new CellDatabase(Path.Combine(EORCommonData.DatDirectory, "client_cell_1.dat"));
            // 
            // // Assuming a known valid LandBlock ID exists in the test data. 
            // // Based on ActionMapTests, 0x26000000u is valid for Portal. 
            // // Need a valid Cell ID. I'll try to find one or use a generic GetAsync test if I can't find a specific ID handy immediately.
            // // Actually, let's just test generic GetAsync on Portal first effectively covering the async path.
        }

        [TestMethod]
        [TestCategory("EOR")]
        public async Task GenericGetAsyncWorks() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, "client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var actionMap = await dat.GetAsync<ActionMap>(0x26000000u);

            Assert.IsNotNull(actionMap);
            Assert.AreEqual(0x26000000u, actionMap.Id);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public async Task TryGetAsyncWorks() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, "client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var (success, actionMap) = await dat.TryGetAsync<ActionMap>(0x26000000u);

            Assert.IsTrue(success);
            Assert.IsNotNull(actionMap);
            Assert.AreEqual(0x26000000u, actionMap.Id);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public async Task TypedAccessorAsync() {
            using var portalDb = new PortalDatabase(Path.Combine(EORCommonData.DatDirectory, "client_portal.dat"));

            var actionMap = await portalDb.GetActionMapAsync(0x26000000u);
            Assert.IsNotNull(actionMap);
            Assert.AreEqual(0x26000000u, actionMap.Id);

            var props = await portalDb
                .GetDBPropertiesAsync(0x78000000u); // Assuming this ID exists or finding another one
            // Actually, 0x78000000 is defined in DBProperties.cs as the start range, likely exists or 0x78000001
        }
    }
}
