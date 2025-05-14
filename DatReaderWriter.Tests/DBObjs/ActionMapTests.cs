using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class ActionMapTests {
        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryGet<ActionMap>(0x26000000u, out var actionMap);
            Assert.IsTrue(res);
            Assert.IsNotNull(actionMap);
            Assert.AreEqual(0x26000000u, actionMap.Id);

            Assert.AreEqual(0x23000005u, actionMap.StringTableId);
            Assert.AreEqual(27, actionMap.InputMaps.Count);
            Assert.AreEqual(16, actionMap.ConflictingMaps.Count);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ActionMap>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x26000000u);
        }
    }
}
