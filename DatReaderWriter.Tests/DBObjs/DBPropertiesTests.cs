using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class DBPropertiesTests {
        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            // TODO: broken

            //TestHelpers.CanReadAndWriteIdentical<DBProperties>("", 0x78000000u, dat.Portal);
            //TestHelpers.CanReadAndWriteIdentical<DBProperties>("", 0x78000001u, dat.Portal);
        }
    }
}
