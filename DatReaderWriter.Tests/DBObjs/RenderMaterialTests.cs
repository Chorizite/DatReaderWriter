using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using DatReaderWriter.Lib.IO.DatBTree;
using DatReaderWriter.Lib.IO.BlockAllocators;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class RenderMaterialTests {
        [TestMethod]
        public void CanInsertAndRead() {
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);
            
            dat.Portal.TryGetFileBytes(0x16000000u, out var fileBytes);
            Assert.IsNotNull(fileBytes);
            Console.WriteLine($"File is {fileBytes.Length} bytes long.");

            var res = dat.TryGet<RenderMaterial>(0x16000000u, out var material);
            Assert.IsTrue(res);
            Assert.IsNotNull(material);
            
            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            //TestHelpers.CanReadAndWriteIdentical<RenderMaterial>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x16000000u);
        }
    }
}
