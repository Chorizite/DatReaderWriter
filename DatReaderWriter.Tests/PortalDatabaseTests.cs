using DatReaderWriter;
using DatReaderWriter.Enums;
using DatReaderWriter.Options;
using DatReaderWriter.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests {
    [TestClass]
    public class PortalDatabaseTests {
        [TestMethod]
        public void CanCreateAndOpen() {
            var datFilePath = Path.GetTempFileName();
            var dat = new PortalDatabase(o => {
                o.AccessType = DatAccessType.ReadWrite;
                o.FilePath = datFilePath;
            });
            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            dat.Dispose();
            dat = new PortalDatabase(datFilePath);

            Assert.IsTrue(dat.BlockAllocator.HasHeaderData);
            Assert.AreEqual(DatFileType.Portal, dat.Header.Type);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        public void FailsTopOpenCellDat() {
            var datFilePath = Path.GetTempFileName();
            var cellDat = new CellDatabase(o => {
                o.AccessType = DatAccessType.ReadWrite;
                o.FilePath = datFilePath;
            });
            cellDat.BlockAllocator.InitNew(DatFileType.Cell, 0);
            cellDat.Dispose();

            try {
                using var portalDat = new PortalDatabase(datFilePath);
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ae) {
                Assert.AreEqual(@$"Tried to open {datFilePath} as a portal database, but it's type is Cell", ae.Message);
            }
            catch (Exception e) {
                Assert.Fail($"Unexpected exception of type {e.GetType()} caught: {e.Message}");
            }

            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanOpenEoRPortalDat() {
            using var dat = new PortalDatabase(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"));
            EORCommonData.AssertGoodHeader(dat.Header);
        }
    }
}
