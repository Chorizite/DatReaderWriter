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
    public class BuildingInfoTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new BuildingInfo() {
                Frame = new Frame(),
                ModelId = 123,
                NumLeaves = 456,
                Portals = [
                    new BuildingPortal() {
                        Flags = PortalFlags.ExactMatch,
                        OtherCellId = 789,
                        OtherPortalId = 987,
                        StabList = [ 234, 456 ]
                    }
                ]
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new BuildingInfo();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.IsNotNull(readObj.Frame);
            Assert.AreEqual(writeObj.ModelId, readObj.ModelId);
            Assert.AreEqual(writeObj.NumLeaves, readObj.NumLeaves);
            Assert.AreEqual(writeObj.Portals.Count, readObj.Portals.Count);
            Assert.AreEqual(writeObj.Portals[0].Flags, readObj.Portals[0].Flags);
            Assert.AreEqual(writeObj.Portals[0].OtherCellId, readObj.Portals[0].OtherCellId);
            Assert.AreEqual(writeObj.Portals[0].OtherPortalId, readObj.Portals[0].OtherPortalId);
            Assert.AreEqual(writeObj.Portals[0].StabList.Count, readObj.Portals[0].StabList.Count);
            Assert.AreEqual(writeObj.Portals[0].StabList[0], readObj.Portals[0].StabList[0]);
            Assert.AreEqual(writeObj.Portals[0].StabList[1], readObj.Portals[0].StabList[1]);
        }
    }
}
