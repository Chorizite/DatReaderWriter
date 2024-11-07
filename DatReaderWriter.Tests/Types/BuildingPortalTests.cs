using System;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class BuildingPortalTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new BuildingPortal() {
                Flags = PortalFlags.PortalSide,
                OtherCellId = 789,
                OtherPortalId = 987,
                StabList = [234, 456]
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new BuildingPortal();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.Flags, readObj.Flags);
            Assert.AreEqual(writeObj.OtherCellId, readObj.OtherCellId);
            Assert.AreEqual(writeObj.OtherPortalId, readObj.OtherPortalId);
            Assert.AreEqual(writeObj.StabList.Count, readObj.StabList.Count);
            CollectionAssert.AreEqual(writeObj.StabList, readObj.StabList);
        }
    }
}
